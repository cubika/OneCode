'****************************** 模块头 ******************************\
' 模块名:    Sender.aspx.vb
' 项目名:    VBASPNETReverseAJAX
' 版权 (c) Microsoft Corporation
'
' 用户使用这个页面发送消息到特定接收者.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'********************************************************************/

Partial Public Class Sender
    Inherits System.Web.UI.Page
    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' 创建一个消息实体来包含所有必要数据.
        Dim message As New Message()
        message.RecipientName = tbRecipientName.Text.Trim()
        message.MessageContent = tbMessageContent.Text.Trim()

        If Not String.IsNullOrWhiteSpace(message.RecipientName) AndAlso Not String.IsNullOrEmpty(message.MessageContent) Then
            ' 调用客户端的适配器把消息立即发送到特定的接收者.
            ClientAdapter.Instance.SendMessage(message)

            ' 显示时间戳.
            lbNotification.Text += DateTime.Now.ToLongTimeString() & ": 消息已发送!<br/>"
        End If
    End Sub
End Class