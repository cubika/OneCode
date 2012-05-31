'*************************** Module Header ******************************'
' 模块名称:  _IServiceProvider.vb
' 项目:	    VBIEExplorerBar
' Copyright (c) Microsoft Corporation.
' 
' 提供了一个查找确定的GUID服务的通用访问机制
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
    <ComImport()>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <Guid("6d5140c1-7436-11ce-8034-00aa006009fa")>
    Friend Interface _IServiceProvider
        ''' <summary>
        ''' 作为任何通过实现IServiceProvider公开的服务的factory方法
        ''' </summary>
        Sub QueryService(ByRef guid As Guid,
                         ByRef riid As Guid,
                         <Out(), MarshalAs(UnmanagedType.Interface)> ByRef Obj As Object)
    End Interface
End Namespace
