using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using XChat2.Client.Controls.ChatBox;
using XChat2.Client.Data;
using XChat2.Client.Data.ChatMessages;

namespace XChat2.Client.Controls
{
    public class ChatBoxControl : ScrollableControl
    {
        private List<IChatBoxEntry> _chatBoxEntries;
        
        private static Color _entryHoverColor = Color.FromArgb(0xEE, 0xEE, 0xFF);
        private static Color _entryHoverEndColor = Color.FromArgb(0xE0, 0xE0, 0xFF);
        private static Color _lineHoverColor = Color.FromArgb(0xCC, 0xCC, 0xFF);
        private static Color _lineHoverEndColor = Color.FromArgb(0x99, 0x99, 0xFF);

        private bool _atBottom = true;

        public ChatBoxControl()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            _chatBoxEntries = new List<IChatBoxEntry>();
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
            this.Scroll += new ScrollEventHandler(ChatBoxControl_Scroll);
            this.AutoScroll = true;
            this.MinimumSize = new Size(100, 100);

            Options.Instance.TimeFormat.OptionChanged += (o, v) => { RecalculateSize(); this.Invalidate(); };
            Options.Instance.ShowTime.OptionChanged += (o, v) => { RecalculateSize(); this.Invalidate(); };
        }        

        void ChatBoxControl_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                _atBottom = (e.NewValue + this.Height == _chatBoxEntries.Sum(entry => entry.Height));
            }

            this.Invalidate();
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
                IChatBoxEntry cbe = _hoveredChatBoxEntry;
                _hoveredChatBoxEntry = null;
                Invalidate(GetRectangleOfChatBoxEntry(cbe));
            }
        }

        private Point _mouseLocation = new Point(-1, -1);
        private IChatBoxEntry _hoveredChatBoxEntry = null;
        private int _hoveredLine = -1;
        private int _hoveredChar = -1;
        void ChatBox_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseLocation = e.Location;
            IChatBoxEntry oldHovered = _hoveredChatBoxEntry;
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

        private Rectangle GetRectangleOfChatBoxEntry(IChatBoxEntry chatBoxEntry)
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

        public void RecalculateSize()
        {
            _chatBoxEntries.ForEach(cbe => cbe.RecalculateSize(this.Width - (this.VScroll ? System.Windows.Forms.SystemInformation.VerticalScrollBarWidth : 0)));
        }

        void ChatBox_Resize(object sender, EventArgs e)
        {
            RecalculateSize();

            if (this.AutoScrollMinSize.Height < Height)
                _atBottom = true;

            else if (_atBottom)
            {
                AutoScrollPosition = new Point(0, AutoScrollMinSize.Height - this.Height);
            }

            this.Invalidate();
        }

        void ChatBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height);
            //e.Graphics.DrawLine(Pens.LightGray, 100, 0, 100, this.Height);

            //if (_atBottom)
            //    e.Graphics.FillRectangle(Brushes.Red, 2, 2, 5, 5);

            int top = 0 + AutoScrollPosition.Y;
            foreach(IChatBoxEntry cbe in _chatBoxEntries)
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

        public void AddChatBoxEntry(IChatBoxEntry chatBoxEntry)
        {
            if(this.InvokeRequired)
                this.Invoke(new Action<IChatBoxEntry>(AddChatBoxEntry), chatBoxEntry);
            else
            {
                chatBoxEntry.Index = _chatBoxEntries.Count;
                _chatBoxEntries.Add(chatBoxEntry);
                chatBoxEntry.RecalculateSize(this.Width);
                if ((this.VerticalScroll.Value + this.Height >= this.VerticalScroll.Maximum && this.VerticalScroll.Visible)
                || (this.AutoScrollMinSize.Height + chatBoxEntry.Lines.Length * ChatBoxChatEntry.lineHeight > this.Height && !this.VerticalScroll.Visible))
                {
                    AutoScrollMinSize = new Size(0, AutoScrollMinSize.Height + chatBoxEntry.Height);
                    //VerticalScroll.Value += chatBoxEntry.Lines.Length * ChatBoxChatEntry.lineHeight;
                    AutoScrollPosition = new Point(0, -AutoScrollPosition.Y + chatBoxEntry.Height);
                }
                this.Invalidate();
            }
        }

        public IChatBoxEntry GetChatBoxEntryAtPoint(Point p)
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

        public Contact Contact
        {
            get { return _history.Contact; }
        }

        private ChatHistory _history;
        public ChatHistory ChatHistory
        {
            get { return _history; }
            set
            {
                _chatBoxEntries.Clear();
                if (_history != null)
                    _history.ChatMessageAdded -= ChatMessageAdded;
                _history = value;
                if (_history != null)
                {
                    _history.ChatMessageAdded += ChatMessageAdded;
                    foreach (IChatMessage cm in _history)
                    {
                        ChatMessageAdded(_history, cm);
                    }
                }
            }
        }        

        private void ChatMessageAdded(ChatHistory sender, IChatMessage added)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ChatHistory, IChatMessage>(ChatMessageAdded), sender, added);
                return;
            }

            if (added is TextChatMessage)
            {
                AddChatBoxEntry(CreateTextEntry((TextChatMessage)added));
            }
            else if (added is ImageChatMessage)
            {
                AddChatBoxEntry(CreateImageEntry((ImageChatMessage)added));
            }
        }

        private IChatBoxEntry CreateTextEntry(TextChatMessage tcm)
        {
            string username = tcm.Direction == Directions.Incoming ? tcm.Contact.Name : tcm.Username;
            Color usernameColor = tcm.Direction == Directions.Incoming ? Color.Red : Color.Blue;
            ChatBoxChatEntry cbce = new ChatBoxChatEntry(tcm, usernameColor);
            return cbce;
        }

        private IChatBoxEntry CreateImageEntry(ImageChatMessage icm)
        {
            string username = icm.Direction == Directions.Incoming ? icm.Contact.Name : icm.Username;
            Color usernameColor = icm.Direction == Directions.Incoming ? Color.Red : Color.Blue;
            ChatBoxImageEntry cbie = new ChatBoxImageEntry(icm, usernameColor);
            return cbie;
        }
        
    }
}
