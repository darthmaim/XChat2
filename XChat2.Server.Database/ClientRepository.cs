using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database
{
    public class ClientRepository : BaseRepository<ClientInformation>, IDisposable
    {
        public ClientRepository(IClientDataSource dataSource)
            : base(dataSource)
        {
        }

        public IEnumerable<uint> GetContacts(uint userID)
        {
            return ((IClientDataSource)DataSource).GetContacts(userID);
        }
    }
}
