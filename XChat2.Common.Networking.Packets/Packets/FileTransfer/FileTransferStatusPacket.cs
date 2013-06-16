using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets.FileTransfer
{
    public class FileTransferStatusPacket : BasePacket, IServerPacket, IClientPacket
    {
        private uint _fileTransferID;
        public uint FileTransferID
        {
            get { return _fileTransferID; }
            set { _fileTransferID = value; }
        }
        
        private FileTranferStatus _status;
        public FileTranferStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        

        //========================================================================
        public FileTransferStatusPacket(FileTranferStatus status)
            : base(KnownPackets.FileTransferStatus)
        {
            _status = status;
        }

        public FileTransferStatusPacket(NetworkStream stream, uint uid)
            : base(KnownPackets.FileTransferStatus, uid)
        {
            _status = (FileTranferStatus)StreamHelper.ReadByte(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteByte(stream, (byte)_status);
        }

        public enum FileTranferStatus : byte
        {
            Unknown = 0,
            Accepted,
            Canceled,
            InProgress,
            Error
        }
    }
}
