'********************************** 模块头 ******************************'
' 模块名:  OLECMD.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 类 OLECMD 通过调用 IOleCommandTarget::QueryStatus,将 来自OLECMDF 枚举中的命
' 令标志与命令标识符关联起来.
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

Imports System
Imports System.Runtime.InteropServices

Namespace NativeMethods
    <StructLayout(LayoutKind.Sequential)>
    Public Class OLECMD
        <MarshalAs(UnmanagedType.U4)>
        Public cmdID As Integer
        <MarshalAs(UnmanagedType.U4)>
        Public cmdf As Integer
    End Class
End Namespace

