namespace XChat2Client
{
    partial class TabbedChatDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabHeaderControl1 = new XChat2.Client.Controls.TabControl.TabHeaderControl();
            this.SuspendLayout();
            // 
            // tabHeaderControl1
            // 
            this.tabHeaderControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabHeaderControl1.Location = new System.Drawing.Point(0, 0);
            this.tabHeaderControl1.Name = "tabHeaderControl1";
            this.tabHeaderControl1.Size = new System.Drawing.Size(404, 21);
            this.tabHeaderControl1.TabIndex = 0;
            this.tabHeaderControl1.Text = "tabHeaderControl1";
            // 
            // TabbedChatDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 291);
            this.ControlBox = false;
            this.Controls.Add(this.tabHeaderControl1);
            this.Name = "TabbedChatDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private XChat2.Client.Controls.TabControl.TabHeaderControl tabHeaderControl1;
    }
}