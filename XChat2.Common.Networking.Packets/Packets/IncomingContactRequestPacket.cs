using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class IncomingContactRequestPacket : BasePacket, IServerPacket
    {
        private uint _userID;
        public uint UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        

        //==========================================================================
        public IncomingContactRequestPacket(uint userID, string username)
            : base(0x33)
        {
            _userID = userID;
            _username = username;
        }

        public IncomingContactRequestPacket(NetworkStream stream, uint uid)
            : base(0x33, uid)
        {
            _userID = StreamHelper.ReadUInt(stream);
            _username = StreamHelper.ReadString(stream);
        }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);
            StreamHelper.WriteUInt(stream, _userID);
            StreamHelper.WriteString(stream, _username);
        }
    }
}
