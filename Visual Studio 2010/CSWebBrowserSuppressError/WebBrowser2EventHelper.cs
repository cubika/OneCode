/****************************** 模块头 *************************************\
* 模块名:   WebBrowser2EventHelper.cs
* 项目名:	CSWebBrowserSuppressError
* 版权(c)   Microsoft Corporation.
* 
* 此WebBrowser2EventHelper类通过触发在WebBrowserEx类中定义的NavigateError事件,
* 来处理来自底层ActiveX控件的NavigateError事件.
* 
* 归因于protected方法WebBrowserEx.OnNavigateError，此类被定义在WebBrowserEx类
* 的内部.
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

namespace CSWebBrowserSuppressError
{
    public partial class WebBrowserEx
    {
        private class WebBrowser2EventHelper : StandardOleMarshalObject, DWebBrowserEvents2
        {
            private WebBrowserEx parent;

            public WebBrowser2EventHelper(WebBrowserEx parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// 触发NavigateError事件.
            /// 如果一个WebBrowser2EventHelper类的实例被关联到底层ActiveX控件,
            /// 当NavigateError事件在此ActiveX控件中被解除时这个方法将会被调用.
            /// </summary>
            public void NavigateError(object pDisp, ref object url,
                ref object frame, ref object statusCode, ref bool cancel)
            {

                // 在WebBrowserEx类中触发NavigateError事件.
                this.parent.OnNavigateError(new WebBrowserNavigateErrorEventArgs(
                    (String)url, (String)frame, (Int32)statusCode, cancel));

            }          
        }
    }
}
