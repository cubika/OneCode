'/********************************* 模块头 **********************************\
'* 模块名:  SilverlightClient.aspx.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* Silverlight客户端主机aspx页面后台代码.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/


Partial Public Class SilverlightClient
    Inherits System.Web.UI.Page
    ' 下列属性将作为默认参数传递给Silverlight.
    Public Property IsAuthenticated As Boolean
    Public Property WelcomeMessage As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        ' 查询会话数据检查用户是否被验证.
        If Session("User") IsNot Nothing Then
            Me.IsAuthenticated = True
            Me.WelcomeMessage = "欢迎: " & DirectCast(Session("User"), String) & "."
        Else
            Me.IsAuthenticated = False
            Me.WelcomeMessage = Nothing
        End If
    End Sub
End Class