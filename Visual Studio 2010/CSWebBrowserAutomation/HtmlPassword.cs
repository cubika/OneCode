/****************************** 模块头 ******************************\
* 模块名:    HtmlPassword.cs
* 项目名:	    CSWebBrowserAutomation
* 版权(c) Microsoft Corporation.
* 
* 这个HtmlPassword类代表一个带有“password”类型的“输入标签”的HtmlElement。
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
    public class HtmlPassword : HtmlInputElement
    {
        public string Value { get; set; }

        /// <summary>
        /// 这个无参数构造函数用于反序列化。
        /// </summary>
        public HtmlPassword() { }

        /// <summary>
        /// 初始化一个HtmlPassword的实例。使用构造函数HtmlInputElementFactory。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HtmlPassword(HtmlElement element)
            : base(element.Id)
        {
            Value = element.GetAttribute("value");
        }

        /// <summary>
        /// 设置HtmlElement的值。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void SetValue(HtmlElement element)
        {
            element.SetAttribute("value", Value);
        }
    }
}
