'********************************* 模块头 ***********************************\
' 模块名: BasePage.vb
' 项目名: VBASPNETEmbedLanguageInUrl
' 版权 (c) Microsoft Corporation
'
' UrlRoutingHandlers将会检查请求的url.
' 这个接口会截断url字符串,检查文件名如果不存在  
' 就把它们传到InvalidPage.aspx页面.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/


Imports System.Web.Compilation
Imports System.Web.Routing

Public Class UrlRoutingHandlers
    Implements IRouteHandler
    ''' <summary>
    ''' 创建RoutingHandler来检查Http请求和
    ''' 返回正确的url地址.
    ''' </summary>
    ''' <param name="context"></param>
    ''' <returns></returns>
    Public Function GetHttpHandler1(ByVal context As System.Web.Routing.RequestContext) As System.Web.IHttpHandler Implements System.Web.Routing.IRouteHandler.GetHttpHandler
        Dim language As String = context.RouteData.Values("language").ToString().ToLower()
        Dim pageName As String = context.RouteData.Values("pageName").ToString()
        If pageName = "ShowMe.aspx" Then
            Return TryCast(BuildManager.CreateInstanceFromVirtualPath("~/ShowMe.aspx", GetType(Page)), Page)
        Else
            Return BuildManager.CreateInstanceFromVirtualPath("~/InvalidPage.aspx", GetType(Page))
        End If
    End Function
End Class
