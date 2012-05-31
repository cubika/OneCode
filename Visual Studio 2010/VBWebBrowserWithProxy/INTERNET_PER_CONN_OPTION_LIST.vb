'*************************** Module Header ******************************'
' 模块名称:  INTERNET_PER_CONN_OPTION_LIST.vb
' 项目名称:		    VBWebBrowserWithProxy
' Copyright (c) Microsoft Corporation.
' 
' INTERNET_PER_CONN_OPTION包含internet连接的一个选项列表。
' 在 http://msdn.microsoft.com/en-us/library/aa385146(VS.85).aspx 中你能获得更多的 
' 详情。
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

<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
Public Structure INTERNET_PER_CONN_OPTION_LIST
    Public Size As Integer

    ' 设定连接，如果是NULL就意味着使用的是局域网。
    Public Connection As IntPtr

    Public OptionCount As Integer
    Public OptionError As Integer

    ' INTERNET_PER_CONN_OPTION的集合。
    Public pOptions As IntPtr
End Structure
