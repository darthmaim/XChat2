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
    public partial class LoginForm : Form
    {
        private string _server;
        private int _port;

        public LoginForm(string server, int port, string username)
        {
            InitializeComponent();        

            this.Text += " (v" + Program.VERSION + ")";
            if (Program.Server != Program.Config["general"].get<string>("server").Value || Program.Port != Program.Config["general"].get<int>("port").Value)
                this.Text += " - " + Program.Server + ":" + Program.Port;

            this.Icon = XChat2.Client.Resources.Images.xchat_notify;

            _server = server;
            _port = port;
            textBox_username.Text = username;
        }

        private void Enable()
        {
            textBox_password.Enabled = true;
            textBox_username.Enabled = true;
            button_login.Enabled = true;
            linkLabel1.Enabled = true;
        }

        private void Disable()
        {
            textBox_password.Enabled = false;
            textBox_username.Enabled = false;
            button_login.Enabled = false;
            linkLabel1.Enabled = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://xchat.darthmaim.de/index.php?page=Register");
            }
            catch { }
        }

        private ClientConnection _connection;

        private void OnLoginSuccessful() {
            if(LoginSuccessful != null)
                LoginSuccessful(this, _connection);
        }
        public event Action<LoginForm, ClientConnection> LoginSuccessful;

        private void button_login_Click(object sender, EventArgs e)
        {
            Disable();
            ShowStatus("Connecting...");


            if (_connection != null && _connection.State != ClientConnection.States.Disconnected)
            {
                _connection.StateChanged -= _connection_StateChanged;
                _connection.Disconnect();
            }

            _connection = new ClientConnection(textBox_username.Text, textBox_password.Text);
            _connection.StateChanged += _connection_StateChanged;
            _connection.StartConnect(_server, _port);
        }

        void _connection_StateChanged(ClientConnection.States newState, ClientConnection.States oldState)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ClientConnection.States, ClientConnection.States>(_connection_StateChanged), newState, oldState);
                return;
            }

            if (newState == ClientConnection.States.Connected)
            {
                _connection.StateChanged -= _connection_StateChanged;
                Enable();
                HideStatus();
                textBox_password.Text = "";
                OnLoginSuccessful();
            }
            else if (newState == ClientConnection.States.Disconnected)
            {
                Enable();
                ShowError("Connection failed");
                textBox_password.Focus();
                textBox_password.SelectAll();
            }
        }

        private void ShowError(string error)
        {
            label_status.Visible = true;
            linkLabel1.Visible = false;
            label_status.Text = error;
            label_status.ForeColor = Color.Red;
        }

        private void ShowStatus(string status)
        {
            label_status.Visible = true;
            linkLabel1.Visible = false;
            label_status.Text = status;
            label_status.ForeColor = Color.Black;
        }

        private void HideStatus()
        {
            label_status.Visible = false;
            linkLabel1.Visible = true;
        }
    }
}
