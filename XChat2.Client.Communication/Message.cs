using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace XChat2.Client.Communication
{
    public class ChatMessage
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private byte[] _imageBuffer;
        public byte[] ImageBuffer
        {
            get { return _imageBuffer; }
            set { _imageBuffer = value; }
        }        

        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private Contact _from;
        public Contact From
        {
            get { return _from; }
            set { _from = value; }
        }

        public enum Types { Text, Image }
        private Types _type;
        public Types Type
        {
            get { return _type; }
            set { _type = value; }
        }
        
        
        public ChatMessage(Common.Networking.Packets.Packets.SendMessagePacket smp, Contact from)
        {
            if(smp.MessageType == 0)
                _text = smp.Message;
            else
            {
                _imageBuffer = smp.ImageBuffer;
            }
            _time = smp.SendTime;
            _from = from;
            _type = smp.MessageType == 0 ? Types.Text : Types.Image;
        }
    }
}
