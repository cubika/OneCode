/****************************** 模块头 ******************************\
* 模块名:  CpuUsageValueArrayChangedEventArg.cs
* 项目名:  CSCpuUsage
* Copyright (c) Microsoft Corporation.
* 
* CpuUsageValueArrayChangedEventArg类被用于CpuUsageMonitorBase类的
* CpuUsageValueArrayChanged事件.
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

namespace CSCpuUsage
{
    public class CpuUsageValueArrayChangedEventArg : EventArgs
    {
        public double[] Values { get; set; }
    }
}
