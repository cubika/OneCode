/****************************** 模块头******************************\
 模块名:  MainForm.cs
 项目:      CSCheckProcessType
 版权 (c) Microsoft Corporation.
 
  这是应用程序的主窗口, 用来注册和处理用户界面的事件
 
 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 All other rights reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace CSCheckProcessType
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            if (!RunningProcess.IsOSVersionSupported)
            {
                MessageBox.Show("该应用程序必须运行在Windows Vista或更高版本的操作系统。");
                btnRefresh.Enabled = false;
            }
        }

        /// <summary>
        ///处理btnRefresh_Click事件。
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DatabindGridView();
        }

        /// <summary>
        /// 绑定DataGridView的数据源。
        /// </summary>
        void DatabindGridView()
        {
            List<RunningProcess> runningProcesses = new List<RunningProcess>();
            foreach (var proc in Process.GetProcesses().OrderBy(p => p.Id))
            {
                RunningProcess runningProcess = new RunningProcess(proc);
                runningProcesses.Add(runningProcess);
            }
            gvProcess.DataSource = runningProcesses;
        }
    }
}
