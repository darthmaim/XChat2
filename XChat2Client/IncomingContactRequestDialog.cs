using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XChat2Client
{
    public partial class IncomingContactRequestDialog : Form
    {
        public IncomingContactRequestDialog()
        {
            InitializeComponent();

            this.Icon = XChat2.Client.Resources.Images.xchat_notify;
        }

        public string Message
        {
            get { return label_text.Text; }
            set { label_text.Text = value; }
        }
        
    }
}
