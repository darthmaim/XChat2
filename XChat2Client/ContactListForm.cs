using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XChat2.Client.Communication;
using XChat2.Client.Controls;
using XChat2.Client.Data;

namespace XChat2Client
{
    public partial class ContactListForm : Form
    {
        private ClientConnection _connection;
        private bool _closing = false;
        
        private Dictionary<Contact, ChatDialogControl> _openChatDialogs = new Dictionary<Contact, ChatDialogControl>();

        public ContactListForm(ClientConnection connection)
        {
            _connection = connection;
            _connection.StateChanged += connection_StateChanged;
            _connection.Exception += (c, x) => MessageBox.Show(x.ToString(), x.GetType().FullName);

            InitializeComponent();
            this.Icon = XChat2.Client.Resources.Images.xchat_notify;

            this.FormClosing += (o, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing && !_closing)
                {
                    e.Cancel = true;
                    this.WindowState = FormWindowState.Minimized;
                }
                else
                {
                    foreach(ChatDialogControl cdc in _openChatDialogs.Values.ToList())
                    {
                        cdc.ParentForm.Close();
                    }
                    if (od != null && !od.IsDisposed)
                        od.Close();
                }
            };

            _connection.MessageReceived += (c, m) => this.Invoke(new Action<Contact>(ShowChatBoxDialog), c);
            _connection.IncomingOptions += (c, v, d) => 
                this.Invoke(new Action<int, byte[]>(Options.Instance.SetData), v, d);
            _connection.IncomingContactRequest += IncomingContactRequest;

            contactListControl1.Contacts = _connection.Contacts;
            contactListControl1.OutstandingContactRequests = _connection.OutstandingContactRequests;
            contactListControl1.ContactClicked += (c) =>
            {
                ShowChatBoxDialog(c);
            };


#if(!DEBUG)
            generalToolStripMenuItem.DropDownItems.Remove(tabbedToolStripMenuItem);
#endif
        }

        private void IncomingContactRequest(uint id, string username)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<uint, string>(IncomingContactRequest), id, username);
                return;
            }
            IncomingContactRequestDialog icrd = new IncomingContactRequestDialog()
            {
                Message = string.Format("Du hast eine Kontaktanfrage von '{0}' erhalten.\r\nWillst du '{0}' zu deinen Kontakten hinzufügen?", username)
            };
            DialogResult dr = icrd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                _connection.AcceptContact(id);
            }
            else if (dr == System.Windows.Forms.DialogResult.No)
            {
                _connection.DeclineContact(id);
            }
        }

        private void ShowChatBoxDialog(Contact c)
        {
            ChatDialogControl cdc = cbcGetChatDialog(c);
            if (cdc.Parent == null)
            {
                ChatDialog cd = new ChatDialog(cdc);
                cd.FormClosed += (o, e) => { _openChatDialogs.Remove(cd.Contact); };
                cd.Show();
            }
            else
            {
                if (ActiveForm == null || ActiveForm == this)
                {
                    cdc.Show();
                    cdc.Focus();
                }
            }
        }

        private ChatDialogControl cbcGetChatDialog(Contact c)
        {
            if (!_openChatDialogs.ContainsKey(c))
            {
                ChatDialogControl cdc = new ChatDialogControl();
                cdc.ChatBox.ChatHistory = c.History;
                cdc.Connection = _connection;
                _openChatDialogs.Add(c, cdc);
            }
            return _openChatDialogs[c];
        }

        void connection_StateChanged(ClientConnection.States newState, ClientConnection.States oldState)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ClientConnection.States, ClientConnection.States>(connection_StateChanged), newState, oldState);
                return;
            }

            if (newState == ClientConnection.States.Disconnected)
            {
                _closing = true;
                this.Close();
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connection.Disconnect();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connection.Disconnect();
            _closing = true;
            _exit = true;
            this.Close();
        }

        private bool _exit;
        public bool Exit
        {
            get { return _exit; }
        }

        private OptionDialog od;
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (od == null || od.IsDisposed)
            {
                if (od != null)
                    od.OptionsSaved -= OptionsSaved;
                od = new OptionDialog();
                od.Show();
                od.OptionsSaved += OptionsSaved;
            }
            else if (od.WindowState == FormWindowState.Minimized)
                od.Show();
            else
                od.Focus();
        }

        void OptionsSaved()
        {
            _connection.SendOptions(Options.Instance);
        }

        SearchContactForm scf;
        private void addContactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scf == null || scf.IsDisposed)
            {
                scf = new SearchContactForm(_connection);
                scf.Show();
            }
            else if (scf.WindowState == FormWindowState.Minimized)
                scf.Show();
            else
                scf.Focus();
        }

        private void tabbedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabbedChatDialog tcd = new TabbedChatDialog();
            tcd.ShowDialog();
        }
        
    }
}
