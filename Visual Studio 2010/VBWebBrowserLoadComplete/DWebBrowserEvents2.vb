'****************************** 模块头 ***********************************\
' 模块名:  DWebBrowserEvents2.vb
' 项目名:  VBWebBrowserLoadComplete
' 版权 (c) Microsoft Corporation.
' 
' DWebBrowserEvents2 接口指定了,应用程序必须实现从 WebBrowser 控件或 Windows 
' Internet Explorer 应用程序中，接收事件通知的事件接收接口. 事件通知包括将用
' 于此应用程序的 DocumentCompleted 和 BeforeNavigate2 的事件.
' 
' To get the full event list of DWebBrowserEvents2, see
' http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************/

Imports System.Runtime.InteropServices


<ComImport(),
 TypeLibType(TypeLibTypeFlags.FHidden),
 InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
 Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")>
Public Interface DWebBrowserEvents2
    ''' <summary>
    ''' 当文档完全加载并初始化时引发.
    ''' </summary>
    <DispId(259)>
    Sub DocumentComplete(<[In](), MarshalAs(UnmanagedType.IDispatch)> ByVal pDisp As Object,
                         <[In]()> ByRef URL As Object)

    <DispId(250)>
    Sub BeforeNavigate2(<[In](), MarshalAs(UnmanagedType.IDispatch)> ByVal pDisp As Object,
                        <[In]()> ByRef URL As Object,
                        <[In]()> ByRef flags As Object,
                        <[In]()> ByRef targetFrameName As Object,
                        <[In]()> ByRef postData As Object,
                        <[In]()> ByRef headers As Object,
                        <[In](), Out()> ByRef cancel As Boolean)
End Interface

