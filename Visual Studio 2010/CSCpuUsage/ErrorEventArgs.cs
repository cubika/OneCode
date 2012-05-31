/****************************** 模块头 **************************************\
* 模块名:  ErrorEventArgs.cs
* 项目名:  CSCpuUsage
* Copyright (c) Microsoft Corporation.
* 
* 类ErrorEventArgs被用于CpuUsageMonitorBase类的ErrorOccurred事件中.
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
    public class ErrorEventArgs : EventArgs
    {
        public Exception Error { get; set; }
    }
}

