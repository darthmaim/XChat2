using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database
{
    public interface IClientDataSource : IDataSource<ClientInformation>
    {
        IEnumerable<uint> GetContacts(uint id);
    }
}
