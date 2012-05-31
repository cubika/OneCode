'****************************** 模块头 ***********************************\
' 模块名:  DWebBrowserEvents2Helper.vb
' 项目名:  VBWebBrowserLoadComplete
' 版权 (c) Microsoft Corporation.
' 
' DWebBrowserEvents2Helper 通过激发定义在 WebBrowserEx 类的 StartNavigating
' 和 LoadCompleted 事件,用于处理来自底层的 ActiveX 控件的BeforeNavigate2 
' 和 DocumentComplete 事件.
' 
' 如果 WebBrowser 控件装载一个普通的、无内嵌框架的 HTML 页面, 在所有事情完成
' 之后, DocumentComplete 事件会被引发一次.
' 
' 如果 WebBrowser 控件装载了许多内嵌框架, DocumentComplete 会被多次引发.
' DocumentComplete 事件有一个 pDisp 参数,它是框架( shdocvw )的 IDispatch.该框
' 架中 DocumentComplete 被引发.
' 
' 然后我们可以检查是否 DocumentComplete 的 pDisp 参数与浏览器的ActiveXInstance
' 相同.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Runtime.InteropServices


Partial Public Class WebBrowserEx
    Private Class DWebBrowserEvents2Helper
        Inherits StandardOleMarshalObject
        Implements DWebBrowserEvents2

        Private parent As WebBrowserEx

        Public Sub New(ByVal parent As WebBrowserEx)
            Me.parent = parent
        End Sub

        ''' <summary>
        ''' 当文件完全加载并初始化时，引发.
        ''' 如果框架是顶层窗口元素,那么该页面就加载完全.
        ''' 
        ''' 然后,在 WebBrowser 完全加载之后,重置 glpDisp 为 null.
        ''' </summary>
        Public Sub DocumentComplete(ByVal pDisp As Object, ByRef URL As Object) _
             Implements DWebBrowserEvents2.DocumentComplete

            Dim _url As String = TryCast(URL, String)

            If String.IsNullOrEmpty(_url) OrElse _
                _url.Equals("about:blank", StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            If pDisp IsNot Nothing AndAlso pDisp.Equals(parent.ActiveXInstance) Then
                Dim e = New WebBrowserDocumentCompletedEventArgs(New Uri(_url))

                parent.OnLoadCompleted(e)
            End If
        End Sub

        ''' <summary>
        ''' 在给定对象中(一个窗口元素或者一个框架集元素)导航发生前引发.
        ''' 
        ''' </summary>
        Public Sub BeforeNavigate2(ByVal pDisp As Object,
                                   ByRef URL As Object,
                                   ByRef flags As Object,
                                   ByRef targetFrameName As Object,
                                   ByRef postData As Object,
                                   ByRef headers As Object,
                                   ByRef cancel As Boolean) _
                               Implements DWebBrowserEvents2.BeforeNavigate2

            Dim _url As String = TryCast(URL, String)

            If String.IsNullOrEmpty(_url) OrElse _
                _url.Equals("about:blank", StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            If pDisp IsNot Nothing AndAlso pDisp.Equals(parent.ActiveXInstance) Then
                Dim e As New WebBrowserNavigatingEventArgs(
                    New Uri(_url), TryCast(targetFrameName, String))

                parent.OnStartNavigating(e)
            End If
        End Sub


    End Class
End Class

