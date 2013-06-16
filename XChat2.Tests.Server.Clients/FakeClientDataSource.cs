using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Server.Database;

namespace XChat2.Tests.Server.Clients
{
    class FakeClientDataSource : IClientDataSource
    {
        private bool _open = false;
        public bool IsOpen
        {
            get { return _open; }
        }

        public void Open()
        {
            if (_open)
                throw new Exception("Already opened");
            _open = true;
        }

        public void Close()
        {
            if (!_open)
                throw new Exception("Already closed");
            _open = false;
        }

        public bool Exists(uint id)
        {
            if (!_open)
                throw new Exception("Closed");
            return (id == 1 || id == 2 || id == 3);
        }

        public ClientInformation GetByID(uint id)
        {
            if (!_open)
                throw new Exception("Closed");

            if (!Exists(id))
                throw new Exception("The id " + id + " doesn't exists");

            if (id == 1)
                return new ClientInformation() { UserID = 1, Username = "TestClient" };
            else if (id == 2)
                return new ClientInformation() { UserID = 2, Username = "TestClient2" };
            else if (id == 3)
                return new ClientInformation() { UserID = 3, Username = "TestClient3" };
            else
                throw new Exception("Unknown");
        }

        public IEnumerable<ClientInformation> GetAll()
        {
            return new List<ClientInformation>() { 
                new ClientInformation() { UserID = 1, Username = "TestClient" },
                new ClientInformation() { UserID = 2, Username = "TestClient2" },
                new ClientInformation() { UserID = 3, Username = "TestClient3" }
            };
        }

        public IEnumerable<uint> GetContacts(uint id)
        {
            if (id == 1)
                return new List<uint>() { 2 };
            else if (id == 2)
                return new List<uint>() { 1 };
            else if (id == 3)
                return new List<uint>() { };
            else
                throw new Exception("Unknown");
        }


        public void Update(ClientInformation data)
        {
            return;
        }
    }
}
