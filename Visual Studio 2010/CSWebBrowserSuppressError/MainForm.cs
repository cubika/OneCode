/****************************** 模块头 *************************************\
* 模块名:   MainForm.cs
* 项目名:	CSWebBrowserSuppressError
* 版权(c)   Microsoft Corporation.
* 
* 这是这个应用程序的主窗体.它是用来初始化界面并处理事件的.
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

namespace CSWebBrowserSuppressError
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 初始化浏览器的ScriptErrorsSuppressed和HtmlElementErrorsSuppressed属性.
            this.wbcSample.ScriptErrorsSuppressed = chkSuppressAllDialog.Checked;
            this.wbcSample.HtmlElementErrorsSuppressed = chkSuppressHtmlElementError.Checked;

            // 给Web浏览器控件的链接错误事件添加一个处理程序.
            this.wbcSample.NavigateError += 
                new EventHandler<WebBrowserNavigateErrorEventArgs>(wbcSample_NavigateError);

            // 获取键值HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main中的
            // DisableJITDebugger的当前值
            this.chkSuppressJITDebugger.Checked = CSWebBrowserSuppressError.WebBrowserEx.JITDebuggerDisabled;
            this.chkSuppressJITDebugger.CheckedChanged += 
                new System.EventHandler(this.chkSuppressJITDebugger_CheckedChanged);
        }


        /// <summary>
        /// 处理btnNavigate_Click事件.
        /// 如果tbUrl的内容不为空,则会链接到填写的url,否则链接到内建的Error.htm页面.
        /// </summary>
        private void btnNavigate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(tbUrl.Text.Trim()))
                {
                    wbcSample.Navigate(tbUrl.Text);
                }
                else
                {
                    wbcSample.Navigate(Environment.CurrentDirectory + @"\HTMLPages\Error.htm");
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("请确保您输入了有效的url.");
            }
        }


        /// <summary>
        /// 如果chkSuppressJITDebugger.Checked的值改变,则启用或禁用Web浏览器的JIT调试器.
        /// </summary>
        private void chkSuppressJITDebugger_CheckedChanged(object sender, EventArgs e)
        {
            WebBrowserEx.JITDebuggerDisabled = chkSuppressJITDebugger.Checked;
            MessageBox.Show("您必须重启应用程序来启用或禁用脚本调试器.");
        }

        /// <summary>
        /// 如果chkSuppressHtmlElementError.Checked的值改变,则设置Web浏览器属性
        /// HtmlElementErrorsSuppressed的值.
        /// </summary>
        private void chkSuppressHtmlElementError_CheckedChanged(object sender, EventArgs e)
        {
            wbcSample.HtmlElementErrorsSuppressed = chkSuppressHtmlElementError.Checked;
        }

        /// <summary>
        /// 如果chkSuppressAllDialog.Checked的值改变,则设置Web浏览器属性
        /// ScriptErrorsSuppressed的值
        /// </summary>
        private void chkSuppressAllDialog_CheckedChanged(object sender, EventArgs e)
        {
            wbcSample.ScriptErrorsSuppressed = chkSuppressAllDialog.Checked;
        }

        /// <summary>
        /// 处理链接错误.
        /// </summary>
        void wbcSample_NavigateError(object sender, WebBrowserNavigateErrorEventArgs e)
        {
            // 如果http状态代码是404,则链接到内建的404.htm页面.
            if (chkSuppressNavigationError.Checked && e.StatusCode == 404)
            {
                wbcSample.Navigate(string.Format(@"{0}\HTMLPages\404.htm", 
                    Environment.CurrentDirectory));
            }
        }

    }
}
