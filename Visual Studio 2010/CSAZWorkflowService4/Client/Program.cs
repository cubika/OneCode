/****************************** 模块头 *************************************\
* Module Name:	Program.cs
* Project:		Client
* Copyright (c) Microsoft Corporation.
* 
* 该项目为客户端程序，用来检查workflow服务工作是否良好.
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
using Client.WorkflowServiceReference;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceClient client = new ServiceClient();
            Console.WriteLine("正在处理 10...");
            Console.WriteLine("服务返回: " + client.ProcessData(10));
            Console.WriteLine("正在处理 30...");
            Console.WriteLine("服务返回: " + client.ProcessData(30));
            Console.ReadLine();
        }
    }
}
