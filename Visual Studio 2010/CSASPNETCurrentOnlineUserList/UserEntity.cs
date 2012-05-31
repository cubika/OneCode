/**************************************** 模块头 *****************************************\
* 模块名:    UserEntity.cs
* 项目名:    CSASPNETCurrentOnlineUserList
* 版权 (c) Microsoft Corporation
*
* 本类用以设定用户身份. 
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
using System.Text;

namespace CSASPNETCurrentOnlineUserList
{
    public class UserEntity
    {
        public UserEntity()
        { }
        // Ticket.
        private string _ticket;

        // UserName.
        private string _username;

        // TrueName.
        private string _truename;

        // Role.
        private string _roleid;

        // 页面最后刷新时间.
        private DateTime _refreshtime;

        // 用户最后活动时间.
        private DateTime _activetime;

        // 用户Ip地址.
        private string _clientip;    

        public string Ticket
        {
            get
            {
                return _ticket;
            }
            set
            {
                _ticket = value;
            }
        }
        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
        public string TrueName
        {
            get
            {
                return _truename;
            }
            set
            {
                _truename = value;
            }
        }
        public string RoleID
        {
            get
            {
                return _roleid;
            }
            set
            {
                _roleid = value;
            }
        }
        public DateTime RefreshTime
        {
            get
            {
                return _refreshtime;
            }
            set
            {
                _refreshtime = value;
            }
        }
        public DateTime ActiveTime
        {
            get
            {
                return _activetime;
            }
            set
            {
                _activetime = value;
            }
        }
        public string ClientIP
        {
            get
            {
                return _clientip;
            }
            set
            {
                _clientip = value;
            }
        }
    }
}