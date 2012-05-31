'****************************** 模块头 *************************************\
' Module Name:	IRestService.vb
' Project:		VBAzureServiceBusSLRest
' Copyright (c) Microsoft Corporation.
' 
' 该文件是WCF REST Service的服务契约.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.ServiceModel.Web
Imports System.ServiceModel
Imports System.IO

<ServiceContract()> _
Public Interface IRestService
	''' <summary>
    ''' 用法: https://namespace.servicebus.windows.net/file/filename
	''' </summary>
	<WebGet(UriTemplate:="/file/{fileName}"), OperationContract()> _
	  Function DownloadFile(ByVal fileName As String) As Stream

	''' <summary>
    ''' 用法: https://namespace.servicebus.windows.net/clientaccesspolicy.xml
	''' </summary>
	<WebGet(UriTemplate:="/clientaccesspolicy.xml"), OperationContract()> _
 Function GetClientAccessPolicy() As Stream

	''' <summary>
    ''' 用法: https://namespace.servicebus.windows.net/file/filename
    ''' 注意这是一个POST操作，不能被浏览器调用.
	''' </summary>
	<WebInvoke(UriTemplate:="/file/{fileName}", Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare), OperationContract()> _
 Function UploadFile(ByVal fileName As String, ByVal content As Stream) As String

End Interface
