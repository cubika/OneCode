'***************************** 模块头 *************************************\
' 模块名:  CWin32WindowWrapper.vb
' 项目名:  VBOneNoteRibbonAddIn
' 版权 (c) Microsoft Corporation.
'
' 包装 Win32 HWND 手柄
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Public Class CWin32WindowWrapper
    Implements IWin32Window

    ' _windowHandle 字段
    Private _windowHandle As IntPtr

    ''' <summary>
    '''     CWin32WindowWrapper 构造器 
    ''' </summary>
    ''' <param name="windowHandle">窗体句柄</param>
    Public Sub New(ByVal windowHandle As IntPtr)
        Me._windowHandle = windowHandle
    End Sub

    ' 总结:
    '     获取由实现者表示的窗口的句柄.
    '
    ' 返回:
    '    由实现者表示的窗口的句柄.
    Public ReadOnly Property Handle() As IntPtr Implements IWin32Window.Handle
        Get
            Return Me._windowHandle
        End Get
    End Property
End Class
