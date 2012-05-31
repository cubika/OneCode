/****************************** 模块头 ******************************\
* 模块名称:  MainForm.cs
* 项目名称:	    CSMultiThreadedWebDownloader
* 版权 (c) Microsoft Corporation.
* 
* 这是本应用程序的主要窗体，它用来初始化UI界面和处理相关的事件。
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
using System.Configuration;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace CSMultiThreadedWebDownloader
{
    public partial class MainForm : Form
    {
        MultiThreadedWebDownloader downloader = null;

        // 表明下载是否暂停。
        bool isPaused = false;

        DateTime lastNotificationTime;

        WebProxy proxy = null;

        public MainForm()
        {
            InitializeComponent();

            // 使用App.Config初始化proxy。
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyUrl"]))
            {
                proxy = new WebProxy(
                    System.Configuration.ConfigurationManager.AppSettings["ProxyUrl"]);

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyUser"])
                    && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyPwd"]))
                {
                    NetworkCredential credential = new NetworkCredential(
                        ConfigurationManager.AppSettings["ProxyUser"],
                        ConfigurationManager.AppSettings["ProxyPwd"]);

                    proxy.Credentials = credential;
                }
                else
                {
                    proxy.UseDefaultCredentials = true;
                }
            }
        }

        /// <summary>
        /// 检查文件信息。
        /// </summary>
        private void btnCheck_Click(object sender, EventArgs e)
        {

            // 初始化 MultiThreadedWebDownloader。
            downloader = new MultiThreadedWebDownloader(tbURL.Text);
            downloader.Proxy = this.proxy;

            try
            {
                downloader.CheckFile();

                //更新UI界面。
                tbURL.Enabled = false;
                btnCheck.Enabled = false;
                tbPath.Enabled = true;
                btnDownload.Enabled = true;
            }
            catch
            {
                // 如果有任何的异常，像System.Net.WebException 或者 
                // System.Net.ProtocolViolationException, 就说明读取文件信息时有错误，
                //并且该文件是不能被下载的。
                MessageBox.Show("获取文件信息有错误."
                   + " 请确保URL是有效的！");
            }
        }

        /// <summary>
        /// 处理btnDownload 单击事件。
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {

                // 检查文件是否存在。
                if (File.Exists(tbPath.Text.Trim()))
                {
                    string message = "已经存在一个同命名的文件，要覆盖它吗？"
                            + "如果不，请改变存储路径。";
                    var result = MessageBox.Show(
                        message,
                        "文件名冲突: " + tbPath.Text.Trim(),
                        MessageBoxButtons.OKCancel);

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        File.Delete(tbPath.Text.Trim());
                    }
                    else
                    {
                        return;
                    }
                }

                if (File.Exists(tbPath.Text.Trim()+".tmp"))
                {
                    File.Delete(tbPath.Text.Trim() + ".tmp");
                }

                // 设置下载的路径。
                downloader.DownloadPath = tbPath.Text.Trim() + ".tmp";


                // 加载 HttpDownloadClient事件。
                downloader.DownloadCompleted += new EventHandler<MultiThreadedWebDownloaderCompletedEventArgs>(
                    DownloadCompleted);
                downloader.DownloadProgressChanged +=
                    new EventHandler<MultiThreadedWebDownloaderProgressChangedEventArgs>(DownloadProgressChanged);
                downloader.StatusChanged += new EventHandler(StatusChanged);
                downloader.ErrorOccurred += new EventHandler<ErrorEventArgs>(ErrorOccurred);
                // 开始下载文件。
                downloader.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

       

        /// <summary>
        /// 处理StatusChanged 事件。
        /// </summary>
        void StatusChanged(object sender, EventArgs e)
        {
            //刷新 进度条.
            lbStatus.Text = downloader.Status.ToString();

            // 更新UI界面。
            switch (downloader.Status)
            {

                case MultiThreadedWebDownloaderStatus.Idle:
                case MultiThreadedWebDownloaderStatus.Canceled:
                case MultiThreadedWebDownloaderStatus.Completed:
                    btnCheck.Enabled = true;
                    btnDownload.Enabled = false;
                    btnPause.Enabled = false;
                    btnCancel.Enabled = false;
                    tbPath.Enabled = false;
                    tbURL.Enabled = true;
                    break;
                case MultiThreadedWebDownloaderStatus.Checked:
                    btnCheck.Enabled = false;
                    btnDownload.Enabled = true;
                    btnPause.Enabled = false;
                    btnCancel.Enabled = false;
                    tbPath.Enabled = true;
                    tbURL.Enabled = false;
                    break;
                case MultiThreadedWebDownloaderStatus.Downloading:
                    btnCheck.Enabled = false;
                    btnDownload.Enabled = false;
                    btnPause.Enabled = true;
                    btnCancel.Enabled = true;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
                case MultiThreadedWebDownloaderStatus.Pausing:
                case MultiThreadedWebDownloaderStatus.Canceling:
                    btnCheck.Enabled = false;
                    btnDownload.Enabled = false;
                    btnPause.Enabled = false;
                    btnCancel.Enabled = false;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
                case MultiThreadedWebDownloaderStatus.Paused:
                    btnCheck.Enabled = false;
                    btnDownload.Enabled = false;
                    btnPause.Enabled = true;
                    btnCancel.Enabled = false;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
            }

            if (downloader.Status == MultiThreadedWebDownloaderStatus.Paused)
            {
                lbSummary.Text =
                   String.Format("接收: {0}KB, 总共: {1}KB, 时间: {2}:{3}:{4}",
                   downloader.DownloadedSize / 1024, downloader.TotalSize / 1024,
                   downloader.TotalUsedTime.Hours, downloader.TotalUsedTime.Minutes,
                   downloader.TotalUsedTime.Seconds);
            }
        }

        /// <summary>
        /// 处理DownloadProgressChanged 事件。
        /// </summary>
        void DownloadProgressChanged(object sender, MultiThreadedWebDownloaderProgressChangedEventArgs e)
        {
            // 每隔一秒刷新一次主要信息 。
            if (DateTime.Now > lastNotificationTime.AddSeconds(1))
            {
                lbSummary.Text = String.Format("接收: {0}KB 总共: {1}KB 速度: {2}KB/s  线程个数: {3}",
                    e.ReceivedSize / 1024, e.TotalSize / 1024, e.DownloadSpeed / 1024,
                    downloader.DownloadThreadsCount);
                prgDownload.Value = (int)(e.ReceivedSize * 100 / e.TotalSize);
                lastNotificationTime = DateTime.Now;
            }
        }

        /// <summary>
        ///处理 DownloadCompleted事件。
        /// </summary>
        void DownloadCompleted(object sender, MultiThreadedWebDownloaderCompletedEventArgs e)
        {
            lbSummary.Text =
                String.Format("接收: {0}KB, 总共: {1}KB, 时间: {2}:{3}:{4}",
                e.DownloadedSize / 1024, e.TotalSize / 1024, e.TotalTime.Hours,
                e.TotalTime.Minutes, e.TotalTime.Seconds);

            File.Move(tbPath.Text.Trim() + ".tmp", tbPath.Text.Trim());
            
            prgDownload.Value = 100;
        }

        void ErrorOccurred(object sender, ErrorEventArgs e)
        {
            lbSummary.Text = e.Error.Message;
            prgDownload.Value = 0;
        }

        /// <summary>
        /// 处理 btnCancel 单击事件。
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            downloader.Cancel();
        }

        /// <summary>
        /// 处理 btnPause点击事件。
        /// </summary>
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (isPaused)
            {
                downloader.Resume();
                btnPause.Text = "暂停";
            }
            else
            {
                downloader.Pause();
                btnPause.Text = "继续";
            }
            isPaused = !isPaused;
        }

    }
}
