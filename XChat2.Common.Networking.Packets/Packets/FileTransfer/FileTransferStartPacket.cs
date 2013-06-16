using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets.FileTransfer
{
    public class FileTransferStartPacket : BasePacket, IServerPacket
    {
        private uint _fileTransferID;
        public uint FileTransferID
        {
            get { return _fileTransferID; }
            set { _fileTransferID = value; }
        }

        private byte[] _authKey;
        public byte[] AuthKey
        {
            get { return _authKey; }
            set { _authKey = value; }
        }

        //========================================================================
        public FileTransferStartPacket(uint fileTransferID, byte[] authKey)
            : base(KnownPackets.FileTransferStart)
        {
            _fileTransferID = fileTransferID;
            _authKey = authKey;
        }

        public FileTransferStartPacket(NetworkStream stream, uint uid)
            : base(KnownPackets.FileTransferStart, uid)
        {
            _fileTransferID = StreamHelper.ReadUInt(stream);
            _authKey = StreamHelper.ReadByteArray(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteUInt(stream, _fileTransferID);
            StreamHelper.WriteByteArray(stream, _authKey);
        }
    }
}
