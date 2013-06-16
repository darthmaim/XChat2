using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class SearchUserPacket : BasePacket, IClientPacket
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        //==========================================================================
        public SearchUserPacket(string username)
            : base(0x30)
        {
            _username = username;
        }

        public SearchUserPacket(NetworkStream stream, uint uid)
            : base(0x30, uid)
        {
            _username = StreamHelper.ReadString(stream);
        }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteString(stream, _username);
        }
    }
}
