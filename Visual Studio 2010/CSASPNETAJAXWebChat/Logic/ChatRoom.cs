/**************************************** 模块头 *****************************************\
* 模块名:      ChatRoom.cs
* 项目名:      CSASPNETAJAXWebChat
* 版权 (c) Microsoft Corporation
*
* 在此文件中, 我们创建了一个用来序列化到客户端的ChatRoom数据的DataContract类.
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
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WebChat.Logic
{
    [DataContract]
    public class ChatRoom
    {
        [DataMember]
        public Guid RoomID { get; private set; }
        [DataMember]
        public string RoomName { get; private set; }
        [DataMember]
        public int MaxUser { get; private set; }
        [DataMember]
        public int CurrentUser { get; private set; }

        public ChatRoom(Guid id)
        {
            WebChat.Data.SessionDBDataContext db = new Data.SessionDBDataContext();
            var room = db.tblChatRooms.SingleOrDefault(r => r.ChatRoomID == id);
            if (room != null)
            {
                RoomID = id;
                RoomName = room.ChatRoomName;
                MaxUser = room.MaxUserNumber;
                CurrentUser = room.tblTalkers.Count(t => t.CheckOutTime == null);
            }
        }

    }
}