'*************************** Module Header ******************************'
' 模块名称:  IInputObject.vb
' 项目:	    VBIEExplorerBar
' Copyright (c) Microsoft Corporation.
' 
' 公开方法，改变用户界面是否激活并为用户在Shell中输入对象过程加速器
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
    <Guid("68284faa-6a48-11d0-8c78-00c04fd918b4")>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Public Interface IInputObject
        Sub UIActivateIO(ByVal fActivate As Integer, ByRef msg As MSG)
        <PreserveSig()>
        Function HasFocusIO() As Integer
        <PreserveSig()>
        Function TranslateAcceleratorIO(ByRef msg As MSG) As Integer
    End Interface
End Namespace
