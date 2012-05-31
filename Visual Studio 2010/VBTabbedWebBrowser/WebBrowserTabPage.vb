'*************************** 模块头 ******************************'
' 模块名:  WebBrowserTabPage.vb
' 项目名:	    VBTabbedWebBrowser
' 版权 (c) Microsoft Corporation.
' 
'这个类继承System.Windows.Forms.TabPage类，并且包含一个WebBrowserEx属性。
'WebBrowserTabPage类的一个实例可以直接添加标签控件。
' 
'它可以显示WebBrowserEx类中的NewWindow3事件，并且处理DocumentTitleChanged事件。
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

Public Class WebBrowserTabPage
    Inherits TabPage
    Private _webBrowser As WebBrowserEx
    Public Property WebBrowser() As WebBrowserEx
        Get
            Return _webBrowser
        End Get
        Private Set(ByVal value As WebBrowserEx)
            _webBrowser = value
        End Set
    End Property

    ' 显示WebBrowserEx类中的NewWindow3事件..
    Public Custom Event NewWindow As EventHandler(Of WebBrowserNewWindowEventArgs)
        AddHandler(ByVal value As EventHandler(Of WebBrowserNewWindowEventArgs))
            AddHandler WebBrowser.NewWindow3, value
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of WebBrowserNewWindowEventArgs))
            RemoveHandler WebBrowser.NewWindow3, value
        End RemoveHandler
        RaiseEvent()
        End RaiseEvent
    End Event

    ''' <summary>
    ''' 初始化WebBrowserEx的实例.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New()
        MyBase.New()
        WebBrowser = New WebBrowserEx()
        WebBrowser.Dock = DockStyle.Fill
        AddHandler WebBrowser.DocumentTitleChanged, AddressOf WebBrowser_DocumentTitleChanged

        Me.Controls.Add(WebBrowser)
    End Sub

    ''' <summary>
    ''' 改变标签的题目.
    ''' </summary>
    Private Sub WebBrowser_DocumentTitleChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.Text = WebBrowser.DocumentTitle
    End Sub

End Class
