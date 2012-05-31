namespace CSRichTextBoxSyntaxHighlighting 
{
    partial class MainForm
    {
        /// <summary>
        /// 设计器必需的变量
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在被使用的资源
        /// </summary>
        /// <param name="disposing">true 如果托管的资源要被处理; 不然, false.</param>
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
        /// 支持设计器的必需方法-不用代码编辑器修改这个方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            CSRichTextBoxSyntaxHighlighting.XMLViewerSettings xmlViewerSettings1 = new CSRichTextBoxSyntaxHighlighting.XMLViewerSettings();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.lbNote = new System.Windows.Forms.Label();
            this.btnProcess = new System.Windows.Forms.Button();
            this.viewer = new CSRichTextBoxSyntaxHighlighting.XMLViewer();
            this.pnlMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMenu
            // 
            this.pnlMenu.Controls.Add(this.lbNote);
            this.pnlMenu.Controls.Add(this.btnProcess);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(775, 73);
            this.pnlMenu.TabIndex = 1;
            // 
            // lbNote
            // 
            this.lbNote.AutoSize = true;
            this.lbNote.Location = new System.Drawing.Point(12, 8);
            this.lbNote.Name = "lbNote";
            this.lbNote.Size = new System.Drawing.Size(353, 60);
            this.lbNote.TabIndex = 2;
            this.lbNote.Text = "将 xml 脚本复制、 粘贴到下面的 RichTextBox中，然后按\"处理\"\r\n注：\r\n1.此查看器并不支持有 Namespace 的 Xml 文件\r\n2.此" +
                "查看器将忽略 XML 中的注释。\r\n3.某些字符应进行编码，像 &&-&";
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(420, 35);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 21);
            this.btnProcess.TabIndex = 1;
            this.btnProcess.Text = "处理";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // viewer
            // 
            this.viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewer.Location = new System.Drawing.Point(0, 73);
            this.viewer.Name = "viewer";
            xmlViewerSettings1.AttributeKey = System.Drawing.Color.Red;
            xmlViewerSettings1.AttributeValue = System.Drawing.Color.Blue;
            xmlViewerSettings1.Element = System.Drawing.Color.DarkRed;
            xmlViewerSettings1.Tag = System.Drawing.Color.Blue;
            xmlViewerSettings1.Value = System.Drawing.Color.Black;
            this.viewer.Settings = xmlViewerSettings1;
            this.viewer.Size = new System.Drawing.Size(775, 352);
            this.viewer.TabIndex = 0;
            this.viewer.Text = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><html><head><title>My home page</title></h" +
                "ead><body bgcolor=\"000000\" text=\"ff0000\">Hello World!</body></html>\n";
            this.viewer.TextChanged += new System.EventHandler(this.viewer_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 425);
            this.Controls.Add(this.viewer);
            this.Controls.Add(this.pnlMenu);
            this.Name = "MainForm";
            this.Text = "SimpleXMLViewer";
            this.pnlMenu.ResumeLayout(false);
            this.pnlMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private XMLViewer viewer;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Label lbNote;
    }
}

