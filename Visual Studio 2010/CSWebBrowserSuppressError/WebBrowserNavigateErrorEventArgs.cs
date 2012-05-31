/****************************** 模块头 *************************************\
* 模块名:   WebBrowserNavigateErrorEventArgs.cs
* 项目名:	CSWebBrowserSuppressError
* 版权(c)   Microsoft Corporation.
* 
* 类WebBrowserNavigateErrorEventArgs定义了WebBrowserEx.NavigateError事件使用
* 的事件参数
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

namespace CSWebBrowserSuppressError
{

    public class WebBrowserNavigateErrorEventArgs : EventArgs
    {
        public String Url { get; set; }

        public String Frame { get; set; }

        public Int32 StatusCode { get; set; }

        public Boolean Cancel { get; set; }

        public WebBrowserNavigateErrorEventArgs(String url, String frame,
            Int32 statusCode, Boolean cancel)
        {
            this.Url = url;
            this.Frame = frame;
            this.StatusCode = statusCode;
            this.Cancel = cancel;
        }

    }
}
