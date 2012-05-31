/****************************** 模块头 *************************************\
* Module Name:	WebRole.cs
* Project:		Client
* Copyright (c) Microsoft Corporation.
* 
* 该项目WebRole项目，用来托管WCF Workflow Service.
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
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WorkflowServiceWebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // 关于处理配置变化的信息
            // 请参阅MSDN： http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
