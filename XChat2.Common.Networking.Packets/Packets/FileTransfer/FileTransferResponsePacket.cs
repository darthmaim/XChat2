using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets.FileTransfer
{
    public class FileTransferResponsePacket : BasePacket, IClientResponsePacketFor<FileTransferRequestPacket>, IServerResponsePacketFor<FileTransferRequestPacket>
    {
        private bool _accepted;
        public bool Accepted
        {
            get { return _accepted; }
            set { _accepted = value; }
        }

        private uint _fileTransferID;
        public uint FileTransferID
        {
            get { return _fileTransferID; }
            set { _fileTransferID = value; }
        }
        
        //========================================================================
        public FileTransferResponsePacket(bool accepted, uint fileTransferID)
            : base(KnownPackets.FileTransferResponse)
        {
            _accepted = accepted;
            _fileTransferID = fileTransferID;
        }

        public FileTransferResponsePacket(NetworkStream stream, uint uid)
            : base(KnownPackets.FileTransferResponse, uid)
        {
            _fileTransferID = StreamHelper.ReadUInt(stream);
            _accepted = StreamHelper.ReadBoolean(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteUInt(stream, _fileTransferID);
            StreamHelper.WriteBoolean(stream, _accepted);
        }
    }
}
