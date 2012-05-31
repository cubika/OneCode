/****************************** 模块头 ******************************\
* 模块名:  MainForm.cs
* 项目名:	  CSDetectWindowsSessionState
* 版权 (c) Microsoft Corporation.
* 
* 这是应用程序的主窗口, 用来注册和处理用户界面的事件.
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
using System.Windows.Forms;
using Microsoft.Win32;

namespace CSDetectWindowsSessionState
{
    public partial class MainForm : Form
    {
        WindowsSession session;

        System.Threading.Timer timer;

        public MainForm()
        {
            InitializeComponent();

            // 初始化WindowsSession对象.
            session = new WindowsSession();

            // 初始化定时器, 但不启动它.
            timer = new System.Threading.Timer(
                new System.Threading.TimerCallback(DetectSessionState),
                null,
                System.Threading.Timeout.Infinite,
                5000);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            // 注册StateChanged事件.
            session.StateChanged += new EventHandler<SessionSwitchEventArgs>(
                session_StateChanged);

        }

        /// <summary>
        /// 处理StateChanged事件.
        /// </summary>
        void session_StateChanged(object sender, SessionSwitchEventArgs e)
        {
            // 显示当前状态.
            lbState.Text = string.Format("当前状态: {0}    检查时间: {1} ",
                e.Reason, DateTime.Now);

            // 记录StateChanged事件.
            lstRecord.Items.Add(string.Format("{0}   {1} \t发生了", 
                DateTime.Now, e.Reason));

            lstRecord.SelectedIndex = lstRecord.Items.Count - 1;
        }

        private void chkEnableTimer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableTimer.Checked)
            {
                timer.Change(0, 5000);
            }
            else
            {
                timer.Change(0, System.Threading.Timeout.Infinite);
            }
            
        }

        void DetectSessionState(object obj)
        {

            // 检查当前会话是否被锁定.
            bool isCurrentLocked = session.IsLocked();

            var state = isCurrentLocked ? SessionSwitchReason.SessionLock
                : SessionSwitchReason.SessionUnlock;

            // 显示当前状态.
            lbState.Text = string.Format("当前状态: {0}    时间: {1} ",
               state, DateTime.Now);

            // 记录StateChanged事件.
            lstRecord.Items.Add(string.Format("{0}   {1}",
                DateTime.Now, state));

            lstRecord.SelectedIndex = lstRecord.Items.Count - 1;

        }
    }
}
