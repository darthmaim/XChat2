using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class OutstandingContactRequestPacket : BasePacket, IServerPacket
    {
        private uint _userID;
        public uint UserID
        {
            get { return _userID; }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
        }

        //========================================================================
        public OutstandingContactRequestPacket(uint userID, string username)
            : base(KnownPackets.OutstandingContactRequest)
        {
            _userID = userID;
            _username = username;
        }

        public OutstandingContactRequestPacket(NetworkStream stream, uint uid)
            : base(KnownPackets.OutstandingContactRequest, uid)
        {
            _userID = StreamHelper.ReadUInt(stream);
            _username = StreamHelper.ReadString(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteUInt(stream, _userID);
            StreamHelper.WriteString(stream, _username);
        }
    }
}
