'******************************* 模块头 ************************************'
' 模块名:  MainForm.vb
' 项目名:  VBRegisterHotkey
' 版权(c)  Microsoft Corporation.
' 
' 这是这个应用程序的主窗体.它是用来初始化界面并处理事件的.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'


Partial Public Class MainForm
    Inherits Form

    Private _hotKeyToRegister As HotKeyRegister = Nothing

    Private _registerKey As Keys = Keys.None

    Private _registerModifiers As KeyModifiers = KeyModifiers.None

    Public Sub New()
        InitializeComponent()
    End Sub


    ''' <summary>
    ''' 处理tbHotKey的KeyDown. 在事件处理中，检查按过的按键,这些按键中必须包括Ctrl，
    ''' Shift或者Alt按键，比如Ctrl+Alt+T。HotKeyRegister.GetModifiers方法能检查是否“T”
    ''' 被按。
    ''' </summary>
    Private Sub tbHotKey_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) _
        Handles tbHotKey.KeyDown
        '  关键事件不能被发送到底层控制。
        e.SuppressKeyPress = True

        ' 检查是否修饰键被按下。
        If e.Modifiers <> Keys.None Then
            Dim key As Keys = Keys.None
            Dim modifiers As KeyModifiers = HotKeyRegister.GetModifiers(e.KeyData, key)

            ' 被按下的键是有效的。
            If key <> Keys.None Then

                Me._registerKey = key
                Me._registerModifiers = modifiers

                ' 在文本框中显示被按下的键。
                tbHotKey.Text = String.Format("{0}+{1}",
                    Me._registerModifiers, Me._registerKey)

                ' 使按钮可利用。
                btnRegister.Enabled = True
            End If
        End If
    End Sub


    ''' <summary>
    ''' 处理btnRegister的Click事件。
    ''' </summary>
    Private Sub btnRegister_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnRegister.Click

        Try
            ' 注册热键。
            _hotKeyToRegister = New HotKeyRegister(Me.Handle, 100,
                Me._registerModifiers, Me._registerKey)

            ' 注册HotKeyPressed事件。
            AddHandler _hotKeyToRegister.HotKeyPressed, AddressOf HotKeyPressed

            ' 更新界面。
            btnRegister.Enabled = False
            tbHotKey.Enabled = False
            btnUnregister.Enabled = True

        Catch _argumentException As ArgumentException
            MessageBox.Show(_argumentException.Message)
        Catch _applicationException As ApplicationException
            MessageBox.Show(_applicationException.Message)
        End Try
    End Sub


    ''' <summary>
    ''' 如果HotKeyPressed事件被触发显示一个文本框。
    ''' </summary>
    Private Sub HotKeyPressed(ByVal sender As Object, ByVal e As EventArgs)

        If Me.WindowState = FormWindowState.Minimized Then
            Me.WindowState = FormWindowState.Normal
        End If
        Me.Activate()

    End Sub


    ''' <summary>
    ''' 处理btnUnregister的Click事件。
    ''' </summary>
    Private Sub btnUnregister_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnUnregister.Click

        ' 处理hotKeyToRegister。
        If _hotKeyToRegister IsNot Nothing Then
            _hotKeyToRegister.Dispose()
            _hotKeyToRegister = Nothing
        End If

        ' 更新界面。
        tbHotKey.Enabled = True
        btnRegister.Enabled = True
        btnUnregister.Enabled = False
    End Sub


    ''' <summary>
    ''' 当窗体关闭时，处理hotKeyToRegister。
    ''' </summary>
    Protected Overrides Sub OnClosed(ByVal e As EventArgs)
        If _hotKeyToRegister IsNot Nothing Then
            _hotKeyToRegister.Dispose()
            _hotKeyToRegister = Nothing
        End If

        MyBase.OnClosed(e)
    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class
