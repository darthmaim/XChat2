using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Common.Networking.Packets;

namespace XChat2.Client.Communication.PacketHandlers
{
    public class ResponsePacketHandler<T> : IPacketHandler where T : IServerResponsePacket
    {
        private Action<IConnection, T> _action;
        public Action<IConnection, T> Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public Type PacketType
        {
            get { return typeof(T); }
        }
        
        public ResponsePacketHandler(Action<IConnection, T> action)
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
