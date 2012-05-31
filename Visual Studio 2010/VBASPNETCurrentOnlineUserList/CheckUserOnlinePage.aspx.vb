'/**************************************** 模块头 *****************************************\
'* 模块名:    CheckUserOnlinePage.aspx
'* 项目名:    VBASPNETCurrentOnlineUserList
'* 版权 (c) Microsoft Corporation
'*
'* 本页面用来获得来自其他包含CheckUserOnline自定义控件页面的请求
'* 同时从CurrentOnlineUser表中删除离线用户.
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/


Public Class CheckUserOnlinePage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Check()
    End Sub
    Public Overridable Property SessionName() As String
        Get
            Dim _obj1 As Object = Me.ViewState("SessionName")
            If _obj1 IsNot Nothing Then
                Return DirectCast(_obj1, String).Trim()
            End If
            Return "Ticket"
        End Get
        Set(ByVal value As String)
            Me.ViewState("SessionName") = value
        End Set
    End Property
    Protected Sub Check()
        Dim _myTicket As String = ""
        If System.Web.HttpContext.Current.Session(Me.SessionName) IsNot Nothing Then
            _myTicket = System.Web.HttpContext.Current.Session(Me.SessionName).ToString()
        End If
        If _myTicket <> "" Then
            ' 初始化保存当前在线用户的信息的datatable.
            Dim _onlinetable As New DataTableForCurrentOnlineUser()

            ' 当页面更新或获得请求时刷新时间.
            _onlinetable.RefreshTime(_myTicket)
            Response.Write("OK：" & DateTime.Now.ToString())
        Else
            Response.Write("Sorry：" & DateTime.Now.ToString())
        End If
    End Sub
End Class
