using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XChat2.Common.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace XChat2.Client.Controls.TabControl
{
    public class TabHeaderControl : Control
    {
        public TabHeaderControl()
        {
            _tabPages = new EventList<ITabPage>();

            _backgroundBrush = new SolidBrush(_backColor);
            _borderPen = new Pen(_borderColor);

            _closePen = new Pen(Color.Gray, 1.5f);
        }

        private EventList<ITabPage> _tabPages;
        public EventList<ITabPage> TabPages
        {
            get { return _tabPages; }
        }

        Color _backColor = Color.FromArgb(255,255,255,255);
        Color _borderColor = Color.FromArgb(0, 0, 0, 0);

        Brush _backgroundBrush;
        Pen _borderPen;

        Pen _closePen;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            int offset = 0;
            for (int i = 0; i < _tabPages.Count; i++)
            {
                int x = offset + (i + 1) * 2;
                int y = this.Height - 20;
                int width = _tabPages[i].Size.Width + 12;
                //Draw background and border
                e.Graphics.FillRectangle(_backgroundBrush, x, y, width, 20);
                e.Graphics.DrawRectangle(_borderPen, x, y, width, 20);

                //Draw Closebutton
                //e.Graphics.FillEllipse(Brushes.Gray, x + width - 11, 5, 10, 10);
                e.Graphics.DrawLine(_closePen, x + width - 9, 5, x + width - 3, 5 + 6);
                e.Graphics.DrawLine(_closePen, x + width - 3, 5, x + width - 9, 5 + 6);

                //Draw Text
                e.Graphics.DrawString(_tabPages[i].Text, _tabPages[i].Font, Brushes.Black, x + 4, y + 4);

                offset += width;
            }
        }

        public bool HitTest(Point p)
        {
            return (p.X > 1 && p.X < 200 && p.Y > 1 && p.Y < 200);
        }
        
    }
}
