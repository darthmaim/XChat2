using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Common.Networking.Packets;
using XChat2.Client.Data;
using XChat2.Client.Communication.PacketHandlers;

namespace XChat2.Client.Communication
{
    public interface IConnection
    {
        void EnqueuePacket(IClientPacket packet);

        void RegisterIncomingPacketHandler<T>(IncomingPacketHandler<T> handler) where T : IServerPacket;
        void RegisterForResponsePacket<T>(T packet, ResponsePacketHandler<IServerResponsePacketFor<T>> handler) where T : IClientPacket;

        void SendMessage(Contact receiver, string m);
        void SendMessage(Contact receiver, byte[] imageBuffer);
    }
}
