using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XChat2.Client.Data;
using System.Diagnostics;
using XChat2.Common.Helper;

namespace XChat2Client.OptionPages
{
    public partial class ChatOptionPage : UserControl, IOptionPage
    {
        public ChatOptionPage()
        {
            InitializeComponent();

            Options.Instance.ShowTime.OptionChanged += (o, v) => checkBox_showTimestamp.Checked = v;
            Options.Instance.TimeFormat.OptionChanged += (o, v) => textBox_timestampFormat.Text = v;

            checkBox_showTimestamp.Checked = Options.Instance.ShowTime;
            textBox_timestampFormat.Text = Options.Instance.TimeFormat;
            panel_showTimestampOptions.Enabled = checkBox_showTimestamp.Checked;

            this.Dock = DockStyle.Fill;
        }

        private void checkBox_showTimestamp_CheckedChanged(object sender, EventArgs e)
        {
            panel_showTimestampOptions.Enabled = checkBox_showTimestamp.Checked;
            labelPreviewTimeFormat.Text = PreviewTime();
            timerTimePreview.Enabled = checkBox_showTimestamp.Checked;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(textBox_timestampFormat.Text.Trim()))
                textBox_timestampFormat.Text = "%T";
            Options.Instance.ShowTime.Value = checkBox_showTimestamp.Checked;
            Options.Instance.TimeFormat.Value = textBox_timestampFormat.Text;
        }

        public UserControl PageControl
        {
            get { return this; }
        }

        public string[] Keywords
        {
            get { return new string[] { "Chat", "Appereance", "Time", "Date", "Format", "Timestamp" }; }
        }

        private void textBox_timestampFormat_TextChanged(object sender, EventArgs e)
        {
            labelPreviewTimeFormat.Text = PreviewTime();
        }

        private string PreviewTime()
        {
            return checkBox_showTimestamp.Checked ? DateTimeStringFormater.Format(textBox_timestampFormat.Text) : "";
        }

        private void timerTimePreview_Tick(object sender, EventArgs e)
        {
            labelPreviewTimeFormat.Text = PreviewTime();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { Process.Start("http://xchat.darthmaim.de/?page=dateTimeFormat"); }
            catch { }
        }
    }
}
