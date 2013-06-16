using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Client.Data.ChatMessages
{
    public class TextChatMessage : IChatMessage
    {
        public TextChatMessage(string text, string username, Contact contact, Directions direction, DateTime time)
        {
            _text = text;
            _contact = contact;
            _direction = direction;
            _time = time;
            _username = username;
        }

        #region IChatMessage
        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private Contact _contact;
        public Contact Contact
        {
            get { return _contact; }
            set { _contact = value; }
        }

        private Directions _direction;
        public Directions Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        #endregion

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}
