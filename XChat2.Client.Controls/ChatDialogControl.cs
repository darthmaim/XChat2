using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XChat2.Client.Communication;

namespace XChat2.Client.Controls
{
    public partial class ChatDialogControl : UserControl
    {
        public ChatDialogControl()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
        }

        public ChatBoxControl ChatBox
        {
            get { return chatBox; }
        }

        public SendMessageControl SendMessageControl
        {
            get { return sendMessageControl1; }
        }

        private IConnection _connection;
        public IConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        private void sendMessageControl1_SendImage(Bitmap image, byte[] imageBuffer)
        {
            if (_connection != null)
            {
                _connection.SendMessage(ChatBox.Contact, imageBuffer);
            }
            else
            {
                ChatBox.AddChatBoxEntry(new ChatBox.ChatBoxPlainEntry("Fehler beim Senden :("));
            }
        }

        private void sendMessageControl1_SendText(string text)
        {
            if (_connection != null)
            {
                _connection.SendMessage(ChatBox.Contact, text);
            }
            else
            {
                ChatBox.AddChatBoxEntry(new ChatBox.ChatBoxPlainEntry("Fehler beim Senden :("));
            }
        }
        
    }
}
