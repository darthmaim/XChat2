using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using XChat2.Common.Networking.P2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace XChat2.Tests.P2P
{
    [TestClass]
    public class PacketTests
    {
        [TestMethod]
        public void TestAuthenticatedPacketIPv4()
        {
            IPAddress address = IPAddress.Parse("123.4.5.67");
            int port = 42;
            byte[] rawPacket = P2PPacketCreator.CreateAuthenticatedPacket(address, port);

            P2PPacketCreator.AuthenticatedPacket packet = P2PPacketCreator.ReadAuthenticatedPacket(rawPacket);
            Assert.AreEqual<IPAddress>(address, packet.IPAddress);
            Assert.AreEqual<int>(port, packet.Port);

        }

        [TestMethod]
        public void TestAuthenticatedPacketIPv6()
        {
            IPAddress address = IPAddress.Parse("2001:0db8:85a3:08d3:1319:8a2e:0370:7344");
            int port = 42;
            byte[] rawPacket = P2PPacketCreator.CreateAuthenticatedPacket(address, port);

            P2PPacketCreator.AuthenticatedPacket packet = P2PPacketCreator.ReadAuthenticatedPacket(rawPacket);
            Assert.AreEqual<IPAddress>(address, packet.IPAddress);
            Assert.AreEqual<int>(port, packet.Port);
        }
    }
}
