'*************************** 模块头 ******************************'
' 模块名:    WebBrowserEx.vb
' 项目名:	    VBWebBrowserAutomation
' 版权(c) Microsoft Corporation.
' 
' 这个WebBrowserEx类继承WebBrowser类.它支持如下功能:
' 1. 阻止指定的网址.
' 2.完成自动化输入html元素.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Linq
Imports System.Security.Permissions
Imports System.ComponentModel

Partial Public Class WebBrowserEx
    Inherits WebBrowser

    Private _privateCanAutoComplete As Boolean

    ''' <summary>
    ''' 指定当前页是否可以完成自动化加载。
    ''' </summary>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(False)>
    Public Property CanAutoComplete() As Boolean
        Get
            Return _privateCanAutoComplete
        End Get
        Private Set(ByVal value As Boolean)
            _privateCanAutoComplete = value
        End Set
    End Property

    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 当文档完成加载后，检查这个页是否加载完。
    ''' automatically.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub OnDocumentCompleted(ByVal e As WebBrowserDocumentCompletedEventArgs)

        ' 检查当前页是否被保存。
        Dim form As StoredSite = StoredSite.GetStoredSite(Me.Url.Host)
        CanAutoComplete = form IsNot Nothing _
            AndAlso form.Urls.Contains(Me.Url.AbsolutePath.ToLower())

        MyBase.OnDocumentCompleted(e)
    End Sub

    ''' <summary>
    ''' 完成页的自动化。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub AutoComplete()
        If CanAutoComplete Then
            Dim form As StoredSite = StoredSite.GetStoredSite(Me.Url.Host)
            form.FillWebPage(Me.Document, True)
        End If
    End Sub

    ''' <summary>
    ''' 检查URL是否包含在禁用列表中.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub OnNavigating(ByVal e As WebBrowserNavigatingEventArgs)
        If BlockSites.Instance.Hosts.Contains(e.Url.Host.ToLower()) Then
            e.Cancel = True
            Me.Navigate(String.Format("{0}\Resources\Block.htm",
                                      Environment.CurrentDirectory))
        Else
            MyBase.OnNavigating(e)
        End If
    End Sub

End Class
