/****************************** 模块头 ******************************\
* 模块名:    Sender.aspx.cs
* 项目名:    CSASPNETReverseAJAX
* 版权 (c) Microsoft Corporation
*
* 用户使用这个页面发送消息到特定接收端.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\********************************************************************/

using System;

namespace CSASPNETReverseAJAX
{
    public partial class Sender : System.Web.UI.Page
    {
        protected void btnSend_Click(object sender, EventArgs e)
        {
            // 创建一个消息实体来包含所有必要数据.
            Message message = new Message();
            message.RecipientName = tbRecipientName.Text.Trim();
            message.MessageContent = tbMessageContent.Text.Trim();

            if (!string.IsNullOrWhiteSpace(message.RecipientName) && !string.IsNullOrEmpty(message.MessageContent))
            {
                // 调用客户端的适配器把消息立即发送到特定的接收端.
                ClientAdapter.Instance.SendMessage(message);

                // 显示时间戳.
                lbNotification.Text += DateTime.Now.ToLongTimeString() + ": 消息已发送!<br/>";
            }
        }
    }
}