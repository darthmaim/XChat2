using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using XChat2.Client.Data;
using XChat2.Common.Helper;
using System.Windows.Forms;

namespace XChat2.Client.Controls.ChatBox
{
    public class ChatBoxFileEntry : IChatBoxEntry
    {
               private int _index;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public ChatBoxFileEntry(string filename, string username, Color usernameColor, DateTime time)
        {
            _username = username;
            _usernameColor = usernameColor;
            _filename = filename;
            _time = time;
        }

        public string Username
        {
            get
            {
                string time = Options.Instance.ShowTime ? "[" + DateTimeStringFormater.Format(Options.Instance.TimeFormat, _time) + "] " : "";
                return time + _username;
            }
        }

        private string _username;
        public string RawUsername
        {
            get { return _username; }
            set { _username = value; }
        }

        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }
        

        private string _filename;
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
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
            get { return ChatBoxChatEntry.lineHeight; }
        }
        
        public void RecalculateSize(int width)
        { }

        public void Draw(Graphics g, int top)
        {
            TextRenderer.DrawText(g, Username, ChatBoxChatEntry.usernameFont, new Point(0, top), _usernameColor);
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
