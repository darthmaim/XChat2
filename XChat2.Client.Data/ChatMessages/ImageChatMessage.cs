using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace XChat2.Client.Data.ChatMessages
{
    public class ImageChatMessage : IChatMessage
    {
        public ImageChatMessage(byte[] image, string username, Contact contact, Directions direction, DateTime time)
        {
            _image = image;
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

        private byte[] _image;
        public byte[] ImageBuffer
        {
            get { return _image; }
            set { _image = value; }
        }
    }
}
