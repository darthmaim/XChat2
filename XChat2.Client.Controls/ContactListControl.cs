using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XChat2.Client.Data;
using XChat2.Common.Collections;

namespace XChat2.Client.Controls
{
    public class ContactListControl : ScrollableControl
    {
        Color _colorOnline1 = Color.Green;
        Color _colorOnline2 = Color.Lime;
        Color _colorOffline1 = Color.FromArgb(0x90, 0, 0);
        Color _colorOffline2 = Color.FromArgb(0xFF, 0, 0);
        Color _colorOutstanding1 = Color.Gray;
        Color _colorOutstanding2 = Color.DarkGray;

        private static Font _usernameFont = new Font("Tahoma", 8.5f, FontStyle.Regular);
        public static Font UsernameFont { get { return _usernameFont; } }

        EventList<Contact> _contacts;
        public EventList<Contact> Contacts
        {
            get { return _contacts; }
            set
            {
                if (_contacts != null)
                {
                    _contacts.ElementAdded -= ContactAdded;
                    _contacts.ElementRemoved -= ContactRemoved;
                }
                _contacts = value;
                if (_contacts != null)
                {
                    _contacts.ElementAdded += ContactAdded;
                    _contacts.ElementRemoved += ContactRemoved;
                }
            }
        }

        private EventList<OutstandingContactRequest> _outstandingContacts;
        public EventList<OutstandingContactRequest> OutstandingContactRequests
        {
            get { return _outstandingContacts; }
            set
            {
                if (_outstandingContacts != null)
                {
                    _outstandingContacts.ElementAdded -= OutstandingContactRequestAdded;
                    _outstandingContacts.ElementRemoved -= OutstandingContactRequestRemoved;
                }
                _outstandingContacts = value;
                if (_contacts != null)
                {
                    _outstandingContacts.ElementAdded += OutstandingContactRequestAdded;
                    _outstandingContacts.ElementRemoved += OutstandingContactRequestRemoved;
                }
            }
        }

        //=======================================================================================================
        private void ContactAdded(EventList<Contact> sender, int index, Contact addedContact)
        {
            addedContact.OnlineStateChanged += Contact_OnlineStateChanged;
            this.Invalidate();
        }

        private void ContactRemoved(EventList<Contact> sender, int index, Contact removedContact)
        {
            removedContact.OnlineStateChanged -= Contact_OnlineStateChanged;
            this.Invalidate();
        }
        //=======================================================================================================


        //=======================================================================================================
        private void OutstandingContactRequestAdded(EventList<OutstandingContactRequest> sender, int index, OutstandingContactRequest addedOutstandingContactRequest)
        {
            this.Invalidate();
        }

        private void OutstandingContactRequestRemoved(EventList<OutstandingContactRequest> sender, int index, OutstandingContactRequest removedOutstandingContactRequest)
        {
            this.Invalidate();
        }
        //=======================================================================================================


        private void Contact_OnlineStateChanged(Contact contact, bool online)
        {
            this.Invalidate(GetRectangle(contact));
        }

        public ContactListControl()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            this.Paint += new PaintEventHandler(ContactListControl_Paint);
            this.MouseClick += new MouseEventHandler(ContactListControl_MouseClick);
            this.MouseMove += new MouseEventHandler(ContactListControl_MouseMove);
            this.MouseLeave += (o, e) =>
            {
                if (_hovered == null)
                    return;
                this.Invalidate(GetRectangle(_hovered));
            };

            this.Dock = DockStyle.Fill;
        }

        private Rectangle GetRectangle(Contact c)
        {
            if (!_contacts.Contains(c))
                throw new ArgumentException();
            int i = _contacts.IndexOf(c);
            return new Rectangle(0, i * 16 + 1, this.Width, 16);
        }

        void ContactListControl_MouseClick(object sender, MouseEventArgs e)
        {
            int y = e.Y - 1;
            int i = (int)Math.Floor(y / 16.0);
            if (i >= 0 && i < _contacts.Count)
            {
                OnContactClicked(_contacts[i]);
            }
        }

        private void OnContactClicked(Contact c)
        {
            if (ContactClicked != null)
                ContactClicked.Invoke(c);
        }

        public delegate void ContactClickedHandler(Contact c);
        public event ContactClickedHandler ContactClicked;

        Contact _hovered = null;
        void ContactListControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_contacts == null)
                return;
            Contact oldHovered = _hovered;
            int y = e.Y - 1;
            int i = (int)Math.Floor(y / 16.0);
            if (i >= 0 && i < _contacts.Count)
            {
                _hovered = _contacts[i];
                if (_hovered != oldHovered)
                {
                    if (oldHovered != null)
                        this.Invalidate(GetRectangle(oldHovered));
                    this.Invalidate(GetRectangle(_hovered));
                }
                else
                {
                    this.Invalidate(GetRectangle(_hovered));
                }
            }
            else
            {
                _hovered = null;

                if (oldHovered != null)
                    this.Invalidate(GetRectangle(oldHovered));
            }
        }

        void ContactListControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height);

            if (_contacts == null)
                return;

            for (int i = 0; i < _contacts.Count; i++)
            {
                Contact current = _contacts[i];
                if (current == _hovered)
                    e.Graphics.FillRectangle(Brushes.CornflowerBlue, 0, 1 + i * 16, this.Width, 16);
                DrawOnlineOfflineCirle(e.Graphics, new Point(3, 3 + i * 16), current.Online ? _colorOnline1 : _colorOffline1, current.Online ? _colorOnline2 : _colorOffline2);
                TextRenderer.DrawText(e.Graphics, current.Name, _usernameFont, new Point(15, 2 + i * 16), Color.Black);
            }
            int offset = 1 + _contacts.Count * 16;
            for (int i = 0; i < _outstandingContacts.Count; i++)
            {
                OutstandingContactRequest current = _outstandingContacts[i];
                DrawOnlineOfflineCirle(e.Graphics, new Point(3, 3 + i * 16 + offset), _colorOutstanding1, _colorOutstanding2);
                TextRenderer.DrawText(e.Graphics, current.Username, _usernameFont, new Point(15, 2 + i * 16 + offset), Color.DarkGray);
            }
        }

        private void DrawOnlineOfflineCirle(Graphics g, Point p, Color c1, Color c2)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            using (System.Drawing.Pen p1 = new Pen(c1))
            using (System.Drawing.Drawing2D.LinearGradientBrush lgb1 = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, p.Y), new Point(0, p.Y + 12), c1,c2))
            using (System.Drawing.Drawing2D.LinearGradientBrush lgb2 = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, p.Y - 1), new Point(0, p.Y + 7), Color.White,c2))
            {
                g.FillEllipse(lgb1, p.X, p.Y, 12, 12);
                g.FillEllipse(lgb2, p.X + 2, p.Y + 1, 8, 6);
                g.DrawEllipse(p1, p.X, p.Y, 12, 12);
            }
        }
    }
}