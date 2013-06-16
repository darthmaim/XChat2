using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class GetAccountInformationPacket : BasePacket, IClientPacket
    {   
        //==========================================================================
        public GetAccountInformationPacket()
            : base(0x03) { }

        public GetAccountInformationPacket(NetworkStream stream, uint uid)
            : base(0x03, uid) { }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);
        }
    }
}
