using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace XChat2.Tests.P2P
{
    [TestClass]
    public class UdpClientTests
    {
        [TestMethod]
        public void TestLocalEndPoint()
        {
            UdpClient client = new UdpClient();
            client.Send(new byte[] { 0x00 }, 1, "127.0.0.1", 1234);
            EndPoint localEP = client.Client.LocalEndPoint;
            Assert.AreEqual<EndPoint>(localEP, client.Client.LocalEndPoint);
            client.Send(new byte[] { 0x00 }, 1, "localhost", 1234);
            Assert.AreEqual<EndPoint>(localEP, client.Client.LocalEndPoint);
            client.Send(new byte[] { 0x00 }, 1, "darthmaim.de", 1234);
            Assert.AreEqual<EndPoint>(localEP, client.Client.LocalEndPoint);
        }
    }
}
