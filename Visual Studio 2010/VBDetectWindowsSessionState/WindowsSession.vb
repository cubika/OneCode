'*************************** 模块头 ******************************'
' 模块名:  WindowsSession.vb
' 项目名:	  VBDetectWindowsSessionState
' 版权 (c) Microsoft Corporation.
' 
' WindowsSession类是用来订阅SystemEvents.SessionSwitch事件, 同时导入OpenInputDesktop
' 方法来检测当前回话是否被锁定.
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
Imports Microsoft.Win32
Imports System.Security.Permissions

Public Class WindowsSession
    Implements IDisposable

    ''' <summary>
    ''' 打开桌面来接收用户输入.
    ''' 这个方法用来检查桌面是否被锁定. 如果返回IntPtr.Zero, 意味着方法失败, 也就是桌面被锁定.
    ''' 注意:
    '''      如果UAC弹出安全桌面, 这个方法同样会失败. 现在没有API能够区分是桌面锁定还是UAC弹出
    '''      安全桌面.
    ''' </summary>
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function OpenInputDesktop(ByVal dwFlags As Integer,
                                             ByVal fInherit As Boolean,
                                             ByVal dwDesiredAccess As Integer) As IntPtr
    End Function

    ''' <summary>
    ''' 关闭桌面对象的句柄.
    ''' </summary>
    ''' <returns>
    ''' 成功返回true.
    ''' </returns>
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function CloseDesktop(ByVal hDesktop As IntPtr) As Boolean
    End Function

    ' Specify whether this instance is disposed.
    Private disposed As Boolean

    Public Event StateChanged As EventHandler(Of SessionSwitchEventArgs)

    ''' <summary>
    ''' 初始化对象.
    ''' 注册SystemEvents.SessionSwitch事件.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New()
        AddHandler SystemEvents.SessionSwitch, AddressOf SystemEvents_SessionSwitch
    End Sub

    

    ''' <summary>
    ''' 处理SystemEvents.SessionSwitch事件.
    ''' </summary>
    Private Sub SystemEvents_SessionSwitch(ByVal sender As Object,
                                           ByVal e As SessionSwitchEventArgs)
        Me.OnStateChanged(e)
    End Sub

    ''' <summary>
    ''' 触发StateChanged事件.
    ''' </summary>
    Protected Overridable Sub OnStateChanged(ByVal e As SessionSwitchEventArgs)
        RaiseEvent StateChanged(Me, e)
    End Sub

    ''' <summary>
    ''' 检查当前会话是否处于锁定.
    ''' 注意:
    '''      如果UAC弹出安全桌面, 这个方法同样会失败. 现在没有API能够区分是桌面锁定还是UAC弹出
    '''      安全桌面. 
    ''' </summary>
    Public Function IsLocked() As Boolean
        Dim hDesktop As IntPtr = IntPtr.Zero

        Try

            ' Opens the desktop that receives user input.
            hDesktop = OpenInputDesktop(0, False, &H100)

            ' If hDesktop is IntPtr.Zero, then the session is locked.
            Return hDesktop = IntPtr.Zero
        Finally

            ' Close an open handle to a desktop object.
            If hDesktop <> IntPtr.Zero Then
                CloseDesktop(hDesktop)
            End If
        End Try
    End Function

    ''' <summary>
    ''' 释放资源.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If disposed Then
            Return
        End If

        If disposing Then
            ' 释放托管资源.
            RemoveHandler SystemEvents.SessionSwitch,
                AddressOf SystemEvents_SessionSwitch

        End If

        disposed = True
    End Sub
End Class


