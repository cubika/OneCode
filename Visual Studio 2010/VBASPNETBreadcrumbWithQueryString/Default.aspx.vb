'********************************* 模块头 *********************************\
' 模块名:             Default.aspx.vb
' 项目名:        VBASPNETBreadcrumbWithQueryString
' 版权(c) Microsoft Corporation
'
' 这是用来显示分类列表的根页面.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            ' 显示分类列表.
            gvCategories.DataSource = Database.Categories
            gvCategories.DataBind()
        End If
    End Sub

End Class