'*************************** Module Header ******************************'
' Module Name:  KeyboardInput.vb
' Project:	    VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' 这个类不再使用原来的UnsafeNativeMethods.SendInput 方法来合成按键。
'  
' 有三种情形：
' 1.单个键被按下，例如“A”。
' 2.带有修饰键的键被按下，例如“Ctrl+A”。
' 3.可以被锁定的键被按下，例如大小写锁定键、数码锁定键、滚动锁定键
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

Imports System.Runtime.InteropServices

Namespace UserInteraction

    Public NotInheritable Class KeyboardInput

        ''' <summary>
        ''' 单个按键被按下。
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Sub SendKey(ByVal key As Integer)
            SendKey(Nothing, key)
        End Sub

        ''' <summary>
        ''' 带有修饰键的按键被按下。
        ''' </summary>
        Public Shared Sub SendKey(ByVal modifierKeys As IEnumerable(Of Integer), ByVal key As Integer)
            If key <= 0 Then
                Return
            End If

            ' 只有单个按键被按下。
            If modifierKeys Is Nothing OrElse modifierKeys.Count() = 0 Then
                Dim inputs = New NativeMethods.INPUT(0) {}
                inputs(0).type = NativeMethods.INPUT_KEYBOARD
                inputs(0).inputUnion.ki.wVk = CShort(Fix(key))
                UnsafeNativeMethods.SendInput(1, inputs, Marshal.SizeOf(inputs(0)))

                ' 带有修饰符的按键被按下。 
            Else

                ' 为了模拟这种情形，输入的信息包括被锁定修饰键、按键事件、释放修饰键事件。
                ' 例如，为了模拟Ctrl+C，我们必须发送三次输入信息。
                ' 1、Ctrl被按下。
                ' 2、C被按下。
                ' 3、Ctrl被释放。
                Dim inputs = New NativeMethods.INPUT(modifierKeys.Count() * 2) {}

                Dim i As Integer = 0

                ' 模拟锁定修饰键。
                For Each modifierKey In modifierKeys
                    inputs(i).type = NativeMethods.INPUT_KEYBOARD
                    inputs(i).inputUnion.ki.wVk = CShort(Fix(modifierKey))
                    i += 1
                Next modifierKey

                ' 模拟按键。
                inputs(i).type = NativeMethods.INPUT_KEYBOARD
                inputs(i).inputUnion.ki.wVk = CShort(Fix(key))
                i += 1

                ' 模拟释放修饰键。
                For Each modifierKey In modifierKeys
                    inputs(i).type = NativeMethods.INPUT_KEYBOARD
                    inputs(i).inputUnion.ki.wVk = CShort(Fix(modifierKey))

                    ' 0x0002 意味着 key-up 事件. 
                    inputs(i).inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP
                    i += 1
                Next modifierKey

                UnsafeNativeMethods.SendInput(CUInt(inputs.Length), inputs, Marshal.SizeOf(inputs(0)))
            End If
        End Sub

        ''' <summary>
        ''' 表示一个可以被锁定的键被按下。这个键应该可以释放。
        ''' </summary>
        Public Shared Sub SendToggledKey(ByVal key As Integer)
            Dim inputs = New NativeMethods.INPUT(1) {}

            ' 按下按键。
            inputs(0).type = NativeMethods.INPUT_KEYBOARD
            inputs(0).inputUnion.ki.wVk = CShort(Fix(key))

            ' 释放按键。
            inputs(1).type = NativeMethods.INPUT_KEYBOARD
            inputs(1).inputUnion.ki.wVk = CShort(Fix(key))
            inputs(1).inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP

            UnsafeNativeMethods.SendInput(2, inputs, Marshal.SizeOf(inputs(0)))
        End Sub
    End Class
End Namespace
