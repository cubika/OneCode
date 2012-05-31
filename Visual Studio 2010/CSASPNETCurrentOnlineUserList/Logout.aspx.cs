/**************************************** 模块头 *****************************************\
* 模块名:    Logout.aspx.cs
* 项目名:    CSASPNETCurrentOnlineUserList
* 版权 (c) Microsoft Corporation
*
* 这个页面用来使用户登出. 
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
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 初始化保存当前在线用户的信息的datatable.
            DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();
            if (this.Session["Ticket"] != null)
            {
                // 登出.
                _onlinetable.Logout(this.Session["Ticket"].ToString());
                this.Session.Clear();
            }
        }
    }
}