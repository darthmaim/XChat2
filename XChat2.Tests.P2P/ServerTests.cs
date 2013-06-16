using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XChat2.Common.Networking.P2P.Server;
using System.Net;
using XChat2.Common.Networking.P2P.Client;
using System.Threading;

namespace XChat2.Tests.P2P
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void TestRun()
        {
            using (HolePunchServer hpc = new HolePunchServer(IPAddress.Any, 1234))
            {
                hpc.RegisterClients(new byte[] { 0xA2 }, new byte[] { 0x42 });

                P2PConnection p2p1 = new P2PConnection("127.0.0.1", 1234, new byte[] { 0xA2 });
                P2PConnection p2p2 = new P2PConnection("127.0.0.1", 1234, new byte[] { 0x42 });

                p2p1.StartAsync();
                p2p2.Start();

                Assert.IsTrue(p2p1.Established);
                Assert.IsTrue(p2p2.Established);

                var data = new byte[] { 0x2A };

                p2p1.Send(data);
                Assert.IsTrue(p2p2.DataAvailable);

                var rec = p2p2.Read();
                Assert.AreEqual<int>(data.Length, rec.Length);
                for(int i = 0; i < data.Length; ++i)
                {
                    Assert.AreEqual<byte>(data[i], rec[i]);
                }
            }
        }
    }
}