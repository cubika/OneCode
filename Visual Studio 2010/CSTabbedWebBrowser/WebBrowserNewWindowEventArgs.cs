/****************************** 模块头 ******************************\
* 模块名:  WebBrowserNavigateErrorEventArgs.cs
* 项目名:	    CSTabbedWebBrowser
* 版权(c) Microsoft Corporation.
* 
* 通过WebBrowserEx.NewWindow3事件，WebBrowserNavigateErrorEventArgs类可以定义事件参数。
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

namespace CSTabbedWebBrowser
{

    public class WebBrowserNewWindowEventArgs : EventArgs
    {
        public String Url { get; set; }
       
        public Boolean Cancel { get; set; }

        public WebBrowserNewWindowEventArgs(String url, Boolean cancel)
        {
            this.Url = url;
            this.Cancel = cancel;
        }

    }
}
