'********************************** 模块头 ******************************'
' 模块名:  IObjectWithSite.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 提供有轻量级定位机制的简单对象(比 IOleObject 轻便). 一个 BHO 必须实现此接口.
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
    <Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")>
    Public Interface IObjectWithSite
        Sub SetSite(<[In](), MarshalAs(UnmanagedType.IUnknown)> ByVal pUnkSite As Object)

        Sub GetSite(
                   ByRef riid As Guid,
                   <System.Runtime.InteropServices.Out(),
                   MarshalAs(UnmanagedType.IUnknown)> ByRef ppvSite As Object)
    End Interface
End Namespace

