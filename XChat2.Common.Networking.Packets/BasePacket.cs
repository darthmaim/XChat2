using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Common.Networking.Packets
{
    public abstract class BasePacket : IPacket
    {
        private static uint _nextUID = 1;

        public BasePacket(byte id)
        {
            this._id = id;
            this._uid = _nextUID++;
        }

        public BasePacket(byte id, uint uid)
        {
            this._id = id;
            this._uid = uid;

            if(_nextUID <= uid)
                _nextUID = uid + 1;
        }

        public BasePacket(KnownPackets packetID) : this((byte)packetID) { }
        public BasePacket(KnownPackets packetID, uint uid) : this((byte)packetID, uid) { }

        public KnownPackets Type
        {
            get { return (KnownPackets)_id; }
        }

        public bool IsResponse
        {
            get
            {
                return typeof(IServerResponsePacket).IsAssignableFrom(this.GetType()) || typeof(IClientResponsePacket).IsAssignableFrom(this.GetType());
            }
        }

        public override string ToString()
        {
            return "[" + this.GetType().Name + " (0x" + _id.ToString("x2") + ")]";
        }

        protected readonly byte _id;
        /// <summary>
        /// The ID of this packet. See KnownPackets for all known packet ID's.
        /// </summary>
        public byte ID
        {
            get { return _id; }
        }

        protected readonly uint _uid;
        /// <summary>
        /// The unique identifier of this packet.
        /// </summary>
        public uint UID
        {
            get { return _uid; }
        }

        public abstract void Send(System.Net.Sockets.NetworkStream stream);

        internal void SendBase(System.Net.Sockets.NetworkStream stream)
        {
            StreamHelper.WriteByte(stream, this.ID);
            StreamHelper.WriteUInt(stream, this.UID);
        }
    }
}
