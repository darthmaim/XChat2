using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XChat2.Tests.Controls.ContactListControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Timer t1 = new Timer() { Enabled = true, Interval = 5000 };
            Random r1 = new Random();
            t1.Tick += (o, e) => { foreach(Client.Controls.OldContactListControl.ContactListControlEntry clce in contactListControl1.GetEntries()) clce.Online = r1.Next(100) % 2 == 1; };
            t1.Start();

            contactListControl1.AddEntry(0, new Client.Controls.OldContactListControl.ContactListControlEntry() { Online = false, Username = "zzz" });
            contactListControl1.AddEntry(2, new Client.Controls.OldContactListControl.ContactListControlEntry() { Online = false, Username = "abc" });
            contactListControl1.AddEntry(1000, new Client.Controls.OldContactListControl.ContactListControlEntry() { Online = true, Username = "darthmaim" });
        }
    }
}
