'/**************************************** 模块头 *****************************************\
' 模块名:  Default.aspx.vb
' 项目名:  VBASPNETuserControlEventsExpose
' 版权 (c) Microsoft Corporation.
' 
' 这个页面载入了usercontrol同时添加usercontrol到网页.
' 然后预定用户控件的MyEvent以反应用户控件的按钮单击.
' 当单击usercontrol的按钮, 网页会显示dropdownlist的选择值.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\******************************************************************************************/


Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myUserControl As MyUserControl = CType(LoadControl("MyUserControl.ascx"), MyUserControl)
        If (Not (myUserControl) Is Nothing) Then
            AddHandler myUserControl.MyEvent, AddressOf Me.userControlBtnClick
            Me.PlaceHolder1.Controls.Add(myUserControl)
        End If
    End Sub
    Public Sub userControlBtnClick(ByVal sender As Object, ByVal e As EventArgs)
        Response.Write(("主页面事件句柄<BR/>所选择的值:" _
                        + (ddlTemp.SelectedItem.Text + "<BR/>")))
    End Sub

End Class