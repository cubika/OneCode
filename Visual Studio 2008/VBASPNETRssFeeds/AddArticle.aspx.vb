'********************************* 模块头 *********************************\
' 模块名:  AddArticle.aspx.vb
' 项目名:  VBASPNETRssFeeds
' 版权 (c) Microsoft Corporation
'
' 这个页面支持一个更新数据库的特性
' 用来测试RSS页面是否正常工作.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/


Partial Public Class AddArticle
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub ArticleFormView_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArticleFormView.PreRender
        Dim PubDateTextBox As TextBox
        PubDateTextBox = CType(ArticleFormView.FindControl("PubDateTextBox"), TextBox)
        If PubDateTextBox IsNot Nothing Then
            PubDateTextBox.Text = DateTime.Now.ToShortDateString()
        End If

    End Sub
End Class