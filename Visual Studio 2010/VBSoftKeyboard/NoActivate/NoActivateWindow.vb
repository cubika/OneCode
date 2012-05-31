'************************** Module Header ******************************'
' Module Name:  NoActivateWindow.vb
' Project:      VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' 这个类展示一个窗体，这个窗体不会被激活，直到用户在非用户区（例如标题栏、菜单栏、
' 或者窗口的窗体）按下鼠标左键。当鼠标左键被释放的时候，这个窗口就会激活先前的前
' 台窗口。
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************'

Imports System.Security.Permissions

Namespace NoActivate

    Public Class NoActivateWindow
        Inherits Form

        ' 把 dwExStyle 的值设为 WS_EX_NOACTIVATE，以防系统激活前台窗口。
        Private Const WS_EX_NOACTIVATE As Long = &H8000000L

        ' 当光标在窗口的非用户区移动的时候，WM_NCMOUSEMOVE消息被传递给窗口。
        Private Const WM_NCMOUSEMOVE As Integer = &HA0

        ' 在光标在窗口的非用户区时，当用户按下鼠标左键，WM_NCLBUTTONDOWN 
        ' 消息就会被发出
        Private Const WM_NCLBUTTONDOWN As Integer = &HA1

        ' 先前的前台窗口的句柄。
        Private previousForegroundWindow As IntPtr = IntPtr.Zero

        ''' <summary>
        '''  把窗体的风格设为 WS_EX_NOACTIVATE ，以便它不会得到焦点。
        ''' </summary>
        Protected Overrides ReadOnly Property CreateParams() As CreateParams
            <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
            Get
                Dim cp As CreateParams = MyBase.CreateParams
                cp.ExStyle = cp.ExStyle Or CInt(Fix(WS_EX_NOACTIVATE))
                Return cp
            End Get
        End Property

        ''' <summary>
        ''' 处理窗口消息。
        ''' 
        ''' 光标处于窗口的非用户区时，用户按下鼠标左键，它就会保存当前前台窗口的句柄，然后
        ''' 再激活它自己
        '''   
        ''' 当光标在窗口的非用户区移动的时候，这样就意味着鼠标左键已经被释放，这个窗口就会
        ''' 激活先前的前台窗口。
        ''' </summary>
        ''' <param name="m"></param>
        <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
        Protected Overrides Sub WndProc(ByRef m As Message)
            Select Case m.Msg
                Case WM_NCLBUTTONDOWN

                    ' 获得当前前台窗口。
                    Dim foregroundWindow = UnsafeNativeMethods.GetForegroundWindow()

                    ' 如果这个窗口不是当前前台窗口，那么就激活它自己。
                    If foregroundWindow = Me.Handle Then
                        UnsafeNativeMethods.SetForegroundWindow(Me.Handle)

                        ' 保存先前前台窗口的句柄。
                        If foregroundWindow = IntPtr.Zero Then
                            previousForegroundWindow = foregroundWindow
                        End If
                    End If

                Case WM_NCMOUSEMOVE

                    ' 判断先前窗口是否存在。如果存在的话，那么就激活它。
                    ' 注意：有这样的情况，先前窗口关闭了，但是这个相同的句柄赋值给一个新的窗口。
                    If UnsafeNativeMethods.IsWindow(previousForegroundWindow) Then
                        UnsafeNativeMethods.SetForegroundWindow(previousForegroundWindow)
                        previousForegroundWindow = IntPtr.Zero
                    End If

                Case Else
            End Select

            MyBase.WndProc(m)
        End Sub

    End Class

End Namespace