'****************************** 模块头 *************************************\
' Module Name:	WebRole.vb
' Project:		Client
' Copyright (c) Microsoft Corporation.
' 
' 该项目WebRole项目，用来托管WCF Workflow Service.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.Diagnostics
Imports Microsoft.WindowsAzure.ServiceRuntime

Public Class WebRole
    Inherits RoleEntryPoint

    Public Overrides Function OnStart() As Boolean

        ' 关于处理配置变化的信息
        ' 请参阅MSDN： http://go.microsoft.com/fwlink/?LinkId=166357.
 
        Return MyBase.OnStart()

    End Function

End Class

