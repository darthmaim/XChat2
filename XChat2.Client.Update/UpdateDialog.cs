using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace XChat2.Client.Update
{
    public partial class UpdateDialog : Form
    {
        WebClient wc = new WebClient();

        private bool _restart = false;
        public bool Restart
        {
            get { return _restart; }
        }

        private string _newVersion;
        public string NewVersion
        {
            get { return _newVersion; }
        }

        private string _executable = Path.GetFileName(Application.ExecutablePath);
        public string Executable
        {
            get { return _executable; }
            set { _executable = value; }
        }

        private string _updateServer;
        private string _currentVersion;
        private bool _forceUpdate;
        private Queue<string> _downloadFiles = new Queue<string>();
        
        internal UpdateDialog(string currentVersion, string updateServer, bool forceUpdate)
        {
            _currentVersion = currentVersion;
            _updateServer = updateServer;
            _forceUpdate = forceUpdate;
            InitializeComponent();

            this.Icon = XChat2.Client.Resources.Images.xchat_notify;

            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
            new Thread(new ThreadStart(startDownloadList)).Start();

            this.Shown += (o, e) =>
                XChat2.Client.Interop.Win7.Taskbar.SetProgressState(this.Handle, Interop.Win7.Taskbar.TaskbarProgressState.Indeterminate);
        }

        void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (MessageBox.Show(e.Error.Message.ToString(), "Beim Update ist ein Fehler aufgetreten (" + e.Error.GetType().FullName + ")", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                {
                    _restart = false;
                    new Thread(new ThreadStart(startDownloadList)).Start();
                    return;
                }
                else
                {
                    this.Invoke(new Action(this.Close));
                    return;
                }
            }
            else
            {
                DownloadNext();
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (MessageBox.Show(e.Error.Message.ToString(), "Beim Update ist ein Fehler aufgetreten (" + e.Error.GetType().FullName + ")", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                {
                    _restart = false;
                    new Thread(new ThreadStart(startDownloadList)).Start();
                    return;
                }
                else
                {
                    this.Invoke(new Action(this.Close));
                    return;
                }
            }
            if(e.Result == "up2date")
            {
                XChat2.Client.Interop.Win7.Taskbar.SetProgressState(this, Interop.Win7.Taskbar.TaskbarProgressState.NoProgress);
                //MessageBox.Show("Kein Update");
                this.Invoke(new Action(this.Close));
            }
            else if (e.Result.StartsWith("error"))
            {
                XChat2.Client.Interop.Win7.Taskbar.SetProgressState(this, Interop.Win7.Taskbar.TaskbarProgressState.Error);
                XChat2.Client.Interop.Win7.Taskbar.SetProgressValue(this, 100, 100);
                MessageBox.Show("Beim Update ist ein Fehler aufgetreten");
                this.Invoke(new Action(this.Close));
            }
            else
            {
                _restart = true;
                string[] data = e.Result.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                _newVersion = data[0];
                if (!Directory.Exists("old"))
                    Directory.CreateDirectory("old");
                for (int i = 1; i < data.Count(); i++)
                {
                    string file = data[i].Substring(1);
                    switch (data[i][0])
                    {
                        case '*':
                            _executable = file;
                            break;
                        case '-':
                            File.Move(file, Path.Combine("old", file));
                            break;
                        case '~':
                            File.Move(file, Path.Combine("old", file));
                            _downloadFiles.Enqueue(file);
                            break;
                        case '+':
                            _downloadFiles.Enqueue(file);
                            break;
                        case ' ':
                            if (_forceUpdate)
                            {
                                File.Move(file, Path.Combine("old", file));
                                _downloadFiles.Enqueue(file);
                            }
                            break;
                        default:
                            throw new Exception("Invalid Update response");
                    }
                }
                DownloadNext();
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Display(null, e.ProgressPercentage);
        }

        private void Display(string labelText, int progressbarValue)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, int>(Display), labelText, progressbarValue);
                return;
            }

            if(labelText != null)
                label1.Text = labelText;
            if (progressBar1.Style != ProgressBarStyle.Blocks)
                progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = progressbarValue;
        }

        private uint _filesDone = 0;

        private void DownloadNext()
        {
            if(_downloadFiles.Count == 0)
            {
                //MessageBox.Show("Geupdated von " + _currentVersion + " auf " + _newVersion);
                this.Invoke(new Action(this.Close));
                return;
            }
            string file = _downloadFiles.Dequeue();
            wc.DownloadFileAsync(new Uri("http://" + _updateServer + "/versions/" + _newVersion + "/files/" + file), file);

            XChat2.Client.Interop.Win7.Taskbar.SetProgressState(this, Interop.Win7.Taskbar.TaskbarProgressState.Normal);
            XChat2.Client.Interop.Win7.Taskbar.SetProgressValue(this, _filesDone, _filesDone + (ulong)_downloadFiles.Count);

            Display("Downloade " + file + "...", 0);
        }

        private void startDownloadList()
        {
            _downloadFiles.Clear();
            string updateUrl = "http://" + _updateServer + "/update.php?v=" + _currentVersion;
            if (_forceUpdate)
                updateUrl += "&force";
            wc.DownloadStringAsync(new Uri(updateUrl));
        }
    }
}
