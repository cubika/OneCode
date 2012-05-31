'/**************************************** 模块头 *****************************************\
'* 模块名:    CurrentOnlineUserList.aspx.vb
'* 项目名:    VBASPNETCurrentOnlineUserList
'* 版权 (c) Microsoft Corporation
'*
'* 本页面用来显示当前在线用户的信息. 
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/

Public Class CurrentOnlineUserList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 检查用户是否登入.
        CheckLogin()
    End Sub
    Public Sub CheckLogin()
        Dim _userticket As String = ""
        If Session("Ticket") IsNot Nothing Then
            _userticket = Session("Ticket").ToString()
        End If
        If _userticket <> "" Then
            ' 初始化保存当前在线用户的信息的datatable.
            Dim _onlinetable As New DataTableForCurrentOnlineUser()

            ' 通过ticket检查用户是否在线.
            If _onlinetable.IsOnline_byTicket(Me.Session("Ticket").ToString()) Then
                ' 更新最近活动时间.
                _onlinetable.ActiveTime(Session("Ticket").ToString())

                ' 绑定保存当前在线用户的信息的datatable到gridview控件.
                gvUserList.DataSource = _onlinetable.ActiveUsers
                gvUserList.DataBind()
            Else
                ' 如果当前用户不在表中,重定向页面到LogoOut.
                Response.Redirect("LogOut.aspx")
            End If
        Else
            Response.Redirect("Login.aspx")
        End If
    End Sub
End Class


