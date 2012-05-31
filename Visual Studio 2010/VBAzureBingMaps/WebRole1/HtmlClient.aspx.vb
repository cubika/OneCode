'/********************************* 模块头 **********************************\
'* 模块名:  HtmlClient.aspx.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* HTML客户端主机aspx页面后台代码.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Partial Public Class HtmlClient
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        ' 查询会话状态数据检查用户是否被授权.
        ' 然后根据相关信息显示登入链接或欢迎信息.
        If Session("User") IsNot Nothing Then
            Me.LoginLink.Visible = False
            Me.UserNameLabel.Visible = True
            Me.UserNameLabel.Text = "欢迎, " & DirectCast(Session("User"), String) & "."
        Else
            Me.LoginLink.Visible = True
            Me.UserNameLabel.Visible = False
        End If
    End Sub
End Class