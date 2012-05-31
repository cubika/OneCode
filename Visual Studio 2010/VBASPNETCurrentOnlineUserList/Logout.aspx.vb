'/**************************************** 模块头 *****************************************\
'* 模块名:    Logout.aspx.vb
'* 项目名:    VBASPNETCurrentOnlineUserList
'* 版权 (c) Microsoft Corporation
'*
'* 这个页面用来使用户登出. 
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/


Public Class Logout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 初始化保存当前在线用户的信息的datatable.
        Dim _onlinetable As New DataTableForCurrentOnlineUser()
        If Me.Session("Ticket") IsNot Nothing Then
            ' 登出.
            _onlinetable.Logout(Me.Session("Ticket").ToString())
            Me.Session.Clear()
        End If
    End Sub

End Class