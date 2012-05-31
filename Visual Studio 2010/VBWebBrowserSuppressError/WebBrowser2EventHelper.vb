'****************************** 模块头 ************************************'
' 模块名:   WebBrowser2EventHelper.vb
' 项目名:	VBWebBrowserSuppressError
' 版权(c)   Microsoft Corporation.
' 
' 此WebBrowser2EventHelper类通过触发在WebBrowserEx类中定义的NavigateError事件,
' 来处理来自底层ActiveX控件的NavigateError事件. 
' 
' 归因于protected方法WebBrowserEx.OnNavigateError，此类被定义在WebBrowserEx类
' 的内部.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************'

Imports System.Runtime.InteropServices

Partial Public Class WebBrowserEx

    Private Class WebBrowser2EventHelper
        Inherits StandardOleMarshalObject
        Implements DWebBrowserEvents2
        Private parent As WebBrowserEx

        Public Sub New(ByVal parent As WebBrowserEx)
            Me.parent = parent
        End Sub

        ''' <summary>
        ''' 触发NavigateError事件.
        ''' 如果一个WebBrowser2EventHelper类的实例被关联到底层ActiveX控件,
        ''' 当NavigateError事件在此ActiveX控件中被解除时这个方法将会被调用.
        ''' </summary>
        Public Sub NavigateError(ByVal pDisp As Object, ByRef url As Object,
                                 ByRef frame As Object, ByRef statusCode As Object,
                                 ByRef cancel As Boolean) _
                             Implements DWebBrowserEvents2.NavigateError

            ' 在WebBrowserEx类中触发NavigateError事件.
            Me.parent.OnNavigateError(
                New WebBrowserNavigateErrorEventArgs(
                    CType(url, String),
                    CType(frame, String),
                    CInt(Fix(statusCode)),
                    cancel))

        End Sub
    End Class
End Class
