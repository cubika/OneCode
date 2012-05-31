/****************************** 模块头 ******************************\
* 模块名:    Message.cs
* 项目名:    CSASPNETReverseAJAX
* 版权 (c) Microsoft Corporation
*
* Message类包含了在一个消息包中所有必须的字段.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\********************************************************************/

namespace CSASPNETReverseAJAX
{
    /// <summary>
    /// 这是个表示消息项目的实体类.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 该名字将会接收这个消息.
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// 消息内容.
        /// </summary>
        public string MessageContent { get; set; }
    }
}