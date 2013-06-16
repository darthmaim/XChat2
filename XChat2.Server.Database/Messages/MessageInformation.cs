using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database.Messages
{
    public class MessageInformation
    {
        private uint _sender;
        public uint Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        private uint _receiver;
        public uint Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }

        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
