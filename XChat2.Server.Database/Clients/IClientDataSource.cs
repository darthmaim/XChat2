using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database.Clients
{
    public interface IClientDataSource : IDataSource<ClientInformation>
    {
        IEnumerable<uint> GetContacts(uint id);

        bool Exists(string username);

        void AddContact(uint user1, uint user2);
        void RemoveContact(uint user1, uint user2);

        ClientInformation GetByUsername(string username);
        IEnumerable<ClientInformation> GetMultipleByUsername(string username);
    }
}
