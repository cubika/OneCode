'*************************** Module Header ******************************'
' 模块名称:  IInputObjectSite.vb
' 项目:	    VBIEExplorerBar
' Copyright (c) Microsoft Corporation.
' 
' 公开一个用于沟通一个用户在shell中输入对象的焦点变化的方法。
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
    <Guid("f1db8392-7331-11d0-8c99-00a0c92dbfe8")>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IInputObjectSite
        <PreserveSig()>
        Function OnFocusChangeIS(<MarshalAs(UnmanagedType.IUnknown)> ByVal punkObj As Object,
                                 ByVal fSetFocus As Integer) As Integer
    End Interface





End Namespace
