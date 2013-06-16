namespace XChat2.Tests.Controls.ContactListControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.contactListControl1 = new XChat2.Client.Controls.OldContactListControl();
            this.SuspendLayout();
            // 
            // contactListControl1
            // 
            this.contactListControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contactListControl1.Location = new System.Drawing.Point(0, 0);
            this.contactListControl1.Name = "contactListControl1";
            this.contactListControl1.Size = new System.Drawing.Size(122, 272);
            this.contactListControl1.TabIndex = 0;
            this.contactListControl1.Text = "contactListControl1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(122, 272);
            this.Controls.Add(this.contactListControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Client.Controls.OldContactListControl contactListControl1;
    }
}

