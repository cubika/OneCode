/****************************** 模块头 ******************************\
* 模块名:    Client.cs
* 项目名:    CSASPNETReverseAJAX
* 版权 (c) Microsoft Corporation
*
* Client类用来使消息的发送和接受同步.
* 当调用DequeueMessage方法时,这个方法将会等待直到调用 
* EnqueueMessage方法插入新的消息.这个类使ManualResetEvent
* 类实现同步. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\********************************************************************/

using System.Collections.Generic;
using System.Threading;

namespace CSASPNETReverseAJAX
{
    /// <summary>
    /// 这个类表示网络客户端可以接收消息.
    /// </summary>
    public class Client
    {
        private ManualResetEvent messageEvent = new ManualResetEvent(false);
        private Queue<Message> messageQueue = new Queue<Message>();

        /// <summary>
        /// 这个类被发送者调用向客户端发送消息.
        /// </summary>
        /// <param name="message">新的消息</param>
        public void EnqueueMessage(Message message)
        {
            lock (messageQueue)
            {
                messageQueue.Enqueue(message);

                // 设置一个新的消息事件.
                messageEvent.Set();
            }
        }

        /// <summary>
        /// 这个方法被客户端调用从消息队列接收消息.
        /// 如果没有消息,它会等待直到新的消息插入.
        /// </summary>
        /// <returns>未读的消息</returns>
        public Message DequeueMessage()
        {
            // 等待一个新的消息.
            messageEvent.WaitOne();

            lock (messageQueue)
            {
                if (messageQueue.Count == 1)
                {
                    messageEvent.Reset();
                }
                return messageQueue.Dequeue();
            }
        }
    }
}