using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XChat2.Client.Controls
{
    public class OldContactListControl : ScrollableControl
    {
        public class ContactListControlEntry {
            private bool _online;
            public bool Online
            {
                get { return _online; }
                set
                {
                    bool old = _online;
                    _online = value;
                    if(old != value)
                        OnNeedsInvalidate();
                }
            }

            private string _username;
            public string Username
            {
                get { return _username; }
                set
                {
                    string old = _username;
                    _username = value;
                    if(old != value)
                    {
                        _size = new Size(TextRenderer.MeasureText(_username, OldContactListControl.UsernameFont).Width + 15, 16);
                        OnNeedsInvalidate();
                    }
                }
            }

            private Size _size;
            public Size Size
            {
                get { return _size; }
            }
            

            private void OnNeedsInvalidate() { if(NeedsInvalidate != null) NeedsInvalidate.Invoke(this); }
            public delegate void NeedsInvalidateHandler(ContactListControlEntry clce);
            public event NeedsInvalidateHandler NeedsInvalidate;
        }

        Color _colorOnline1 = Color.Green;
        Color _colorOnline2 = Color.Lime;
        Color _colorOffline1 = Color.FromArgb(0x90, 0, 0);
        Color _colorOffline2 = Color.FromArgb(0xFF, 0, 0);

        private static Font _usernameFont = new Font("Tahoma", 8.5f, FontStyle.Regular);
        public static Font UsernameFont { get { return _usernameFont; } }

        Dictionary<int, ContactListControlEntry> _entries;
        List<ContactListControlEntry> _orderedEntries;

        public OldContactListControl()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            
            _entries = new Dictionary<int, ContactListControlEntry>();
            _orderedEntries = new List<ContactListControlEntry>();
            this.Paint += new PaintEventHandler(ContactListControl_Paint);
            this.MouseClick += new MouseEventHandler(ContactListControl_MouseClick);
            this.MouseMove += new MouseEventHandler(ContactListControl_MouseMove);
            this.MouseLeave += (o, e) =>
            {
                if(_hovered == null) return;
                int i = _orderedEntries.IndexOf(_hovered);
                _hovered = null;
                this.Invalidate(new Rectangle(0, i * 16 + 1, this.Width, 16));
            };

            this.Dock = DockStyle.Fill;
        }

        void ContactListControl_MouseClick(object sender, MouseEventArgs e)
        {
            int y = e.Y - 1;
            int i = (int)Math.Floor(y / 16.0);
            if(i >= 0 && i < _entries.Count) {
                ContactListControlEntry clce = _orderedEntries[i];
                    OnEntryClicked((from k in _entries where k.Value == clce select k.Key).First());
                }
        }

        private void OnEntryClicked(int id)
        {
            if(EntryClicked != null)
                EntryClicked.Invoke(id);
        }

        public delegate void EntryClickedHandler(int id);
        public event EntryClickedHandler EntryClicked;

        ContactListControlEntry _hovered = null;
        void ContactListControl_MouseMove(object sender, MouseEventArgs e)
        {
            ContactListControlEntry oldHovered = _hovered;
            int y = e.Y - 1;
            int i = (int)Math.Floor(y / 16.0);
            if(i >= 0 && i < _entries.Count && i != _orderedEntries.IndexOf(oldHovered))
            {
                _hovered = _orderedEntries[i];
                this.Invalidate(new Rectangle(0, _orderedEntries.IndexOf(oldHovered) * 16 + 1, this.Width, 16));
                this.Invalidate(new Rectangle(0, i * 16 + 1, this.Width, 16));
            }
            else if((i < 0 || i >= _orderedEntries.Count) && oldHovered != null)
            {
                _hovered = null;
                this.Invalidate(new Rectangle(0, _orderedEntries.IndexOf(oldHovered) * 16 + 1, this.Width, 16));
            }
        }

        private void entryNeedsInvalidate(ContactListControlEntry clce)
        {
            this.AutoScrollMinSize = new Size(calculateWidth(), 4 + 16 * _orderedEntries.Count);
            this.Invalidate(new Rectangle(0, _orderedEntries.IndexOf(clce) * 16 + 1, clce.Size.Width, 16));
        }

        public void AddEntry(int id, ContactListControlEntry clce)
        {
            clce.NeedsInvalidate += entryNeedsInvalidate;
            _entries.Add(id, clce);
            _orderedEntries = (from v in _entries.Values orderby v.Username select v).ToList<ContactListControlEntry>();
            this.AutoScrollMinSize = new Size(calculateWidth(), 4 + 16 * _orderedEntries.Count);
            this.Invalidate();
        }

        public ContactListControlEntry GetEntry(int id)
        {
            return _entries[id];
        }

        public bool ContainsEntry(int id)
        {
            return _entries.ContainsKey(id);
        }

        public void RemoveEntry(int id)
        {
            _entries[id].NeedsInvalidate -= entryNeedsInvalidate;
            _entries.Remove(id);
            _orderedEntries = (from v in _entries.Values orderby v.Username select v).ToList<ContactListControlEntry>();
        }

        public void ClearEntries()
        {
            _entries.Clear();
            _orderedEntries.Clear();
        }

        public ContactListControlEntry[] GetEntries()
        {
            return _entries.Values.ToArray();
        }

        private int calculateWidth()
        {
            int minWidth = 0;
            foreach(ContactListControlEntry clc in _entries.Values)
            {
                if(clc.Size.Width > minWidth) minWidth = clc.Size.Width;
            }
            return minWidth;
        }

        void ContactListControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height);

            for(int i = 0; i < _orderedEntries.Count(); i++)
            {
                ContactListControlEntry current = _orderedEntries[i];
                if(current == _hovered)
                    e.Graphics.FillRectangle(Brushes.CornflowerBlue, 0, 1 + i * 16, this.Width, 16);
                DrawOnlineOfflineCirle(e.Graphics, new Point(3, 3 + i * 16), current.Online);
                TextRenderer.DrawText(e.Graphics, current.Username, _usernameFont, new Point(15, 2 + i * 16), Color.Black);
            }
        }

        private void DrawOnlineOfflineCirle(Graphics g, Point p, bool o)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            using(System.Drawing.Pen p1 = new Pen(o ? _colorOnline1 : _colorOffline1))
            using(System.Drawing.Drawing2D.LinearGradientBrush lgb1 = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, p.Y), new Point(0, p.Y + 12), o ? _colorOnline1 : _colorOffline1, o ? _colorOnline2 : _colorOffline2))
            using(System.Drawing.Drawing2D.LinearGradientBrush lgb2 = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, p.Y - 1), new Point(0, p.Y + 7), Color.White, o ? _colorOnline2 : _colorOffline2))
            {
                g.FillEllipse(lgb1, p.X, p.Y, 12, 12);
                g.FillEllipse(lgb2, p.X + 2, p.Y + 1, 8, 6);
                g.DrawEllipse(p1, p.X, p.Y, 12, 12);
            }
        }
    }
}
