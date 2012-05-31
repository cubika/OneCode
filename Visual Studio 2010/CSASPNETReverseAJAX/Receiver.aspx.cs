/****************************** 模块头 ******************************\
* 模块名:    Receiver.aspx.cs
* 项目名:    CSASPNETReverseAJAX
* 版权 (c) Microsoft Corporation
*
* 用户将会使用独特的用户名通过这个页面来登陆.当在服务器上有一条
* 新的消息时,服务器会直接把消息发送到客户端.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\********************************************************************/

using System;

namespace CSASPNETReverseAJAX
{
    public partial class Receiver : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = tbUserName.Text.Trim();

            // 加入收件人列表.
            if (!string.IsNullOrEmpty(userName))
            {
                ClientAdapter.Instance.Join(userName);

                Session["userName"] = userName;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // 激活JavaScript等待循环.
            if (Session["userName"] != null)
            {
                string userName = (string)Session["userName"];

                // 调用JavaScript的waitEvent方法来开始等待循环.
                ClientScript.RegisterStartupScript(this.GetType(), "ActivateWaitingLoop", "waitEvent();", true);

                lbNotification.Text = string.Format("你的用户名是 <b>{0}</b>. 正在等待新的消息.", userName);

                // 禁用登陆.
                tbUserName.Visible = false;
                btnLogin.Visible = false;
            }
        }
    }
}