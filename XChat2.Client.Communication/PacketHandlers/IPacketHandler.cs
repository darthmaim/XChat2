using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Common.Networking.Packets;

namespace XChat2.Client.Communication.PacketHandlers
{
    public interface IPacketHandler
    {
        void Invoke(IConnection connection, IPacket packet);
    }
}
