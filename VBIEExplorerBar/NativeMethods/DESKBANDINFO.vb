'*************************** Module Header ******************************'
' 模块名称:  DESKBANDINFO.vb
' 项目:	    VBIEExplorerBar
' Copyright (c) Microsoft Corporation.
' 
' 为一个band对象保留并接受信息。该结构与IDeskBand.GetBandInfo方法一起使用。
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
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Public Structure DESKBANDINFO
        Public dwMask As UInt32
        Public ptMinSize As POINT
        Public ptMaxSize As POINT
        Public ptIntegral As POINT
        Public ptActual As POINT
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=255)>
        Public wszTitle As String
        Public dwModeFlags As DBIM
        Public crBkgnd As Int32
    End Structure
End Namespace
