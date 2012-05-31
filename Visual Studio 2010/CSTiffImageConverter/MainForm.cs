/****************************** 模块头 ***********************************\ 
* 模块名:    MainForm.cs 
* 项目名:    CSTiffImageConverter
* 版权 (c) Microsoft Corporation. 
* 
* 该实例演示了如果实现JPEG图像与TIFF图像之间的相互转换. 此示例还允许从所选的 JPEG 
* 图像创建一个多页 TIFF 图像.
* 
* TIFF (原本代表的是标签图像文件格式），是一种灵活的、 适应性强的文件格式，可用来
* 处理单个文件中的图像和数据，通过文件头标签（大小、定义、图像数据的安排、应用图
* 像压缩）来定义图像几何.例如，一个TIFF 文件可以是一个容器来容纳压缩 （有损）JPEG 
* 和（无损） PackBits 压缩图像.TIFF 文件也可以包括基于矢量的剪切路径(轮廓、裁剪部
* 分 、图像帧）.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace CSTiffImageConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnConvertToTiff_Click(object sender, EventArgs e)
        {
            dlgOpenFileDialog.Multiselect = true;
            dlgOpenFileDialog.Filter = "Image files (.jpg, .jpeg)|*.jpg;*.jpeg";

            if (dlgOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TiffImageConverter.ConvertJpegToTiff(dlgOpenFileDialog.FileNames, chkIsMultipage.Checked);
                    MessageBox.Show("图像转换完成.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("出现错误: " + ex.Message, "错误");
                }
            }
        }

        private void btnConvertToJpeg_Click(object sender, EventArgs e)
        {
            dlgOpenFileDialog.Multiselect = false;
            dlgOpenFileDialog.Filter = "Image files (.tif, .tiff)|*.tif;*.tiff";

            if (dlgOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TiffImageConverter.ConvertTiffToJpeg(dlgOpenFileDialog.FileName);
                    MessageBox.Show("图像转换完成.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("出现错误: " + ex.Message, "错误");
                }
            }
        }
    }
}
