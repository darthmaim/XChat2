using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class HandshakePacket : BasePacket, IClientPacket
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private byte[] _passwordHash;
        public byte[] PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = value; }
        }
        
        //==========================================================================
        public HandshakePacket(string username, byte[] passwordHash) : base(0x01) {
            _username = username;
            _passwordHash = passwordHash;
        }

        public HandshakePacket(NetworkStream stream, uint uid) : base(0x01, uid)
        {
            _username = StreamHelper.ReadString(stream);
            _passwordHash = StreamHelper.ReadBytes(stream, 16);
        }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteString(stream, _username);
            StreamHelper.WriteBytes(stream, _passwordHash);
        }
    }
}
