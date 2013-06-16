using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database.Clients
{
    public class ClientRepository : BaseRepository<ClientInformation>, IDisposable
    {
        public ClientRepository(IClientDataSource dataSource)
            : base(dataSource)
        {
        }

        public ClientInformation GetByUsername(string username)
        {
            return ((IClientDataSource)DataSource).GetByUsername(username);
        }

        public bool Exists(string username)
        {
            return ((IClientDataSource)DataSource).Exists(username);
        }

        public IEnumerable<uint> GetContacts(uint userID)
        {
            return ((IClientDataSource)DataSource).GetContacts(userID);
        }

        public void AddContact(uint user1, uint user2)
        {
            ((IClientDataSource)DataSource).AddContact(user1, user2);
        }

        public void RemoveContact(uint user1, uint user2)
        {
            ((IClientDataSource)DataSource).RemoveContact(user1, user2);
        }
    }
}
