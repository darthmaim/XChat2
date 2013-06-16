using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class SearchUserResponsePacket : BasePacket, IServerResponsePacketFor<SearchUserPacket>
    {
        private Dictionary<uint, string> _users;
        public Dictionary<uint, string> Users
        {
            get { return _users; }
            set { _users = value; }
        }

        private uint _responseTo;
        public uint ResponseTo
        {
            get { return _responseTo; }
            set { _responseTo = value; }
        }
        
        //==========================================================================
        public SearchUserResponsePacket(Dictionary<uint, string> users, uint responseTo)
            : base(0x31)
        {
            _users = users;
            _responseTo = responseTo;
        }

        public SearchUserResponsePacket(NetworkStream stream, uint uid)
            : base(0x31, uid)
        {
            _responseTo = StreamHelper.ReadUInt(stream);
            int count = StreamHelper.ReadInt(stream);
            _users = new Dictionary<uint,string>(count);
            for(int i = 0; i < count; i++)
            {
                uint key = StreamHelper.ReadUInt(stream);
                string val = StreamHelper.ReadString(stream);
                _users.Add(key, val);
            }
        }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);
            StreamHelper.WriteUInt(stream, _responseTo);
            StreamHelper.WriteInt(stream, _users.Count);

            for(int i = 0; i < _users.Count; i++)
            {
                StreamHelper.WriteUInt(stream, _users.Keys.ElementAt(i));
                StreamHelper.WriteString(stream, _users.Values.ElementAt(i));
            }
        }
    }
}
