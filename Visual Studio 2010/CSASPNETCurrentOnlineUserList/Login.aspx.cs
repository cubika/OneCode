/**************************************** 模块头 *****************************************\
* 模块名:    Login.aspx.cs
* 项目名:    CSASPNETCurrentOnlineUserList
* 版权 (c) Microsoft Corporation
*
* 这个页面用来使用户登入. 
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************************/

using System;
using System.Web.UI.WebControls;

namespace CSASPNETCurrentOnlineUserList
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string _error;

            // 检查用户输入数据值.
            if (check_text(out _error))
            {
                // 初始化保存当前在线用户的信息的datatable.
                DataTableForCurrentOnlineUser onLineTable = new DataTableForCurrentOnlineUser();

                // 用户身份实例.
                UserEntity _user = new UserEntity();
                _user.Ticket = DateTime.Now.ToString("yyyyMMddHHmmss");
                _user.UserName = tbUserName.Text.Trim();
                _user.TrueName = tbTrueName.Text.Trim();
                _user.ClientIP = this.Request.UserHostAddress;
                _user.RoleID = "MingXuGroup";

                // 使用session变量保存ticket.
                this.Session["Ticket"] = _user.Ticket;

                // 登入.
                onLineTable.Login(_user, true);
                Response.Redirect("CurrentOnlineUserList.aspx");
            }
            else
            {
                this.lbMessage.Visible = true;
                this.lbMessage.Text = _error;
            }
        }
        public bool check_text(out string error)
        {
            error = "";
            if (this.tbUserName.Text.Trim() == "")
            {
                error = "请输入用户名";
                return false;
            }
            if (this.tbTrueName.Text.Trim() == "")
            {
                error = "请输入真名";
                return false;
            }
            return true;
        }
    }
}