using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class HandshakeResponsePacket : BasePacket, IServerResponsePacketFor<Packets.HandshakePacket>
    {
        private bool _success;

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }
        
        //========================================================================
        public HandshakeResponsePacket(uint uid, bool success) : base(0x02, uid) {
            _success = success;
        }

        public HandshakeResponsePacket(NetworkStream stream, uint uid)
            : base(0x02, uid)
        {
            _success = StreamHelper.ReadBoolean(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteBoolean(stream, _success);
        }
    }
}
