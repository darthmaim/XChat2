using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XChat2.Client.Interop;

namespace XChat2Client
{
    public partial class TabbedChatDialog : Form
    {
        public TabbedChatDialog()
        {
            InitializeComponent();

            tabHeaderControl1.TabPages.Add(new XChat2.Client.Controls.TabControl.BaseTabPage<int>() { Text = "TEST" });

            this.MouseDown += MouseDownEvent;
            this.tabHeaderControl1.MouseDown += MouseDownEvent;
        }

        void MouseDownEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Y < this.tabHeaderControl1.Height && !tabHeaderControl1.HitTest(e.Location))
            {
                User32.ReleaseCapture();
                User32.SendMessage(this.Handle, User32.WM_NCLBUTTONDOWN, User32.HT_CAPTION, 0);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Aero.DwmIsCompositionEnabled())
            {
                this.BackColor = Color.Black;
                Aero.MARGINS m = new Aero.MARGINS() { Bottom = 0, Left = 0, Right = 0, Top = this.tabHeaderControl1.Height };
                Aero.DwmExtendFrameIntoClientArea(this.Handle, ref m);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.FillRectangle(Brushes.White, 0, tabHeaderControl1.Height, this.Width, this.Height - tabHeaderControl1.Height);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.FillRectangle(Brushes.White, 0, tabHeaderControl1.Height, this.Width, this.Height - tabHeaderControl1.Height);
        }
    }
}
