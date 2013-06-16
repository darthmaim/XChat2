using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XChat2.Client.Controls;
using XChat2.Client.Data;
using XChat2.Client.Interop;

namespace XChat2Client
{
    public partial class ChatDialog : Form
    {
        public ChatDialog(ChatDialogControl cdc)
        {
            InitializeComponent();

            this.Icon = XChat2.Client.Resources.Images.xchat_notify;

            this.Controls.Add(cdc);
            _chatDialogControl = cdc;

            this.Text = Contact.Name;

            this.Shown += (o, e) =>
            {
                Contact.MessageReceived += (c, m) => this.Invoke(new Action(() =>
                {
                    if (Form.ActiveForm != this)
                    {
                        User32.FlashWindowEx(this);
                    }
                }));
            };
            
            //this.FormClosing += (o,e) => { e.Cancel = true; this.Hide(); };
        }

        private ChatDialogControl _chatDialogControl;
        public ChatDialogControl ChatDialogControl
        {
            get { return _chatDialogControl; }
        }

        public Contact Contact
        {
            get { return _chatDialogControl.ChatBox.Contact; }
        }
    }
}
