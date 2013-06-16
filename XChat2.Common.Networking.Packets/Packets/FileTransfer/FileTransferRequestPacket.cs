using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets.FileTransfer
{
    public class FileTransferRequestPacket : BasePacket, IClientPacket, IServerPacket
    {
        private uint _contact;
        public uint Contact
        {
            get { return _contact; }
            set { _contact = value; }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private uint _fileTransferID = 0;
        public uint FileTransferID
        {
            get { return _fileTransferID; }
            set { _fileTransferID = value; }
        }
        
        
        //========================================================================
        public FileTransferRequestPacket(uint contact, string filename)
            : base(KnownPackets.FileTransferRequest)
        {
            _contact = contact;
            _fileName = filename;
        }

        public FileTransferRequestPacket(NetworkStream stream, uint uid)
            : base(KnownPackets.FileTransferRequest, uid)
        {
            _contact = StreamHelper.ReadUInt(stream);
            _fileName = StreamHelper.ReadString(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteUInt(stream, _contact);
            StreamHelper.WriteString(stream, _fileName);
        }
    }
}
