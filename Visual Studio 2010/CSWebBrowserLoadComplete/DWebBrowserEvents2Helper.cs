/*********************************** 模块头 ********************************\
 * 模块名:  DWebBrowserEvents2Helper.cs
 * 项目名:  CSWebBrowserLoadComplete
 * 版权 (c) Microsoft Corporation.
 * 
 * DWebBrowserEvents2Helper 通过激发定义在 WebBrowserEx 类的 StartNavigating
 * 和 LoadCompleted 事件,用于处理来自底层的 ActiveX 控件的BeforeNavigate2 
 * 和 DocumentComplete 事件. 
 * 
 * 如果 WebBrowser 控件装载一个普通的、无内嵌框架的 HTML 页面, 在所有事情完成
 * 之后, DocumentComplete 事件会被引发一次.
 *
 * 如果 WebBrowser 控件装载了许多内嵌框架, DocumentComplete 会被多次引发.
 * DocumentComplete 事件有一个 pDisp 参数,它是框架( shdocvw )的 IDispatch.该框
 * 架中 DocumentComplete 被引发. 
 *
 * 然后我们可以检查是否 DocumentComplete 的 pDisp 参数与浏览器的ActiveXInstance
 * 相同. 
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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CSWebBrowserLoadComplete
{
    public partial class WebBrowserEx
    {
        private class DWebBrowserEvents2Helper : StandardOleMarshalObject, DWebBrowserEvents2
        {
            private WebBrowserEx parent;

            public DWebBrowserEvents2Helper(WebBrowserEx parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// 当文件完全加载并初始化时，引发.
            /// 如果框架是顶层窗口元素,那么该页面就加载完全.
            /// 
            /// 然后,在 WebBrowser 完全加载之后,重置 glpDisp 为 null.
            /// </summary>
            public void DocumentComplete(object pDisp, ref object URL)
            {
                string url = URL as string;

                if (string.IsNullOrEmpty(url)
                    || url.Equals("about:blank", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (pDisp != null && pDisp.Equals(parent.ActiveXInstance))
                {
                    var e = new WebBrowserDocumentCompletedEventArgs(new Uri(url)); 

                    parent.OnLoadCompleted(e);
                }
            }

            /// <summary>
            /// 在给定对象中(一个窗口元素或者一个框架集元素)导航发生前引发.
            /// 
            /// </summary>
            public void BeforeNavigate2(object pDisp, ref object URL, ref object flags,
                ref object targetFrameName, ref object postData, ref object headers,
                ref bool cancel)
            {
                string url = URL as string;

                if (string.IsNullOrEmpty(url)
                    || url.Equals("about:blank", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (pDisp != null && pDisp.Equals(parent.ActiveXInstance))
                {
                    WebBrowserNavigatingEventArgs e = new WebBrowserNavigatingEventArgs(
                        new Uri(url), targetFrameName as string);

                    parent.OnStartNavigating(e);
                }
            }
        }
    }
}
