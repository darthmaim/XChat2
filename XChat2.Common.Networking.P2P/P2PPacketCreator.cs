using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("XChat2.Tests.P2P")]
namespace XChat2.Common.Networking.P2P
{
    internal static class P2PPacketCreator
    {
        internal static byte[] CreateServerHandshake(byte[] serverAuthKey)
        {
            byte[] packet = new byte[serverAuthKey.Length + 1];
            Array.Copy(serverAuthKey, 0, packet, 1, serverAuthKey.Length);
            packet[0] = 0x01;
            return packet;
        }

        internal static byte[] CreateAuthenticationFailedPacket()
        {
            return new byte[] { (byte)KnownP2PPackets.AuthenticationFailed };
        }

        internal static byte[] CreateAuthenticatedPacket(IPAddress address, int port)
        {
            byte[] rawIPAddress = address.GetAddressBytes();
            byte[] packet = new byte[1 + rawIPAddress.Length + sizeof(int)];
            packet[0] = (byte)KnownP2PPackets.Authenticated;
            Array.Copy(rawIPAddress, 0, packet, 1, rawIPAddress.Length);
            Array.Copy(BitConverter.GetBytes(port), 0, packet, packet.Length - sizeof(int), sizeof(int));
            return packet;
        }

        internal static AuthenticatedPacket ReadAuthenticatedPacket(byte[] response)
        {
            AuthenticatedPacket packet = new AuthenticatedPacket() { PacketID = response[0] };
            if (packet.PacketID != (byte)KnownP2PPackets.Authenticated)
                throw new ArgumentException("response isn't a Authenticated-Packet");

            int sizeOfIPArray = response.Length - 1 - sizeof(int);
            packet.RawIPAddress = new byte[sizeOfIPArray];
            Array.Copy(response, 1, packet.RawIPAddress, 0, sizeOfIPArray);

            packet.Port = BitConverter.ToInt32(response, response.Length - sizeof(int));

            return packet;
        }

        internal struct AuthenticatedPacket
        {
            private byte _packetID;
            public byte PacketID
            {
                get { return _packetID; }
                set { _packetID = value; }
            }

            private byte[] _ipAddress;
            public byte[] RawIPAddress
            {
                get { return _ipAddress; }
                set { _ipAddress = value; }
            }

            public IPAddress IPAddress
            {
                get { return new IPAddress(RawIPAddress); }
            }

            private int _port;
            public int Port
            {
                get { return _port; }
                set { _port = value; }
            }

            private bool _isHolePuncher;
            public bool IsHolepuncher
            {
                get { return _isHolePuncher; }
                set { _isHolePuncher = value; }
            }
        }

        internal static byte[] CreateStartPacket()
        {
            return new byte[] { (byte)KnownP2PPackets.DataStart };
        }
    }
}
