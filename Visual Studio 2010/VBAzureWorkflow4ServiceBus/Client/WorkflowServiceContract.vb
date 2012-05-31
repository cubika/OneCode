'****************************** 模块头 *************************************\
' Module Name:	WorkflowServiceContract.vb
' Project:		Client
' Copyright (c) Microsoft Corporation.
' 
' 该文件用来保存Windows Azure Platform AppFabric账户信息.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.ServiceModel

<ServiceContract()> _
Public Interface IProcessDataWorkflowService
	' Methods
	<OperationContract()> _
	Function ProcessData(ByVal request As ProcessDataRequest) As ProcessDataResponse
End Interface

Public Interface IProcessDataWorkflowServiceChannel
	Inherits IProcessDataWorkflowService, IClientChannel
End Interface

' 信息合约.
<MessageContract(IsWrapped:=False)> _
Public Class ProcessDataRequest
	Public Sub New()
	End Sub

	Public Sub New(ByVal int As Integer?)
		Me.int = int
	End Sub

	<MessageBodyMember(Namespace:="http://schemas.microsoft.com/2003/10/Serialization/", Order:=0)> _
	Public int As Integer?
End Class

<MessageContract(IsWrapped:=False)> _
Public Class ProcessDataResponse
	Public Sub New()
	End Sub

	Public Sub New(ByVal [string] As String)
		Me.string = [string]
	End Sub

	<MessageBodyMember(Namespace:="http://schemas.microsoft.com/2003/10/Serialization/", Order:=0)> _
 Public [string] As String
End Class








