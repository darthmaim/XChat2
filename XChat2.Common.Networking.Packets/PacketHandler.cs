using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Packets = XChat2.Common.Networking.Packets.Packets;
using XChat2.Common.Exceptions;

namespace XChat2.Common.Networking.Packets
{
    public class PacketHandler
    {
        public static T ReadPacket<T>(NetworkStream stream) where T: IPacket
        {
            byte _packetID = StreamHelper.ReadByte(stream);
            uint _uid = StreamHelper.ReadUInt(stream);

            IPacket incoming = null;

            switch(_packetID)
            {
                case (byte)KnownPackets.KeepAlive:
                    incoming = new Packets.KeepAlivePacket(stream, _uid);
                    break;
                case (byte)KnownPackets.Handshake:
                    incoming = new Packets.HandshakePacket(stream, _uid);
                    break;
                case (byte)KnownPackets.HandshakeResponse:
                    incoming = new Packets.HandshakeResponsePacket(stream, _uid);
                    break;
                case (byte)KnownPackets.GetAccountInformation:
                    incoming = new Packets.GetAccountInformationPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.GetAccountInformationResponse:
                    incoming = new Packets.GetAccountInformationResponsePacket(stream, _uid);
                    break;
                case (byte)KnownPackets.Exit:
                    incoming = new Packets.ExitPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.Option:
                    incoming = new Packets.OptionPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.ContactList:
                    incoming = new Packets.ContactListPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.OnlineStatusChanged:
                    incoming = new Packets.OnlineStatusChangedPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.SendMessage:
                    incoming = new Packets.SendMessagePacket(stream, _uid);
                    break;
                case (byte)KnownPackets.SearchUser:
                    incoming = new Packets.SearchUserPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.SearchUserResponse:
                    incoming = new Packets.SearchUserResponsePacket(stream, _uid);
                    break;
                case (byte)KnownPackets.SendContactRequest:
                    incoming = new Packets.SendContactRequestPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.IncomingContactRequest:
                    incoming = new Packets.IncomingContactRequestPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.AcceptContactRequest:
                    incoming = new Packets.AcceptContactRequestPacket(stream, _uid);
                    break;
                case (byte)KnownPackets.OutstandingContactRequest:
                    incoming = new Packets.OutstandingContactRequestPacket(stream, _uid);
                    break;
                default:
                    throw new UnknownPacketException("UnknownPacket 0x" + _packetID.ToString("X2"));
            }

            if(!typeof(T).IsAssignableFrom(incoming.GetType()))
                throw new UnexpectedPacketException("Expect packet of type " + typeof(T).Name + " but get " + incoming.GetType().Name);

            return (T)incoming;
        }
    }
}
