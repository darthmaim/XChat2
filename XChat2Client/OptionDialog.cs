using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XChat2.Client.Data;

namespace XChat2Client
{
    public partial class OptionDialog : Form
    {
        private OptionPages.IOptionPage _currentPage;
        public OptionPages.IOptionPage CurrentPage
        {
            get { return _currentPage; }
        }

        static Font defaultFont = new Font("Tahoma", 8.75f, FontStyle.Regular);
        static Font highlightFont = new Font("Tahoma", 8.75f, FontStyle.Bold);

        public OptionDialog()
        {
            InitializeComponent();

            this.Icon = XChat2.Client.Resources.Images.xchat_notify;

            treeView1.Nodes.Add(new TreeNode("Chat") { Tag = new OptionPages.ChatOptionPage() });

            treeView1.SelectedNode = treeView1.Nodes[0];

            DefaultFormat();
        }

        private void button_apply_Click(object sender, EventArgs e)
        {
            _currentPage.Save();
            if (OptionsSaved != null)
                OptionsSaved();
        }

        public delegate void OptionsSavedHandler();
        public event OptionsSavedHandler OptionsSaved;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _currentPage = (OptionPages.IOptionPage)e.Node.Tag;
            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add(_currentPage.PageControl);
            _currentPage.PageControl.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                DefaultFormat();
            }
            else
            {
                foreach (TreeNode node in treeView1.Nodes)
                {
                    OptionPages.IOptionPage page = (OptionPages.IOptionPage)node.Tag;
                    if (page.Keywords.Count(keyword => keyword.ToLower().Contains(textBox1.Text.ToLower())) > 0)
                    {
                        node.EnsureVisible();
                        node.ForeColor = Color.Black;
                        node.NodeFont = highlightFont;
                    }
                    else
                    {
                        node.ForeColor = Color.Gray;
                        node.NodeFont = defaultFont;
                    }
                }
            }
        }

        private void DefaultFormat()
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                node.ForeColor = Color.Black;
                node.NodeFont = defaultFont;
            }
        }
    }
}
