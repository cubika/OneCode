/******************************** 模块头 ****************************\
模块名:  ProcessWatcher.cs
项目名:      CSProcessWatcher
版权 (c) Microsoft Corporation.

使用WMI查询语言来查询进程事件.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Management;

namespace CSProcessWatcher
{
    public delegate void ProcessEventHandler(WMI.Win32.Process proc);

    public class ProcessWatcher : ManagementEventWatcher
    {
        // 进程事件
        public event ProcessEventHandler ProcessCreated;
        public event ProcessEventHandler ProcessDeleted;
        public event ProcessEventHandler ProcessModified;

        // WMI查询字符串
        static readonly string processEventQuery = @"SELECT * FROM 
             __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process' 
             and TargetInstance.Name = '{0}'";
                
        public ProcessWatcher(string processName)
        {
            Init(processName);
        }

        private void Init(string processName)
        {
            this.Query.QueryLanguage = "WQL";
            this.Query.QueryString = string.Format(processEventQuery, processName);
            this.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
        }

        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string eventType = e.NewEvent.ClassPath.ClassName;
            WMI.Win32.Process proc = new WMI.Win32.Process(e.NewEvent["TargetInstance"] as ManagementBaseObject);

            switch (eventType)
            {
                case "__InstanceCreationEvent":
                    if (ProcessCreated != null)
                    {
                        ProcessCreated(proc);
                    }
                    break;

                case "__InstanceDeletionEvent":
                    if (ProcessDeleted != null)
                    {
                        ProcessDeleted(proc);
                    }
                    break;

                case "__InstanceModificationEvent":
                    if (ProcessModified != null)
                    {
                        ProcessModified(proc);
                    }
                    break;
            }
        }
    }
}
