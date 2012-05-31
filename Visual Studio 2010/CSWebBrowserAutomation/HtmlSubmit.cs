/****************************** 模块头 ******************************\
* 模块名:    HtmlSubmit.cs
* 项目名:	    CSWebBrowserAutomation
* 版权(c) Microsoft Corporation.
* 
* 这个HtmlSubmit类代表一个带有“submit”类型的“输入标签”的HtmlElement。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Windows.Forms;
using System.Security.Permissions;

namespace CSWebBrowserAutomation
{
    public class HtmlSubmit : HtmlInputElement
    {

        /// <summary>
        ///这个无参数构造函数用于反序列化。
        /// </summary>
        public HtmlSubmit() { }

        /// <summary>
        ///初始化一个HtmlSubmit的实例。这个构造函数是用于 
        /// HtmlInputElementFactory.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HtmlSubmit(HtmlElement element)
            : base(element.Id)
        {
        }

    }
}
