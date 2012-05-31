/**************************************** 模块头 *****************************************\
* 模块名:    CheckUserOnlinePage.aspx
* 项目名:    CSASPNETCurrentOnlineUserList
* 版权 (c) Microsoft Corporation
*
* 本页面用来获得来自其他包含CheckUserOnline自定义控件页面的请求
* 同时从CurrentOnlineUser表中删除离线用户.
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
    public partial class CheckUserOnlinePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Check();
        }
        public virtual string SessionName
        {
            get
            {
                object _obj1 = this.ViewState["SessionName"];
                if (_obj1 != null) { return ((string)_obj1).Trim(); }
                return "Ticket";
            }
            set
            {
                this.ViewState["SessionName"] = value;
            }
        }
        protected void Check()
        {
            string _myTicket = "";
            if (System.Web.HttpContext.Current.Session[this.SessionName] != null)
            {
                _myTicket = System.Web.HttpContext.Current.Session[this.SessionName].ToString();
            }
            if (_myTicket != "")
            {
                // 初始化保存当前在线用户的信息的datatable.
                DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();

                // 当页面更新或获得请求时刷新时间.
                _onlinetable.RefreshTime(_myTicket);
                Response.Write("OK：" + DateTime.Now.ToString());
            }
            else
            {
                Response.Write("Sorry：" + DateTime.Now.ToString());
            }
        }
    }
}