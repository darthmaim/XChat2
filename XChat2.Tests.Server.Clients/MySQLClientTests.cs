using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XChat2.Server.Database.Clients;

namespace XChat2.Tests.Server.Clients
{
    [TestClass]
    public class MySQLClientTests
    {
        private MySQLClientDataSource _mysql;

        [TestInitialize]
        public void Init()
        {
            _mysql = new MySQLClientDataSource("127.0.0.1", "xchat2", "xchat2", "rcqhXrEx3S2KRUJh");
        }

        [TestMethod]
        public void TestGetAll()
        {
            using (ClientRepository cr = new ClientRepository(_mysql))
            {
                Assert.AreEqual<int>(3, cr.GetAll().Count());
            }
        }

        [TestMethod]
        public void TestGetByID()
        {
            using (ClientRepository cr = new ClientRepository(_mysql))
            {
                Assert.IsTrue(cr.GetByID(1).UserID == 1);
                Assert.IsTrue(cr.GetByID(2).UserID == 2);
            }
        }

        [TestMethod]
        public void TestExists()
        {
            using (ClientRepository cr = new ClientRepository(_mysql))
            {
                Assert.IsTrue(cr.Exists(1));
                Assert.IsFalse(cr.Exists(42));
            }
        }

        [TestMethod]
        public void TestGetContacts()
        {
            using (ClientRepository cr = new ClientRepository(_mysql))
            {
                List<ClientInformation> contacts = new List<ClientInformation>();
                foreach (uint c in cr.GetContacts(1).ToList())
                    contacts.Add(cr.GetByID(c));
                Assert.AreEqual<int>(1, contacts.Count);
            }
        }

        [TestMethod]
        public void TestAddRemoveContacts()
        {
            using (ClientRepository cr = new ClientRepository(_mysql))
            {
                Assert.IsFalse(cr.GetContacts(1).Contains<uint>(2), "User 1 has user 2 as contact");
                cr.AddContact(1, 2);
                Assert.IsTrue(cr.GetContacts(1).Contains<uint>(2), "Couldn't add user 2 as contact to user 1");
                cr.RemoveContact(1, 2);
                Assert.IsFalse(cr.GetContacts(1).Contains<uint>(2), "Couldn't remove user 2 as contact from user 1");
            }
        }
    }
}
