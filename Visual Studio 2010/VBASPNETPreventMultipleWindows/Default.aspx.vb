'/****************************** 模块头 ******************************\
' 模块名: Default.aspx.vb
' 项目名: VBASPNETPreventMultipleWindows
' 版权 (c) Microsoft Corporation
'
' 本项目阐述如何在应用程序中检测并阻止多个窗体或标签的使用。
' 当用户想在一个新窗体或标签中打开这个链接,应用程序回拒绝这些请求。
' 这个方法可以解决许多安全问题,像共享sessions,保护dupicated登陆,数据并发等。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************/


Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class