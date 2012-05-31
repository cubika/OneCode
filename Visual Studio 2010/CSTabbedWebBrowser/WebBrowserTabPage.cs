/****************************** 模块头 ******************************\
* 模块名:  WebBrowserTabPage.cs
* 项目名:	    CSTabbedWebBrowser
* 版权 (c) Microsoft Corporation.
* 
* 这个类继承System.Windows.Forms.TabPage类，并且包含一个WebBrowserEx属性。
* WebBrowserTabPage类的一个实例可以直接添加标签控件。
* 
* 它可以显示WebBrowserEx类中的NewWindow3事件，并且处理DocumentTitleChanged事件。
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

namespace CSTabbedWebBrowser
{
    public class WebBrowserTabPage : TabPage
    {
        public WebBrowserEx WebBrowser { get; private set; }

        // 显示WebBrowserEx类中的NewWindow3事件.
        public event EventHandler<WebBrowserNewWindowEventArgs> NewWindow
        {
            add
            {
                WebBrowser.NewWindow3 += value;
            }
            remove
            {
                WebBrowser.NewWindow3 -= value;
            }
        }

        /// <summary>
        /// 初始化WebBrowserEx的实例。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WebBrowserTabPage()
            : base()
        {
            WebBrowser = new WebBrowserEx();
            WebBrowser.Dock = DockStyle.Fill;
            WebBrowser.DocumentTitleChanged += new EventHandler(WebBrowser_DocumentTitleChanged);

            this.Controls.Add(WebBrowser);
        }

        /// <summary>
        /// 改变标签的题目.
        /// </summary>
        void WebBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.Text = WebBrowser.DocumentTitle;
        }

    }
}
