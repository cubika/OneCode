/****************************** 模块头 ******************************\
* 模块名:  MainForm.cs
* 项目:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
* 应用程序的主窗体，用来初始化UI和处理事件。
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace CSFTPUpload
{
    public partial class MainForm : Form
    {

        FTPClientManager client = null;

        NetworkCredential currentCredentials = null;

        public MainForm()
        {
            InitializeComponent();

            RefreshUI();
        }

        #region URL navigation

        /// <summary>
        /// 处理btnConnect单击事件
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {


            //通过tbFTPServer.Text连接服务器
            Connect(this.tbFTPServer.Text.Trim());

        }

        void Connect(string urlStr)
        {
            try
            {
                Uri url = new Uri(urlStr);


                //地址必须是ftp格式。
                if (!url.Scheme.Equals("ftp", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ApplicationException("地址格式必须为 ftp. ");
                }


                //设置包含文件的文件夹地址。
                if (url.IsFile)
                {
                    url = new Uri(url, "..");
                }

              
                //显示UICredentialsProvider窗体得到新认证。
                using (UICredentialsProvider provider = 
                    new UICredentialsProvider(this.currentCredentials))
                {


                    //显示UICredentialsProvider窗体为一个对话框
                    var result = provider.ShowDialog();

                
                    //假如用户输入认证 ，按“OK”按钮
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {

   
                        //重设当前认证。
                        this.currentCredentials = provider.Credentials;

                    }
                    else
                    {
                        return;
                    }
                }


                //初始化FTP客户端实例。
                client = new FTPClientManager(url, currentCredentials);

                client.UrlChanged += new EventHandler(client_UrlChanged);
                client.StatusChanged += new EventHandler(client_StatusChanged);
                client.ErrorOccurred += new EventHandler<ErrorEventArgs>(client_ErrorOccurred);
                client.FileUploadCompleted +=
                    new EventHandler<FileUploadCompletedEventArgs>(client_FileUploadCompleted);
                client.NewMessageArrived +=
                    new EventHandler<NewMessageEventArg>(client_NewMessageArrived);

                //刷新UI和列出子目录和文件
                RefreshUI();
            }


            catch (System.Net.WebException webEx)
            {
                if ((webEx.Response as FtpWebResponse).StatusCode == FtpStatusCode.NotLoggedIn)
                {
                       //重新连接服务器
                        Connect(urlStr);

                        return;
                }
                else
                {
                    MessageBox.Show(webEx.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 记录FTPClient信息
        /// </summary>
        void client_NewMessageArrived(object sender, NewMessageEventArg e)
        {
            string log = string.Format("{0} {1}",
                 DateTime.Now, e.NewMessage);
            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// 当一个文件被上传记录FileUploadCompleted事件
        /// </summary>
        void client_FileUploadCompleted(object sender, FileUploadCompletedEventArgs e)
        {
            string log = string.Format("{0} 上传 {1} 到 {2}成功. 长度: {3}. ",
                DateTime.Now, e.LocalFile.FullName, e.ServerPath, e.LocalFile.Length);

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// 假如有一个错误记录ErrorOccurred事件。
        /// </summary>
        void client_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            this.lstLog.Items.Add(
                string.Format("{0} {1} ", DateTime.Now, e.ErrorException.Message));

            var innerException = e.ErrorException.InnerException;

            //记录所有的innerException
            while (innerException != null)
            {
                this.lstLog.Items.Add(
                              string.Format("\t\t\t {0} ", innerException.Message));
                innerException = innerException.InnerException;
            }

            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// 加入FTPClient状态改变刷新UI。
        /// </summary>
        void client_StatusChanged(object sender, EventArgs e)
        {
            RefreshUI();

            string log = string.Format("{0} FTPClient 状态改变到 {1}. ",
             DateTime.Now, client.Status.ToString());

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        void RefreshUI()
        {
            //假如客户端正上传文件，所有按钮为不好用。
            if (client == null ||
                client.Status != FTPClientManagerStatus.Idle)
            {

                btnBrowseLocalFolder.Enabled = false;
                btnUploadFolder.Enabled = false;

                btnBrowseLocalFile.Enabled = false;
                btnUploadFile.Enabled = false;

                btnDelete.Enabled = false;

                btnNavigateParentFolder.Enabled = false;
                lstFileExplorer.Enabled = false;
            }
            else
            {

                btnBrowseLocalFolder.Enabled = true;
                btnUploadFolder.Enabled = true;

                btnBrowseLocalFile.Enabled = true;
                btnUploadFile.Enabled = true;

                btnDelete.Enabled = true;

                btnNavigateParentFolder.Enabled = true;
                lstFileExplorer.Enabled = true;
            }

            btnConnect.Enabled = client == null ||
                client.Status == FTPClientManagerStatus.Idle;

            RefreshSubDirectoriesAndFiles();

        }

        /// <summary>
        /// 处理FTPClient的UrlChanged事件
        /// </summary>
        void client_UrlChanged(object sender, EventArgs e)
        {
            RefreshSubDirectoriesAndFiles();

            string log = string.Format("{0} 当前路径改变到 {1}. ",
              DateTime.Now, client.Url);

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// 处理1stFileExplorer双击事件
        /// </summary>
        private void lstFileExplorer_DoubleClick(object sender, EventArgs e)
        {

            //假如一个选项被选择，这个选项代表一个文件夹，导航到一个子目录。
            if (lstFileExplorer.SelectedItems.Count == 1
                && (lstFileExplorer.SelectedItem as FTPFileSystem).IsDirectory)
            {
                this.client.Naviagte(
                    (lstFileExplorer.SelectedItem as FTPFileSystem).Url);
            }
        }

        /// <summary>
        /// 处理btnNavigateParentFolder单击事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNavigateParentFolder_Click(object sender, EventArgs e)
        {

            //导航到父文件夹
            this.client.NavigateParent();
        }

        /// <summary>
        /// 列出子目录和文件
        /// </summary>
        void RefreshSubDirectoriesAndFiles()
        {
            if (client == null)
            {
                return;
            }

            lbCurrentUrl.Text = string.Format("当前路径: {0}", client.Url);

            var subDirs = client.GetSubDirectoriesAndFiles();

            //排序列表
            var orderedDirs = from dir in subDirs
                              orderby dir.IsDirectory descending, dir.Name
                              select dir;

            lstFileExplorer.Items.Clear();
            foreach (var subdir in orderedDirs)
            {
                lstFileExplorer.Items.Add(subdir);
            }
        }


        #endregion

        #region Upload a Folder

        /// <summary>
        /// 处理btnBrowseLocalFolder单击事件
        /// </summary>
        private void btnBrowseLocalFolder_Click(object sender, EventArgs e)
        {
            BrowserLocalFolder();
        }

        /// <summary>
        /// 处理btnUploadFolder单击事件
        /// </summary>
        private void btnUploadFolder_Click(object sender, EventArgs e)
        {

            //如果tbLocalFolder.Text 为空，显示一个 FolderBrowserDialog
            if (string.IsNullOrWhiteSpace(tbLocalFolder.Text)
                && BrowserLocalFolder() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            try
            {
                DirectoryInfo dir = new DirectoryInfo(tbLocalFolder.Text);

                if (!dir.Exists)
                {
                    throw new ApplicationException(
                        string.Format(" The folder {0} does not exist!", dir.FullName));
                }
                //上传选项
                client.UploadFolder(dir, client.Url, chkCreateFolder.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 显示一个 FolderBrowserDialog.
        /// </summary>
        DialogResult BrowserLocalFolder()
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(tbLocalFolder.Text))
                {
                    folderBrowser.SelectedPath = tbLocalFolder.Text;
                }
                var result = folderBrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    tbLocalFolder.Text = folderBrowser.SelectedPath;
                }
                return result;
            }
        }

        #endregion


        #region Upload files

        private void btnBrowseLocalFile_Click(object sender, EventArgs e)
        {
            BrowserLocalFiles();
        }

        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (tbLocalFile.Tag == null
                && BrowserLocalFiles() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            try
            {
                List<FileInfo> files = new List<FileInfo>();
                string[] selectedFiles = tbLocalFile.Tag as string[];

                foreach (var selectedFile in selectedFiles)
                {
                    FileInfo fileInfo = new FileInfo(selectedFile);
                    if (!fileInfo.Exists)
                    {
                        throw new ApplicationException(
                            string.Format(" 文件 {0} 不存在!", selectedFile));
                    }
                    else
                    {
                        files.Add(fileInfo);
                    }
                }

                if (files.Count > 0)
                {
                    client.UploadFoldersAndFiles(files, client.Url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>

        /// 展示一个FolderBrowserDialog
        /// </summary>
        DialogResult BrowserLocalFiles()
        {
            using (OpenFileDialog fileBrowser = new OpenFileDialog())
            {
                fileBrowser.Multiselect = true;
                var result = fileBrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    tbLocalFile.Tag = fileBrowser.FileNames;

                    StringBuilder filesText = new StringBuilder();
                    foreach (var file in fileBrowser.FileNames)
                    {
                        filesText.Append(file + ";");
                    }
                    tbLocalFile.Text = filesText.ToString();
                }
                return result;
            }
        }


        #endregion

        #region Delete files

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstFileExplorer.SelectedItems.Count == 0)
            {
                MessageBox.Show("请在FTP文件管理器中选择删除项");
            }

            var itemsToDelete = lstFileExplorer.SelectedItems.Cast<FTPFileSystem>();

            this.client.DeleteItemsOnFTPServer(itemsToDelete);

            RefreshUI();
        }

        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

    }
}
