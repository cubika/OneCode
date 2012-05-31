/****************************** 模块头 *************************************\
* 模块名:   WebBrowserEx.cs
* 项目名:	CSWebBrowserSuppressError
* 版权(c)   Microsoft Corporation.
* 
* 此WebBrowserEx类继承自WebBrowser类，并提供了如下特征:
* 1. 禁用了JIT调试器.
* 2. 忽略了浏览器中载入的html文档对象的html元素错误.
* 3. 处理链接错误.
* 
* WebBrowser类自身也有个ScriptErrorsSuppressed属性,用来隐藏它所有源于底层
* ActiveX控件的对话框,不仅仅只是脚本错误.
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


namespace CSWebBrowserSuppressError
{
    public partial class WebBrowserEx : WebBrowser
    {
        /// <summary>
        /// 获取或设置JIT调试器是否需要被禁用.您必须重启浏览器使之生效.
        /// </summary>
        public static bool JITDebuggerDisabled
        {
            get
            {
                using (RegistryKey ieMainKey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Internet Explorer\Main"))
                {
                    string keyvalue = ieMainKey.GetValue("Disable Script Debugger") as string;
                    return string.Equals(keyvalue, "yes", StringComparison.OrdinalIgnoreCase);
                }
            }
            set
            {
                var newValue = value ? "yes" : "no";

                using (RegistryKey ieMainKey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Internet Explorer\Main", true))
                {
                    string keyvalue = ieMainKey.GetValue("Disable Script Debugger") as string;
                    if (!keyvalue.Equals(newValue, StringComparison.OrdinalIgnoreCase))
                    {
                        ieMainKey.SetValue("Disable Script Debugger", newValue);
                    }
                }
            }
        }

        // 忽略html元素错误.
        public bool HtmlElementErrorsSuppressed { get; set; }

        AxHost.ConnectionPointCookie cookie;

        WebBrowser2EventHelper helper;

        public event EventHandler<WebBrowserNavigateErrorEventArgs> NavigateError;

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WebBrowserEx()
        {
        }

        /// <summary>
        /// 注册Document.Window.Error事件.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            base.OnDocumentCompleted(e);

            // 当窗口内部运行的脚本遇到一个运行时错误时产生.
            this.Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
        }

        /// <summary>
        /// 处理此浏览器中载入的html文档对象的html元素错误.
        /// 如果HtmlElementErrorsSuppressed设定成true,则设定处理标识为true以防止浏览器
        /// 显示这个错误.
        /// </summary>
        protected void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            if (HtmlElementErrorsSuppressed)
            {
                e.Handled = true;
            }
        }


        /// <summary>
        /// 给底层ActiveX控件关联一个能够处理控件事件(包括链接错误事件)的客户端.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void CreateSink()
        {
            base.CreateSink();

            helper = new WebBrowser2EventHelper(this);
            cookie = new AxHost.ConnectionPointCookie(
                this.ActiveXInstance, helper, typeof(DWebBrowserEvents2));
        }


        /// <summary>
        /// 从底层ActiveX控件解除给其关联的附加在CreateSink方法的事件处理客户端.
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
        /// 引发NavigateError事件.
        /// </summary>
        protected virtual void OnNavigateError(WebBrowserNavigateErrorEventArgs e)
        {
            if (this.NavigateError != null)
            {
                this.NavigateError(this, e);
            }
        }
    }
}
