/**************************************** 模块头 *****************************************\
* 模块名:    DataTableForCurrentOnlineUser.cs
* 项目名:    CSASPNETCurrentOnlineUserList
* 版权 (c) Microsoft Corporation
*
* 这个类用来初始化保存当前在线用户的信息的datatable.
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
using System.Data;
using System.Configuration;

namespace CSASPNETCurrentOnlineUserList
{
    public class DataTableForCurrentOnlineUser
    {
        private static DataTable _activeusers;
        private int _activeTimeout;
        private int _refreshTimeout;
        /// <summary>
        /// 初始化UserOnlineTable.
        /// </summary> 
        private void UsersTableFormat()
        {
            if (_activeusers == null)
            {
                _activeusers = new DataTable("ActiveUsers");
                DataColumn myDataColumn;
                System.Type mystringtype;
                mystringtype = System.Type.GetType("System.String");
                System.Type mytimetype;
                mytimetype = System.Type.GetType("System.DateTime");
                myDataColumn = new DataColumn("Ticket", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("UserName", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("TrueName", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("RoleID", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("RefreshTime", mytimetype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("ActiveTime", mytimetype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("ClientIP", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
            }
        }

        public DataTableForCurrentOnlineUser()
        {
            // 初始化保存当前在线用户的信息的datatable.
            UsersTableFormat();

            // 初始化用户活动时间(分钟).
            try
            {
                _activeTimeout = int.Parse(ConfigurationManager.AppSettings["ActiveTimeout"]);
            }
            catch
            {
                _activeTimeout = 60;
            }

            // 初始化刷新时间(分钟).
            try
            {
                _refreshTimeout = int.Parse(ConfigurationManager.AppSettings["RefreshTimeout"]);
            }
            catch
            {
                _refreshTimeout = 1;
            }
        }
        public DataTable ActiveUsers
        {
            get { return _activeusers.Copy(); }
        }

        /// <summary>
        /// 登录方法.
        /// </summary>
        public void Login(UserEntity user, bool singleLogin)
        {
            // 清除离线用户记录.
            DelTimeOut();
            if (singleLogin)
            {
                // 让已登录用户登出.
                this.Logout(user.UserName, false);
            }
            DataRow _myrow;
            try
            {
                _myrow = _activeusers.NewRow();
                _myrow["Ticket"] = user.Ticket.Trim();
                _myrow["UserName"] = user.UserName.Trim();
                _myrow["TrueName"] = "" + user.TrueName.Trim();
                _myrow["RoleID"] = "" + user.RoleID.Trim();
                _myrow["ActiveTime"] = DateTime.Now;
                _myrow["RefreshTime"] = DateTime.Now;
                _myrow["ClientIP"] = user.ClientIP.Trim();
                _activeusers.Rows.Add(_myrow);
            }
            catch
            {
                throw;
            }
            _activeusers.AcceptChanges();

        }

        /// <summary>
        /// 用户登出,取决于ticket或username.
        /// </summary> 
        private void Logout(string strUserKey, bool byTicket)
        {
            // 清除离线用户记录.
            DelTimeOut();
            strUserKey = strUserKey.Trim();
            string _strExpr;
            _strExpr = byTicket ? "Ticket='" + strUserKey + "'" : "UserName='" + strUserKey + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                for (int i = 0; i < _curuser.Length; i++)
                {
                    _curuser[i].Delete();
                }
            }
            _activeusers.AcceptChanges();
        }

        /// <summary>
        /// 根据ticket登出用户.
        /// </summary>
        /// <param name="strTicket">用户的ticket</param>
        public void Logout(string strTicket)
        {
            this.Logout(strTicket, true);
        }

        /// <summary>
        /// 清除离线用户记录.
        /// </summary>
        private bool DelTimeOut()
        {
            string _strExpr;
            _strExpr = "ActiveTime < '" + DateTime.Now.AddMinutes(0 - _activeTimeout) +
                "'or RefreshTime < '" + DateTime.Now.AddMinutes(0 - _refreshTimeout) + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                for (int i = 0; i < _curuser.Length; i++)
                {
                    _curuser[i].Delete();
                }
            }
            _activeusers.AcceptChanges();
            return true;
        }

        /// <summary>
        /// 更新用户上次活动时间.
        /// </summary>
        public void ActiveTime(string strTicket)
        {
            DelTimeOut();
            string _strExpr;
            _strExpr = "Ticket='" + strTicket + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                for (int i = 0; i < _curuser.Length; i++)
                {
                    _curuser[i]["ActiveTime"] = DateTime.Now;
                    _curuser[i]["RefreshTime"] = DateTime.Now;
                }
            }
            _activeusers.AcceptChanges();
        }

        /// <summary>
        /// 当页面更新或获得请求时刷新时间.
        /// </summary>
        public void RefreshTime(string strTicket)
        {
            DelTimeOut();
            string _strExpr;
            _strExpr = "Ticket='" + strTicket + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                for (int i = 0; i < _curuser.Length; i++)
                {
                    _curuser[i]["RefreshTime"] = DateTime.Now;
                }
            }
            _activeusers.AcceptChanges();
        }

        private UserEntity SingleUser(string strUserKey, bool byTicket)
        {
            strUserKey = strUserKey.Trim();
            string _strExpr;
            UserEntity user = new UserEntity();
            _strExpr = byTicket ? "Ticket='" + strUserKey + "'" : "UserName='" + strUserKey + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                user.Ticket = (string)_curuser[0]["Ticket"];
                user.UserName = (string)_curuser[0]["UserName"];
                user.TrueName = (string)_curuser[0]["TrueName"];
                user.RoleID = (string)_curuser[0]["RoleID"];
                user.ActiveTime = (DateTime)_curuser[0]["ActiveTime"];
                user.RefreshTime = (DateTime)_curuser[0]["RefreshTime"];
                user.ClientIP = (string)_curuser[0]["ClientIP"];
            }
            else
            {
                user.UserName = "";
            }
            return user;
        }

        /// <summary>
        /// 依据ticket搜索用户.
        /// </summary>
        public UserEntity SingleUser_byTicket(string strTicket)
        {
            return this.SingleUser(strTicket, true);
        }

        /// <summary>
        /// 依据username搜索用户.
        /// </summary>
        public UserEntity SingleUser_byUserName(string strUserName)
        {
            return this.SingleUser(strUserName, false);
        }

        /// <summary>
        /// 通过ticket检查用户是否在线.
        /// </summary>
        public bool IsOnline_byTicket(string strTicket)
        {
            return (bool)(this.SingleUser(strTicket, true).UserName != "");
        }

        /// <summary>
        /// 通过username检查用户是否在线.
        /// </summary>
        public bool IsOnline_byUserName(string strUserName)
        {
            return (bool)(this.SingleUser(strUserName, false).UserName != "");
        }
    }
}