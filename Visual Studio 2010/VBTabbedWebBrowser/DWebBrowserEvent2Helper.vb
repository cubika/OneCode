'*************************** 模块头 ******************************'
' 模块名:  WebBrowser2EventHelper.vb
' 项目名:	    VBTabbedWebBrowser
' 版权 (c) Microsoft Corporation.
' 
'  WebBrowser2EventHelper 类通过提高在WebBrowserEx中定义的NewWindow3 事件
' 从底层的ActiveX 控件 去处理NewWindow3 事件
'  
' 
' 由于方法WebBrowserEx.OnNewWindow3受保护的，WebBrowser2EventHelper类 定义在WebBrowserEx类中
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

Partial Public Class WebBrowserEx
    Private Class DWebBrowserEvent2Helper
        Inherits StandardOleMarshalObject
        Implements DWebBrowserEvents2

        Private _parent As WebBrowserEx

        Public Sub New(ByVal parent As WebBrowserEx)
            Me._parent = parent
        End Sub

        ''' <summary>
        ''' 创建 NewWindows3 事件.
        ''' 当在ActiveX 空间中，NewWindow3事件被触发时，
        ''' 如果WebBrowser2EventHelper的一个实例与底层的ActiveX 空间相关联
        ''' 这个方法就会被调用
        ''' </summary>
        Public Sub NewWindow3(ByRef ppDisp As Object, ByRef Cancel As Boolean,
                              ByVal dwFlags As UInteger, ByVal bstrUrlContext As String,
                              ByVal bstrUrl As String) Implements DWebBrowserEvents2.NewWindow3
            Dim e = New WebBrowserNewWindowEventArgs(bstrUrl, Cancel)
            Me._parent.OnNewWindow3(e)
            Cancel = e.Cancel
        End Sub
    End Class
End Class

