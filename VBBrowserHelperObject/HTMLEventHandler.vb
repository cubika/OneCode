'*************************** 模块头 ******************************'
' 模块名称：HTMLEventHandler.vb
' 项目名称： VBBrowserHelperObject
' 版权：Copyright (c) Microsoft Corporation.
' 
' 这个 ComVisible 类 HTMLEventHandler 能够复制给DispHTMLDocument 接口的事件属性 ,
' 就像 oncontextmenu, onclick等. 
' 他还可以用在其它HTML Elements元素中 
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

' 事件处理方法的委托.
Public Delegate Sub HtmlEvent(ByVal e As mshtml.IHTMLEventObj)

<ComVisible(True)>
Public Class HTMLEventHandler

    Private htmlDocument As mshtml.HTMLDocument

    Public Event eventHandler As HtmlEvent

    Public Sub New(ByVal htmlDocument As mshtml.HTMLDocument)
        Me.htmlDocument = htmlDocument
    End Sub

    <DispId(0)>
    Public Sub FireEvent()
        RaiseEvent eventHandler(Me.htmlDocument.parentWindow.event)
    End Sub
End Class
