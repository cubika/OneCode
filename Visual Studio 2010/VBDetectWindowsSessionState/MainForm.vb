'*************************** 模块头 ******************************'
' 模块名:  MainForm.vb
' 项目名:	  VBDetectWindowsSessionState
' 版权 (c) Microsoft Corporation.
' 
' 这是应用程序的主窗口, 用来注册和处理用户界面的事件.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports Microsoft.Win32

Partial Public Class MainForm
    Inherits Form
    Private _session As WindowsSession
    Private _timer As System.Threading.Timer

    Public Sub New()
        InitializeComponent()

        ' 初始化WindowsSession对象.
        _session = New WindowsSession()

        ' 初始化定时器, 但不启动它.
        _timer = New System.Threading.Timer(
            New System.Threading.TimerCallback(AddressOf DetectSessionState),
            Nothing, System.Threading.Timeout.Infinite, 5000)
    End Sub

    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MyBase.Load

        ' 注册StateChanged事件.
        AddHandler _session.StateChanged, AddressOf session_StateChanged

    End Sub

    ''' <summary>
    ''' 处理StateChanged事件.
    ''' </summary>
    Private Sub session_StateChanged(ByVal sender As Object,
                                     ByVal e As SessionSwitchEventArgs)
        ' 显示当前状态.
        lbState.Text = "当前状态: " & e.Reason.ToString()

        ' 记录StateChanged事件.
        lstRecord.Items.Add(String.Format("{0}   {1} " & vbTab & "发生了",
                                          Date.Now, e.Reason.ToString()))

        lstRecord.SelectedIndex = lstRecord.Items.Count - 1

    End Sub

    Private Sub chkEnableTimer_CheckedChanged(ByVal sender As System.Object,
                                              ByVal e As System.EventArgs) _
                                          Handles chkEnableTimer.CheckedChanged
        If chkEnableTimer.Checked Then
            _timer.Change(0, 5000)
        Else
            _timer.Change(0, System.Threading.Timeout.Infinite)
        End If

    End Sub

    Sub DetectSessionState(ByVal obj As Object)

        ' 检查当前会话是否被锁定.
        Dim isCurrentLocked As Boolean = _session.IsLocked()

        Dim state = If(isCurrentLocked, SessionSwitchReason.SessionLock,
                       SessionSwitchReason.SessionUnlock)

        ' 显示当前状态.
        lbState.Text = String.Format("当前状态: {0}    时间: {1} ",
                                     state, Date.Now)

        ' 记录StateChanged事件.
        lstRecord.Items.Add(String.Format("{0}   {1} ", Date.Now, state))

        lstRecord.SelectedIndex = lstRecord.Items.Count - 1
    End Sub
End Class
