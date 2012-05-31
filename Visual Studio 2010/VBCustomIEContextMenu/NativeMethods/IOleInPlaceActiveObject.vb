'********************************** 模块头 ******************************'
' 模块名:  IOleInPlaceActiveObject.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 这个接口被对象应用程序实现，以便在他们活动时,为他们的对象提供支持.
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
Imports System.Security

Namespace NativeMethods
    <ComImport()>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <SuppressUnmanagedCodeSecurity()>
    <Guid("00000117-0000-0000-C000-000000000046")>
    Public Interface IOleInPlaceActiveObject
        <PreserveSig()>
        Function GetWindow(<Out()> ByRef hwnd As IntPtr) As Integer

        Sub ContextSensitiveHelp(ByVal fEnterMode As Integer)

        <PreserveSig()>
        Function TranslateAccelerator(<[In]()> ByRef lpmsg As NativeMethods.MSG) As Integer

        Sub OnFrameWindowActivate(ByVal fActivate As Boolean)

        Sub OnDocWindowActivate(ByVal fActivate As Integer)

        Sub ResizeBorder(<[In]()> ByVal prcBorder As NativeMethods.COMRECT,
                         <[In]()> ByVal pUIWindow As NativeMethods.IOleInPlaceUIWindow,
                         ByVal fFrameWindow As Boolean)

        Sub EnableModeless(ByVal fEnable As Integer)
    End Interface
End Namespace

