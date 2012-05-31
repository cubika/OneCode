/****************************** 模块头 ******************************\
* 模块名:  WebBrowserEx.cs
* 项目名:	    CSTabbedWebBrowser
* 版权 (c) Microsoft Corporation.
* 
* WebBrowserEx类 继承 WebBrowser类 并且 可以处理NewWindow3事件。 
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
using Microsoft.Win32;


namespace CSTabbedWebBrowser
{
    public partial class WebBrowserEx : WebBrowser
    {

        AxHost.ConnectionPointCookie cookie;

        DWebBrowserEvent2Helper helper;

        public event EventHandler<WebBrowserNewWindowEventArgs> NewWindow3;

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WebBrowserEx()
        {
        }

        /// <summary>
        /// 用一个客户端与ActiveX 控件联系起来，进行处理事件控件，包含 NewWindow3事件。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void CreateSink()
        {
            base.CreateSink();

            helper = new DWebBrowserEvent2Helper(this);
            cookie = new AxHost.ConnectionPointCookie(
                this.ActiveXInstance, helper, typeof(DWebBrowserEvents2));
        }


        /// <summary>
        /// 从底层ActiveX 控件 释放附属在CreateSink方法中的事件-处理客户端
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void DetachSink()
        {
            if (cookie != null)
            {
                cookie.Disconnect();
                cookie = null;
            }
            base.DetachSink();
        }


        /// <summary>
        ///  创建 NewWindow3 事件.
        /// </summary>
        protected virtual void OnNewWindow3(WebBrowserNewWindowEventArgs e)
        {                     
            if (this.NewWindow3 != null)
            {
                this.NewWindow3(this, e);
            }
        }
    }
}
