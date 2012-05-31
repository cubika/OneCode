'*************************** Module Header ******************************'
' 模块名称:  IDockingWindow.vb
' 项目:	    VBIEExplorerBar
' Copyright (c) Microsoft Corporation.
' 
' 公开的方法，通知对接窗口对象的变化，包括显示、隐藏和即将到来的移动。
' 此接口由可以与一个Windows资源管理器的边界空间对接的窗口对象实现。
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
    <Guid("012dd920-7b26-11d0-8ca9-00a0c92dbfe8")>
    Public Interface IDockingWindow
        Sub GetWindow(<Out()> ByRef phwnd As IntPtr)

        Sub ContextSensitiveHelp(<[In]()> ByVal fEnterMode As Boolean)

        Sub ShowDW(<[In]()> ByVal fShow As Boolean)

        Sub CloseDW(<[In]()> ByVal dwReserved As UInteger)

        Sub ResizeBorderDW(ByVal prcBorder As IntPtr,
                           <[In](), MarshalAs(UnmanagedType.IUnknown)> ByVal punkToolbarSite As Object,
                           ByVal fReserved As Boolean)
    End Interface
End Namespace
