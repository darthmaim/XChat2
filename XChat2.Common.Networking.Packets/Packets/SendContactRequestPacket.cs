using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class SendContactRequestPacket : BasePacket, IClientPacket
    {
        private uint _userID;
        public uint UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        //==========================================================================
        public SendContactRequestPacket(uint userID)
            : base(0x32)
        {
            _userID = userID;
        }

        public SendContactRequestPacket(NetworkStream stream, uint uid)
            : base(0x32, uid)
        {
            _userID = StreamHelper.ReadUInt(stream);
        }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);
            StreamHelper.WriteUInt(stream, _userID);
        }
    }
}
