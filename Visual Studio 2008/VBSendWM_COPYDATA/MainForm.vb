'******************************* 模块头 *********************************'
' 模块名:  MainForm.vb
' 项目名:      VBSendWM_COPYDATA
' 版权 (c) Microsoft Corporation.
'
' 基于windows 消息 WM_COPYDATA 进程间通信(IPC) 是一种在本地机器上windows应用程序交换数据机制。
' 接受程序必须是一个windows应用程序。数据被传递必须不包含指针或者不能被应用程序接受的指向对象的引用。
' 当发送WM_COPYDATA消息时，引用数据不能被发送进程别的线程改变。 接受应用程序应该只考虑只读数据。
' 如果接受应用程序想要在SendMessage返回之后进入数据， 它必须拷贝数据到本地缓存。

' 
' 这个代码例子示范了通过SendMessage（WM_COPYDATA）发送一个客户端数据结构（MY_STRUCT）到接受应用程序
' 如果数据结构传值失败，应用程序显示一个诊断错误代码。一个典型的错误代码是0x5（非法访问），它是由于用户
' 接口权限隔离导致的。用户接口权限隔离阻止进程发送被选择的窗口消息和其他一些用户进程APIs，这些用户进程拥有
' 比较高的完整性。 当接受程序（VBReceiveWM_COPYDATA）运行在一个比发送程序高的完整性时候，你将会看到一个
' "SendMessage(WM_COPYDATA) failed w/err 0x00000005"错误信息.

' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*******************************************************************************'

Imports System.Runtime.InteropServices
Imports System.Security


Public Class MainForm

    Private Sub btnSendMessage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnSendMessage.Click
        ' 找到接受窗口的句柄（FindWindow）
        Dim hTargetWnd As IntPtr = NativeMethod.FindWindow(Nothing, "VBReceiveWM_COPYDATA")
        If (hTargetWnd = IntPtr.Zero) Then
            MessageBox.Show("不能找到 ""VBReceiveWM_COPYDATA"" 窗口", _
                   "错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' 准备好用来发送的带数据的COPYDATASTRUCT（COPYDATASTRUCT）
        Dim myStruct As MyStruct

        Dim nNumber As Integer
        If Not Integer.TryParse(Me.tbNumber.Text, nNumber) Then
            MessageBox.Show("无效的数字!")
            Return
        End If

        myStruct.Number = nNumber
        myStruct.Message = Me.tbMessage.Text

        ' 封装托管结构到本地内存块
        Dim myStructSize As Integer = Marshal.SizeOf(myStruct)
        Dim pMyStruct As IntPtr = Marshal.AllocHGlobal(myStructSize)
        Try
            Marshal.StructureToPtr(myStruct, pMyStruct, True)

            Dim cds As New COPYDATASTRUCT
            cds.cbData = myStructSize
            cds.lpData = pMyStruct

            ' 通过WM_COPYDATA消息发送COPYDATASTRUCT struct到接受窗口（应用程序必须使用SendMessage）
            ' 取代PostMessage 发送WM_COPYDATA消息，原因是接受程序必须接受，而这是有保证的

            NativeMethod.SendMessage(hTargetWnd, WM_COPYDATA, Me.Handle, cds)

            Dim result As Integer = Marshal.GetLastWin32Error
            If (result <> 0) Then
                MessageBox.Show(String.Format( _
                    "SendMessage(WM_COPYDATA) failed w/err 0x{0:X}", result))
            End If
        Finally
            Marshal.FreeHGlobal(pMyStruct)
        End Try

    End Sub


    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Friend Structure MyStruct
        Public Number As Integer

        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=&H100)> _
        Public Message As String
    End Structure


#Region "Native API Signatures and Types"

    ''' <summary>
    ''' 应用程序发送WM_COPYDATA消息传送数据到另一个应用程序
    ''' </summary>
    Friend Const WM_COPYDATA As Integer = &H4A


    ''' <summary>
    ''' COPYDATASTRUCT 结构包含通过WM_COPYDATA消息被传送到另一个程序的数据 
    ''' </summary>
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure COPYDATASTRUCT
        Public dwData As IntPtr
        Public cbData As Integer
        Public lpData As IntPtr
    End Structure


    <SuppressUnmanagedCodeSecurity()> _
    Friend Class NativeMethod

        ''' <summary>
        ''' 发送指定的消息到窗口或者系统，SendMessage函数为指定的窗口调用窗口过程
        ''' 直到窗口过程处理完消息才返回
        ''' </summary>
        ''' <param name="hWnd">
        ''' 处理窗口，它的窗口过程将要接受消息
        ''' </param>
        ''' <param name="Msg">指定消息被发送</param>
        ''' <param name="wParam">
        ''' 指定额外的特别的消息信息
        ''' </param>
        ''' <param name="lParam">
        ''' 指定额外特别的消息信息
        ''' </param>
        ''' <returns></returns>
        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Public Shared Function SendMessage( _
            ByVal hWnd As IntPtr, _
            ByVal Msg As Integer, _
            ByVal wParam As IntPtr, _
            ByRef lParam As COPYDATASTRUCT) _
            As IntPtr
        End Function


        ''' <summary>
        ''' FindWindow函数 检索顶层窗口
        ''' 它的类名和窗口名匹配特定的字符
        ''' 这个函数不搜索子窗口
        ''' 这个函数执行大小写敏感的搜索
        ''' </summary>
        ''' <param name="lpClassName">Class name</param>
        ''' <param name="lpWindowName">Window caption</param>
        ''' <returns></returns>
        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Public Shared Function FindWindow( _
            ByVal lpClassName As String, _
            ByVal lpWindowName As String) _
            As IntPtr
        End Function

    End Class

#End Region

    Private Sub groupBox1_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles groupBox1.Enter

    End Sub
End Class
