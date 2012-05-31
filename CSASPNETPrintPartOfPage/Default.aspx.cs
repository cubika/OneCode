/********************************** 模块头 **********************************\
* 模块名: Default.aspx.cs
* 项目名: CSASPNETPrintPartOfPage
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何用层叠样式表和JavaScript打印部分页面.
* 我们需要在部分网页上设置div层,并使用JavaScript代码来
* 打印它的有用部分.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Printing;

namespace CSASPNETPrintPartOfPage
{
    public partial class Default : System.Web.UI.Page
    {
        // 定义一些字符串,用来添加div元素.
        public string PrintImageBegin;
        public string PrintImageEnd;
        public string PrintListBegin;
        public string PrintListEnd;
        public string PrintButtonBegin;
        public string PrintButtonEnd;
        public string PrintSearchBegin;
        public string PrintSearchEnd;
        public const string EnablePirnt = "<div class=nonPrintable>";
        public const string EndDiv = "</div>";
        protected void Page_Load(object sender, EventArgs e)
        {
            // 检查文本框的状态,设置div元素.
            if (chkDisplayImage.Checked)
            { 
                PrintImageBegin = string.Empty;
                PrintImageEnd = string.Empty;
            }
            else
            { 
                PrintImageBegin = EnablePirnt;
                PrintImageEnd = EndDiv;
            }
            if (chkDisplayList.Checked)
            {
                PrintListBegin = string.Empty;
                PrintListEnd = string.Empty;
            }
            else
            { 
                PrintListBegin = EnablePirnt;
                PrintListEnd = EndDiv; 
            }
            if (chkDisplayButton.Checked)
            { 
                PrintButtonBegin = string.Empty;
                PrintButtonEnd = string.Empty;
            }
            else
            {
                PrintButtonBegin = EnablePirnt; 
                PrintButtonEnd = EndDiv; 
            }
            if (chkDisplaySearch.Checked)
            { 
                PrintSearchBegin = string.Empty;
                PrintSearchEnd = string.Empty; 
            }
            else
            {
                PrintSearchBegin = EnablePirnt;
                PrintSearchEnd = EndDiv; 
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PrintOperation", "print()", true);
        }
    }
}