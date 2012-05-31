/****************************** 模块头 ******************************\
* 模块名:  MainForm.cs
* 项目名:	    CSTabbedWebBrowser
* 版权(c) Microsoft Corporation.
* 
* 是应用程序的主形式. 它是用来初始化用户界面和处理时间的。
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
using System.Windows.Forms;

namespace CSTabbedWebBrowser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // 初始化复选框.
            chkEnableTab.Checked = TabbedWebBrowserContainer.IsTabEnabled;

            chkEnableTab.CheckedChanged+=new EventHandler(chkEnableTab_CheckedChanged);

        }

        /// <summary>
        /// 处理tbUrl的KeyDown事件。当键值输入后，导航在给tbUrl网址
        /// </summary>
        private void tbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                webBrowserContainer.Navigate(tbUrl.Text);
            }
        }

        /// <summary>
        /// 当点击后“退按”钮时，处理事件。
        /// </summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            webBrowserContainer.GoBack();
        }

        /// <summary>
        /// 当点击“向前”按钮时，处理事件。
        /// </summary>
        private void btnForward_Click(object sender, EventArgs e)
        {
            webBrowserContainer.GoForward();
        }

        /// <summary>
        /// 当点击后刷新钮时，处理事件。
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            webBrowserContainer.RefreshWebBrowser();
        }

        /// <summary>
        /// 当点击“新建标签”按钮时，处理事件。
        /// </summary>
        private void btnNewTab_Click(object sender, EventArgs e)
        {
            webBrowserContainer.NewTab("about:blank");
        }

        /// <summary>
        /// 当点击“关闭标签”按钮时，处理事件。
        /// </summary>
        private void btnCloseTab_Click(object sender, EventArgs e)
        {
            webBrowserContainer.CloseActiveTab();
        }

        /// <summary>
        /// 处理chkEnableTab的 CheckedChanged 事件。
        /// </summary>
        private void chkEnableTab_CheckedChanged(object sender, EventArgs e)
        {
            TabbedWebBrowserContainer.IsTabEnabled = chkEnableTab.Checked;
            MessageBox.Show("这个上下文菜单\"启动选项卡\"在应用程序重新启动之后才会生效。");
            Application.Restart();
        }

    }
}
