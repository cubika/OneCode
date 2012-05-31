/**************************************** 模块头 *****************************************\
* 模块名:    CurrentOnlineUserList.aspx.cs
* 项目名:    CSASPNETCurrentOnlineUserList
* 版权 (c) Microsoft Corporation
*
* 本页面用来显示当前在线用户的信息. 
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
    public partial class CurrentOnlineUserList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 检查用户是否登入.
            CheckLogin();
        }
        public void CheckLogin()
        {
            string _userticket = "";
            if (Session["Ticket"] != null)
            {
                _userticket = Session["Ticket"].ToString();
            }
            if (_userticket != "")
            {
                // 初始化保存当前在线用户的信息的datatable.
                DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();

                // 通过ticket检查用户是否在线.
                if (_onlinetable.IsOnline_byTicket(this.Session["Ticket"].ToString()))
                {
                    // 更新最近活动时间.
                    _onlinetable.ActiveTime(Session["Ticket"].ToString());

                    // 绑定保存当前在线用户的信息的datatable到gridview控件.
                    gvUserList.DataSource = _onlinetable.ActiveUsers;
                    gvUserList.DataBind();
                }
                else
                {
                    // 如果当前用户不在表中,重定向页面到LogoOut.
                    Response.Redirect("LogOut.aspx");
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}