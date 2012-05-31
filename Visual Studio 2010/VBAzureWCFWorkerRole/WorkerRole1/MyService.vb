'******************************** 模块头 *********************************\
'* 模块名:   MyService.vb
'* 项目名:   VBWorkerRoleHostingWCF 
'* 版权 (c) Microsoft Corporation.
'* 
'* 这个模块包含了MyService的服务合同.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\**************************************************************************

Imports System.ServiceModel

<ServiceContract()> _
Public Interface IMyService

    <OperationContract()> _
    Function DoWork() As String
End Interface


Public Class MyService
    Implements IMyService

    Public Function DoWork() As String Implements IMyService.DoWork
        Return "Hello World"
    End Function

End Class
