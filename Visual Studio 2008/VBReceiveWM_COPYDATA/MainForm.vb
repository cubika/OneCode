'******************************* 模块头 *********************************'
' 模块名:  MainForm.vb
' 项目名:      VBReceiveWM_COPYDATA
' 版权 (c) Microsoft Corporation.
'
' 基于windows 消息 WM_COPYDATA 进程间通信(IPC) 是一种在本地机器上windows应用程序交换数据机制。
' 接受程序必须是一个windows应用程序。数据被传递必须不包含指针或者不能被应用程序接受的指向对象的引用。
' 当发送WM_COPYDATA消息时，引用数据不能被发送进程别的线程改变。 接受应用程序应该只考虑只读数据。
' 如果接受应用程序想要在SendMessage返回之后进入数据， 它必须拷贝数据到本地缓存。

' 
' 这个代码例子示范了通过处理WM_COPYDATA消息从发送程序（VBSendWM_COPYDATA）接受一个客户数据结构（MYStruct）
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

'*******************************************************************************'

Imports System.Runtime.InteropServices


Public Class MainForm

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If (m.Msg = WM_COPYDATA) Then
            ' 从lParam中获取COPYDATASTRUCT结构
            Dim cds As COPYDATASTRUCT = m.GetLParam(GetType(COPYDATASTRUCT))

            ' 如果大小匹配
            If (cds.cbData = Marshal.SizeOf(GetType(MyStruct))) Then
                ' 从非托管内存块封装数据到MyStruct托管结构体
                Dim myStruct As MyStruct = Marshal.PtrToStructure(cds.lpData, _
                    GetType(MyStruct))

                ' 显示Mystruct数据成员
                Me.lbNumber.Text = myStruct.Number.ToString
                Me.lbMessage.Text = myStruct.Message
            End If
        End If

        MyBase.WndProc(m)
    End Sub


    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Friend Structure MyStruct
        Public Number As Integer

        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=&H100)> _
        Public Message As String
    End Structure


#Region "Native API Signatures and Types"

    ''' <summary>
    ''' 程序发送WM_COPYDATA消息传数据到另一个程序。
    ''' </summary>
    Friend Const WM_COPYDATA As Integer = &H4A


    ''' <summary>
    ''' COPYDATASTRUCT结构包含一个需要通过WM_COPYDATA被传到另一个程序的数据
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure COPYDATASTRUCT
        Public dwData As IntPtr
        Public cbData As Integer
        Public lpData As IntPtr
    End Structure

#End Region

    Private Sub label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles label1.Click

    End Sub

    Private Sub lbMessage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbMessage.Click

    End Sub
End Class