namespace XChat2Client.OptionPages
{
    partial class ChatOptionPage
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel_showTimestampOptions = new System.Windows.Forms.Panel();
            this.labelPreviewTimeFormat = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_timestampFormat = new System.Windows.Forms.TextBox();
            this.checkBox_showTimestamp = new System.Windows.Forms.CheckBox();
            this.timerTimePreview = new System.Windows.Forms.Timer(this.components);
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.panel_showTimestampOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.panel_showTimestampOptions);
            this.groupBox1.Controls.Add(this.checkBox_showTimestamp);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(551, 105);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Timestamp";
            // 
            // panel_showTimestampOptions
            // 
            this.panel_showTimestampOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_showTimestampOptions.Controls.Add(this.linkLabel1);
            this.panel_showTimestampOptions.Controls.Add(this.labelPreviewTimeFormat);
            this.panel_showTimestampOptions.Controls.Add(this.label1);
            this.panel_showTimestampOptions.Controls.Add(this.textBox_timestampFormat);
            this.panel_showTimestampOptions.Location = new System.Drawing.Point(6, 42);
            this.panel_showTimestampOptions.Name = "panel_showTimestampOptions";
            this.panel_showTimestampOptions.Size = new System.Drawing.Size(542, 57);
            this.panel_showTimestampOptions.TabIndex = 3;
            // 
            // labelPreviewTimeFormat
            // 
            this.labelPreviewTimeFormat.AutoSize = true;
            this.labelPreviewTimeFormat.Location = new System.Drawing.Point(246, 6);
            this.labelPreviewTimeFormat.Name = "labelPreviewTimeFormat";
            this.labelPreviewTimeFormat.Size = new System.Drawing.Size(122, 13);
            this.labelPreviewTimeFormat.TabIndex = 3;
            this.labelPreviewTimeFormat.Text = "labelPreviewTimeFormat";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Timestamp format";
            // 
            // textBox_timestampFormat
            // 
            this.textBox_timestampFormat.Location = new System.Drawing.Point(140, 3);
            this.textBox_timestampFormat.Name = "textBox_timestampFormat";
            this.textBox_timestampFormat.Size = new System.Drawing.Size(100, 20);
            this.textBox_timestampFormat.TabIndex = 2;
            this.textBox_timestampFormat.TextChanged += new System.EventHandler(this.textBox_timestampFormat_TextChanged);
            // 
            // checkBox_showTimestamp
            // 
            this.checkBox_showTimestamp.AutoSize = true;
            this.checkBox_showTimestamp.Location = new System.Drawing.Point(6, 19);
            this.checkBox_showTimestamp.Name = "checkBox_showTimestamp";
            this.checkBox_showTimestamp.Size = new System.Drawing.Size(107, 17);
            this.checkBox_showTimestamp.TabIndex = 0;
            this.checkBox_showTimestamp.Text = "Show Timestamp";
            this.checkBox_showTimestamp.UseVisualStyleBackColor = true;
            this.checkBox_showTimestamp.CheckedChanged += new System.EventHandler(this.checkBox_showTimestamp_CheckedChanged);
            // 
            // timerTimePreview
            // 
            this.timerTimePreview.Interval = 1000;
            this.timerTimePreview.Tick += new System.EventHandler(this.timerTimePreview_Tick);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(27, 32);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(117, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Hilfe: DateTimeFormats";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // ChatOptionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.groupBox1);
            this.Name = "ChatOptionPage";
            this.Size = new System.Drawing.Size(557, 115);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel_showTimestampOptions.ResumeLayout(false);
            this.panel_showTimestampOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel_showTimestampOptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_timestampFormat;
        private System.Windows.Forms.CheckBox checkBox_showTimestamp;
        private System.Windows.Forms.Label labelPreviewTimeFormat;
        private System.Windows.Forms.Timer timerTimePreview;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
