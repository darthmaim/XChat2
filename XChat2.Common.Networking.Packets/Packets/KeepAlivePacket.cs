using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class KeepAlivePacket : BasePacket, IClientPacket, IServerPacket
    {
       
        //==========================================================================
        public KeepAlivePacket()
            : base(KnownPackets.KeepAlive, 0)
        { }

        public KeepAlivePacket(NetworkStream stream, uint uid)
            : base(KnownPackets.KeepAlive, uid)
        { }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);
        }
    }
}
