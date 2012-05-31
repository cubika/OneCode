'*************************** Module Header ******************************'
' Module Name:  KeyBoardForm.vb
' Project:	    VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' 这是展示软键盘的主要窗体。它继承自 GlassForm 类，以便他能拥有 Vista 和 Windows7 
' 的玻璃风格。当窗体被加载的时候，它将会加载 KeysMapping.xml 来初始化键盘按钮。
' 
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


Partial Public Class KeyBoardForm
    Inherits NoActivate.NoActivateWindow

    Private _keyButtonList As IEnumerable(Of KeyButton) = Nothing
    Private ReadOnly Property KeyButtonList() As IEnumerable(Of KeyButton)
        Get
            If _keyButtonList Is Nothing Then
                _keyButtonList = Me.Controls.OfType(Of KeyButton)()
            End If
            Return _keyButtonList
        End Get
    End Property



    Private _pressedModifierKeyCodes As List(Of Integer) = Nothing

    ''' <summary>
    ''' 被按下的修饰键。
    ''' </summary>
    Private ReadOnly Property PressedModifierKeyCodes() As List(Of Integer)
        Get
            If _pressedModifierKeyCodes Is Nothing Then
                _pressedModifierKeyCodes = New List(Of Integer)()
            End If
            Return _pressedModifierKeyCodes
        End Get
    End Property

    ''' <summary>
    ''' 把窗体风格设置为 WS_EX_NOACTIVATE，以便它将得不到焦点。
    ''' </summary>
    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or CInt(&H8000000L)
            Return cp
        End Get
    End Property

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "处理键盘事件"

    Private Sub KeyBoardForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        Try
            InitializeKeyButtons()
        Catch _ex As Exception
            MessageBox.Show(_ex.Message)
        End Try


        ' 注册按键的单击事件。
        For Each btn As KeyButton In Me.KeyButtonList
            AddHandler btn.Click, AddressOf KeyButton_Click
        Next btn
    End Sub

    ''' <summary>
    ''' 处理按键的单击事件。
    ''' </summary>
    Private Sub KeyButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As KeyButton = TryCast(sender, KeyButton)
        If btn Is Nothing Then
            Return
        End If

        ' 同步按键对，像 LShiftKey（左Shift键） 和 RShiftKey(右Shift键）。
        SyncKeyPairs(btn)

        ' 处理特殊的按键，比如 AppsKey。
        If ProcessSpecialKey(btn) Then
            Return
        End If

        ' 如果数码锁定键、Shift 键或者大小写锁定键被按下，更新按键的文本。
        If btn.Key = Keys.NumLock OrElse btn.Key = Keys.ShiftKey _
            OrElse btn.Key = Keys.CapsLock Then

            UpdateKeyButtonsText(keyButtonLShift.IsPressed,
                                 keyButtonNumLock.IsPressed,
                                 keyButtonCapsLock.IsPressed)
        End If

        ' 大小写锁定键、数码锁定键、或者滚动锁定键被按下。
        If btn.IsLockKey Then
            UserInteraction.KeyboardInput.SendToggledKey(btn.KeyCode)

            ' 修饰键被按下。
        ElseIf btn.IsModifierKey Then
            ' 修饰键被按下两次。
            If PressedModifierKeyCodes.Contains(btn.KeyCode) Then
                UserInteraction.KeyboardInput.SendToggledKey(btn.KeyCode)

                ' 清空修饰键按钮的 pressed 状态。
                ResetModifierKeyButtons()
            Else
                PressedModifierKeyCodes.Add(btn.KeyCode)
            End If

            ' 正常的按键被按下。
        Else
            Dim btnKeyCode As Integer = btn.KeyCode

            ' 如果数字键盘上的按键，并且数码锁定键没有被按下，那么使用 UnNumLockKeyCode.
            If btn.IsNumberPadKey AndAlso (Not keyButtonNumLock.IsPressed) _
                AndAlso btn.UnNumLockKeyCode > 0 Then

                btnKeyCode = btn.UnNumLockKeyCode
            End If

            UserInteraction.KeyboardInput.SendKey(PressedModifierKeyCodes, btnKeyCode)

            ' 清空所有修饰键按钮的 pressed 状态。
            ResetModifierKeyButtons()
        End If
    End Sub

    ''' <summary>
    ''' 同步按键对，像 LShiftKey（左Shift键） 和 RShiftKey（右Shift键）。
    ''' </summary>
    Private Sub SyncKeyPairs(ByVal btn As KeyButton)
        If btn Is keyButtonLShift Then
            keyButtonRShift.IsPressed = keyButtonLShift.IsPressed
        End If
        If btn Is keyButtonRShift Then
            keyButtonLShift.IsPressed = keyButtonRShift.IsPressed
        End If

        If btn Is keyButtonLAlt Then
            keyButtonRAlt.IsPressed = keyButtonLAlt.IsPressed
        End If
        If btn Is keyButtonRAlt Then
            keyButtonLAlt.IsPressed = keyButtonRAlt.IsPressed
        End If

        If btn Is keyButtonLControl Then
            keyButtonRControl.IsPressed = keyButtonLControl.IsPressed
        End If
        If btn Is keyButtonRControl Then
            keyButtonLControl.IsPressed = keyButtonRControl.IsPressed
        End If
    End Sub

    ''' <summary>
    ''' 处理特殊的按键，例如 AppsKey.
    ''' </summary>
    Private Function ProcessSpecialKey(ByVal btn As KeyButton) As Boolean
        Dim handled As Boolean = True
        Select Case btn.Key

            ' 使用 Shift+F10 模拟 Apps 键. 
            Case Keys.Apps
                UserInteraction.KeyboardInput.SendKey(New Integer() {CInt(Keys.ShiftKey)},
                                                      CInt(Keys.F10))
            Case Else
                handled = False
        End Select
        Return handled
    End Function

    ''' <summary>
    ''' 初始化键钮。
    ''' </summary>
    Private Sub InitializeKeyButtons()
        Dim capsLockState As Short = UserInteraction.UnsafeNativeMethods.GetKeyState(CInt(Keys.CapsLock))
        keyButtonCapsLock.IsPressed = (capsLockState And &H1) <> 0

        Dim numLockState As Short = UserInteraction.UnsafeNativeMethods.GetKeyState(CInt(Keys.NumLock))
        keyButtonNumLock.IsPressed = (numLockState And &H1) <> 0


        Dim scrLockState As Short = UserInteraction.UnsafeNativeMethods.GetKeyState(CInt(Keys.Scroll))
        keyButtonScrollLock.IsPressed = (scrLockState And &H1) <> 0

        Dim keysMappingDoc = XDocument.Load("Resources\KeysMapping.xml")
        For Each key In keysMappingDoc.Root.Elements()

            Dim keyCode As Integer = Integer.Parse(key.Element("KeyCode").Value)

            Dim btns As IEnumerable(Of KeyButton) =
                KeyButtonList.Where(Function(btn) btn.KeyCode = keyCode)

            For Each btn As KeyButton In btns
                btn.NormalText = key.Element("NormalText").Value

                If key.Elements("ShiftText").Count() > 0 Then
                    btn.ShiftText = key.Element("ShiftText").Value
                End If

                If key.Elements("UnNumLockText").Count() > 0 Then
                    btn.UnNumLockText = key.Element("UnNumLockText").Value
                End If

                If key.Elements("UnNumLockKeyCode").Count() > 0 Then
                    Integer.Parse(key.Element("UnNumLockKeyCode").Value)
                End If

                btn.UpdateDisplayText(False, keyButtonNumLock.IsPressed,
                                      keyButtonCapsLock.IsPressed)
            Next btn


        Next key
    End Sub

    ''' <summary>
    ''' 更新按键的文本。
    ''' </summary>
    Private Sub UpdateKeyButtonsText(ByVal isShiftKeyPressed As Boolean,
                                     ByVal isNumLockPressed As Boolean,
                                     ByVal isCapsLockPressed As Boolean)
        For Each btn In Me.KeyButtonList
            btn.UpdateDisplayText(isShiftKeyPressed, isNumLockPressed, isCapsLockPressed)
        Next btn
    End Sub

    ''' <summary>
    ''' 清空所有修饰符按键的 pressed 状态。
    ''' </summary>
    Private Sub ResetModifierKeyButtons()
        PressedModifierKeyCodes.Clear()

        keyButtonLShift.IsPressed = False
        keyButtonRShift.IsPressed = False
        keyButtonRControl.IsPressed = False
        keyButtonLControl.IsPressed = False
        keyButtonRAlt.IsPressed = False
        keyButtonLAlt.IsPressed = False
        keyButtonWin.IsPressed = False

        For Each keybtn In KeyButtonList
            keybtn.UpdateDisplayText(False,
                                     keyButtonNumLock.IsPressed,
                                     keyButtonCapsLock.IsPressed)
        Next keybtn
    End Sub

#End Region

End Class

