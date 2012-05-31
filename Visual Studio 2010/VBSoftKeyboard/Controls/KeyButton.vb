'*************************** Module Header ******************************'
' Module Name:  KeyButton.vb
' Project:      VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
'  这个展示一个键盘按钮。
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


Partial Public Class KeyButton
    Inherits Button

    ' 默认的风格。
    Private Shared _normalBackColor As Color = Color.Black
    Private Shared _mouseOverBackColor As Color = Color.DimGray
    Private Shared _pressedBackColor As Color = Color.White

    Private Shared _normalLabelForeColor As Color = Color.White
    Private Shared _pressedlLabelForeColor As Color = Color.Black

    ''' <summary>
    ''' 按键的键值。
    ''' </summary>
    Public Property KeyCode() As Integer

    Private _key As Keys
    Public ReadOnly Property Key() As Keys
        Get
            If _key = Keys.None Then
                _key = CType(KeyCode, Keys)
            End If

            Return _key
        End Get
    End Property

    ''' <summary>
    ''' 如果NumLock键被按下了，数字键盘上按键的键值。
    ''' </summary>
    Public Property UnNumLockKeyCode() As Integer

    ''' <summary>
    ''' 如果NumLock键没有按下，数字键盘上按键的文本。
    ''' </summary>
    Public Property UnNumLockText() As String

    ''' <summary>
    ''' 按键的正常文本。
    ''' </summary>
    Public Property NormalText() As String

    ''' <summary>
    ''' Shift 键被按下后按键的文本。
    ''' </summary>
    Public Property ShiftText() As String

    ''' <summary>
    ''' 指定是不是修饰键。
    ''' </summary>
    Public ReadOnly Property IsModifierKey() As Boolean
        Get
            Return Key = Keys.ControlKey _
                OrElse Key = Keys.ShiftKey _
                OrElse Key = Keys.Menu _
                OrElse Key = Keys.LWin _
                OrElse Key = Keys.RWin
        End Get
    End Property

    ''' <summary>
    ''' 指定是不是锁定键。
    ''' </summary>
    Public ReadOnly Property IsLockKey() As Boolean
        Get
            Return Key = Keys.Capital OrElse Key = Keys.Scroll OrElse Key = Keys.NumLock
        End Get
    End Property

    ''' <summary>
    ''' 指定是不是一个字母键。
    ''' </summary>
    Public ReadOnly Property IsLetterKey() As Boolean
        Get
            Return Key >= Keys.A AndAlso Key <= Keys.Z
        End Get
    End Property

    ''' <summary>
    ''' 指定是不是数字键盘上的键。
    ''' </summary>
    Public ReadOnly Property IsNumberPadKey() As Boolean
        Get
            Return Key >= Keys.NumPad0 AndAlso Key <= Keys.NumPad9
        End Get
    End Property

    Private _isPressed As Boolean

    ''' <summary>
    ''' 指定按键是否被按下。
    ''' </summary>
    Public Property IsPressed() As Boolean
        Get
            Return _isPressed
        End Get
        Set(ByVal value As Boolean)
            If _isPressed <> value Then
                _isPressed = value

                Me.OnIsPressedChange(EventArgs.Empty)
            End If
        End Set

    End Property

    Private _isMouseOver As Boolean

    ''' <summary>
    ''' 指定鼠标是否在这个按键上。
    ''' </summary>
    Private Property IsMouseOver() As Boolean
        Get
            Return _isMouseOver
        End Get
        Set(ByVal value As Boolean)
            If _isMouseOver <> value Then
                _isMouseOver = value

                Me.OnIsMouseOverChange(EventArgs.Empty)
            End If
        End Set
    End Property

    Public Sub New()
        Me.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BackColor = System.Drawing.Color.Black
        Me.ForeColor = System.Drawing.Color.White
    End Sub

    ''' <summary>
    ''' 更新按键的文本。
    ''' </summary>
    Public Sub UpdateDisplayText(ByVal isShiftKeyPressed As Boolean,
                                 ByVal isNumLockPressed As Boolean,
                                 ByVal isCapsLockPressed As Boolean)
        If Me.IsLetterKey Then
            Me.Text = If(isShiftKeyPressed Xor isCapsLockPressed, ShiftText, NormalText)
        ElseIf Not String.IsNullOrEmpty(Me.ShiftText) Then
            Me.Text = If(isShiftKeyPressed, ShiftText, NormalText)
        ElseIf Me.IsNumberPadKey Then
            Me.Text = If(isNumLockPressed, NormalText, UnNumLockText)
        End If

    End Sub

#Region "更新键盘按键的风格"

    ''' <summary>
    ''' MouseDown 事件的句柄。
    ''' 改变 IsPressed 属性的值，将会引起按钮的更新。
    ''' </summary>
    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)

        IsPressed = Not IsPressed
    End Sub

    ''' <summary>
    ''' MouseUp 事件的句柄。
    ''' 如果按键不是修饰键也不是锁定键，把它的IsPressed属性值设为false，这会使得按钮更新。
    ''' </summary>
    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)

        If (Not IsModifierKey) AndAlso (Not IsLockKey) Then
            IsPressed = False
        End If
    End Sub

    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        MyBase.OnMouseEnter(e)
        IsMouseOver = True
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        MyBase.OnMouseLeave(e)
        IsMouseOver = False
    End Sub

    Protected Overridable Sub OnIsMouseOverChange(ByVal e As EventArgs)
        ReDrawKeyButton()
    End Sub

    Protected Overridable Sub OnIsPressedChange(ByVal e As EventArgs)
        ReDrawKeyButton()
    End Sub

    Protected Overridable Sub ReDrawKeyButton()
        If IsPressed Then
            Me.BackColor = KeyButton._pressedBackColor
            Me.ForeColor = KeyButton._pressedlLabelForeColor
        ElseIf IsMouseOver Then
            Me.BackColor = KeyButton._mouseOverBackColor
            Me.ForeColor = KeyButton._normalLabelForeColor
        Else
            Me.BackColor = KeyButton._normalBackColor
            Me.ForeColor = KeyButton._normalLabelForeColor
        End If
    End Sub

#End Region
End Class
