/****************************** 模块头 ******************************\
* 模块名:  WindowsSession.cs
* 项目名:	  CSDetectWindowsSessionState
* 版权 (c) Microsoft Corporation.
* 
* WindowsSession类是用来订阅SystemEvents.SessionSwitch事件, 同时导入OpenInputDesktop
* 方法来检测当前回话是否被锁定.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.Permissions;

namespace CSDetectWindowsSessionState
{
    public class WindowsSession : IDisposable
    {

        /// <summary>
        /// 打开桌面来接收用户输入.
        /// 这个方法用来检查桌面是否被锁定. 如果返回IntPtr.Zero, 意味着方法失败, 也就是桌面被锁定.
        /// 注意:
        ///      如果UAC弹出安全桌面, 这个方法同样会失败. 现在没有API能够区分是桌面锁定还是UAC弹出
        ///      安全桌面.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr OpenInputDesktop(
            int dwFlags,
            bool fInherit,
            int dwDesiredAccess);

        /// <summary>
        /// 关闭桌面对象的句柄.
        /// </summary>
        /// <returns>
        /// 成功返回true.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseDesktop(IntPtr hDesktop);

        // 指示资源是否已被释放.
        private bool disposed;

        public event EventHandler<SessionSwitchEventArgs> StateChanged;

        /// <summary>
        /// 初始化对象.
        /// 注册SystemEvents.SessionSwitch事件.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WindowsSession()
        {
            SystemEvents.SessionSwitch +=
                new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        /// <summary>
        /// 处理SystemEvents.SessionSwitch事件.
        /// </summary>
        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {          
            this.OnStateChanged(e);
        }

        /// <summary>
        /// 触发StateChanged事件.
        /// </summary>
        protected virtual void OnStateChanged(SessionSwitchEventArgs e)
        {
            if (StateChanged != null)
            {
                StateChanged(this, e);
            }
        }

        /// <summary>
        /// 检查当前会话是否处于锁定.
        /// 注意:
        ///      如果UAC弹出安全桌面, 这个方法同样会失败. 现在没有API能够区分是桌面锁定还是UAC弹出
        ///      安全桌面.
        /// </summary>
        public bool IsLocked()
        {
            IntPtr hDesktop = IntPtr.Zero; ;
            try
            {

                // 打开桌面来接收用户输入.
                hDesktop = OpenInputDesktop(0, false, 0x0100);

                // 如果hDesktop为IntPtr.Zero, 那么会话处于锁定状态.
                return hDesktop == IntPtr.Zero;
            }
            finally
            {

                // 关闭桌面对象的句柄.
                if (hDesktop != IntPtr.Zero)
                {
                    CloseDesktop(hDesktop);
                }
            }
        }

        /// <summary>
        /// 释放资源.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // 释放托管资源.
                SystemEvents.SessionSwitch -=
                    new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            }

            disposed = true;
        }
    }
}

