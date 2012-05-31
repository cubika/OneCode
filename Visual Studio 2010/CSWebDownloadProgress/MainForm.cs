/******************************** 模块头 **************************************\
* 模块名:	MainForm.cs
* 项目名:   CSWebDownloadProgress
* 版权(c)   Microsoft Corporation.
* 
* 这是这个应用程序的主要窗体. 它用来初始化界面和处理事件.
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
using System.IO;
using System.Windows.Forms;

namespace CSWebDownloadProgress
{
    public partial class MainForm : Form
    {
        HttpDownloadClient client = null;

        // 指定下载是否已暂停.
        bool isPaused = false;

        DateTime lastNotificationTime;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 处理下载按钮点击事件.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {

            try
            {
                //构建临时文件路径.
                string tempPath = tbPath.Text.Trim() + ".tmp";

                // 检查文件是否存在.
                if (File.Exists(tempPath))
                {
                    string message = "已经存在重名文件，"
                        + "你想要删除它吗？如果不，请更改本地路径.";
                    var result = MessageBox.Show(message, "文件名冲突",
                        MessageBoxButtons.OKCancel);

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        File.Delete(tempPath);
                    }
                    else
                    {
                        return;
                    }
                }

                // 初始化一个HttpDownloadClient实例.
                // 首先存储文件到一个临时文件.
                client = new HttpDownloadClient(tbURL.Text,tempPath );

                //// 注册一个HttpDownloadClient事件.
                client.DownloadCompleted += new EventHandler<HttpDownloadCompletedEventArgs>(
                    DownloadCompleted);
                client.DownloadProgressChanged +=
                    new EventHandler<HttpDownloadProgressChangedEventArgs>(DownloadProgressChanged);
                client.StatusChanged += new EventHandler(StatusChanged);

                // 开始下载文件.
                client.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 处理StatusChanged事件.
        /// </summary>
        void StatusChanged(object sender, EventArgs e)
        {
            // 刷新状态.
            lbStatus.Text = client.Status.ToString();

            // 刷新按钮.
            switch (client.Status)
            {
                case HttpDownloadClientStatus.Idle:
                case HttpDownloadClientStatus.Canceled:
                case HttpDownloadClientStatus.Completed:
                    btnDownload.Enabled = true;
                    btnPause.Enabled = false;
                    btnCancel.Enabled = false;
                    tbPath.Enabled = true;
                    tbURL.Enabled = true;
                    break;
                case HttpDownloadClientStatus.Downloading:
                    btnDownload.Enabled = false;
                    btnPause.Enabled = true;
                    btnCancel.Enabled = true;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
                case HttpDownloadClientStatus.Pausing:
                case HttpDownloadClientStatus.Canceling:
                    btnDownload.Enabled = false;
                    btnPause.Enabled = false;
                    btnCancel.Enabled = false;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
                case HttpDownloadClientStatus.Paused:
                    btnDownload.Enabled = false;
                    btnPause.Enabled = true;
                    btnCancel.Enabled = false;
                    tbPath.Enabled = false;
                    tbURL.Enabled = false;
                    break;
            }

            if (client.Status == HttpDownloadClientStatus.Paused)
            {
                lbSummary.Text =
                   String.Format("已接收: {0}KB, 总共: {1}KB, 时间: {2}:{3}:{4}",
                   client.DownloadedSize / 1024, client.TotalSize / 1024,
                   client.TotalUsedTime.Hours, client.TotalUsedTime.Minutes,
                   client.TotalUsedTime.Seconds);
            }
        }

        /// <summary>
        /// 处理DownloadProgressChanged事件.
        /// </summary>
        void DownloadProgressChanged(object sender, HttpDownloadProgressChangedEventArgs e)
        {
            // 每秒刷新摘要.
            if (DateTime.Now > lastNotificationTime.AddSeconds(1))
            {
                lbSummary.Text = String.Format("已接收: {0}KB, 总共: {1}KB, 速度: {2}KB/s",
                    e.ReceivedSize / 1024, e.TotalSize / 1024, e.DownloadSpeed / 1024);
                prgDownload.Value = (int)(e.ReceivedSize * 100 / e.TotalSize);
                lastNotificationTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 处理DownloadCompleted事件.
        /// </summary>
        void DownloadCompleted(object sender, HttpDownloadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                lbSummary.Text =
                    String.Format("已接收: {0}KB, 总共: {1}KB, 时间: {2}:{3}:{4}",
                    e.DownloadedSize / 1024, e.TotalSize / 1024, e.TotalTime.Hours,
                    e.TotalTime.Minutes, e.TotalTime.Seconds);

                if (File.Exists(tbPath.Text.Trim()))
                {
                    File.Delete(tbPath.Text.Trim());
                }

                File.Move(tbPath.Text.Trim() + ".tmp", tbPath.Text.Trim());
                prgDownload.Value = 100;
            }
            else
            {
                lbSummary.Text = e.Error.Message;
                if (File.Exists(tbPath.Text.Trim() + ".tmp"))
                {
                    File.Delete(tbPath.Text.Trim() + ".tmp");
                }
                prgDownload.Value = 0;
            }

        }

        /// <summary>
        /// 处理删除按钮点击事件.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            client.Cancel();
        }

        /// <summary>
        /// 处理暂停按钮点击事件.
        /// </summary>
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (isPaused)
            {
                client.Resume();
                btnPause.Text = "暂停";
            }
            else
            {
                client.Pause();
                btnPause.Text = "重新开始";
            }
            isPaused = !isPaused;
        }
    }
}
