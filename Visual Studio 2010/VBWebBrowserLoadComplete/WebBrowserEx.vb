'****************************** 模块头 ***********************************\
' 模块名  WebBrowserEx.vb
' 项目名:  VBWebBrowserLoadComplete
' 版权 (c) Microsoft Corporation.
' 
' WebBrowserEx 类继承了 WebBrowser 类，并提供了 LoadCompleted 事件.
' 
' 在页面没有嵌套框架的情况下,DocumentComplete 事件在所有事情完成后，会被引发
' 一次. 在有多个嵌套框架时, DocumentComplete 事件会被多次引发. 因此如果事件
' DocumentCompleted 被引发, 它并不意味着该页面被加载完全.
' 
' 因此, 要检查一个页面是否已经加载完全,你需要检查是否事件的发送次数与 WebBrowser
' 控件的相同.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************/

Imports System.Security.Permissions


<PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust"),
PermissionSetAttribute(SecurityAction.InheritanceDemand, Name:="FullTrust")>
Partial Public Class WebBrowserEx
    Inherits WebBrowser
    Private cookie As AxHost.ConnectionPointCookie

    Private helper As DWebBrowserEvents2Helper

    Public Event LoadCompleted As EventHandler(Of WebBrowserDocumentCompletedEventArgs)

    Public Event StartNavigating As EventHandler(Of WebBrowserNavigatingEventArgs)

    ''' <summary>
    ''' 将底层的 ActiveX 控件与可以处理控件事件包括 NavigateError 事件的客户端 
    ''' 关联起来.
    ''' </summary>
    Protected Overrides Sub CreateSink()
        MyBase.CreateSink()

        helper = New DWebBrowserEvents2Helper(Me)
        cookie = New AxHost.ConnectionPointCookie(
            Me.ActiveXInstance, helper, GetType(DWebBrowserEvents2))
    End Sub

    ''' <summary>
    ''' 从底层的ActiveX控件中,释放附加在 CreateSink 方法中处理事件的客户端. 
    ''' </summary>
    Protected Overrides Sub DetachSink()
        If cookie IsNot Nothing Then
            cookie.Disconnect()
            cookie = Nothing
        End If
        MyBase.DetachSink()
    End Sub

    ''' <summary>
    ''' 激发 LoadCompleted 事件.
    ''' </summary>
    Protected Overridable Sub OnLoadCompleted(ByVal e As WebBrowserDocumentCompletedEventArgs)

        RaiseEvent LoadCompleted(Me, e)
    End Sub

    ''' <summary>
    ''' 激发 StartNavigating 事件.
    ''' </summary>
    Protected Overridable Sub OnStartNavigating(ByVal e As WebBrowserNavigatingEventArgs)
        RaiseEvent StartNavigating(Me, e)
    End Sub
End Class

