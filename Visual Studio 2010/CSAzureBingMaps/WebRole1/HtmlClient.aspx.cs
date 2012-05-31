/********************************* 模块头 **********************************\
* 模块名:  HtmlClient.aspx.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* HTML客户端主机aspx页面后台代码.
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

namespace AzureBingMaps.WebRole
{
    public partial class HtmlClient : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 查询会话状态数据检查用户是否被授权.
            // 然后根据相关信息显示登入链接或欢迎信息.
            if (Session["User"] != null)
            {
                this.LoginLink.Visible = false;
                this.UserNameLabel.Visible = true;
                this.UserNameLabel.Text = "欢迎, " + (string)Session["User"] + ".";
            }
            else
            {
                this.LoginLink.Visible = true;
                this.UserNameLabel.Visible = false;
            }
        }
    }
}