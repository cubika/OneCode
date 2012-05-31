/****************************** 模块头 ******************************\
模块名:  MainForm.cs
项目名:      CSManipulateImagesInWordDocument
版权 (c) Microsoft Corporation.
 
这是应用程序的主窗口
 
 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Drawing;

namespace CSManipulateImagesInWordDocument
{
    public partial class MainForm : Form
    {
        WordDocumentImageManipulator documentManipulator;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 处理 btnOpenFile 点击事件.
        /// </summary>
        private void btnOpenFile_Click(object sender, EventArgs e)
        {

            // 打开一个OpenFileDialog实例
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Word 文档 (*.docx)|*.docx";

                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        lstImage.Items.Clear();
                        if (picView.Image != null)
                        {
                            picView.Image.Dispose();
                        }
                        picView.Image = null;
                        lbFileName.Text = string.Empty;


                        // 初始化一个WordDocumentImageManipulator 实例.
                        OpenWordDocument(dialog.FileName);

                        // 更新 lstImage listbox.
                        UpdateImageList();

                        lbFileName.Text = dialog.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化一个WordDocumentImageManipulator实例.
        /// </summary>
        void OpenWordDocument(string filepath)
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
            {
                throw new ArgumentException("文件路径");
            }

            FileInfo file = new FileInfo(filepath);

            // 释放之前实例的资源
            if (documentManipulator != null)
            {
                documentManipulator.Dispose();
            }

            documentManipulator = new WordDocumentImageManipulator(file);

            // 注册 ImagesChanged事件.
            documentManipulator.ImagesChanged += new EventHandler(documentManipulator_ImagesChanged);

        }

        /// <summary>
        /// 更新 lstImage listbox.
        /// </summary>
        void UpdateImageList()
        {
            if (picView.Image != null)
            {
                picView.Image.Dispose();
            }
            picView.Image = null;

            lstImage.Items.Clear();

            // 显示Blip元素的 “嵌入”属性. 这个属性是ImagePart 的ID的引用           
            lstImage.DisplayMember = "嵌入";
            foreach (var blip in documentManipulator.GetAllImages())
            {
                lstImage.Items.Add(blip);
            }
        }

        /// <summary>
        /// 处理 ImagesChanged 事件.
        /// </summary>
        void documentManipulator_ImagesChanged(object sender, EventArgs e)
        {
            UpdateImageList();
        }

        /// <summary>
        /// 处理 lstImage SelectedIndexChanged event 来显示 picView 中的图片        
        /// </summary>
        private void lstImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var imgBlip = lstImage.SelectedItem as Blip;
            if (imgBlip == null)
            {
                return;
            }

            // 释放在picView中之前的图片的资源
            if (picView.Image != null)
            {
                picView.Image.Dispose();
                picView.Image = null;
            }

            try
            {
                var newImg = documentManipulator.GetImageInBlip(imgBlip);
                picView.Image = new Bitmap(newImg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 处理 btnDelete click 事件.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstImage.SelectedItem != null)
            {
                var result = MessageBox.Show(
                    "你想删除这张图片吗？",
                    "删除图片",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        documentManipulator.DeleteImage(lstImage.SelectedItem as Blip);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 处理 btnReplace Click 事件.
        /// </summary>
        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (lstImage.SelectedItem != null)
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "图片文件 (*.jpeg;*.jpg;*.png)|*.jpeg;*.jpg;*.png";

                    var result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        var confirm = MessageBox.Show(
                            "你想替换这张图片吗？",
                            "替换图片",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if (confirm == System.Windows.Forms.DialogResult.Yes)
                        {
                            try
                            {
                                documentManipulator.ReplaceImage(
                                    lstImage.SelectedItem as Blip,
                                    new FileInfo(dialog.FileName));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 处理 btnExport Click 事件.
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (lstImage.SelectedItem != null && picView.Image != null)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "图片文件 (*.jpeg)|*.jpeg";
                    var result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        picView.Image.Save(dialog.FileName, ImageFormat.Jpeg);
                    }
                }
            }
        }
    }
}
