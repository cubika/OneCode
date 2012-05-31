'****************************** 模块头 *************************************\
' Module Name:	Program.vb
' Project:		Client
' Copyright (c) Microsoft Corporation.
' 
' 该项目为客户端程序，用来检查workflow服务工作是否良好.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports Client.WorkflowServiceReference

Module Program

    Sub Main()
        Dim client = New ServiceClient()
        Console.WriteLine("正在处理 10...")
        Console.WriteLine("服务返回: " + client.ProcessData(10))
        Console.WriteLine("正在处理 30...")
        Console.WriteLine("服务返回: " + client.ProcessData(30))
        Console.ReadLine()
    End Sub

End Module
