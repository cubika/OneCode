/****************************** 模块头 ******************************\
* 模块名:  WebBrowser2EventHelper.cs
* 项目名:	    CSTabbedWebBrowser
* 版权 (c) Microsoft Corporation.
* 
* WebBrowser2EventHelper 类通过提高在WebBrowserEx中定义的NewWindow3 事件
* 从底层的ActiveX 控件 去处理NewWindow3 事件。
*  
* 由于方法WebBrowserEx.OnNewWindow3受保护的，WebBrowser2EventHelper类 定义在WebBrowserEx类中。
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

namespace CSTabbedWebBrowser
{
    public partial class WebBrowserEx
    {
        private class DWebBrowserEvent2Helper 
            : StandardOleMarshalObject, DWebBrowserEvents2
        {
            private WebBrowserEx parent;

            public DWebBrowserEvent2Helper(WebBrowserEx parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// 提高 NewWindows3 事件.
            /// 当在ActiveX 空间中，NewWindow3事件被触发时，
            /// 如果WebBrowser2EventHelper的一个实例与底层的ActiveX 空间相关联，
            /// 这个方法就会被调用。
            /// </summary>
            public void NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, 
                string bstrUrlContext, string bstrUrl)
            {
                var e = new WebBrowserNewWindowEventArgs(bstrUrl, Cancel);
                this.parent.OnNewWindow3(e);
                Cancel=e.Cancel;
            }
        }
    }
}
