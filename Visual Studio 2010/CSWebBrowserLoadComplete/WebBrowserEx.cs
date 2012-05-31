/********************************* 模块头 **********************************\
 * 模块名:  WebBrowserEx.cs
 * 项目名:  CSWebBrowserLoadComplete
 * 版权 (c) Microsoft Corporation.
 * 
 * WebBrowserEx 类继承了 WebBrowser 类，并提供了 LoadCompleted 事件.
 * 
 * 在页面没有嵌套框架的情况下,DocumentComplete 事件在所有事情完成后，会被引发
 * 一次. 在有多个嵌套框架时, DocumentComplete 事件会被多次引发. 因此如果事件
 * DocumentCompleted 被引发, 它并不意味着该页面被加载完全.
 * 
 * 因此, 要检查一个页面是否已经加载完全,你需要检查是否事件的发送次数与 WebBrowser
 * 控件的相同.
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
using System.Security.Permissions;
using System.Windows.Forms;

namespace CSWebBrowserLoadComplete
{
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public partial class WebBrowserEx : WebBrowser
    {
        AxHost.ConnectionPointCookie cookie;

        DWebBrowserEvents2Helper helper;

        public event EventHandler<WebBrowserNavigatingEventArgs> StartNavigating;

        public event EventHandler<WebBrowserDocumentCompletedEventArgs> LoadCompleted;

        /// <summary>
        /// 将底层的 ActiveX 控件与可以处理控件事件包括 NavigateError 事件的客户端 
        /// 关联起来.
        /// </summary>
        protected override void CreateSink()
        {

            base.CreateSink();

            helper = new DWebBrowserEvents2Helper(this);
            cookie = new AxHost.ConnectionPointCookie(
                this.ActiveXInstance, helper, typeof(DWebBrowserEvents2));         
        }

        /// <summary>
        /// 从底层的ActiveX控件中,释放附加在 CreateSink 方法中处理事件的客户端. 
        /// </summary>
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
        /// 激发 LoadCompleted 事件.
        /// </summary>
        protected virtual void OnLoadCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            if (LoadCompleted != null)
            {
                this.LoadCompleted(this, e);
            }
        }

        /// <summary>
        /// 激发 StartNavigating 事件.
        /// </summary>
        protected virtual void OnStartNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (StartNavigating != null)
            {
                this.StartNavigating(this, e);
            }
        }
    }
}
