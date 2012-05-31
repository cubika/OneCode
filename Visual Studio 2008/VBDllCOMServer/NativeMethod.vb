'****************************** 模块头 ******************************'
' 模块名:      NativeMethod.vb
' 项目名:      VBDllCOMServer
' 版权  (c) Microsoft Corporation.
' 
' 一些关于Native API P/Invoke的注释
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


''' <summary>
''' Native 方法
''' </summary>
''' <remarks></remarks>
Friend Class NativeMethod

    ''' <summary>
    ''' 获取当前进程ID。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("kernel32.dll", EntryPoint:="GetCurrentProcessId")> _
    Friend Shared Function GetCurrentProcessId() As UInteger
    End Function

    ''' <summary>
    ''' 获取当前线程ID。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("kernel32.dll", EntryPoint:="GetCurrentThreadId")> _
    Friend Shared Function GetCurrentThreadId() As UInteger
    End Function

End Class
