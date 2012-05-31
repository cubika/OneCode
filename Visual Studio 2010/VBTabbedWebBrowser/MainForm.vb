'*************************** 模块头 ******************************\
' 模块名:  MainForm.vb
' 项目名:	    VBTabbedWebBrowser
' 版权 (c) Microsoft Corporation.
' 
' 是应用程序的主形式. 它是用来初始化用户界面和处理时间的
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************


Partial Public Class MainForm
    Inherits Form

    Public Sub New()
        InitializeComponent()

        ' 初始化复选框.
        chkEnableTab.Checked = TabbedWebBrowserContainer.IsTabEnabled

        AddHandler chkEnableTab.CheckedChanged, AddressOf chkEnableTab_CheckedChanged

    End Sub

    ''' <summary>
    ''' 处理tbUrl的KeyDown事件。当键值输入后，导航在给tbUrl网址
    ''' </summary>
    Private Sub tbUrl_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) _
        Handles tbUrl.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            webBrowserContainer.Navigate(tbUrl.Text)
        End If
    End Sub

    ''' <summary>
    ''' 当点击后“退按”钮时，处理事件。
    ''' </summary>
    Private Sub btnBack_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBack.Click
        webBrowserContainer.GoBack()
    End Sub

    ''' <summary>
    ''' 当点击“向前”按钮时，处理事件。
    ''' </summary>
    Private Sub btnForward_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnForward.Click
        webBrowserContainer.GoForward()
    End Sub

    ''' <summary>
    ''' 当点击后刷新钮时，处理事件。
    ''' </summary>
    Private Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnRefresh.Click
        webBrowserContainer.RefreshWebBrowser()
    End Sub

    ''' <summary>
    ''' 当点击“新建标签”按钮时，处理事件
    ''' </summary>
    Private Sub btnNewTab_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnNewTab.Click
        webBrowserContainer.NewTab("about:blank")
    End Sub

    ''' <summary>
    ''' 当点击“关闭标签”按钮时，处理事件。
    ''' </summary>
    Private Sub btnCloseTab_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnCloseTab.Click
        webBrowserContainer.CloseActiveTab()
    End Sub

    ''' <summary>
    ''' 处理chkEnableTab的 CheckedChanged 事件。
    ''' </summary>
    Private Sub chkEnableTab_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        TabbedWebBrowserContainer.IsTabEnabled = chkEnableTab.Checked
        MessageBox.Show("这个上下文菜单""启动选项卡""在应用程序重新启动之后才会生效。")
        Application.Restart()
    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class
