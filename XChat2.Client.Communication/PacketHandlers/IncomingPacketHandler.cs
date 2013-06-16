using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Common.Networking.Packets;

namespace XChat2.Client.Communication.PacketHandlers
{
    public class IncomingPacketHandler<T> : IPacketHandler where T: IServerPacket
    {
        private Action<IConnection, T> _action;
        public Action<IConnection, T> Action
        {
            get { return _action; }
            set { _action = value; }
        }


        public IncomingPacketHandler(Action<IConnection, T> action)
        {
            _action = action;
        }

        public void Invoke(IConnection connection, IPacket packet)
        {
            if (_action != null)
                _action(connection, (T)packet);
        }
    }
}
