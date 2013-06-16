using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets
{
    public interface IPacket
    {
        /// <summary>
        /// PacketID (same as <see cref="Type"/>; see also <seealso cref="XChat2.Common.Networking.KnownPackets"/>)
        /// </summary>
        byte ID { get; }
        /// <summary>
        /// Unique ID (responses have the same UID as the packets the refer to)
        /// </summary>
        uint UID { get; }

        /// <summary>
        /// PacketType (same as <see cref="ID"/>)
        /// </summary>
        KnownPackets Type { get; }

        /// <summary>
        /// True if this packet is a response (= the packet is implementing IServerResponse or IClientResponse)
        /// </summary>
        bool IsResponse { get; }

        void Send(NetworkStream stream);
    }

    public interface IServerPacket : IPacket
    {
    }

    public interface IServerResponsePacket : IServerPacket
    {
    }

    public interface IClientPacket : IPacket
    {
    }

    public interface IClientResponsePacket : IClientPacket
    {
    }

    public interface IServerResponsePacketFor<T> : IServerResponsePacket where T : IClientPacket
    {
    }

    public interface IClientResponsePacketFor<T> : IClientResponsePacket where T : IServerPacket
    {
    }
}
