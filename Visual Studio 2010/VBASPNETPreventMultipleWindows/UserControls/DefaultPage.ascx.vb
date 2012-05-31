'****************************** 模块头 ******************************\
' 模块名: DefaultPage.ascx.vb
' 项目名: VBASPNETPreventMultipleWindows
' 版权 (c) Microsoft Corporation
'
' 这是一个用在开始页面的用户控件.它将得到一个随机的字符串并把它赋值给window.name。
' 最后，跳转到Main.aspx页面。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/


Public Class DefaultPage
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    '/ <summary>
    '/ 当被调用时，这个方法将会得到一个随机的字符串
    '/ 并存储在session中
    '/ </summary>
    '/ <returns>返回这个随机字符串</returns>
    Public Function GetWindowName() As String
        Dim WindowName As String = Guid.NewGuid().ToString().Replace("-", "")
        Session("WindowName") = WindowName
        Return WindowName
    End Function

End Class