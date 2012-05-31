/************************************* Module Header *********************************\
 * 模块名称:        SaveProjectDialog.cs
 * 项目 :           CSVSXSaveProject
 * 版权所有（c）微软公司
 *
 * 这个对话框是用来显示项目中的所有文件，以及所有在项目内的文件夹 
 * 用户可以根据自己的需要来拷贝文件
 * 
 * The source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/default.aspx
 * All other rights reserved
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
 * EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.CSVSXSaveProject.Files;
using EnvDTE;
using System.Text;

namespace Microsoft.CSVSXSaveProject
{
    public partial class SaveProjectDialog : Form
    {
        #region 声明用于存储信息的变量。
        /// <summary>
        /// 项目文件夹的路径。
        /// </summary>
        public string OriginalFolderPath { get; set; }

        /// <summary>
        /// 您在文件夹浏览器对话框中所选择的文件夹的路径
        /// </summary>
        public string NewFolderPath { get; private set; }

        /// <summary>
        /// 项目中包含的文件，或是在项目文件夹下的文件。
        /// </summary>
        public List<Files.ProjectFileItem> FilesItems { get; set; }

        /// <summary>
        /// 指定新的项目是否应该打开。
        /// </summary>
        public bool OpenNewProject
        {
            get
            {
                return chkOpenProject.Checked;
            }
        }

        #endregion

        /// <summary>
        /// 构建SaveProject对话框。
        /// </summary>
        public SaveProjectDialog()
        {
            InitializeComponent();

            // 将自动生成的列设置为false。
            this.dgvFiles.AutoGenerateColumns = false;
        }

        /// <summary>
        /// 当点击按钮的时候，将项目“另存为”。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            //获取文件夹浏览器对话框中你想要保存的文件夹的路径。
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                // 将新建文件夹按钮设置为可用。
                dialog.ShowNewFolderButton = true;

                // 获取文件夹浏览器对话框的结果。
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // 获取文件夹路径。
                    this.NewFolderPath = dialog.SelectedPath;

                    // 拷贝用户选择的文件。
                    CopySelectedItems();

                    // 当按下“ok”按钮，关闭这个窗口。
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                    return;
                }
            }       
        }

        /// <summary>
        /// 保存项目对话框已经加载。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProjectDialog_Load(object sender, EventArgs e)
        {
            // 将数据源和文件项目关联起来。
            dgvFiles.DataSource = FilesItems;

            foreach (DataGridViewRow row in dgvFiles.Rows)
            {
                ProjectFileItem item = row.DataBoundItem as ProjectFileItem;

                row.Cells["colCopy"].ReadOnly = !item.IsUnderProjectFolder;
            }
        }

        #region 创建和复制文件  
        /// <summary>
        /// 拷贝你在DataGridView中选择的项目。
        /// </summary>
        private void CopySelectedItems()
        {
            // 从解决方案资源管理器中获取文件的信息。
            List<Files.ProjectFileItem> fileItems =
                dgvFiles.DataSource as List<Files.ProjectFileItem>;

            // 文件从原来的目录复制到新的路径。
            foreach (var fileItem in fileItems)
            {
                if(fileItem.IsUnderProjectFolder && fileItem.NeedCopy)
                {
                   // 获取你所保存的项目文件的绝对路径。
                    FileInfo newFile = new FileInfo(string.Format("{0}\\{1}",
                        NewFolderPath,
                        fileItem.FullName.Substring(OriginalFolderPath.Length)));

                    // 用文件全名创建新的目录。
                    if (!newFile.Directory.Exists)
                    {
                        Directory.CreateDirectory(newFile.Directory.FullName);
                    }

                    // 拷贝文件。
                    fileItem.Fileinfo.CopyTo(newFile.FullName);
                }       
            }
        }

        #endregion

        /// <summary>
        /// 取消“项目另存为”操作。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // 取消并关闭窗体。
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }  
    }
}
