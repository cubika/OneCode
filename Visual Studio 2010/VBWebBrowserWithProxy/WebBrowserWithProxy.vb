'*************************** Module Header ******************************'
' 模块名称:  WebBrowserControl.vb
' 项目名称:	    VBWebBrowserWithProxy
' Copyright (c) Microsoft Corporation.
' 
' WebBrowserControl继承了WebBrowser类并且具有设置代理的功能。 
' 原始的internet 设置将被备份，被指定的代理将在浏览过程中被 
' 使用，并且，在浏览过程被重置时，原始的internet 设置将被恢复。


' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Text
Imports Microsoft.Win32
Imports System.Security.Permissions
Imports System.ComponentModel

Public Class WebBrowserWithProxy
    Inherits WebBrowser

    ' 代理服务器连接
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    Browsable(False)>
    Public Property Proxy() As InternetProxy

    ' 储存原始的internet连接选项以便在连接以后你能恢复它。
    Private currentInternetSettings As INTERNET_PER_CONN_OPTION_LIST

    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New()
    End Sub


    ''' <summary>
    ''' 处理Navigating事件。在Navigating事件中，连接还没有被打开并且你还能编辑它。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub OnNavigating(ByVal e As WebBrowserNavigatingEventArgs)
        MyBase.OnNavigating(e)

        ' 备份现有的连接选项。
        currentInternetSettings = WinINet.BackupConnectionProxy()

        ' 设置或注活代理
        If Proxy IsNot Nothing AndAlso (Not String.IsNullOrEmpty(Proxy.Address)) Then
            WinINet.SetConnectionProxy(Proxy.Address)
        Else
            WinINet.DisableConnectionProxy()
        End If
    End Sub

    ''' <summary>
    ''' 处理Navigated事件。在Navigated事件中，与internet的连接被完成。 
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub OnNavigated(ByVal e As WebBrowserNavigatedEventArgs)
        MyBase.OnNavigated(e)

        ' 恢复原始的连接选项
        WinINet.RestoreConnectionProxy(currentInternetSettings)
    End Sub

    ''' <summary>
    ''' 封装Navigate方法并且在需要的情况下设置代理授信头信息。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub [Goto](ByVal url As String)
        Dim uri As Uri = Nothing
        Dim result As Boolean = uri.TryCreate(url, UriKind.RelativeOrAbsolute, uri)
        If Not result Then
            Throw New ArgumentException("The url is not valid. ")
        End If

        ' 如果代理包含有用户名和密码，代理授信头信息将被要求设置
        If Proxy IsNot Nothing AndAlso (Not String.IsNullOrEmpty(Proxy.UserName)) _
            AndAlso (Not String.IsNullOrEmpty(Proxy.Password)) Then

            ' 这个头信息是由个64位字符串储存的证书。
            Dim credentialStringValue = String.Format("{0}:{1}",
                                                      Proxy.UserName, Proxy.Password)
            Dim credentialByteArray = ASCIIEncoding.ASCII.GetBytes(credentialStringValue)
            Dim credentialBase64String = Convert.ToBase64String(credentialByteArray)
            Dim authHeader As String = String.Format("Proxy-Authorization: Basic {0}",
                                                     credentialBase64String)

            Navigate(uri, String.Empty, Nothing, authHeader)
        Else
            Navigate(uri)
        End If
    End Sub
End Class
