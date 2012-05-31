'/********************************** 模块头 ***********************************\
' 模块名:        SharedSessionModule.vb
' 项目名:        VBASPNETShareSessionBetweenSubDomains
' 版权(c) Microsoft Corporation
'
' SharedSessionModule使网站使用相同的应用程序ID和会话ID来实现共享会话.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************/

Imports System.Web
Imports System.Configuration
Imports System.Reflection

''' <summary>
''' 子域内应用程序间用来共享对话的HttpModule.
''' </summary>
Public Class SharedSessionModule
    Implements IHttpModule
    ' 内存缓存设定.
    Protected Shared applicationName As String = ConfigurationManager.AppSettings("ApplicationName")
    Protected Shared rootDomain As String = ConfigurationManager.AppSettings("RootDomain")

#Region "IHttpModule Members"
    ''' <summary>
    ''' 初始化模块准备处理请求.
    ''' </summary>
    ''' <param name="context">
    ''' 提供了在ASP.NET应用程序对所有的应用程序对象的
    ''' 共同的方法，属性和事件的访问的System.Web.HttpApplication.
    ''' </param>
    Public Sub Init(ByVal context As HttpApplication) Implements IHttpModule.Init
        ' 模块同时需要应用程序名和根域才能工作.
        If String.IsNullOrEmpty(applicationName) OrElse String.IsNullOrEmpty(rootDomain) Then
            Return
        End If

        ' 运行时改变应用程序名.
        Dim runtimeInfo As FieldInfo = GetType(HttpRuntime).GetField("_theRuntime", BindingFlags.[Static] Or BindingFlags.NonPublic)
        Dim theRuntime As HttpRuntime = DirectCast(runtimeInfo.GetValue(Nothing), HttpRuntime)
        Dim appNameInfo As FieldInfo = GetType(HttpRuntime).GetField("_appDomainAppId", BindingFlags.Instance Or BindingFlags.NonPublic)

        appNameInfo.SetValue(theRuntime, applicationName)

        ' 订阅事件.
        AddHandler context.PostRequestHandlerExecute, AddressOf context_PostRequestHandlerExecute
    End Sub

    ''' <summary>
    ''' 处理模块实现所使用的资源(除内存外).
    ''' </summary>
    Public Sub Dispose() Implements IHttpModule.Dispose
    End Sub
#End Region

    ''' <summary>
    ''' 在发送到客户端的响应内容前，更改cookie到根域
    ''' 保存当前会话ID.
    ''' </summary>
    ''' <param name="sender">
    ''' 提供了在ASP.NET应用程序对所有的应用程序对象的
    ''' 共同的方法，属性和事件的访问的System.Web.HttpApplication实例.
    ''' </param>
    Private Sub context_PostRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)
        Dim context As HttpApplication = DirectCast(sender, HttpApplication)

        ' 在cookie中保存指定当前会话ASP.NET会话Id.
        Dim cookie As HttpCookie = context.Response.Cookies("ASP.NET_SessionId")

        If context.Session IsNot Nothing AndAlso Not String.IsNullOrEmpty(context.Session.SessionID) Then
            ' 需要在每次请求保存当前会话Id.
            cookie.Value = context.Session.SessionID

            ' 所有使用保存此Cookie的应用程序在同一根域
            ' 因此可以被共享.
            If rootDomain <> "localhost" Then
                cookie.Domain = rootDomain
            End If

            ' 所有虚拟应用程序和文件夹也共享此Cookie.
            cookie.Path = "/"
        End If
    End Sub
End Class
