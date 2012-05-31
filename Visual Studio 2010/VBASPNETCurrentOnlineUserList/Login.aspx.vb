'/**************************************** 模块头 *****************************************\
'* 模块名:    Login.aspx.vb
'* 项目名:    VBASPNETCurrentOnlineUserList
'* 版权 (c) Microsoft Corporation
'*
'* 这个页面用来使用户登入. 
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/


Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim _error As String = ""

        ' 检查用户输入数据值.
        If check_text(_error) Then
            ' 初始化保存当前在线用户的信息的datatable.
            Dim _onLineTable As New DataTableForCurrentOnlineUser()

            ' 用户身份实例.
            Dim _user As New UserEntity()
            _user.Ticket = DateTime.Now.ToString("yyyyMMddHHmmss")
            _user.UserName = tbUserName.Text.Trim()
            _user.TrueName = tbTrueName.Text.Trim()
            _user.ClientIP = Me.Request.UserHostAddress
            _user.RoleID = "MingXuGroup"

            ' 使用session变量保存the ticket.
            Me.Session("Ticket") = _user.Ticket

            ' 登入.
            _onLineTable.Login(_user, True)
            Response.Redirect("CurrentOnlineUserList.aspx")
        Else
            Me.lbMessage.Visible = True
            Me.lbMessage.Text = _error
        End If
    End Sub
    Public Function check_text(ByRef errormessage As String) As Boolean
        errormessage = ""
        If Me.tbUserName.Text.Trim() = "" Then
            errormessage = "Please enter the username"
            Return False
        End If
        If Me.tbTrueName.Text.Trim() = "" Then
            errormessage = "Please enter the truename"
            Return False
        End If
        Return True
    End Function


End Class