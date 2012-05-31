'*************************** Module Header ******************************'
' 模块名称:  INTERNET_PER_CONN_OPTION.vb
' 项目名称:	    VBWebBrowserWithProxy
' Copyright (c) Microsoft Corporation.
' 
' 这个文件定义了一个叫做INTERNET_PER_CONN_OPTION的数据结构和它使用的一些常量。
' 这个数据结构包含internet设置的项目和值。 
' 在 http://msdn.microsoft.com/en-us/library/aa385145(VS.85).aspx t中你能了解 
' 更多。
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

''' <summary>
''' INTERNET_PER_CONN_OPTION_OptionUnion数据结构中所用的常量
''' </summary>
Public Enum INTERNET_PER_CONN_OptionEnum
    INTERNET_PER_CONN_FLAGS = 1
    INTERNET_PER_CONN_PROXY_SERVER = 2
    INTERNET_PER_CONN_PROXY_BYPASS = 3
    INTERNET_PER_CONN_AUTOCONFIG_URL = 4
    INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5
    INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6
    INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7
    INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8
    INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9
    INTERNET_PER_CONN_FLAGS_UI = 10
End Enum

''' <summary>
''' INTERNET_PER_CONN_OPTON数据结构中所用的常量。
''' </summary>
Public Enum INTERNET_OPTION_PER_CONN_FLAGS
    PROXY_TYPE_DIRECT = &H1 ' 直接接入Internet
    PROXY_TYPE_PROXY = &H2 ' 使用代理接入
    PROXY_TYPE_AUTO_PROXY_URL = &H4 ' 自动代理的URL
    PROXY_TYPE_AUTO_DETECT = &H8 ' 使用自动代理检测
End Enum

''' <summary>
''' 以下数据结构在 INTERNET_PER_CONN_OPTION中使用。
''' 当一个选项结构被实例化时，只有其中的一个能被使用。
''' 数据结构的布置和各项属性的设置有助于减小数据构的大小。
''' </summary>
<StructLayout(LayoutKind.Explicit)>
Public Structure INTERNET_PER_CONN_OPTION_OptionUnion
    ' INTERNET_OPTION_PER_CONN_FLAGS数据结构中的值。
    <FieldOffset(0)>
    Public dwValue As Integer
    <FieldOffset(0)>
    Public pszValue As IntPtr
    <FieldOffset(0)>
    Public ftValue As System.Runtime.InteropServices.ComTypes.FILETIME
End Structure

<StructLayout(LayoutKind.Sequential)>
Public Structure INTERNET_PER_CONN_OPTION
    ' INTERNET_PER_CONN_OptionEnum数据结构中的值。
    Public dwOption As Integer
    Public Value As INTERNET_PER_CONN_OPTION_OptionUnion
End Structure
