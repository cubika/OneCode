'*************************** Module Header ******************************'
' Module Name:  UnsafeNativeMethods.vb
' Project:	    VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' 这个类覆盖了 User32.dll 中的 GetKeyState 和 SendInput。
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
    Friend NotInheritable Class UnsafeNativeMethods

        ''' <summary>
        ''' 获取特定虚拟按键的状态。这个状态指定这个键是浮起、按下还是锁定（每次按键时，
        ''' 打开和断开相互交替）。
        ''' </summary>
        ''' <param name="nVirtKey">一个虚拟按键。 </param>
        ''' <returns>
        ''' 如果高位是1，那么这个键是按下的，否则，它是浮起的。
        ''' 如果低位是1，这个键是被锁定的。比如大小写锁定键，如果被打开的话就被锁定了。
        ''' 如果低位是0，这个键就是断开的并且未被锁定。
        ''' 当切换键被锁定的时候，它在键盘上的指示灯会亮起来，当解锁的时候，指示灯就关闭。
        ''' </returns>
        <DllImport("User32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function GetKeyState(ByVal nVirtKey As Integer) As Short
        End Function

        ''' <summary>
        ''' 合成按键、鼠标运动和按钮点击。
        ''' </summary>
        ''' <param name="nInputs"> pInputs数组中的结构体数目。</param>
        ''' <param name="pInputs">
        ''' 一个INPUT结构体组成的数组。每一个结构体代表一个将要被插入到键盘或者鼠标输入流的事件。
        ''' </param>
        ''' <param name="cbSize">
        ''' 一个INPUT结构体的大小，以字节为单位。如果cbSize不是INPUT结构体的大小，方法就执行失败。
        ''' </param>
        ''' <returns>
        ''' 如果这个函数返回 0，表明输入被其他线程阻塞。 
        ''' </returns>
        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function SendInput(ByVal nInputs As UInteger,
                                         ByVal pInputs() As NativeMethods.INPUT,
                                         ByVal cbSize As Integer) As UInteger
        End Function

    End Class
End Namespace
