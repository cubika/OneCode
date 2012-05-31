/**************************************** 模块头 *****************************************\
* 模块名:      RoomTalker.cs
* 项目名:      CSASPNETAJAXWebChat
* 版权 (c) Microsoft Corporation
*
* 在此文件中, 我们创建了一个用来序列化到客户端的聊天者数据的DataContract类.
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
    public class RoomTalker
    {
        [DataMember]
        public string TalkerAlias { get; private set; }
        [DataMember]
        public string TalkerSession { get; private set; }
        [DataMember]
        public string TalkerIP { get; private set; }
        [DataMember]
        public bool IsFriend { get; private set; }

        public RoomTalker(WebChat.Data.tblTalker talker, HttpContext context)
        {
            TalkerAlias = talker.tblSession.UserAlias;
            TalkerIP = talker.tblSession.IP;
            TalkerSession = talker.tblSession.SessionID;
            IsFriend = (TalkerSession != context.Session.SessionID);
        }
    }
}