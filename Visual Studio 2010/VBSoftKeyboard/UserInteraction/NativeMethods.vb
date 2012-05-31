'*************************** Module Header ******************************'
' Project:	    VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' 这个类包括了SendInput方法中使用到的结构体和常量
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
    Friend NotInheritable Class NativeMethods

        ' INPUT结构体中使用到的常量。
        Public Const INPUT_MOUSE As Integer = 0
        Public Const INPUT_KEYBOARD As Integer = 1
        Public Const INPUT_HARDWARE As Integer = 2

        ' KEYBDINPUT结构体中使用到的常量。
        Public Const KEYEVENTF_EXTENDEDKEY As Integer = &H1
        Public Const KEYEVENTF_KEYUP As Integer = &H2

        ''' <summary>
        ''' 被SendInput使用，以保存用于合成输入事件的信息，例如击键次数、鼠标移动、鼠标单击。
        ''' http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure INPUT
            ''' <summary>
            ''' INPUT_MOUSE    0
            ''' INPUT_KEYBOARD 1
            ''' INPUT_HARDWARE 2
            ''' </summary>
            Public type As Integer
            Public inputUnion As NativeMethods.INPUTUNION
        End Structure


        ''' <summary>
        ''' 一个INPUTUNION结构体只包括一个域。 
        ''' http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Explicit)>
        Public Structure INPUTUNION
            <FieldOffset(0)>
            Public hi As NativeMethods.HARDWAREINPUT
            <FieldOffset(0)>
            Public ki As NativeMethods.KEYBDINPUT
            <FieldOffset(0)>
            Public mi As NativeMethods.MOUSEINPUT
        End Structure

        ''' <summary>
        ''' 有关模拟硬件事件的信息。
        ''' http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure HARDWAREINPUT
            Public uMsg As Integer
            Public wParamL As Short
            Public wParamH As Short
        End Structure

        ''' <summary>
        ''' 有关模拟键盘事件的信息。
        ''' http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure KEYBDINPUT
            Public wVk As Short
            Public wScan As Short
            Public dwFlags As Integer
            Public time As Integer
            Public dwExtraInfo As IntPtr
        End Structure

        ''' <summary>
        ''' 有关模拟鼠标事件的信息。
        ''' http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure MOUSEINPUT
            Public dx As Integer
            Public dy As Integer
            Public mouseData As Integer
            Public dwFlags As Integer
            Public time As Integer
            Public dwExtraInfo As IntPtr
        End Structure
    End Class

End Namespace
