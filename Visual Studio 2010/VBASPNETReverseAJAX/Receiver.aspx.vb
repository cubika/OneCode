'****************************** 模块头 ******************************\
' 模块名:    Receiver.aspx.vb
' 项目名:    VBASPNETReverseAJAX
' 版权 (c) Microsoft Corporation
'
' 用户将会使用独特的用户名通过这个页面来登陆.当在服务器上有一条
' 新的消息时,服务器会直接把消息发送到客户端.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'********************************************************************/

Partial Public Class Receiver
    Inherits System.Web.UI.Page
    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim userName As String = tbUserName.Text.Trim()

        ' 加入收件人列表.
        If Not String.IsNullOrEmpty(userName) Then
            ClientAdapter.Instance.Join(userName)

            Session("userName") = userName
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs)
        ' 激活JavaScript等待循环.
        If Session("userName") IsNot Nothing Then
            Dim userName As String = DirectCast(Session("userName"), String)

            ' 调用JavaScript的waitEvent方法来开始等待循环.
            ClientScript.RegisterStartupScript(Me.[GetType](), "ActivateWaitingLoop", "waitEvent();", True)

            lbNotification.Text = String.Format("你的用户名是 <b>{0}</b>. 正在等待新的消息.", userName)

            ' 禁用登陆.
            tbUserName.Visible = False
            btnLogin.Visible = False
        End If
    End Sub
End Class