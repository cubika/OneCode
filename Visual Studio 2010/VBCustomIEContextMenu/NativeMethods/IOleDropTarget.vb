'********************************** 模块头 ******************************'
' 模块名:  IOleDropTarget.vb
' 项目名:  VBCustomIEContextMenu
' 版权  (c) Microsoft Corporation.
' 
' 此接口允许对象和其容器派遣彼此的命令.例如，一个对象的工具栏可能包含如打印、
' 打印预览、 保存、 新建和缩放命令的按钮.
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
    <ComImport()>
    <Guid("00000122-0000-0000-C000-000000000046")>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IOleDropTarget

        <PreserveSig()>
        Function OleDragEnter(<[In](), MarshalAs(UnmanagedType.Interface)> ByVal pDataObj As Object,
                              <[In](), MarshalAs(UnmanagedType.U4)> ByVal grfKeyState As Integer,
                              <[In](), MarshalAs(UnmanagedType.U8)> ByVal pt As Long,
                              <[In](), Out()> ByRef pdwEffect As Integer) As Integer

        <PreserveSig()>
        Function OleDragOver(<[In](), MarshalAs(UnmanagedType.U4)> ByVal grfKeyState As Integer,
                             <[In](), MarshalAs(UnmanagedType.U8)> ByVal pt As Long,
                             <[In](), Out()> ByRef pdwEffect As Integer) As Integer

        <PreserveSig()>
        Function OleDragLeave() As Integer

        <PreserveSig()>
        Function OleDrop(<[In](), MarshalAs(UnmanagedType.Interface)> ByVal pDataObj As Object,
                         <[In](), MarshalAs(UnmanagedType.U4)> ByVal grfKeyState As Integer,
                         <[In](), MarshalAs(UnmanagedType.U8)> ByVal pt As Long,
                         <[In](), Out()> ByRef pdwEffect As Integer) As Integer
    End Interface
End Namespace
