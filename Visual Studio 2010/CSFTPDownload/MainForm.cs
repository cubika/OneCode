/******************************模块头 ******************************\
* 模块名:  MainForm.cs
* 项目名:	    CSFTPDownload
* 版权(c) Microsoft Corporation.
* 
* 这是这个应用程序的主窗体.它是用来初始化界面并处理事件的.
 * 
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
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace CSFTPDownload
{
    public partial class MainForm : Form
    {

        FTPClientManager client = null;

        NetworkCredential currentCredentials = null;

        public MainForm()
        {
            InitializeComponent();         
        }

        #region URL navigation

        /// <summary>
        /// 处理btnConnect按钮的单击事件.
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {


            //通过tbFTPServer.Text指定一个连接服务。
            Connect(this.tbFTPServer.Text.Trim());

        }

        void Connect(string urlStr)
        {
            try
            {
                Uri url = new Uri(urlStr);

                // URL的模式必须是FTP的. 
                if (!url.Scheme.Equals("ftp", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ApplicationException("URL的模式必须是FTP的. ");
                }


                // 把这个url转到这个文件夹并且这个文件夹包括这个文件。
                if (url.IsFile)
                {
                    url = new Uri(url, "..");
                }



                //显示窗体UICredentialsProvider获得新的凭据.
                using (UICredentialsProvider provider =
                    new UICredentialsProvider(this.currentCredentials))
                {

                    // 显示UICredentialsProvider作为一个对话框.
                    var result = provider.ShowDialog();

                    // 如果用户输入了凭据并且按下了 "确定" 按钮.
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {

                        //重置当前的凭据.
                        this.currentCredentials = provider.Credentials;

                    }
                    else
                    {
                        return;
                    }
                }

                // 初始化FTPClient实例.
                client = new FTPClientManager(url, currentCredentials);

                client.UrlChanged += new EventHandler(client_UrlChanged);
                client.StatusChanged += new EventHandler(client_StatusChanged);
                client.ErrorOccurred += new EventHandler<ErrorEventArgs>(client_ErrorOccurred);
                client.FileDownloadCompleted +=
                    new EventHandler<FileDownloadCompletedEventArgs>(client_FileDownloadCompleted);
                client.NewMessageArrived +=
                    new EventHandler<NewMessageEventArg>(client_NewMessageArrived);

                // 子目录和文件的列表.
                RefreshSubDirectoriesAndFiles();
            }


            catch (System.Net.WebException webEx)
            {
                if ((webEx.Response as FtpWebResponse).StatusCode == FtpStatusCode.NotLoggedIn)
                {
                    //重新连接服务器。
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
        /// 记录 FTPClient的消息.
        /// </summary>
        void client_NewMessageArrived(object sender, NewMessageEventArg e)
        {
            string log = string.Format("{0} {1}",
                 DateTime.Now, e.NewMessage);
            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        ///当一个文件被下载时记录FileDownloadCompleted事件
        /// </summary>
        void client_FileDownloadCompleted(object sender, FileDownloadCompletedEventArgs e)
        {
            string log = string.Format("{0} 下载从 {1} 到 {2} 完成. 长度: {3}. ",
                DateTime.Now, e.ServerPath, e.LocalFile.FullName, e.LocalFile.Length);

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// 如果是一个错误将记录ErrorOccurred事件。
        /// </summary>
        void client_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            this.lstLog.Items.Add(
                string.Format("{0} {1} ", DateTime.Now, e.ErrorException.Message));

            var innerException = e.ErrorException.InnerException;
            
            // 记录所有的innerException.
            while (innerException != null)
            {
                this.lstLog.Items.Add(
                              string.Format("\t\t\t {0} ", innerException.Message));
                innerException = innerException.InnerException;
            }

            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        ///如果 FTPClient的状态改变了，刷新UI。
        /// </summary>
        void client_StatusChanged(object sender, EventArgs e)
        {


            //如果客户端正在下载文件将禁用所有的按钮 
            if (client.Status == FTPClientManagerStatus.Downloading)
            {
                btnBrowseDownloadPath.Enabled = false;
                btnConnect.Enabled = false;
                btnDownload.Enabled = false;
                btnNavigateParentFolder.Enabled = false;
                lstFileExplorer.Enabled = false;
            }
            else
            {
                btnBrowseDownloadPath.Enabled = true;
                btnConnect.Enabled = true;
                btnDownload.Enabled = true;
                btnNavigateParentFolder.Enabled = true;
                lstFileExplorer.Enabled = true;
            }

            string log = string.Format("{0} FTPClient状态改变为 {1}. ",
               DateTime.Now, client.Status.ToString());

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// 处理FTPClient的UrlChanged事件。
        /// </summary>
        void client_UrlChanged(object sender, EventArgs e)
        {
            RefreshSubDirectoriesAndFiles();

            string log = string.Format("{0} 当前的URL改变为： {1}. ",
              DateTime.Now, client.Url);

            this.lstLog.Items.Add(log);
            this.lstLog.SelectedIndex = this.lstLog.Items.Count - 1;
        }

        /// <summary>
        /// 处理lstFileExplorer的 DoubleClick事件
        /// </summary>
        private void lstFileExplorer_DoubleClick(object sender, EventArgs e)
        {
            //如果只选择了一个项目并且这个项目代表了一个文件夹，然后导航到了一个子目录
            if (lstFileExplorer.SelectedItems.Count == 1
                && (lstFileExplorer.SelectedItem as FTPFileSystem).IsDirectory)
            {
                this.client.Naviagte(
                    (lstFileExplorer.SelectedItem as FTPFileSystem).Url);
            }
        }

        /// <summary>
        ///处理btnNavigateParentFolder的事件.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNavigateParentFolder_Click(object sender, EventArgs e)
        {

            //导航到上一级目录.
            this.client.NavigateParent();
        }

        /// <summary>
        ///子目录和文件的列表.
        /// </summary>
        void RefreshSubDirectoriesAndFiles()
        {
            lbCurrentUrl.Text = string.Format("当前的路径: {0}", client.Url);

            var subDirs = client.GetSubDirectoriesAndFiles();

            //列表的排序.
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

        #region Download File/Folders

        /// <summary>
        /// 处理btnBrowseDownloadPath的Click事件
        /// </summary>
        private void btnBrowseDownloadPath_Click(object sender, EventArgs e)
        {
            BrowserDownloadPath();
        }

        /// <summary>
        ///处理btnDownload的Click事件.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {

            //在文件资源管理器中一个或者多个文件或文件夹被选择。
            if (lstFileExplorer.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "在文件资源管理器中请选择一个或多个文件/文件夹",
                    "没有被选择的文件");
                return;
            }

            //如果这个tbDownloadPath.Text文本是空的时，就显示一个文件夹浏览对话框。
            if (string.IsNullOrWhiteSpace(tbDownloadPath.Text)
                && BrowserDownloadPath() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            
            var directoriesAndFiles = lstFileExplorer.SelectedItems.Cast<FTPFileSystem>();

            //下载这个选中的项目.
            client.DownloadDirectoriesAndFiles(directoriesAndFiles, tbDownloadPath.Text);

        }

        /// <summary>
        /// 显示一个文件夹浏览对话框.
        /// </summary>
        DialogResult BrowserDownloadPath()
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(tbDownloadPath.Text))
                {
                    folderBrowser.SelectedPath = tbDownloadPath.Text;
                }
                var result = folderBrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    tbDownloadPath.Text = folderBrowser.SelectedPath;
                }
                return result;
            }
        }
        #endregion

        private void 文件资源列表_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void grpDownload_Enter(object sender, EventArgs e)
        {

        }
    }
}
