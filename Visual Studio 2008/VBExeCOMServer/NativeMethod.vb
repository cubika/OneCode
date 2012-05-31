'********************************* 模块头 **********************************'
' 模块名:    NativeMethod.vb
' 项目名:      VBExeCOMServer
' Copyright (c) Microsoft Corporation.
' 
' 一些关于NativeAPI P/Invoke注释
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directives"

Imports System.Runtime.InteropServices

#End Region


Friend Class NativeMethod

    ''' <summary>
    ''' 获取当前线程ID
    ''' </summary>
    ''' <returns></returns>
    <DllImport("kernel32.dll")> _
    Friend Shared Function GetCurrentThreadId() As UInt32
    End Function


    ''' <summary>
    ''' 获取当前进程ID
    ''' </summary>
    ''' <returns></returns>
    <DllImport("kernel32.dll")> _
    Friend Shared Function GetCurrentProcessId() As UInt32
    End Function


    ''' <summary>
    ''' GetMessage函数用于获取从住线程消息队列中的消息。在没有检索到用户发布的
    ''' 消息之前，该函数会一直调度传入消息
    ''' </summary>
    ''' <param name="lpMsg">
    ''' 指向一个MSG的结构。这个结构用于接收从线程消息队列中收到的消息。.
    ''' </param>
    ''' <param name="hWnd">
    ''' 收到消息的窗体的句柄。
    ''' </param>
    ''' <param name="wMsgFilterMin">
    ''' 指定一个整数变量。该变量用于设定最少被接受的消息数量。
    ''' </param>
    ''' <param name="wMsgFilterMax">
    ''' 指定一个整数变量。该变量用于设定最大被接受的消息数量。
    ''' </param>
    ''' <returns></returns>
    <DllImport("user32.dll")> _
    Friend Shared Function GetMessage(<Out()> ByRef lpMsg As MSG, _
                                      ByVal hWnd As IntPtr, _
                                      ByVal wMsgFilterMin As UInt32, _
                                      ByVal wMsgFilterMax As UInt32) As Boolean
    End Function


    ''' <summary>
    ''' TranslateMessage函数用于翻译一个虚拟按键消息到一个字符消息。此字符消息会
    ''' 再次发送到线程的消息队列中。它在下一次的GetMessage或PeekMessage函数被执行
    ''' 时被读取
    ''' </summary>
    ''' <param name="lpMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("user32.dll")> _
    Friend Shared Function TranslateMessage(<[In]()> ByRef lpMsg As MSG) As Boolean
    End Function


    ''' <summary>
    ''' DispatchMessage函数调度一个函数至一个窗体进程。通常的它被用于调度一个被
    ''' GetMessgae获取的消息。
    ''' </summary>
    ''' <param name="lpMsg"></param>
    ''' <returns></returns>
    <DllImport("user32.dll")> _
    Friend Shared Function DispatchMessage(<[In]()> ByRef lpMsg As MSG) As IntPtr
    End Function


    ''' <summary>
    ''' PostThreadMessage函数发送一个消息至一割指定线程的消息队列。它并等待该线程
    ''' 处理这个消息。
    ''' </summary>
    ''' <param name="idThread">
    ''' 用于指定该消息将要被发送到那个线程
    ''' </param>
    ''' <param name="Msg">用于指定消息的种类</param>
    ''' <param name="wParam">
    ''' 用于指定额外的消息相关信息
    ''' </param>
    ''' <param name="lParam">
    ''' 用于指定额外的消息相关信息
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("user32.dll")> _
    Friend Shared Function PostThreadMessage(ByVal idThread As UInt32, _
                                             ByVal Msg As UInt32, _
                                             ByVal wParam As UIntPtr, _
                                             ByVal lParam As IntPtr) As Boolean
    End Function


    Friend Const WM_QUIT As Integer = &H12

End Class


<StructLayout(LayoutKind.Sequential)> _
Friend Structure MSG
    Public hWnd As IntPtr
    Public message As UInt32
    Public wParam As IntPtr
    Public lParam As IntPtr
    Public time As UInt32
    Public pt As POINT
End Structure


<StructLayout(LayoutKind.Sequential)> _
Friend Structure POINT
    Public X As Integer
    Public Y As Integer

    Public Sub New(ByVal x As Integer, ByVal y As Integer)
        Me.X = x
        Me.Y = y
    End Sub
End Structure