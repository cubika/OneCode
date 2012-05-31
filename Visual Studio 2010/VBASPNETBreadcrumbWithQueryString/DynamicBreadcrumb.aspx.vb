'********************************* 模块头 *********************************\
' 模块名:             DynamicBreadcrumb.aspx.vb
' 项目名:        VBASPNETBreadcrumbWithQueryString
' 版权(c) Microsoft Corporation
'
' 此页面显示即使一个页面不在站点地图中,我们依然可以动态创建breadcrumb. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Public Class DynamicBreadcrumb
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
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
    ''' 包含事件数据的SiteMapResolveEventArgs.
    ''' </param>
    ''' <returns>
    ''' 表示SiteMapResolveEventHandler处理结果的SiteMapNode.
    ''' </returns>
    Private Function SiteMap_SiteMapResolve(ByVal sender As Object,
                                            ByVal e As SiteMapResolveEventArgs) As SiteMapNode
        ' 一次请求只执行一次.
        RemoveHandler SiteMap.SiteMapResolve, AddressOf SiteMap_SiteMapResolve

        ' 我们可以在此创建很多SiteMapNodes.
        Dim childNode As New SiteMapNode(SiteMap.Provider, "2")
        childNode.Url = "/child.aspx"
        childNode.Title = "子页面"

        childNode.ParentNode = New SiteMapNode(SiteMap.Provider, "1")
        childNode.ParentNode.Url = "/root.aspx"
        childNode.ParentNode.Title = "根页面"

        ' 我们也可以关联动态节点到已有的站点地图.
        Dim nodeFromSiteMap As SiteMapNode = GetSiteMapNode("item")
        If nodeFromSiteMap IsNot Nothing Then
            childNode.ParentNode.ParentNode = nodeFromSiteMap
        End If

        ' 在breadcrumb中使用新的SiteMapNode.
        Return childNode
    End Function

    ''' <summary>
    ''' 自站点地图获取siteMapNode.
    ''' </summary>
    ''' <param name="key">
    ''' siteMapNode的resourceKey.
    ''' </param>
    ''' <returns></returns>
    Private Function GetSiteMapNode(ByVal key As String) As SiteMapNode
        Return GetSiteMapNode(SiteMap.RootNode, key)
    End Function
    Private Function GetSiteMapNode(ByVal rootNode As SiteMapNode, ByVal key As String) As SiteMapNode
        If rootNode.ResourceKey = key Then
            Return rootNode
        ElseIf rootNode.HasChildNodes Then
            For Each childNode As SiteMapNode In rootNode.ChildNodes
                Dim resultNode As SiteMapNode = GetSiteMapNode(childNode, key)
                If resultNode IsNot Nothing Then
                    Return resultNode
                End If
            Next
        End If
        Return Nothing
    End Function

End Class