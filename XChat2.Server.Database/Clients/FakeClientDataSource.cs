using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using XChat2.Common.Hashes;

namespace XChat2.Server.Database.Clients
{
    public class FakeClientDataSource : IClientDataSource
    {
        Dictionary<uint, ClientInformation> _data = new Dictionary<uint, ClientInformation>()
            {
                { 1, new ClientInformation() { UserID = 1, Username = "TestClient", Password = HashGenerator.ByteArrayToHexString(HashGenerator.GetMD5Hash("123"))} },
                { 2, new ClientInformation() { UserID = 2, Username = "TestClient2", Password = HashGenerator.ByteArrayToHexString(HashGenerator.GetMD5Hash("123"))} },
                { 3, new ClientInformation() { UserID = 3, Username = "TestClient3", Password = HashGenerator.ByteArrayToHexString(HashGenerator.GetMD5Hash("123"))} },
            }; 

        private bool _open = false;
        public bool IsOpen
        {
            get { return _open; }
        }

        public void Open()
        {
            if (_open)
                Debug.WriteLine("Already opened");
            _open = true;
        }

        public void Close()
        {
            if (!_open)
                Debug.WriteLine("Already closed");
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
            return _data[id];
        }

        public IEnumerable<ClientInformation> GetAll()
        {
            return _data.Values.ToList();
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
        }

        public void AddContact(uint user1, uint user2)
        { 
        }

        public void RemoveContact(uint user1, uint user2)
        { 
        }

        public ClientInformation GetByUsername(string username)
        {
            return _data.First(kv => kv.Value.Username.ToLower() == username.ToLower()).Value;
        }

        public IEnumerable<ClientInformation> GetMultipleByUsername(string username)
        {
            return from c in _data where c.Value.Username.ToLower().Contains(username.ToLower()) select c.Value;
        }

        public bool Exists(string username)
        {
            return _data.Any(kv => kv.Value.Username.ToLower() == username.ToLower());
        }
    }
}
