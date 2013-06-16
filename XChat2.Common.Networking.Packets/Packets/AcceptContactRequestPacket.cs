using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class AcceptContactRequestPacket : BasePacket, IClientResponsePacketFor<Packets.IncomingContactRequestPacket>
    {
        private uint _userID;
        public uint UserID
        {
            get { return _userID; }
        }

        private bool _accept;
        public bool Accept
        {
            get { return _accept; }
        }
        

        //==========================================================================
        public AcceptContactRequestPacket(uint userID, bool accept)
            : base(KnownPackets.AcceptContactRequest)
        {
            _userID = userID;
            _accept = accept;
        }

        public AcceptContactRequestPacket(NetworkStream stream, uint uid)
            : base(KnownPackets.AcceptContactRequest, uid)
        {
            _userID = StreamHelper.ReadUInt(stream);
            _accept = StreamHelper.ReadBoolean(stream);
        }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteUInt(stream, _userID);
            StreamHelper.WriteBoolean(stream, _accept);
        }
    }
}
