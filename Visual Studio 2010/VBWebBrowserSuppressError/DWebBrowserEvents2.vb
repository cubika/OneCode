'******************************* 模块头 ************************************'
' 模块名:   DWebBrowserEvents2.vb
' 项目名:	VBWebBrowserSuppressError
' 版权(c)   Microsoft Corporation.
' 
' 接口DWebBrowserEvents2指明了一个应用程序必须实现的一个事件接收器接口，用来从Web
' 浏览器控件或者Window IE浏览器应用程序接收事件通知.这些事件通知包括这个应用程序中
' 会用到的链接错误事件.
' 
' 请从此处获取DWebBrowserEvents2接口的所有事件的列表:
' http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.Runtime.InteropServices

<ComImport(), TypeLibType(TypeLibTypeFlags.FHidden),
InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")>
Public Interface DWebBrowserEvents2
    <DispId(271)>
    Sub NavigateError(<[In](), MarshalAs(UnmanagedType.IDispatch)> ByVal pDisp As Object,
                          <[In]()> ByRef url As Object,
                          <[In]()> ByRef frame As Object,
                          <[In]()> ByRef statusCode As Object,
                          <[In](), Out()> ByRef cancel As Boolean)
End Interface