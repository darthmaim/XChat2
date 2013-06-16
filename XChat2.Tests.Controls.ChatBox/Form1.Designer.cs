namespace XChat2.Tests.Controls.ChatBox
{
    partial class Form1
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
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.chatBox1 = new XChat2.Client.Controls.ChatBoxControl();
            this.SuspendLayout();
            // 
            // chatBox1
            // 
            this.chatBox1.AutoScroll = true;
            this.chatBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatBox1.Location = new System.Drawing.Point(0, 0);
            this.chatBox1.MinimumSize = new System.Drawing.Size(100, 100);
            this.chatBox1.Name = "chatBox1";
            this.chatBox1.Size = new System.Drawing.Size(292, 272);
            this.chatBox1.TabIndex = 0;
            this.chatBox1.Text = "chatBox1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 272);
            this.Controls.Add(this.chatBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Client.Controls.ChatBoxControl chatBox1;
    }
}

