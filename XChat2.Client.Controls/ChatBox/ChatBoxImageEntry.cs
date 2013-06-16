using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using XChat2.Client.Data.ChatMessages;
using XChat2.Client.Data;
using XChat2.Common.Helper;

namespace XChat2.Client.Controls.ChatBox
{
    public class ChatBoxImageEntry : IChatBoxEntry
    {
        private int _index;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public ChatBoxImageEntry(ImageChatMessage icm, Color usernameColor)
        {
            _usernameColor = usernameColor;
            _message = icm;
            _image = XChat2.Common.Compression.Imaging.ByteArrayToBitmap(icm.ImageBuffer);
        }

        public string Username
        {
            get
            {
                string username = _message.Direction == Directions.Incoming ? _message.Contact.Name : _message.Username;
                string time = Options.Instance.ShowTime ? "[" + DateTimeStringFormater.Format(Options.Instance.TimeFormat, _message.Time) + "] " : "";
                return time + username;
            }
        }

        private ImageChatMessage _message;
        public ImageChatMessage Message
        {
            get { return _message; }
            set { _message = value; }
        }
        

        private Color _usernameColor;
        public Color UsernameColor
        {
            get { return _usernameColor; }
            set { _usernameColor = value; }
        }

        public string Text
        {
            get { return ""; }
        }

        public string[] Lines
        {
            get { return new string[0]; }
        }


        public int Height
        {
            get { return ChatBoxChatEntry.lineHeight + _image.Height; }
        }

        private Image _image;
        public Image Image
        {
            get { return _image; }
        }


        public void RecalculateSize(int width)
        { }

        public void Draw(Graphics g, int top)
        {
            TextRenderer.DrawText(g, Username, ChatBoxChatEntry.usernameFont, new Point(0, top), _usernameColor);
            g.DrawImage(_image, 0, top + ChatBoxChatEntry.lineHeight);
        }

        public int LeftMarginInLine(int line)
        { return 0; }

        public bool IsSelected
        {
            get { return false; }
            set { }
        }

        public int SelectionStart
        {
            get { return -1; }
            set { }
        }

        public int SelectionEnd
        {
            get { return -1; }
            set { }
        }

        public string SelectedText
        {
            get { return ""; }
        }
    }

}
