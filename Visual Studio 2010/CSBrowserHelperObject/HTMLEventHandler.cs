/****************************** 模块头 ******************************\
* 模块名称： HTMLEventHandler.cs
* 项目名称： CSBrowserHelperObject
* 版权：Copyright (c) Microsoft Corporation.
* 
* 这个 ComVisible 类 HTMLEventHandler 能够复制给DispHTMLDocument 接口的事件属性 ,
* 就像 oncontextmenu, onclick等. 
* 他还可以用在其它HTML Elements元素中 
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

using System.Runtime.InteropServices;

namespace CSBrowserHelperObject
{

    // 事件处理方法的委托.
    public delegate void HtmlEvent(mshtml.IHTMLEventObj e);

    [ComVisible(true)]
    public class HTMLEventHandler
    {

        private mshtml.HTMLDocument htmlDocument;

        public event HtmlEvent eventHandler;

        public HTMLEventHandler(mshtml.HTMLDocument htmlDocument)
        {
            this.htmlDocument = htmlDocument;
        }

        [DispId(0)]
        public void FireEvent()
        {
            this.eventHandler(this.htmlDocument.parentWindow.@event);
        }
    }
}
