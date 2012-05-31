'/**************************************** 模块头 *****************************************\
' 模块名:  MyUserControl.ascx.vb
' 项目名:  VBASPNETuserControlEventsExpose
' 版权 (c) Microsoft Corporation.
' 
' 用户控件以公用方式声明了委托和事件.
' 因此事件将被预定.
' 当btnTest被单击,他将触发名为'MyEvent'的事件.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\******************************************************************************************/
Partial Public Class MyUserControl
    Inherits System.Web.UI.UserControl

    Public Delegate Sub MyEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Event MyEvent As MyEventHandler

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnTest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnTest.Click
        RaiseEvent MyEvent(sender, e)
        Response.Write("用户控件按钮被点击<BR/>")
    End Sub
End Class