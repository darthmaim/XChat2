using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Client.Data.ChatMessages;

namespace XChat2.Client.Data
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

        private ChatHistory _history;
        public ChatHistory History
        {
            get { return _history; }
            set { _history = value; }
        }

        private void OnOnlineStateChanged(bool _online)
        {
            if(OnlineStateChanged != null)
                OnlineStateChanged.Invoke(this, _online);
        }

        public delegate void OnlineStateChangedHandler(Contact contact, bool online);
        public event OnlineStateChangedHandler OnlineStateChanged;

        public void OnMessageReceived(IChatMessage message)
        {
            if(MessageReceived != null)
                MessageReceived(this, message);
        }

        public delegate void MessageReceivedHandler(Contact contact, IChatMessage Message);
        public event MessageReceivedHandler MessageReceived;

        public Contact(uint id, string name)
        {
            _id = id;
            _name = name;
            _history = new ChatHistory(this);
        }

        public void OnIncomingFile(string filename, uint fileTransferID)
        {
            if (IncomingFile != null)
                IncomingFile(this, filename, fileTransferID);
        }

        public delegate void IncomingFileHandler(Contact contact, string filename, uint fileTransferID);
        public event IncomingFileHandler IncomingFile;
    }
}
