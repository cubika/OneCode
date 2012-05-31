'\**************************** 模块头 ********************************\
' 模块名:  Login.aspx.vb
' 项目名:  VBASPNETAutoLogin
' 版权 (c) Microsoft Corporation.
' 
' 这个页面用来演示用户的登录信息.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Partial Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Protected Sub LoginButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LoginButton.Click
        Response.Clear()
        Response.Write("你已经登录本网站." + "<br>")
        Response.Write("你的用户名：" + Request.Form("UserName").ToString() + "<br>")
        Response.Write("你的密码：" + Request.Form("Password").ToString() + "<br>")
        Response.End()
    End Sub
End Class