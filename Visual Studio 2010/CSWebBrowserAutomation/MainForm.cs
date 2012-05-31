/****************************** 模块头 ******************************\
* 模块名:  MainForm.cs
* 项目名:	    CSWebBrowserAutomation
* 版权 (c) Microsoft Corporation.
* 
* 这是这个应用程序的主要部分.它是用来初始化UI和处理事件的。
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
using System.Security.Permissions;

namespace CSWebBrowserAutomation
{
    public partial class MainForm : Form
    {

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public MainForm()
        {
            InitializeComponent();

            // 注册事件.
            webBrowser.Navigating += new WebBrowserNavigatingEventHandler(webBrowser_Navigating);
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);       
        }

        /// <summary>
        /// 当webBrowser 在导航中的时候禁用btnAutoComplete。
        /// </summary>
        void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            btnAutoComplete.Enabled = false;
        }

        /// <summary>
        /// 当网页被加载后刷新UI。
        /// </summary>
        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            btnAutoComplete.Enabled = webBrowser.CanAutoComplete;
            tbUrl.Text = e.Url.ToString();           
        }
     
        /// <summary>
        /// 处理btnAutoComplete的Click事件。
        /// </summary>
        private void btnAutoComplete_Click(object sender, EventArgs e)
        {
             try
            {
                webBrowser.AutoComplete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 处理btnGo的Click事件。
        /// </summary>
        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                webBrowser.Navigate(tbUrl.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
