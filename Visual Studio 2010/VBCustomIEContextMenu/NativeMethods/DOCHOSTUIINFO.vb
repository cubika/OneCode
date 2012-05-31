'********************************** 模块头 ******************************'
' 模块名:   DOCHOSTUIINFO.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 类 DOCHOSTUIINFO 被 IDocHostUIHandler::GetHostInfo 方法用来允许　MSHTML 来
' 检索有关主机用户界面要求的信息
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

Namespace NativeMethods
    <StructLayout(LayoutKind.Sequential), ComVisible(True)>
    Public Class DOCHOSTUIINFO
        <MarshalAs(UnmanagedType.U4)>
        Public cbSize As Integer = Marshal.SizeOf(GetType(NativeMethods.DOCHOSTUIINFO))
        <MarshalAs(UnmanagedType.I4)>
        Public dwFlags As Integer
        <MarshalAs(UnmanagedType.I4)>
        Public dwDoubleClick As Integer
        <MarshalAs(UnmanagedType.I4)>
        Public dwReserved1 As Integer
        <MarshalAs(UnmanagedType.I4)>
        Public dwReserved2 As Integer
    End Class
End Namespace

