using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XChat2.Client.Controls
{
    public class ChatBox : ScrollableControl
    {
        public interface ChatBoxEntry
        {
            int Index { get; set; }

            string Text { get; }
            string[] Lines { get; }
            int Height { get; }
            void RecalculateSize(int width);
            void Draw(Graphics g, int top);

            int LeftMarginInLine(int line);

            bool IsSelected { get; set; }
            int SelectionStart { get; set; }
            int SelectionEnd { get; set; }
            string SelectedText { get; }
        }

        public class ChatBoxChatEntry : ChatBoxEntry
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

            private string _username;
            public string Username
            {
                get { return _username; }
                set { _username = value; }
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
            
            private string _message;
            public string Message
            {
                get { return _message; }
                set { _message = value; }
            }

            public string Text { get { return _message; } }

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
                    if(!_isSelected)
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
                    if(_selectionStart > _selectionEnd)
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
                    _selectionEnd = value; if(_selectionStart > _selectionEnd)
                    {
                        int temp = _selectionStart;
                        _selectionStart = _selectionEnd;
                        _selectionEnd = temp;
                    }
                }
            }
        

            public string SelectedText
            {
                get { return _isSelected ? _message.Substring(_selectionStart, _selectionEnd - _selectionStart + 1) : ""; }
            }

            public ChatBoxChatEntry(string username, Color usernameColor, string Message)
            {
                this._username = username;
                this._usernameColor = usernameColor;
                this._message = Message;
            }

            public void RecalculateSize(int width)
            {
                _usernameSize = TextRenderer.MeasureText(_username, usernameFont);
                _messageSize = TextRenderer.MeasureText(_message, messageFont);

                _lines = Helper.wrapText(_message, width - _usernameSize.Width, width - 10, messageFont);
            }

            public void Draw(Graphics graphics, int top)
            {
                TextRenderer.DrawText(graphics, _username, usernameFont, new Point(0, top), _usernameColor);
                DrawMessageWrapped(graphics, top);
            }

            private void DrawMessageWrapped(Graphics graphics, int top)
            {
                for(int i = 0; i < _lines.Length; i++)
                {
                    TextRenderer.DrawText(graphics, _lines[i], messageFont, new Point(LeftMarginInLine(i), top + lineHeight * i), Color.Black);
                }
            }

            public int LeftMarginInLine(int line)
            {
                return line == 0 ? _usernameSize.Width : 10;
            }
        }

        public class ChatBoxPlainEntry : ChatBoxEntry
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
                    if(!_isSelected) 
                        _selectionStart = _selectionEnd = 0;
                }
            }

            private int _selectionStart = 0;
            public int SelectionStart
            {
                get { return _selectionStart; }
                set
                {
                    _selectionStart = value; if(_selectionStart > _selectionEnd)
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
                    _selectionEnd = value; if(_selectionStart > _selectionEnd)
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
                for(int i = 0; i < _lines.Length; i++)
                {
                    TextRenderer.DrawText(g, _lines[i], plainFont, new Point(0, top + ChatBoxChatEntry.lineHeight * i), Color.Gray);
                }
            }

            public int LeftMarginInLine(int line)
            {
                return 0;
            }
        }

        public class ChatBoxImageEntry : ChatBoxEntry
        {
            private int _index;
            public int Index
            {
                get { return _index; }
                set { _index = value; }
            }

            public ChatBoxImageEntry(string username, Color usernameColor, Image image)
            {
                _username = username;
                _usernameColor = usernameColor;
                _image = image;
            }

            private string _username;
            public string Username
            {
                get { return _username; }
                set { _username = value; }
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
                set { _image = value; }
            }
            

            public void RecalculateSize(int width)
            { }

            public void Draw(Graphics g, int top)
            {
                TextRenderer.DrawText(g, _username, ChatBoxChatEntry.usernameFont, new Point(0, top), _usernameColor);
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

        private List<ChatBoxEntry> _chatBoxEntries;
        
        private static Color _entryHoverColor = Color.FromArgb(0xEE, 0xEE, 0xFF);
        private static Color _entryHoverEndColor = Color.FromArgb(0xE0, 0xE0, 0xFF);
        private static Color _lineHoverColor = Color.FromArgb(0xCC, 0xCC, 0xFF);
        private static Color _lineHoverEndColor = Color.FromArgb(0x99, 0x99, 0xFF);

        public ChatBox()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            _chatBoxEntries = new List<ChatBoxEntry>();
            this.MouseLeave += new EventHandler(ChatBox_MouseLeave);
            this.Paint += new PaintEventHandler(ChatBox_Paint);
            this.Resize += new EventHandler(ChatBox_Resize);
            this.MouseMove += new MouseEventHandler(ChatBox_MouseMove);
            this.MouseDown += new MouseEventHandler(ChatBox_MouseDown);
            this.MouseUp += new MouseEventHandler(ChatBox_MouseUp);
            this.KeyUp += (o, e) => {
                if(e.Control && e.KeyCode == Keys.C) 
                    Clipboard.SetText(this.GetSelection()); 
            };
            this.Scroll += (o2, e2) => this.Invalidate();
            this.AutoScroll = true;
            this.MinimumSize = new Size(100, 100);
        }

        private bool _selecting = false;
        private int _selectionStartEntry = -1;
        private int _selectionEndEntry = -1;
        void ChatBox_MouseDown(object sender, MouseEventArgs e)
        {
            _chatBoxEntries.ForEach(c => c.IsSelected = false);
            if(_hoveredChatBoxEntry != null && _hoveredLine != -1 && _hoveredChar != -1)
            {
                _selectionStartEntry = _hoveredChatBoxEntry.Index;
                _selectionEndEntry = _hoveredChatBoxEntry.Index;

                _selecting = true;

                int selStart = 0;
                for(int i = 0; i < _hoveredLine; i++)
                    selStart += _hoveredChatBoxEntry.Lines[i].Length;
                selStart += _hoveredChar;

                _hoveredChatBoxEntry.IsSelected = true;
                _hoveredChatBoxEntry.SelectionStart = selStart;
                _hoveredChatBoxEntry.SelectionEnd = selStart + 1;
            }
        }

        void ChatBox_MouseUp(object sender, MouseEventArgs e)
        {
            _selecting = false;

            //if(_chatBoxEntries.Exists(c => c.SelectionStart == c.SelectionEnd && c.IsSelected))
            //{
            //    _chatBoxEntries.ForEach(c => c.IsSelected = false);
            //}
        }

        void ChatBox_MouseLeave(object sender, EventArgs e)
        {
            if(_hoveredChatBoxEntry != null)
            {
                ChatBoxEntry cbe = _hoveredChatBoxEntry;
                _hoveredChatBoxEntry = null;
                Invalidate(GetRectangleOfChatBoxEntry(cbe));
            }
        }

        private Point _mouseLocation = new Point(-1, -1);
        private ChatBoxEntry _hoveredChatBoxEntry = null;
        private int _hoveredLine = -1;
        private int _hoveredChar = -1;
        void ChatBox_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseLocation = e.Location;
            ChatBoxEntry oldHovered = _hoveredChatBoxEntry;
            _hoveredChatBoxEntry = GetChatBoxEntryAtPoint(e.Location);

            if(_hoveredChatBoxEntry != null)
            {
                if(_hoveredChatBoxEntry is ChatBoxImageEntry)
                {
                    _hoveredLine = -1;
                    _hoveredChar = -1;
                }
                else
                {
                    Rectangle r = GetRectangleOfChatBoxEntry(_hoveredChatBoxEntry);
                    int top = e.Location.Y - r.Top;
                    _hoveredLine = top / ChatBoxChatEntry.lineHeight;

                    _hoveredChar = Helper.GetHoveredChar(_hoveredChatBoxEntry.Lines[_hoveredLine], e.Location.X - _hoveredChatBoxEntry.LeftMarginInLine(_hoveredLine), ChatBoxChatEntry.messageFont);
                }
            }

            if(_selecting && _hoveredChatBoxEntry != null && _hoveredLine != -1 && _hoveredChar != -1)
            {
                _hoveredChatBoxEntry.IsSelected = true;

                int selectedCharIndex = 0;
                for(int i = 0; i < _hoveredLine; i++)
                    selectedCharIndex += _hoveredChatBoxEntry.Lines[i].Length;
                selectedCharIndex += _hoveredChar;

                if(oldHovered != null && oldHovered.IsSelected && oldHovered.Index < _hoveredChatBoxEntry.Index)
                {
                    oldHovered.SelectionEnd = oldHovered.Text.Length - 1;
                    _hoveredChatBoxEntry.SelectionStart = 5;
                    _hoveredChatBoxEntry.SelectionEnd = selectedCharIndex;
                }
                else if(oldHovered != null && oldHovered.IsSelected && oldHovered.Index > _hoveredChatBoxEntry.Index)
                {
                    oldHovered.IsSelected = false;
                    _hoveredChatBoxEntry.SelectionEnd = selectedCharIndex;
                }
                else
                {
                    _hoveredChatBoxEntry.SelectionEnd = selectedCharIndex;
                }

                if(_hoveredChatBoxEntry.SelectionStart < _hoveredChatBoxEntry.SelectionEnd)
                {
                    int temp = _hoveredChatBoxEntry.SelectionStart;
                    _hoveredChatBoxEntry.SelectionStart = _hoveredChatBoxEntry.SelectionEnd;
                    _hoveredChatBoxEntry.SelectionEnd = temp;
                }
            }

            if(oldHovered != null)
                this.Invalidate(GetRectangleOfChatBoxEntry(oldHovered));
            if(_hoveredChatBoxEntry != null && _hoveredChatBoxEntry != oldHovered)
                this.Invalidate(GetRectangleOfChatBoxEntry(_hoveredChatBoxEntry));
            //this.Invalidate();
        }

        private Rectangle GetRectangleOfChatBoxEntry(ChatBoxEntry chatBoxEntry)
        {
            if(!_chatBoxEntries.Contains(chatBoxEntry))
                throw new ArgumentOutOfRangeException();
            int top = 0 + AutoScrollPosition.Y;
            for(int i = 0; i < _chatBoxEntries.Count; i++)
            {
                if(_chatBoxEntries[i] == chatBoxEntry)
                    return new Rectangle(0, top, this.Width, _chatBoxEntries[i].Height);
                top += _chatBoxEntries[i].Height;
            }
            throw new ArgumentOutOfRangeException();
        }

        public string GetSelection()
        {
            string selection = "";

            _chatBoxEntries.ForEach(c => selection += c.SelectedText + Environment.NewLine);

            return selection;
        }

        void ChatBox_Resize(object sender, EventArgs e)
        {
            _chatBoxEntries.ForEach(cbe => cbe.RecalculateSize(this.Width - (this.VScroll ? System.Windows.Forms.SystemInformation.VerticalScrollBarWidth : 0)));
            this.Invalidate();
        }

        void ChatBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height);
            //e.Graphics.DrawLine(Pens.LightGray, 100, 0, 100, this.Height);

            int top = 0 + AutoScrollPosition.Y;
            foreach(ChatBoxEntry cbe in _chatBoxEntries)
            {
                if(cbe == _hoveredChatBoxEntry)
                {
                    using( Brush b = new LinearGradientBrush(new Point(0, top), new Point(0, top + cbe.Height), _entryHoverColor, _entryHoverEndColor))
                       e.Graphics.FillRectangle(b, 0, top, this.Width, cbe.Height);
                    if(_hoveredLine != -1)
                        using(Brush b = new LinearGradientBrush(new Point(0, top + _hoveredLine * ChatBoxChatEntry.lineHeight), new Point(0, top + (_hoveredLine+1) * ChatBoxChatEntry.lineHeight), _lineHoverColor, _lineHoverEndColor))
                            e.Graphics.FillRectangle(b, 0, top + _hoveredLine * ChatBoxChatEntry.lineHeight, this.Width, ChatBoxChatEntry.lineHeight);
                    if(_hoveredChar != -1)
                    {
                        Rectangle r =  Helper.RectangeOfCharAtIndex(cbe.Lines[_hoveredLine], _hoveredChar, ChatBoxChatEntry.messageFont);
                        e.Graphics.FillRectangle(Brushes.White, new Rectangle(r.X + cbe.LeftMarginInLine(_hoveredLine), top + _hoveredLine * ChatBoxChatEntry.lineHeight, r.Width, ChatBoxChatEntry.lineHeight));
                    }   
                }

                if(cbe.IsSelected)
                {
                    int index = 0;

                    GraphicsPath gp = new GraphicsPath();
                    gp.StartFigure();
                    for(int i = 0; i < cbe.Lines.Length; i++)
                    {
                        bool isSelStartLine = cbe.SelectionStart >= index && cbe.SelectionStart < index + cbe.Lines[i].Length;
                        bool isSelEndLine = cbe.SelectionEnd > index && cbe.SelectionEnd <= index + cbe.Lines[i].Length;
                        bool isBetweenSelection = cbe.SelectionStart < index && cbe.SelectionEnd > index + cbe.Lines[i].Length;

                        if(isSelStartLine && isSelEndLine)
                        {
                            Rectangle r1 = Helper.RectangeOfCharAtIndex(cbe.Lines[i], cbe.SelectionStart - index, ChatBoxChatEntry.messageFont);
                            Rectangle r2 = Helper.RectangeOfCharAtIndex(cbe.Lines[i], cbe.SelectionEnd - index, ChatBoxChatEntry.messageFont);
                            gp.AddRectangle(new Rectangle(r1.X + cbe.LeftMarginInLine(i), top + ChatBoxChatEntry.lineHeight * i, (r2.X + r2.Width) - r1.X, ChatBoxChatEntry.lineHeight));
                            break;
                        }
                        if(isSelStartLine)
                        {
                            Rectangle r1 = Helper.RectangeOfCharAtIndex(cbe.Lines[i], cbe.SelectionStart - index, ChatBoxChatEntry.messageFont);
                            Rectangle r2 = Helper.RectangeOfCharAtIndex(cbe.Lines[i], cbe.Lines[i].Length - 1, ChatBoxChatEntry.messageFont);
                            gp.AddRectangle(new Rectangle(r1.X + cbe.LeftMarginInLine(i), top + ChatBoxChatEntry.lineHeight * i, r2.X + r2.Width, ChatBoxChatEntry.lineHeight));
                        }
                        else if(isBetweenSelection)
                        {
                            Rectangle r = Helper.RectangeOfCharAtIndex(cbe.Lines[i], cbe.Lines[i].Length, ChatBoxChatEntry.messageFont);
                            gp.AddRectangle(new Rectangle(cbe.LeftMarginInLine(i), top + ChatBoxChatEntry.lineHeight * i, r.X + r.Width, ChatBoxChatEntry.lineHeight));
                        }
                        else if(isSelEndLine)
                        {
                            Rectangle r = Helper.RectangeOfCharAtIndex(cbe.Lines[i], cbe.SelectionEnd - index, ChatBoxChatEntry.messageFont);
                            gp.AddRectangle(new Rectangle(cbe.LeftMarginInLine(i), top + ChatBoxChatEntry.lineHeight * i, r.X + r.Width, ChatBoxChatEntry.lineHeight));
                        }
                        index += cbe.Lines[i].Length;
                    }
                    gp.CloseFigure();
                    e.Graphics.FillPath(Brushes.Blue, gp);
                }

                cbe.Draw(e.Graphics, top);
                top += cbe.Height;
            }
            //TextRenderer.DrawText(e.Graphics, _mouseLocation.ToString(), SystemFonts.DefaultFont, new Point(100, 0), Color.Black);

            AutoScrollMinSize = new Size(0, top - AutoScrollPosition.Y);
        }

        public void AddChatBoxEntry(ChatBoxEntry chatBoxEntry)
        {
            if(this.InvokeRequired)
                this.Invoke(new Action<ChatBoxEntry>(AddChatBoxEntry), chatBoxEntry);
            else
            {
                chatBoxEntry.Index = _chatBoxEntries.Count;
                _chatBoxEntries.Add(chatBoxEntry);
                chatBoxEntry.RecalculateSize(this.Width);
                if((this.VerticalScroll.Value + this.Height >= this.VerticalScroll.Maximum && this.VerticalScroll.Visible)
                || (this.AutoScrollMinSize.Height + chatBoxEntry.Lines.Length * ChatBoxChatEntry.lineHeight > this.Height && !this.VerticalScroll.Visible))
                {
                    AutoScrollMinSize = new Size(0, AutoScrollMinSize.Height + chatBoxEntry.Height);
                    //VerticalScroll.Value += chatBoxEntry.Lines.Length * ChatBoxChatEntry.lineHeight;
                    AutoScrollPosition = new Point(0, -AutoScrollPosition.Y + chatBoxEntry.Height);
                }
                this.Invalidate();
            }
        }

        public ChatBoxEntry GetChatBoxEntryAtPoint(Point p)
        {
            int top = 0 + AutoScrollPosition.Y;
            for(int i = 0; i < _chatBoxEntries.Count; i++)
            {
                if(top <= p.Y && top + _chatBoxEntries[i].Height > p.Y)
                    return _chatBoxEntries[i];
                top += _chatBoxEntries[i].Height;
            }
            return null;
        }
    }
}
