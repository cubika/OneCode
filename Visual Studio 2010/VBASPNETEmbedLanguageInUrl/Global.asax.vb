Imports System.Web.SessionState
Imports System.Web.Routing

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        RegisterRoutes(RouteTable.Routes)
    End Sub
    ''' <summary>
    ''' Url地址
    ''' </summary>
    ''' <param name="routes"></param>
    Public Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.Add("Page", New Route("{language}/{pageName}", New UrlRoutingHandlers()))
    End Sub


    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' 当session被启动时引发
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' 在每个请求开始时引发
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' 试图认证使用时引发
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' 当错误出现时引发
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' 当session结束时引发
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' 当应用程序结束时引发
    End Sub

End Class