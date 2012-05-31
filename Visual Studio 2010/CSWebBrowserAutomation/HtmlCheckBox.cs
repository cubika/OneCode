/****************************** 模块头 ******************************\
* 模块名:    HtmlCheckBox.cs
* 项目名:	    CSWebBrowserAutomation
* 版权(c)  Microsoft Corporation.
* 
* 这个HtmlCheckBox类代表带有一个“checkbox”类型的“输入标签”的HtmlElement。
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
    public class HtmlCheckBox : HtmlInputElement
    {
        public bool Checked { get; set; }

        /// <summary>
        /// 这个无参数构造函数用于反序列化。
        /// </summary>
        public HtmlCheckBox() { }

        /// <summary>
        /// 初始化一个HtmlCheckBox的实例。 使用构造函数HtmlInputElementFactory。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HtmlCheckBox(HtmlElement element)
            : base(element.Id)
        {

            // 如果checkbox的有属性是“checked”它将被检查。
            string chekced = element.GetAttribute("checked");
            Checked = !string.IsNullOrEmpty(chekced);
        }

        /// <summary>
        /// 设置HtmlElement的值.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override void SetValue(HtmlElement element)
        {
            // 如果checkbox的有属性是“checked”它将被检查。
            if (Checked)
            {
                element.SetAttribute("checked", "checked");
            }
            else
            {
                element.SetAttribute("checked", null);
            }
        }
    }
}
