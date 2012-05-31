'****************************** 模块头 ***********************************\
' 模块名:  MainForm.vb
' 项目名:  VBWebBrowserLoadComplete
' 版权 (c) Microsoft Corporation.
' 
' 这是该应用程序的主要窗体,用于初始化用户界面和处理事件.
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
Partial Public Class MainForm
    Inherits Form

    ' DocumentCompleted 事件被引发次数的计数.
    Private documentCompletedCount As Integer = 0

    ' DocumentCompleted 事件被引发次数的计数.
    Private loadCompletedCount As Integer = 0

    Public Sub New()
        InitializeComponent()

        ' 登记 System.Windows.Forms.WebBrowser 的事件.
        AddHandler webEx.DocumentCompleted, AddressOf webEx_DocumentCompleted
        AddHandler webEx.Navigating, AddressOf webEx_Navigating
        AddHandler webEx.Navigated, AddressOf webEx_Navigated

        ' 登记 WebBrowserEx 的事件.
        AddHandler webEx.LoadCompleted, AddressOf webEx_LoadCompleted
        AddHandler webEx.StartNavigating, AddressOf webEx_StartNavigating

        Me.tbURL.Text = String.Format("{0}\Resource\FramesPage.htm",
               Environment.CurrentDirectory)
    End Sub

    ''' <summary>
    ''' 导航到一个 URL.
    ''' </summary>
    Private Sub btnGo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGo.Click
        Try
            Dim url As New Uri(tbURL.Text)
            webEx.Navigate(url)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


    Private Sub webEx_DocumentCompleted(ByVal sender As Object,
        ByVal e As WebBrowserDocumentCompletedEventArgs)
        documentCompletedCount += 1
        DisplayStatus("已完成文档: " & e.Url.ToString)
    End Sub

    Private Sub webEx_Navigating(ByVal sender As Object,
                                 ByVal e As WebBrowserNavigatingEventArgs)
        DisplayStatus("正在导航 : " & e.Url.ToString)
    End Sub

    Private Sub webEx_Navigated(ByVal sender As Object,
                                ByVal e As WebBrowserNavigatedEventArgs)
        DisplayStatus("完成导航: " & e.Url.ToString)
    End Sub

    Private Sub webEx_LoadCompleted(ByVal sender As Object,
                                    ByVal e As WebBrowserDocumentCompletedEventArgs)
        loadCompletedCount += 1
        DisplayStatus("加载完成: " & e.Url.ToString)
    End Sub

    Private Sub webEx_StartNavigating(ByVal sender As Object,
                                      ByVal e As WebBrowserNavigatingEventArgs)
        documentCompletedCount = 0
        loadCompletedCount = 0
        DisplayStatus("开始导航 : " & e.Url.ToString)
    End Sub

    ''' <summary>
    ''' 显示消息.
    ''' </summary>
    Private Sub DisplayStatus(ByVal msg As String)
        Dim now As Date = Date.Now

        lstActivities.Items.Insert(0, String.Format("{0:HH:mm:ss}:{1:000} {2}",
                                                    now, now.Millisecond, msg))

        lstActivities.SelectedIndex = 0

        Me.lbStatus.Text = String.Format(
            "已完成文档:{0} 加载完成:{1}",
            documentCompletedCount, loadCompletedCount)

    End Sub

End Class
