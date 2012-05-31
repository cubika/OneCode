/******************************** 模块头 ****************************\
模块名:  Program.cs
项目名:      CSProcessWatcher
版权 (c) Microsoft Corporation.

这个实例演示如何使用Windows Management Instrumentation(WMI)来检测进程的创建/修改/关闭事件.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Management;
#endregion

namespace CSProcessWatcher
{
    class Program
    {
        private static string processName = "notepad.exe"; // 默认进程名.

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                processName = args[0];
            }

            ProcessWatcher procWatcher = new ProcessWatcher(processName);
            procWatcher.ProcessCreated += new ProcessEventHandler(procWatcher_ProcessCreated);
            procWatcher.ProcessDeleted += new ProcessEventHandler(procWatcher_ProcessDeleted);
            procWatcher.ProcessModified += new ProcessEventHandler(procWatcher_ProcessModified);
            procWatcher.Start();

            Console.WriteLine(processName + " 正在被监控...");
            Console.WriteLine("按回车键停止监控...");

            Console.ReadLine();

            procWatcher.Stop();

        }

        private static void procWatcher_ProcessCreated(WMI.Win32.Process proc)
        {
            Console.Write("\n进程被创建\n " + proc.Name + " " + proc.ProcessId + "  " + "时间:" + DateTime.Now);
        }

        private static void procWatcher_ProcessDeleted(WMI.Win32.Process proc)
        {
            Console.Write("\n进程被关闭\n " + proc.Name + " " + proc.ProcessId + "  " + "时间:" + DateTime.Now);
        }

        private static void procWatcher_ProcessModified(WMI.Win32.Process proc)
        {
            Console.Write("\n进程被修改\n " + proc.Name + " " + proc.ProcessId + "  " + "时间:" + DateTime.Now);
        }
    }
}
