'****************************** 模块头 ******************************\
' 模块名:    ClientAdapter.vb
' 项目名:    VBASPNETReverseAJAX
' 版权 (c) Microsoft Corporation
'
' ClientAdapter类管理多个客户端实体.表示层调用它的方法可以
' 很容易的发送和接收消息.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'********************************************************************/

Imports System.Collections.Generic

''' <summary>
''' 这个类用来向多个客户端发送事件/消息.
''' </summary>
Public Class ClientAdapter
    ''' <summary>
    ''' 收件人列表.
    ''' </summary>
    Private recipients As New Dictionary(Of String, Client)()

    ''' <summary>
    ''' 向特定的收件人发送消息.
    ''' </summary>
    Public Sub SendMessage(ByVal message As Message)
        If recipients.ContainsKey(message.RecipientName) Then
            Dim client As Client = recipients(message.RecipientName)

            client.EnqueueMessage(message)
        End If
    End Sub

    ''' <summary>
    ''' 调用一个单独的接收人来等待接收消息.
    ''' </summary>
    ''' <returns>消息内容</returns>
    Public Function GetMessage(ByVal userName As String) As String
        Dim messageContent As String = String.Empty

        If recipients.ContainsKey(userName) Then
            Dim client As Client = recipients(userName)

            messageContent = client.DequeueMessage().MessageContent
        End If

        Return messageContent
    End Function

    ''' <summary>
    ''' 添加一个用户到收件人列表.
    ''' </summary>
    Public Sub Join(ByVal userName As String)
        recipients(userName) = New Client()
    End Sub

    ''' <summary>
    ''' 单例模式.
    ''' 这个模式将会确保在系统中只有一个类的实例.
    ''' </summary>
    Public Shared Instance As New ClientAdapter()
    Private Sub New()
    End Sub
End Class