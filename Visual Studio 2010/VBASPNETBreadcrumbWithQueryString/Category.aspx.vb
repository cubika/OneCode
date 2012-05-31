'********************************* 模块头 *********************************\
' 模块名:             Category.aspx.vb
' 项目名:        VBASPNETBreadcrumbWithQueryString
' 版权(c) Microsoft Corporation
'
' 此页面显示项目列表,并显示breadcrumb分类名称.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Public Class Category
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack AndAlso Not String.IsNullOrEmpty(Request.QueryString("name")) Then
            ' 显示项目列表.
            gvItems.DataSource = Database.GetItemsByCategory(Request.QueryString("name"))
            gvItems.DataBind()

            ' 处理SiteMapResolve事件动态改变当前SiteMapNode.
            AddHandler SiteMap.SiteMapResolve, AddressOf SiteMap_SiteMapResolve
        End If
    End Sub

    ''' <summary>
    ''' 当访问CurrentNode属性时发生.
    ''' </summary>
    ''' <param name="sender">
    ''' 事件源, SiteMapProvider类的实例.
    ''' </param>
    ''' <param name="e">
    ''' 包含事件数据的SiteMapResolveEventArg.
    ''' </param>
    ''' <returns>
    ''' 表示SiteMapResolveEventHandler处理结果的SiteMapNode.
    ''' </returns>
    Private Function SiteMap_SiteMapResolve(ByVal sender As Object,
                                            ByVal e As SiteMapResolveEventArgs) As SiteMapNode
        ' 一次请求只执行一次.
        RemoveHandler SiteMap.SiteMapResolve, AddressOf SiteMap_SiteMapResolve

        If SiteMap.CurrentNode IsNot Nothing Then
            ' SiteMap.CurrentNode是只读的,因此我们必须复制一份进行操作.
            Dim currentNode As SiteMapNode = SiteMap.CurrentNode.Clone(True)

            currentNode.Title = Request.QueryString("name")

            ' 在breadcrumb中使用已被修改的项.
            Return currentNode
        End If
        Return Nothing
    End Function

End Class