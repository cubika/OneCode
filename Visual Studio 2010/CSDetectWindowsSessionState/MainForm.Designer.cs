using System.Security.Permissions;
namespace CSDetectWindowsSessionState
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
       [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void Dispose(bool disposing)
        {
            if (disposing && session != null)
            {
                session.Dispose();
            }

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
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkEnableTimer = new System.Windows.Forms.CheckBox();
            this.lbState = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lstRecord = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkEnableTimer);
            this.panel1.Controls.Add(this.lbState);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(783, 29);
            this.panel1.TabIndex = 0;
            // 
            // chkEnableTimer
            // 
            this.chkEnableTimer.AutoSize = true;
            this.chkEnableTimer.Location = new System.Drawing.Point(468, 7);
            this.chkEnableTimer.Name = "chkEnableTimer";
            this.chkEnableTimer.Size = new System.Drawing.Size(188, 17);
            this.chkEnableTimer.TabIndex = 1;
            this.chkEnableTimer.Text = "启动定时器每5秒检测会话状态";
            this.chkEnableTimer.UseVisualStyleBackColor = true;
            this.chkEnableTimer.CheckedChanged += new System.EventHandler(this.chkEnableTimer_CheckedChanged);
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Location = new System.Drawing.Point(13, 8);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(55, 13);
            this.lbState.TabIndex = 0;
            this.lbState.Text = "当前状态";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lstRecord);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(783, 149);
            this.panel2.TabIndex = 1;
            // 
            // lstRecord
            // 
            this.lstRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRecord.FormattingEnabled = true;
            this.lstRecord.Location = new System.Drawing.Point(0, 0);
            this.lstRecord.Name = "lstRecord";
            this.lstRecord.Size = new System.Drawing.Size(783, 149);
            this.lstRecord.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 178);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.Text = "检测Windows会话状态";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox lstRecord;
        private System.Windows.Forms.CheckBox chkEnableTimer;
    }
}

