namespace XChat2Client
{
    partial class IncomingContactRequestDialog
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
            if(disposing && (components != null))
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
            this.gradientPanel1 = new XChat2.Client.Controls.GradientPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button_accept = new System.Windows.Forms.Button();
            this.button_decline = new System.Windows.Forms.Button();
            this.label_text = new System.Windows.Forms.Label();
            this.gradientPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gradientPanel1
            // 
            this.gradientPanel1.Controls.Add(this.button1);
            this.gradientPanel1.Controls.Add(this.button_accept);
            this.gradientPanel1.Controls.Add(this.button_decline);
            this.gradientPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gradientPanel1.GradientBottomColor = System.Drawing.Color.CornflowerBlue;
            this.gradientPanel1.GradientTopColor = System.Drawing.Color.AliceBlue;
            this.gradientPanel1.Location = new System.Drawing.Point(0, 80);
            this.gradientPanel1.Name = "gradientPanel1";
            this.gradientPanel1.Size = new System.Drawing.Size(313, 42);
            this.gradientPanel1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.button1.Location = new System.Drawing.Point(145, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "&Ignorieren";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button_accept
            // 
            this.button_accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_accept.BackColor = System.Drawing.SystemColors.Control;
            this.button_accept.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.button_accept.Location = new System.Drawing.Point(64, 10);
            this.button_accept.Name = "button_accept";
            this.button_accept.Size = new System.Drawing.Size(75, 23);
            this.button_accept.TabIndex = 1;
            this.button_accept.Text = "&Ja";
            this.button_accept.UseVisualStyleBackColor = true;
            // 
            // button_decline
            // 
            this.button_decline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_decline.BackColor = System.Drawing.SystemColors.Control;
            this.button_decline.DialogResult = System.Windows.Forms.DialogResult.No;
            this.button_decline.Location = new System.Drawing.Point(226, 10);
            this.button_decline.Name = "button_decline";
            this.button_decline.Size = new System.Drawing.Size(75, 23);
            this.button_decline.TabIndex = 0;
            this.button_decline.Text = "&Nein";
            this.button_decline.UseVisualStyleBackColor = true;
            // 
            // label_text
            // 
            this.label_text.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_text.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label_text.Location = new System.Drawing.Point(0, 0);
            this.label_text.Name = "label_text";
            this.label_text.Padding = new System.Windows.Forms.Padding(10);
            this.label_text.Size = new System.Drawing.Size(313, 80);
            this.label_text.TabIndex = 1;
            this.label_text.Text = "Du hast eine Kontaktanfrage von \'{0}\' erhalten.\r\nWillst du \'{0}\' zu deinen Kontak" +
                "ten hinzufügen?";
            // 
            // IncomingContactRequestDialog
            // 
            this.AcceptButton = this.button_accept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.button_decline;
            this.ClientSize = new System.Drawing.Size(313, 122);
            this.Controls.Add(this.label_text);
            this.Controls.Add(this.gradientPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "IncomingContactRequestDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XChat2 - Neue Kontaktanfrage";
            this.gradientPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private XChat2.Client.Controls.GradientPanel gradientPanel1;
        private System.Windows.Forms.Button button_accept;
        private System.Windows.Forms.Button button_decline;
        private System.Windows.Forms.Label label_text;
        private System.Windows.Forms.Button button1;
    }
}