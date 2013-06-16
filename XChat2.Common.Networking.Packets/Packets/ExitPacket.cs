using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class ExitPacket : BasePacket, IServerPacket, IClientPacket
    {
        private string _reason;
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        //========================================================================
        public ExitPacket(string reason)
            : base(KnownPackets.Exit)
        {
            _reason = reason;
        }

        public ExitPacket(NetworkStream stream, uint uid)
            : base(KnownPackets.Exit, uid)
        {
            _reason = StreamHelper.ReadString(stream);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteString(stream, _reason);
        }
    }
}
