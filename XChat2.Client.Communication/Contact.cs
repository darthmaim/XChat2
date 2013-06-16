using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Client.Communication
{
    public class Contact
    {
        private uint _id;
        public uint ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private bool _online;
        public bool Online
        {
            get { return _online; }
            set
            {
                _online = value;
                OnOnlineStateChanged(_online);
            }
        }

        private void OnOnlineStateChanged(bool _online)
        {
            if(OnlineStateChanged != null)
                OnlineStateChanged.Invoke(this, _online);
        }

        public delegate void OnlineStateChangedHandler(Contact contact, bool online);
        public event OnlineStateChangedHandler OnlineStateChanged;

        internal void OnMessageReceiving(ChatMessage message)
        {
            if(MessageReceived != null)
                MessageReceived.Invoke(this, message);
        }

        public delegate void MessageReceivedHandler(Contact contact, ChatMessage Message);
        public event MessageReceivedHandler MessageReceived;

        public Contact(uint id, string name)
        {
            _id = id;
            _name = name;
        }
    }
}
