'*************************** 模块头 ******************************'
' 模块名:  WebBrowserEx.vb
' 项目名:	    VBTabbedWebBrowser
' 版权 (c) Microsoft Corporation.
' 
' WebBrowserEx类 继承 WebBrowser类 并且 可以处理NewWindow3事件。 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Security.Permissions
Imports Microsoft.Win32


Partial Public Class WebBrowserEx
    Inherits WebBrowser

    Private _cookie As AxHost.ConnectionPointCookie

    Private _helper As DWebBrowserEvent2Helper

    Public Event NewWindow3 As EventHandler(Of WebBrowserNewWindowEventArgs)

    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 用一个客户端与ActiveX 控件联系起来，进行处理事件控件，包含 NewWindow3事件。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub CreateSink()
        MyBase.CreateSink()

        _helper = New DWebBrowserEvent2Helper(Me)
        _cookie = New AxHost.ConnectionPointCookie(
            Me.ActiveXInstance, _helper, GetType(DWebBrowserEvents2))
    End Sub


    ''' <summary>
    ''' 从底层ActiveX 控件 释放附属在CreateSink方法中的事件-处理客户端
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub DetachSink()
        If _cookie IsNot Nothing Then
            _cookie.Disconnect()
            _cookie = Nothing
        End If
        MyBase.DetachSink()
    End Sub


    ''' <summary>
    '''  创建 NewWindow3 事件.
    ''' </summary>
    Protected Overridable Sub OnNewWindow3(ByVal e As WebBrowserNewWindowEventArgs)
        RaiseEvent NewWindow3(Me, e)
    End Sub
End Class
