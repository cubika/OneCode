/**************************************** 模块头 *****************************************\
* 模块名:       Message.cs
* 项目名:       CSASPNETAJAXWebChat
* 版权 (c) Microsoft Corporation
*
* 在此文件中, 我们创建了一个用来序列化到客户端的Message数据的DataContract类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

using System;
using System.Web;
using System.Runtime.Serialization;

namespace WebChat.Logic
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string Talker { get; private set; }
        [DataMember]
        public string MessageData { get; private set; }
        [DataMember]
        public DateTime SendTime { get; private set; }
        
        [DataMember]
        public bool IsFriend { get; private set; }

        public Message(WebChat.Data.tblMessagePool message,HttpContext session)
        {
            Talker = message.tblTalker.tblSession.UserAlias;
            MessageData = message.message;
            SendTime = message.SendTime;
            IsFriend = (message.tblTalker.tblSession.SessionID != session.Session.SessionID);
        }
    }
}