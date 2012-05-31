/****************************** 模块头 **************************************\
* 模块名:  TotalCpuUsageMonitor.cs
* 项目名:  CSCpuUsage
* Copyright (c) Microsoft Corporation.
* 
* 它继承了CpuUsageMonitorBase类，被用来得到总的CPU使用率.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CSCpuUsage
{
    public class TotalCpuUsageMonitor : CpuUsageMonitorBase
    {
        const string categoryName = "Processor";
        const string counterName = "% Processor Time";
        const string instanceName = "_Total";

        public TotalCpuUsageMonitor(int timerPeriod, int valueArrayCapacity)
            : base(timerPeriod, valueArrayCapacity, categoryName, counterName, instanceName)
        { }
    }
}