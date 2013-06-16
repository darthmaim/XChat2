using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XChat2.Client.Communication;

namespace XChat2Client
{
    public partial class SearchContactForm : Form
    {
        private ClientConnection _connection;
        public ClientConnection MyProperty
        {
            get { return _connection; }
        }

        public SearchContactForm(ClientConnection connection)
        {
            _connection = connection;

            InitializeComponent();

            this.Icon = XChat2.Client.Resources.Images.xchat_notify;

            searchContactResultControl1.AddButtonClicked += new XChat2.Client.Controls.SearchContactResultControl.AddButtonClickedHandler(searchContactResultControl1_AddButtonClicked);
        }

        void searchContactResultControl1_AddButtonClicked(uint userID)
        {
            _connection.SendContactRequest(userID);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text.Trim())) return;
            _connection.StartSearchContact(textBox1.Text, searchComplete);
            this.Enabled = false;
        }

        private void searchComplete(Dictionary<uint, string> result)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action<Dictionary<uint, string>>(searchComplete), result);
                return;
            }
            this.Enabled = true;
            searchContactResultControl1.SetUsernames(result);
        }
    }
}
