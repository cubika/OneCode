/****************************** 模块头 **************************************\
* 模块名:  ProcessCpuUsageMonitor.cs
* 项目名:  CSCpuUsage
* Copyright (c) Microsoft Corporation.
* 
* 它继承了类CpuUsageMonitorBase，可用来监视某一特定进程的CPU使用率. 它也提供了一
* 个得到可用进程的方法.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Diagnostics;
using System.Linq;

namespace CSCpuUsage
{
    public class ProcessCpuUsageMonitor : CpuUsageMonitorBase
    {
        const string categoryName = "Process";
        const string counterName = "% Processor Time";

        static PerformanceCounterCategory category;
        public static PerformanceCounterCategory Category
        {
            get
            {
                if (category == null)
                {
                    category = new PerformanceCounterCategory(categoryName);
                }
                return category;
            }
        }

        public ProcessCpuUsageMonitor(string processName, int timerPeriod, int valueArrayCapacity)
            : base(timerPeriod, valueArrayCapacity, categoryName, counterName, processName)
        {
        }


        protected override bool IsInstanceExist()
        {
            return Category.InstanceExists(this.cpuPerformanceCounter.InstanceName);
        }

        public static string[] GetAvailableProcesses()
        {
            return Category.GetInstanceNames().OrderBy(name => name).ToArray();
        }
    }
}
