using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using XChat2.Common.Networking.Packets;
using Packets = XChat2.Common.Networking.Packets.Packets;
using XChat2.Common.Exceptions;
using XChat2.Common.Hashes;
using XChat2.Server.ConsoleHelpers;
using XChat2.Common.Collections;

namespace XChat2.Server.Clients
{
    public class Connection
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private Thread _tcpClientThread;
        private bool _run = true;

        private SyncQueue<IServerPacket> _sendPacketQueue;
        private SyncQueue<IClientPacket> _receivePacketQueue;

        private Client _client = null;

        public Client Client
        {
            get { return _client; }
        }

        private string _endPointString;

        public Connection(TcpClient tcpClient)
        {
            _sendPacketQueue = new SyncQueue<IServerPacket>();
            _receivePacketQueue = new SyncQueue<IClientPacket>();

            _endPointString = tcpClient.Client.RemoteEndPoint.ToString();

            this._tcpClient = tcpClient;

            _stream = _tcpClient.GetStream();

            _tcpClientThread = new Thread(new ThreadStart(Run));
            _tcpClientThread.IsBackground = true;
            _tcpClientThread.Start();
        }

        private bool _connected = true;
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }

        private string _disconnectReason = "error";

        byte _clientKeepAliveCounter = 0;
        private void Run()
        {
            try
            {
                Packets.HandshakePacket handshakePacket = WaitForPacket<Packets.HandshakePacket>();
                if(Client.ClientExists(handshakePacket.Username))
                {
                    Client c = Client.GetClient(handshakePacket.Username);
                    if(c.PasswordHash.SequenceEqual(handshakePacket.PasswordHash))
                    {
                        _client = c;
                    }
                }
                if(_client == null)
                {
                    SendPacket(new Packets.HandshakeResponsePacket(handshakePacket.UID, false));
                    //OnDisconnect("authentication failed");
                    _disconnectReason = "authentication failed";
                    _run = false;
                    return;
                }
                ConsoleHelper.WriteLine(_client.ToString() + " logged in!");
                SendPacket(new Packets.HandshakeResponsePacket(handshakePacket.UID, true));

                SendContactlist();
                SendOptions();

                _client.AddConnection(this);

                byte keepAliveCounter = 0;

                while(_run)
                {
                    _clientKeepAliveCounter++;

                    if (_clientKeepAliveCounter >= 100)
                    {
                        throw new ConnectionLostException("The client is not responding");
                    }

                    if(keepAliveCounter++ >= 50)
                    {
                        keepAliveCounter = 0;
                        EnqueuePacket(new Packets.KeepAlivePacket());
                    }

                    while(_stream.DataAvailable)
                    {
                        ReceivePacket();
                    }

                    while(_receivePacketQueue.Count > 0)
                    {
                        IClientPacket incoming = _receivePacketQueue.Dequeue();

                        ProcessIncomingPacket(incoming);
                    }

                    while(_sendPacketQueue.Count > 0)
                    {
                        IServerPacket outgoing = SendPacket(_sendPacketQueue.Dequeue());

                        ProcessOutgoingPacket(outgoing);
                    }

                    Thread.Sleep(100);
                }
            }
            catch(Exception x)
            {
                Console.WriteLine(x);
                _disconnectReason = x.GetType().FullName;
            }
            finally
            {
                OnDisconnect(_disconnectReason);
            }
        }

        private void ProcessOutgoingPacket(IServerPacket outgoing)
        {
            switch(outgoing.Type)
            {
                case KnownPackets.Exit:
                    _run = false;
                    break;
            }
        }
        
        private void ProcessIncomingPacket(IClientPacket incoming)
        {
            _clientKeepAliveCounter = 0;
            switch(incoming.Type)
            {
                case KnownPackets.GetAccountInformation:
                    EnqueuePacket(new Packets.GetAccountInformationResponsePacket(incoming.UID, _client.Username, _client.ID));
                    break;
                case KnownPackets.SendMessage:
                    Packets.SendMessagePacket smp = (Packets.SendMessagePacket)incoming;
                    _client.SendMessage(this, smp);
                    //Client.Contacts.Single((x) => { return x.ID == smp.Receiver; }).EnqueueMessagePacket(smp);
                    break;
                case KnownPackets.SearchUser:
                    string user = ((Packets.SearchUserPacket)incoming).Username;
                    EnqueuePacket(new Packets.SearchUserResponsePacket(Clients.Client.SearchClients(user), incoming.UID));
                    break;
                case KnownPackets.SendContactRequest:
                    uint userID = ((Packets.SendContactRequestPacket)incoming).UserID;
                    Clients.Client.GetClient(userID).AddContactRequest(_client.ID);
                    _client.AddOutstandingContactRequest(userID);
                    _client.SendOutstandingContactRequests();
                    break;
                case KnownPackets.AcceptContactRequest:
                    Packets.AcceptContactRequestPacket acrp = (Packets.AcceptContactRequestPacket)incoming;
                    if (acrp.Accept)
                        _client.AcceptContactRequest(acrp.UserID);
                    else
                        _client.DeclineContactRequest(acrp.UserID);
                    Client.GetClient(acrp.UserID).RemoveOutstandingContactRequest(_client.ID);
                    break;
                case KnownPackets.Option:
                    Packets.OptionPacket op = (Packets.OptionPacket)incoming;
                    _client.Options.Update(op.Version, op.Data);
                    OnOptionsUpdated();
                    break;
                case KnownPackets.Exit:
                    _disconnectReason = ((Packets.ExitPacket)incoming).Reason;
                    _run = false;
                    break;
                //case KnownPackets.P2PRequest:
                //   Packets.P2P.P2PRequestPacket p2prp = (Packets.P2P.P2PRequestPacket)incoming;
                //   Clients.Client.GetClient(p2prp.Partner).

            }
        }

        public void SendContactlist()
        {
            EnqueuePacket(new Packets.ContactListPacket(
                _client.Contacts.Select<Client, Packets.ContactListPacket.ContactInformation>(
                    (c) =>
                    {
                        return new Packets.ContactListPacket.ContactInformation()
                        {
                            ID = c.ID,
                            Name = c.Username
                        };
                    }
                ).ToArray()
            ));
            foreach(Client c in _client.Contacts)
                if(c.Online)
                    EnqueuePacket(new Packets.OnlineStatusChangedPacket(c.ID, true));

        }

        public void SendOptions()
        {
            EnqueuePacket(new Packets.OptionPacket(_client.Options.Version, _client.Options.Data));
        }

        private IClientPacket ReceivePacket()
        {
            IClientPacket p = PacketHandler.ReadPacket<IClientPacket>(_stream);
            _receivePacketQueue.Enqueue(p);
            return p;
        }

        private void OnDisconnect(string reason)
        {
            this._run = false;
            this._connected = false;

            if(Disconnect != null)
                Disconnect.Invoke(this, reason);
        }

        public delegate void DisconnectHandler(Connection sender, string reason);
        public event DisconnectHandler Disconnect;

        private void OnOptionsUpdated()
        {
            if (OptionsUpdated != null)
                OptionsUpdated(this);
        }

        public delegate void OptionsUpdatedHandler(Connection sender);
        public event OptionsUpdatedHandler OptionsUpdated;

        private T WaitForPacket<T>() where T: IClientPacket
        {
            while(!_stream.DataAvailable)
                Thread.Sleep(3);
            IPacket p = PacketHandler.ReadPacket<IClientPacket>(_stream);
            if(!typeof(T).IsAssignableFrom(p.GetType()))
                throw new UnexpectedPacketException("Expect " + typeof(T).Name + " but got " + p.ToString());
            return (T)p;
        }

        private T WaitForResponseOrEnqueue<T>(uint uid) where T : IClientResponsePacket
        {
            while(!_stream.DataAvailable)
                Thread.Sleep(3);

            IClientPacket p = PacketHandler.ReadPacket<IClientPacket>(_stream);
            while(!p.IsResponse || p.UID != uid)
                _receivePacketQueue.Enqueue(p);

            if(!typeof(T).IsAssignableFrom(p.GetType()))
                throw new UnexpectedPacketException("Got response but expected " + typeof(T).ToString() + " and got " + p.ToString());
            return (T)p;
        }

        public void StartDisconnect()
        {
            StartDisconnect("disconnect");
        }

        public void StartDisconnect(string reason)
        {
            EnqueuePacket(new Packets.ExitPacket(reason));
        }

        private IServerPacket SendPacket(IServerPacket packet)
        {
            try
            {
                packet.Send(_stream);
                _stream.Flush();
            }
            catch(System.IO.IOException)
            {
                _run = false;
            }
            return packet;
        }

        public IServerPacket EnqueuePacket(IServerPacket packet)
        {
            _sendPacketQueue.Enqueue(packet);
            return packet;
        }

        public override string ToString()
        {
            return _endPointString + " (" + (_client != null ? _client.ToString() : "Unauthenticated") + ")";
        }
    }
}
