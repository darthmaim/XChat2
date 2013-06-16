using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class OptionPacket : BasePacket, IServerPacket, IClientPacket
    {
        private int _version;
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }
        
        //========================================================================
        public OptionPacket(int version, byte[] data)
            : base(KnownPackets.Option)
        {
            _version = version;
            _data = data;
        }

        public OptionPacket(NetworkStream stream, uint uid)
            : base(KnownPackets.Option, uid)
        {
            _version = StreamHelper.ReadInt(stream);
            int length = StreamHelper.ReadInt(stream);
            _data = StreamHelper.ReadBytes(stream, length);
        }
        //========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteInt(stream, _version);
            StreamHelper.WriteInt(stream, _data.Length);
            StreamHelper.WriteBytes(stream, _data);
        }
    }
}
