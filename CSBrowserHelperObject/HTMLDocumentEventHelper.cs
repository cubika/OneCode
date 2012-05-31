/****************************** 模块头 ******************************\
* 模块名称:      HTMLDocumentEventHelper.cs
* 项目名称:	    CSBrowserHelperObject
* 版权：Copyright (c) Microsoft Corporation.
* 
* 这个 ComVisible 类 HTMLDocumentEventHelper是用来设置HTMLDocument的事件处理的。
* 接口 DispHTMLDocument 定义了很多事件，就像 
* oncontextmenu, onclick等, 这些事件可以被设置到
* HTMLEventHandler 实例中.
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
using mshtml;

namespace CSBrowserHelperObject
{
    [ComVisible(true)]
    public class HTMLDocumentEventHelper
    {
        private HTMLDocument document;

        public HTMLDocumentEventHelper(HTMLDocument document)
        {
            this.document = document;
        }

        public event HtmlEvent oncontextmenu
        {
            add
            {
                DispHTMLDocument dispDoc = this.document as DispHTMLDocument;

                object existingHandler = dispDoc.oncontextmenu;
                HTMLEventHandler handler = existingHandler is HTMLEventHandler ?
                    existingHandler as HTMLEventHandler : 
                    new HTMLEventHandler(this.document);

                // 为 oncontextmenu 事件设置事件处理句柄.
                dispDoc.oncontextmenu = handler;

                handler.eventHandler += value;
            }
            remove
            {
                DispHTMLDocument dispDoc = this.document as DispHTMLDocument;
                object existingHandler = dispDoc.oncontextmenu;

                HTMLEventHandler handler = existingHandler is HTMLEventHandler ?
                    existingHandler as HTMLEventHandler : null;

                if (handler != null)
                    handler.eventHandler -= value;
            }
        }
    }

}
