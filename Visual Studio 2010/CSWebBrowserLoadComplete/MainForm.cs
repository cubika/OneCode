/********************************** 模块头 ********************************\
 * 模块名:  MainForm.cs
 * 项目名:  CSWebBrowserLoadComplete
 * 版权 (c) Microsoft Corporation.
 * 
 * 这是该应用程序的主要窗体,用于初始化用户界面和处理事件.
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
using System.Security.Permissions;
using System.Windows.Forms;

namespace CSWebBrowserLoadComplete
{
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public partial class MainForm : Form
    {

        // DocumentCompleted 事件被引发次数的计数.
        int documentCompletedCount = 0;

        //DocumentCompleted 事件被引发次数的计数.
        int loadCompletedCount = 0;

        public MainForm()
        {
            InitializeComponent();

            // 登记 System.Windows.Forms.WebBrowser 的事件.
            webEx.DocumentCompleted += webEx_DocumentCompleted;           
            webEx.Navigating += webEx_Navigating;
            webEx.Navigated += webEx_Navigated;

            // 登记 WebBrowserEx 的事件.
            webEx.StartNavigating += webEx_StartNavigating;
            webEx.LoadCompleted += webEx_LoadCompleted;
            
            this.tbURL.Text = string.Format("{0}\\Resource\\FramesPage.htm",
                Environment.CurrentDirectory);

        }

        /// <summary>
        /// 导航到一个 URL.
        /// </summary>
        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                Uri url = new Uri(tbURL.Text);
                webEx.Navigate(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void webEx_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            DisplayStatus("正在导航: " + e.Url);
        }

        private void webEx_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            DisplayStatus("完成导航: " + e.Url);
        }

        private void webEx_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            documentCompletedCount++;
            DisplayStatus("已完成文档: " + e.Url);
        }

        private void webEx_LoadCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            loadCompletedCount++;
            DisplayStatus("加载完成: " + e.Url);
        }


        private void webEx_StartNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            documentCompletedCount = 0;
            loadCompletedCount = 0;
            DisplayStatus("开始导航: " + e.Url);
        }

        /// <summary>
        /// 显示消息.
        /// </summary>
        private void DisplayStatus(string msg)
        {
            DateTime now = DateTime.Now;

            lstActivities.Items.Insert(0,
                string.Format("{0:HH:mm:ss}:{1:000} {2}", 
                now,now.Millisecond,msg));

            lstActivities.SelectedIndex = 0;

            this.lbStatus.Text = string.Format(
                "已完成文档:{0} 加载完成:{1}",
                documentCompletedCount, loadCompletedCount);
        }

    }
}
