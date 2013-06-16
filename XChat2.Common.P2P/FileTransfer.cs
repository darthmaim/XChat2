using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using XChat2.Client.Communication;
using XChat2.Common.Networking.Packets;
using Packets = XChat2.Common.Networking.Packets.Packets;
using XChat2.Common.Networking.Packets.Packets.FileTransfer;
using XChat2.Client.Communication.PacketHandlers;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using XChat2.Common.Networking.P2P.Client;
using XChat2.Common.Configuration;
using XChat2.Common.Exceptions;

namespace XChat2.Client.P2P
{
    public class FileTransfer
    {
        /*   |   | Client A  |    Server    | Client B
         * --+---+-----------+--------------+-------------   // - p2p initialisation -
         * x | t |    o----0x40---->o       |                //Requests FileTransfer
         *   | t |           |      o-----0x40----->o        //       -""-
         *   | t |           |      o<-----0x41-----o        //accepts/declines
         *   | t |           |      o<-----0x42-----o        //sends port
         *   | t |           |      o-----0x43----->o        //cancels for all other connections
         * x | t |    o<----0x41----o       |                //sends the response of B and fileTransferID (uid = request.uid)
         * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
         *                 P2PConnection
         * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
         *   | u |    o---------------------------->o        //dataInformation (totalsize, ...)
         *   | u |    o---------------------------->o        //data
         */

        private IConnection _connection;
        public IConnection Connection
        {
            get { return _connection; }
            private set { _connection = value; }
        }

        private uint _contactID;
        public uint ContactID
        {
            get { return _contactID; }
            private set { _contactID = value; }
        }

        private string _filename;
        public string Filename
        {
            get { return _filename; }
            private set { _filename = value; }
        }

        private uint _fileTransferID;
        public uint FileTransferID
        {
            get { return _fileTransferID; }
            private set { _fileTransferID = value; }
        }

        private Config _config;

        #region State
        private FileTransferStates _state = FileTransferStates.Waiting;
        public FileTransferStates State
        {
            get { return _state; }
            private set
            {
                FileTransferStates oldState = value;
                _state = value;
                OnStateChanged(oldState, value);
            }
        }
        
        private void OnStateChanged(FileTransferStates oldState, FileTransferStates newState) {
            if(oldState != newState && StateChanged != null)
                StateChanged(this, oldState, newState);
        }

        public delegate void StateChangedHandler(FileTransfer sender, FileTransferStates oldState, FileTransferStates newState);
        public event StateChangedHandler StateChanged;

        public enum FileTransferStates
        {
            Waiting,
            Starting,
            InProgress,
            Done,
            Canceled,
            Error
        }
        #endregion

        private IPAddress _address;
        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public FileTransfer(uint contact, string filename, IConnection connection, Config config)
        {
            _filename = filename;
            _contactID = contact;
            _connection = connection;

            registerFor<FileTransferStartPacket>();
            registerFor<FileTransferStatusPacket>();

            FileTransferRequestPacket request = new FileTransferRequestPacket(contact, filename);
            connection.EnqueuePacket(request);
        }

        private void registerFor<T>() where T: IServerPacket
        {
            _connection.RegisterIncomingPacketHandler<T>(new IncomingPacketHandler<T>(new Action<IConnection, T>(IncomingPacket<T>)));
        }

        private void registerForResponse<T>(T packet) where T: IClientPacket
        {
            _connection.RegisterForResponsePacket<T>(packet, new ResponsePacketHandler<IServerResponsePacketFor<T>>(new Action<IConnection,IServerResponsePacketFor<T>>(IncomingResponse)));
        }

        private void IncomingResponse<T>(IConnection connection, IServerResponsePacketFor<T> packet) where T: IClientPacket
        {
            if (packet.GetType() != typeof(FileTransferResponsePacket))
                throw new XChat2.Common.Exceptions.UnexpectedPacketException("Expected a FileTransferResponsePacket, but got " + packet.ToString());
            FileTransferResponsePacket response = (FileTransferResponsePacket)packet;
            _fileTransferID = response.FileTransferID;
            if (response.Accepted)
            {
                State = FileTransferStates.Starting;
            }
            else
                State = FileTransferStates.Canceled;
        }

        private UdpClient _udpClient;

        private void IncomingPacket<T>(IConnection connection, T packet) where T:IServerPacket 
        {
            switch (packet.Type)
            {
                case KnownPackets.FileTransferStart:
                    {
                        FileTransferStartPacket response = packet as FileTransferStartPacket;
                        if (FileTransferID == response.FileTransferID && State == FileTransferStates.Starting)
                        {
                            _authKey = response.AuthKey;
                        }
                        break;
                    }
            }
        }

        private byte[] _authKey;

        Thread runThread;

        private void startSender()
        {
            runThread = new Thread(runSender);
            runThread.Name = "[FileTransferSendThread (" + _fileTransferID + ")]";
            runThread.Start();
        }

        private void startReceiver()
        {
            runThread = new Thread(runReceiver);
            runThread.Name = "[FileTransferReceiveThread (" + _fileTransferID + ")]";
            runThread.Start();
        }

        private void runSender()
        {
            using (P2PConnection connection = new P2PConnection(_config["P2P"].get<string>("server").Value, _config["P2P"].get<int>("port").Value, _authKey))
            {
                try
                {
                    if (!connection.Start())
                    {
                        State = FileTransferStates.Error;
                        return;
                    }
                }
                catch (P2PException)
                {
                    State = FileTransferStates.Error;
                    return;
                }


            }
        }

        private void runReceiver()
        {
            using (P2PConnection connection = new P2PConnection(_config["P2P"].get<string>("server").Value, _config["P2P"].get<int>("port").Value, _authKey))
            {
                try
                {
                    if (!connection.Start())
                    {
                        State = FileTransferStates.Error;
                        return;
                    }
                }
                catch (P2PException)
                {
                    State = FileTransferStates.Error;
                    return;
                }


            }
        }
    }
}
