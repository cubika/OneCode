/****************************** 模块头 *******************************\
* 模块名:    ClientAdapter.cs
* 项目名:    CSASPNETReverseAJAX
* 版权 (c) Microsoft Corporation
*
* ClientAdapter类管理多个客户端实体.表示层调用它的方法可以
* 很容易的发送和接收消息.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*********************************************************************/

using System.Collections.Generic;

namespace CSASPNETReverseAJAX
{
    /// <summary>
    /// 这个类用来向多个客户端发送事件/消息.
    /// </summary>
    public class ClientAdapter
    {
        /// <summary>
        /// 收件人列表.
        /// </summary>
        private Dictionary<string, Client> recipients = new Dictionary<string,Client>();

        /// <summary>
        /// 向特定的收件人发送消息.
        /// </summary>
        public void SendMessage(Message message)
        {
            if (recipients.ContainsKey(message.RecipientName))
            {
                Client client = recipients[message.RecipientName];

                client.EnqueueMessage(message);
            }
        }

        /// <summary>
        /// 调用一个单独的接收人来等待接收消息.
        /// </summary>
        /// <returns>消息内容</returns>
        public string GetMessage(string userName)
        {
            string messageContent = string.Empty;

            if (recipients.ContainsKey(userName))
            {
                Client client = recipients[userName];

                messageContent = client.DequeueMessage().MessageContent;
            }

            return messageContent;
        }

        /// <summary>
        /// 添加一个用户到收件人列表.
        /// </summary>
        public void Join(string userName)
        {
            recipients[userName] = new Client();
        }

        /// <summary>
        /// 单例模式
        /// 这个模式将会确保在系统中只有一个类的实例.
        /// </summary>
        public static ClientAdapter Instance = new ClientAdapter();
        private ClientAdapter() { }
    }
}