using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace XChat2.Client.Controls
{
    public class GradientPanel : Panel
    {
        private Color _gradientTopColor = Color.LightGray;
        [Category("Gradient")]
        public Color GradientTopColor
        {
            get { return _gradientTopColor; }
            set { _gradientTopColor = value; this.Invalidate(); }
        }

        private Color _gradientBottomColor = Color.DarkGray;
        [Category("Gradient")]
        public Color GradientBottomColor
        {
            get { return _gradientBottomColor; }
            set { _gradientBottomColor = value; this.Invalidate(); }
        }

        public GradientPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using(LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, this.Height), _gradientTopColor, _gradientBottomColor))
                e.Graphics.FillRectangle(lgb, new Rectangle(0, 0, this.Width, this.Height));
        }
    }
}
