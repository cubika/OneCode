'****************************** 模块头 ************************************'
' 模块名:   WebBrowserEx.vb
' 项目名:	VBWebBrowserSuppressError
' 版权(c)   Microsoft Corporation.
' 
' 此WebBrowserEx类继承自WebBrowser类，并提供了如下特征:
' 1. 禁用了JIT调试器.
' 2. 忽略了浏览器中载入的html文档对象的html元素错误.
' 3. 处理链接错误.
' 
' WebBrowser类自身也有个ScriptErrorsSuppressed属性,用来隐藏它所有源于底层
' ActiveX控件的对话框,不仅仅只是脚本错误.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************'

Imports System.Security.Permissions
Imports Microsoft.Win32

Partial Public Class WebBrowserEx
    Inherits WebBrowser

    ''' <summary>
    ''' 获取或设置JIT调试器是否需要被禁用.您必须重启浏览器使之生效.
    ''' </summary>
    Public Shared Property JITDebuggerDisabled() As Boolean
        Get
            Using ieMainKey As RegistryKey = Registry.CurrentUser.OpenSubKey(
                "Software\Microsoft\Internet Explorer\Main")
                Dim keyvalue As String =
                    TryCast(ieMainKey.GetValue("Disable Script Debugger"), String)
                Return String.Equals(keyvalue, "yes", StringComparison.OrdinalIgnoreCase)
            End Using
        End Get
        Set(ByVal value As Boolean)
            Dim newValue = If(value, "yes", "no")

            Using ieMainKey As RegistryKey = Registry.CurrentUser.OpenSubKey(
                "Software\Microsoft\Internet Explorer\Main", True)
                Dim keyvalue As String =
                    TryCast(ieMainKey.GetValue("Disable Script Debugger"), String)
                If Not keyvalue.Equals(newValue, StringComparison.OrdinalIgnoreCase) Then
                    ieMainKey.SetValue("Disable Script Debugger", newValue)
                End If
            End Using
        End Set
    End Property

    ' 忽略html元素错误.
    Public Property HtmlElementErrorsSuppressed() As Boolean

    Private cookie As AxHost.ConnectionPointCookie

    Private helper As WebBrowser2EventHelper

    Public Event NavigateError As EventHandler(Of WebBrowserNavigateErrorEventArgs)

    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 注册Document.Window.Error事件.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub OnDocumentCompleted(ByVal e As WebBrowserDocumentCompletedEventArgs)
        MyBase.OnDocumentCompleted(e)

        ' 当窗口内部运行的脚本遇到一个运行时错误时产生.
        AddHandler Document.Window.Error, AddressOf Window_Error

    End Sub



    ''' <summary>
    ''' 处理此浏览器中载入的html文档对象的html元素错误.
    ''' 如果HtmlElementErrorsSuppressed设定成true,则设定处理标识为true以防止浏览器
    ''' 显示这个错误.
    ''' </summary>
    Protected Sub Window_Error(ByVal sender As Object, ByVal e As HtmlElementErrorEventArgs)
        If HtmlElementErrorsSuppressed Then
            e.Handled = True
        End If
    End Sub

    ''' <summary>
    ''' 给底层ActiveX控件关联一个能够处理控件事件(包括链接错误事件)的客户端.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub CreateSink()
        MyBase.CreateSink()

        helper = New WebBrowser2EventHelper(Me)
        cookie = New AxHost.ConnectionPointCookie(Me.ActiveXInstance, helper,
                                                  GetType(DWebBrowserEvents2))
    End Sub

    ''' <summary>
    ''' 从底层ActiveX控件解除给其关联的附加在CreateSink方法的事件处理客户端.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub DetachSink()
        If Not cookie Is Nothing Then
            cookie.Disconnect()
            cookie = Nothing

        End If

        MyBase.DetachSink()
    End Sub

    ''' <summary>
    ''' 引发NavigateError事件.
    ''' </summary>
    Protected Overridable Sub OnNavigateError(ByVal e As WebBrowserNavigateErrorEventArgs)
        RaiseEvent NavigateError(Me, e)
    End Sub

End Class

