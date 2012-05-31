'********************************** 模块头 ******************************'
' 模块名:  IOleCommandTarget.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
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
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <ComVisible(True)>
    <Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")>
    Public Interface IOleCommandTarget
        <PreserveSig()>
        Function QueryStatus(ByRef pguidCmdGroup As Guid,
                             ByVal cCmds As Integer,
                             <[In](), Out()> ByVal prgCmds As NativeMethods.OLECMD,
                             <[In](), Out()> ByVal pCmdText As IntPtr) _
                         As <MarshalAs(UnmanagedType.I4)> Integer

        <PreserveSig()>
        Function Exec(ByRef pguidCmdGroup As Guid,
                      ByVal nCmdID As Integer,
                      ByVal nCmdexecopt As Integer,
                      <[In](), MarshalAs(UnmanagedType.LPArray)> ByVal pvaIn() As Object,
                      ByVal pvaOut As Integer) As <MarshalAs(UnmanagedType.I4)> Integer
    End Interface
End Namespace

