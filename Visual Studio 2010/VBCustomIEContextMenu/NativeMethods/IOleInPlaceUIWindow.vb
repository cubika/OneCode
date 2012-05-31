'********************************** 模块头 ******************************'
' 模块名:  IOleInPlaceUIWindow.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 当一个对象被激活时,或如果对象的大小发生了变化,重新协商边框区域时,这个接口被
' 对象的应用程序用来协商在文档或框架窗口的边框区域.
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
    <Guid("00000115-0000-0000-C000-000000000046")>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IOleInPlaceUIWindow
        Function GetWindow() As IntPtr

        <PreserveSig()>
        Function ContextSensitiveHelp(ByVal fEnterMode As Integer) As Integer

        <PreserveSig()>
        Function GetBorder(<Out()> ByVal lprectBorder As NativeMethods.COMRECT) As Integer

        <PreserveSig()>
        Function RequestBorderSpace(<[In]()> ByVal pborderwidths As NativeMethods.COMRECT) As Integer

        <PreserveSig()>
        Function SetBorderSpace(<[In]()> ByVal pborderwidths As NativeMethods.COMRECT) As Integer

        Sub SetActiveObject(<[In](), MarshalAs(UnmanagedType.Interface)> ByVal pActiveObject As IOleInPlaceActiveObject,
                            <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal pszObjName As String)
    End Interface
End Namespace

