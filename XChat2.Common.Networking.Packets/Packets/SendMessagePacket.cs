using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Drawing;
using System.IO;

namespace XChat2.Common.Networking.Packets.Packets
{
    public class SendMessagePacket : BasePacket, IClientPacket, IServerPacket, IDisposable
    {
        private uint _receiver;
        public uint Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        private uint _sender;
        public uint Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private long _sendTime;
        public DateTime SendTime
        {
            get { return DateTime.FromBinary(_sendTime).ToLocalTime(); }
            set { _sendTime = value.ToUniversalTime().ToBinary(); }
        }

        //1: Text; 2: Image
        private byte _messageType;
        public byte RawMessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        public MessageTypes MessageType
        {
            get { return (MessageTypes)_messageType; }
            set { _messageType = (byte)value; }
        }

        public enum MessageTypes : byte
        {
            Text = 1, Image = 2
        }

        private byte[] _imageBuffer;
        public byte[] ImageBuffer
        {
            get { return _imageBuffer; }
            set { _imageBuffer = value; }
        }
        
        //==========================================================================
        public SendMessagePacket(uint receiver, uint sender, string message)
            : base(0x20)
        {
            _receiver = receiver;
            _sender = sender;
            MessageType = MessageTypes.Text;
            _message = message;
            _sendTime = DateTime.UtcNow.ToBinary();
        }

        public SendMessagePacket(uint receiver, uint sender, byte[] imageBuffer)
            : base(0x20)
        {
            _receiver = receiver;
            _sender = sender;
            MessageType = MessageTypes.Image;
            _imageBuffer = imageBuffer;
            _sendTime = DateTime.UtcNow.ToBinary();
        }

        public SendMessagePacket(NetworkStream stream, uint uid)
            : base(0x20, uid)
        {
            _receiver = StreamHelper.ReadUInt(stream);
            _sender = StreamHelper.ReadUInt(stream);
            _sendTime = StreamHelper.ReadLong(stream);
            _messageType = StreamHelper.ReadByte(stream);
            if(MessageType == MessageTypes.Text)
                _message = StreamHelper.ReadString(stream);
            else if (MessageType == MessageTypes.Image)
            {
                int bufferSize = StreamHelper.ReadInt(stream);
                _imageBuffer = StreamHelper.ReadBytes(stream, bufferSize);
            }
        }
        //==========================================================================

        public override void Send(NetworkStream stream)
        {
            base.SendBase(stream);

            StreamHelper.WriteUInt(stream, _receiver);
            StreamHelper.WriteUInt(stream, _sender);
            StreamHelper.WriteLong(stream, _sendTime);
            StreamHelper.WriteByte(stream, _messageType);
            if(MessageType == MessageTypes.Text)
                StreamHelper.WriteString(stream, _message);
            else if(MessageType == MessageTypes.Image)
            {
                StreamHelper.WriteInt(stream, _imageBuffer.Length);
                StreamHelper.WriteBytes(stream, _imageBuffer);
            }
        }

        ~SendMessagePacket()
        {
            Dispose();
        }

        public void Dispose()
        {
            //if(_image != null)
            //    _image.Dispose();
            //GC.SuppressFinalize(this);
        }
    }
}
