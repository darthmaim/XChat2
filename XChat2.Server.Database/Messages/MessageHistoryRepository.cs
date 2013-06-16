using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database.Messages
{
    class MessageHistoryRepository : BaseRepository<MessageInformation>
    {
        public MessageHistoryRepository(IMessageHistoryDataSource dataSource)
            : base(dataSource)
        {
        }
    }
}
