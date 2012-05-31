'****************************** 模块头 *************************************\
' Module Name:	RestService.vb
' Project:		VBAzureServiceBusSLRest
' Copyright (c) Microsoft Corporation.
' 
' 这是WCF REST Service的服务实现.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.IO
Imports System.ServiceModel.Web
Imports System.Net

Public Class RestService
	Implements IRestService
	Public Function DownloadFile(ByVal fileName As String) As Stream _
	 Implements IRestService.DownloadFile
		Try
			Using fs As FileStream = File.Open(fileName, FileMode.Open, FileAccess.Read)
				Dim stream As New MemoryStream
				Dim buffer As Byte() = New Byte(fs.Length - 1) {}
				fs.Read(buffer, 0, buffer.Length)
				stream.Write(buffer, 0, buffer.Length)
				stream.Position = 0
				Return stream
			End Using
		Catch ex As IOException
			WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound
			Return New MemoryStream
		End Try
	End Function

	Public Function GetClientAccessPolicy() As Stream _
	 Implements IRestService.GetClientAccessPolicy
		Return Me.DownloadFile("clientaccesspolicy.xml")
	End Function

	Public Function UploadFile(ByVal fileName As String, ByVal content As Stream) As String _
	 Implements IRestService.UploadFile
        ' 我们不知道HTTP请求流的长度,所以我们必须使用以下代码读出流的长度:
		Dim bufferSize As Integer = 4096
		Dim bytesRead As Integer = 1
		Dim totalBytesRead As Integer = 0
		Try
			Using fileStream As FileStream = File.Create(fileName)
				Dim buffer As Byte() = New Byte(bufferSize - 1) {}
				bytesRead = content.Read(buffer, 0, bufferSize)
				Do While (bytesRead > 0)
					fileStream.Write(buffer, 0, bytesRead)
					bytesRead = content.Read(buffer, 0, bufferSize)
					totalBytesRead = (totalBytesRead + bytesRead)
				Loop
			End Using

            ' 按照REST服务的最近实践，当资源被创建时，返回一个201 Created状态代码代理默认的200.
			' 这意味着Silverlight客户端必须使用HTTP栈，替代默认的浏览器HTTP栈使用服务来工作.
			Return RestService.WriteResponse(HttpStatusCode.Created, "Created.")
		Catch ex As Exception
			Return RestService.WriteResponse(HttpStatusCode.InternalServerError, ex.Message)
		End Try
	End Function

	Private Shared Function WriteResponse(ByVal statusCode As HttpStatusCode, ByVal message As String) As String
		WebOperationContext.Current.OutgoingResponse.StatusCode = statusCode
		Return message
	End Function

End Class
