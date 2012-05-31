/******************************* 模块头 ***********************************\
* 模块名:             HighlightCodePage.aspx.cs
* 项目名:        CSASPNETHighlightCodeInPage
* 版权(c) Microsoft Corporation
*
* 这个页面用以使用户高亮它的代码. 
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Web.UI.WebControls;
using System.Collections;

namespace CSASPNETHighlightCodeInPage
{
    public partial class HighlightCodePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lbError.Visible = false;
            this.lbResult.Visible = false;
        }

        protected void btnHighLight_Click(object sender, EventArgs e)
        {
            string _error = string.Empty;

            // 检查用户输入的数据.
            if (CheckControlValue(this.ddlLanguage.SelectedValue, 
                this.tbCode.Text, out _error))
            {
                // 初始化根据匹配选项用来保存不同语言代码
                // 及其相关正则表达式的散列表变量.
                Hashtable _htb = CodeManager.Init();

                // 初始化合适的集合对象.
                RegExp _rg = new RegExp();
                _rg = (RegExp)_htb[this.ddlLanguage.SelectedValue];
                this.lbResult.Visible = true;
                if (this.ddlLanguage.SelectedValue != "html")
                {
                    // 在标签控件中显示高亮的代码.
                    this.lbResult.Text = CodeManager.Encode(
                        CodeManager.HighlightCode(
                        Server.HtmlEncode(this.tbCode.Text)
                        .Replace("&quot;", "\""),
                        this.ddlLanguage.SelectedValue, _rg)
                        );
                }
                else
                {
                    // 在标签控件中显示高亮的代码.
                    this.lbResult.Text = CodeManager.Encode(
                        CodeManager.HighlightHTMLCode(this.tbCode.Text, _htb)
                        );
                }
            }
            else
            {
                this.lbError.Visible = true;
                this.lbError.Text = _error;
            }
        }
        public bool CheckControlValue(string selectValue, 
            string inputValue, 
            out string error)
        {
            error = string.Empty;
            if (selectValue == "-1")
            {
                error = "请选择语言.";
                return false;
            }
            if (string.IsNullOrEmpty(inputValue))
            {
                error = "请粘贴您的代码到文本框控件.";
                return false;
            }
            return true;
        }
    }
}