namespace CSTiffImageConverter
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chkIsMultipage = new System.Windows.Forms.CheckBox();
            this.btnConvertToTiff = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btnConvertToJpeg = new System.Windows.Forms.Button();
            this.dlgOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.chkIsMultipage);
            this.groupBox1.Controls.Add(this.btnConvertToTiff);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 126);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Jpeg -> Tiff";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(22, 18);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(303, 34);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "单击\"转换为 Tiff\" 按钮来浏览 jpeg 图像,将它们转换为 tiff 文件,并保存在相同的位置.";
            // 
            // chkIsMultipage
            // 
            this.chkIsMultipage.AutoSize = true;
            this.chkIsMultipage.Checked = true;
            this.chkIsMultipage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsMultipage.Location = new System.Drawing.Point(22, 60);
            this.chkIsMultipage.Name = "chkIsMultipage";
            this.chkIsMultipage.Size = new System.Drawing.Size(222, 16);
            this.chkIsMultipage.TabIndex = 2;
            this.chkIsMultipage.Text = "选中以创建多页 tiff （单个） 文件";
            this.chkIsMultipage.UseVisualStyleBackColor = true;
            // 
            // btnConvertToTiff
            // 
            this.btnConvertToTiff.Location = new System.Drawing.Point(70, 92);
            this.btnConvertToTiff.Name = "btnConvertToTiff";
            this.btnConvertToTiff.Size = new System.Drawing.Size(179, 21);
            this.btnConvertToTiff.TabIndex = 0;
            this.btnConvertToTiff.Text = "转换为 Tiff";
            this.btnConvertToTiff.UseVisualStyleBackColor = true;
            this.btnConvertToTiff.Click += new System.EventHandler(this.btnConvertToTiff_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.btnConvertToJpeg);
            this.groupBox2.Location = new System.Drawing.Point(8, 138);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(349, 141);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tiff -> Jpeg";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(23, 30);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(303, 34);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "单击\"转换为 Jpeg\" 按钮来浏览 tiff 图像，将它们转换为 jpeg 文件，并保存在相同的位置.";
            // 
            // btnConvertToJpeg
            // 
            this.btnConvertToJpeg.Location = new System.Drawing.Point(70, 91);
            this.btnConvertToJpeg.Name = "btnConvertToJpeg";
            this.btnConvertToJpeg.Size = new System.Drawing.Size(179, 21);
            this.btnConvertToJpeg.TabIndex = 1;
            this.btnConvertToJpeg.Text = "转换为 Jpeg";
            this.btnConvertToJpeg.UseVisualStyleBackColor = true;
            this.btnConvertToJpeg.Click += new System.EventHandler(this.btnConvertToJpeg_Click);
            // 
            // dlgOpenFileDialog
            // 
            this.dlgOpenFileDialog.Filter = "Image files (.jpg, .jpeg, .tif)|*.jpg;*.jpeg;*.tif;*.tiff";
            this.dlgOpenFileDialog.Multiselect = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 287);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "CSTiff图像转换器";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox chkIsMultipage;
        private System.Windows.Forms.Button btnConvertToTiff;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button btnConvertToJpeg;
        private System.Windows.Forms.OpenFileDialog dlgOpenFileDialog;

    }
}

