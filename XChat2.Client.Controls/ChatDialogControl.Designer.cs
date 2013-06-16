namespace XChat2.Client.Controls
{
    partial class ChatDialogControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chatBox = new XChat2.Client.Controls.ChatBoxControl();
            this.sendMessageControl1 = new XChat2.Client.Controls.SendMessageControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Silver;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chatBox);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.sendMessageControl1);
            this.splitContainer1.Panel2MinSize = 47;
            this.splitContainer1.Size = new System.Drawing.Size(439, 262);
            this.splitContainer1.SplitterDistance = 210;
            this.splitContainer1.TabIndex = 1;
            // 
            // chatBox
            // 
            this.chatBox.AutoScroll = true;
            this.chatBox.ChatHistory = null;
            this.chatBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatBox.Location = new System.Drawing.Point(0, 0);
            this.chatBox.MinimumSize = new System.Drawing.Size(100, 100);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(439, 210);
            this.chatBox.TabIndex = 0;
            this.chatBox.Text = "chatBox1";
            // 
            // sendMessageControl1
            // 
            this.sendMessageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sendMessageControl1.Location = new System.Drawing.Point(0, 0);
            this.sendMessageControl1.Mode = XChat2.Client.Controls.SendMessageControl.Modes.Text;
            this.sendMessageControl1.Name = "sendMessageControl1";
            this.sendMessageControl1.Size = new System.Drawing.Size(439, 48);
            this.sendMessageControl1.TabIndex = 2;
            this.sendMessageControl1.Text = "sendMessageControl1";
            this.sendMessageControl1.SendText += new XChat2.Client.Controls.SendMessageControl.SendTextHandler(this.sendMessageControl1_SendText);
            this.sendMessageControl1.SendImage += new XChat2.Client.Controls.SendMessageControl.SendImageHandler(this.sendMessageControl1_SendImage);
            // 
            // ChatDialogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ChatDialogControl";
            this.Size = new System.Drawing.Size(439, 262);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ChatBoxControl chatBox;
        private SendMessageControl sendMessageControl1;
    }
}
