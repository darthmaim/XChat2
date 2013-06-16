using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class OnlineStatusChangedPacket : BasePacket, IServerPacket
    {
        private uint _clientID;
        public uint ClientID
        {
            get { return _clientID; }
            set { _clientID = value; }
        }

        private bool _online;
        public bool Online
        {
            get { return _online; }
            set { _online = value; }
        }
        
        

        //========================================================================
        public OnlineStatusChangedPacket(uint clientID, bool online)
            : base(0x11) 
        {
            _clientID = clientID;
            _online = online;
        }

        public OnlineStatusChangedPacket(NetworkStream stream, uint uid)
            : base(0x11, uid)
        {
            _clientID = StreamHelper.ReadUInt(stream);
            _online = StreamHelper.ReadBoolean(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteUInt(stream, _clientID);
            StreamHelper.WriteBoolean(stream, _online);
        }
    }
}
