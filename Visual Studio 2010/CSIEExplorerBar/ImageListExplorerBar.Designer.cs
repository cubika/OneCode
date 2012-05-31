namespace CSIEExplorerBar
{
    partial class ImageListExplorerBar
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果托管资源应废弃则为真，否则为假。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// 支持设计器所需的方法 - 不要用代码编辑器修改这个方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlCmd = new System.Windows.Forms.Panel();
            this.btnGetImg = new System.Windows.Forms.Button();
            this.pnlImgList = new System.Windows.Forms.Panel();
            this.lstImg = new System.Windows.Forms.ListBox();
            this.pnlCmd.SuspendLayout();
            this.pnlImgList.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCmd
            // 
            this.pnlCmd.Controls.Add(this.btnGetImg);
            this.pnlCmd.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCmd.Location = new System.Drawing.Point(0, 0);
            this.pnlCmd.Name = "pnlCmd";
            this.pnlCmd.Size = new System.Drawing.Size(366, 36);
            this.pnlCmd.TabIndex = 0;
            // 
            // btnGetImg
            // 
            this.btnGetImg.Location = new System.Drawing.Point(16, 4);
            this.btnGetImg.Name = "btnGetImg";
            this.btnGetImg.Size = new System.Drawing.Size(129, 23);
            this.btnGetImg.TabIndex = 0;
            this.btnGetImg.Text = "获取所有图片";
            this.btnGetImg.UseVisualStyleBackColor = true;
            this.btnGetImg.Click += new System.EventHandler(this.btnGetImg_Click);
            // 
            // pnlImgList
            // 
            this.pnlImgList.Controls.Add(this.lstImg);
            this.pnlImgList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImgList.Location = new System.Drawing.Point(0, 36);
            this.pnlImgList.Name = "pnlImgList";
            this.pnlImgList.Size = new System.Drawing.Size(366, 412);
            this.pnlImgList.TabIndex = 1;
            // 
            // lstImg
            // 
            this.lstImg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstImg.FormattingEnabled = true;
            this.lstImg.Location = new System.Drawing.Point(0, 0);
            this.lstImg.Name = "lstImg";
            this.lstImg.Size = new System.Drawing.Size(366, 412);
            this.lstImg.TabIndex = 0;
            this.lstImg.DoubleClick += new System.EventHandler(this.lstImg_DoubleClick);
            // 
            // ImageListExplorerBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlImgList);
            this.Controls.Add(this.pnlCmd);
            this.Name = "ImageListExplorerBar";
            this.Size = new System.Drawing.Size(366, 448);
            this.pnlCmd.ResumeLayout(false);
            this.pnlImgList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCmd;
        private System.Windows.Forms.Button btnGetImg;
        private System.Windows.Forms.Panel pnlImgList;
        private System.Windows.Forms.ListBox lstImg;

    }
}
