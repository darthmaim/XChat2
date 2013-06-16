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

    public class ChatBoxChatEntry : IChatBoxEntry
    {
        internal static Font usernameFont = new Font("Tahoma", 8.5f, FontStyle.Bold);
        internal static Font messageFont = new Font("Tahoma", 8.5f, FontStyle.Regular);
        internal static int lineHeight = TextRenderer.MeasureText("a", messageFont).Height;

        private int _index;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
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

        private Color _usernameColor;
        public Color UsernameColor
        {
            get { return _usernameColor; }
            set { _usernameColor = value; }
        }

        private Size _usernameSize;
        public Size UsernameSize
        {
            get { return _usernameSize; }
            private set { _usernameSize = value; }
        }

        public string Text { get { return _message.Text; } }

        private Size _messageSize;
        public Size MessageSize
        {
            get { return _messageSize; }
            private set { _messageSize = value; }
        }

        private string[] _lines;
        public string[] Lines
        {
            get { return _lines; }
            private set { _lines = value; }
        }

        public int Height
        {
            get { return _lines.Length * lineHeight; }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (!_isSelected)
                    _selectionStart = _selectionEnd = 0;
            }
        }

        private int _selectionStart = -1;
        public int SelectionStart
        {
            get { return _selectionStart; }
            set
            {
                _selectionStart = value;
                if (_selectionStart > _selectionEnd)
                {
                    int temp = _selectionStart;
                    _selectionStart = _selectionEnd;
                    _selectionEnd = temp;
                }
            }
        }

        private int _selectionEnd = -1;
        public int SelectionEnd
        {
            get { return _selectionEnd; }
            set
            {
                _selectionEnd = value;
                if (_selectionStart > _selectionEnd)
                {
                    int temp = _selectionStart;
                    _selectionStart = _selectionEnd;
                    _selectionEnd = temp;
                }
            }
        }

        private TextChatMessage _message;
        public TextChatMessage Message
        {
            get { return _message; }
            set { _message = value; }
        }
        


        public string SelectedText
        {
            get { return _isSelected ? Text.Substring(_selectionStart, _selectionEnd - _selectionStart + 1) : ""; }
        }

        public ChatBoxChatEntry(TextChatMessage tcm, Color usernameColor)
        {
            this._usernameColor = usernameColor;
            this._message = tcm;
        }

        public void RecalculateSize(int width)
        {
            _usernameSize = TextRenderer.MeasureText(Username, usernameFont);
            _messageSize = TextRenderer.MeasureText(Text, messageFont);

            _lines = Helper.wrapText(Text, width - _usernameSize.Width, width - 10, messageFont);
        }

        public void Draw(Graphics graphics, int top)
        {
            TextRenderer.DrawText(graphics, Username, usernameFont, new Point(0, top), _usernameColor);
            DrawMessageWrapped(graphics, top);
        }

        private void DrawMessageWrapped(Graphics graphics, int top)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                TextRenderer.DrawText(graphics, _lines[i], messageFont, new Point(LeftMarginInLine(i), top + lineHeight * i), Color.Black);
            }
        }

        public int LeftMarginInLine(int line)
        {
            return line == 0 ? _usernameSize.Width : 10;
        }
    }

}
