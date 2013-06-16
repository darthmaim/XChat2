using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XChat2.Server.Clients;
using Exceptions = XChat2.Common.Exceptions;
using XChat2.Server.Database.Clients;

namespace XChat2.Tests.Server.Clients
{
    [TestClass]
    public class ClientTests
    {
        [TestInitialize]
        public void TestInit()
        {
            //Client.Init(null);
        }

        [TestMethod]
        public void TestClientRepo()
        {
            FakeClientDataSource fakeClientDataSource = new FakeClientDataSource();

            Assert.IsFalse(fakeClientDataSource.IsOpen);

            using (ClientRepository cr = new ClientRepository(fakeClientDataSource))
            {
                Assert.IsTrue(fakeClientDataSource.IsOpen);

                Assert.IsTrue(cr.Exists(1));
                Assert.IsTrue(cr.GetByID(2).UserID == 2);
                Assert.AreEqual<int>(3, cr.GetAll().Count());

                Assert.IsTrue(fakeClientDataSource.IsOpen);
            }
            Assert.IsFalse(fakeClientDataSource.IsOpen);
        }

        //[TestMethod]
        //public void TestGetClient1()
        //{
        //    Client origin = Client.AddClient("test", "123", 42);
        //    Client c = Client.GetClient("test");

        //    Assert.IsInstanceOfType(c, typeof(Client));
        //    Assert.AreSame(origin, c);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(Exceptions.UnknownUserException))]
        //public void TestGetClient2()
        //{
        //    Client.AddClient("test", "123", 42);
        //    Client.GetClient("test2");
        //}

        //[TestMethod]
        //public void TestGetClient3()
        //{
        //    Client origin = Client.AddClient("test", "123", 42);
        //    Client c = Client.GetClient(42);

        //    Assert.IsInstanceOfType(c, typeof(Client));
        //    Assert.AreSame(origin, c);
        //}


        //[TestMethod]
        //public void TestSearchClients()
        //{
        //    Client.AddClient("test", "123", 42);
        //    Client.AddClient("test2", "123", 43);
        //    Client.AddClient("a", "123");

        //    var clients = Client.SearchClients("test");

        //    Assert.IsTrue(clients.Count == 2);

        //    Assert.IsTrue(clients[42] == "test");
        //    Assert.IsTrue(clients[43] == "test2");
        //}

        //[TestMethod]
        //public void TestContacts()
        //{
        //    Client c1 = Client.AddClient("c1", "123");
        //    Client c2 = Client.AddClient("c2", "123");
        //    Client c3 = Client.AddClient("c3", "123");

        //    c1.AddContact(c2);
        //    c1.AddContact(c3);

        //    Assert.IsTrue(c1.Contacts.Contains(c2));
        //    Assert.IsTrue(c1.Contacts.Contains(c3));
        //    Assert.IsTrue(c2.Contacts.Contains(c1));
        //    Assert.IsFalse(c2.Contacts.Contains(c3));
        //    Assert.IsTrue(c3.Contacts.Contains(c1));
        //    Assert.IsFalse(c3.Contacts.Contains(c2));
        //}
    }
}
