using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace XChat2.Client.Controls.ChatBox
{
    public class ChatBoxPlainEntry : IChatBoxEntry
    {
        public static Font plainFont = new Font("Tahoma", 8.5f, FontStyle.Italic);

        private int _index;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private string[] _lines;
        public string[] Lines
        {
            get { return _lines; }
            private set { _lines = value; }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            private set { _text = value; }
        }

        public int Height
        {
            get { return _lines.Length * ChatBoxChatEntry.lineHeight; }
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

        private int _selectionStart = 0;
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


        private int _selectionEnd = 0;
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


        public string SelectedText
        {
            get { return _isSelected ? _text.Substring(_selectionStart, _selectionEnd - _selectionStart + 1) : ""; }
        }

        public ChatBoxPlainEntry(string text)
        {
            this._text = text;
        }

        public void RecalculateSize(int width)
        {
            _lines = Helper.wrapText(_text, width, width, plainFont);
        }

        public void Draw(Graphics g, int top)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                TextRenderer.DrawText(g, _lines[i], plainFont, new Point(0, top + ChatBoxChatEntry.lineHeight * i), Color.Gray);
            }
        }

        public int LeftMarginInLine(int line)
        {
            return 0;
        }
    }
}