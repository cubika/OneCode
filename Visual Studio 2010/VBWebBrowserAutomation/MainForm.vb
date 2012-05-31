'****************************** 模块头 ******************************\
' 模块名:    MainForm.vb
' 项目名:	    VBWebBrowserAutomation
' 版权 (c) Microsoft Corporation.
' 
' 这是这个应用程序的主要部分.它是用来初始化UI和处理事件的。
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

Partial Public Class MainForm
    Inherits Form

    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New()
        InitializeComponent()

        ' 注册事件.
        AddHandler webBrowser.Navigating, AddressOf webBrowser_Navigating
        AddHandler webBrowser.DocumentCompleted, AddressOf webBrowser_DocumentCompleted
    End Sub

    ''' <summary>
    ''' 当webBrowser 在导航中的时候禁用btnAutoComplete。
    ''' </summary>
    Private Sub webBrowser_Navigating(ByVal sender As Object,
                                      ByVal e As WebBrowserNavigatingEventArgs)
        btnAutoComplete.Enabled = False
    End Sub

    ''' <summary>
    ''' 当网页被加载后刷新UI。
    ''' </summary>
    Private Sub webBrowser_DocumentCompleted(ByVal sender As Object,
                                             ByVal e As WebBrowserDocumentCompletedEventArgs)
        btnAutoComplete.Enabled = webBrowser.CanAutoComplete
        tbUrl.Text = e.Url.ToString()
    End Sub

    ''' <summary>
    ''' 处理btnAutoComplete的Click事件。
    ''' </summary>
    Private Sub btnAutoComplete_Click(ByVal sender As Object,
                                      ByVal e As EventArgs) Handles btnAutoComplete.Click
        Try
            webBrowser.AutoComplete()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 处理btnGo的Click事件。
    ''' </summary>
    Private Sub btnGo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGo.Click
        Try
            webBrowser.Navigate(tbUrl.Text)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Class
