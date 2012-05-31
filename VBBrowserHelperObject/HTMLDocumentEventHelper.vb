'*************************** 模块头 ******************************'
' 模块名称:  HTMLDocumentEventHelper.vb
' 项目名称:	VBBrowserHelperObject
' 版权：Copyright (c) Microsoft Corporation.
' 
' 这个 ComVisible 类 HTMLDocumentEventHelper是用来设置HTMLDocument的事件处理的。
' 接口 DispHTMLDocument 定义了很多事件，就像 
' oncontextmenu, onclick等, 这些事件可以被设置到
' HTMLEventHandler 实例中.
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
Imports mshtml

<ComVisible(True)>
Public Class HTMLDocumentEventHelper
    Private document As HTMLDocument

    Public Sub New(ByVal document As HTMLDocument)
        Me.document = document
    End Sub

    Public Custom Event oncontextmenu As HtmlEvent
        AddHandler(ByVal value As HtmlEvent)
            Dim dispDoc As DispHTMLDocument = TryCast(Me.document, DispHTMLDocument)

            Dim existingHandler As Object = dispDoc.oncontextmenu
            Dim handler As HTMLEventHandler =
                If(TypeOf existingHandler Is HTMLEventHandler,
                   TryCast(existingHandler, HTMLEventHandler),
                   New HTMLEventHandler(Me.document))

            ' 为 oncontextmenu 事件设置事件处理句柄.
            dispDoc.oncontextmenu = handler

            AddHandler handler.eventHandler, value

        End AddHandler
        RemoveHandler(ByVal value As HtmlEvent)
            Dim dispDoc As DispHTMLDocument = TryCast(Me.document, DispHTMLDocument)
            Dim existingHandler As Object = dispDoc.oncontextmenu

            Dim handler As HTMLEventHandler = 
                If(TypeOf existingHandler Is HTMLEventHandler, 
                   TryCast(existingHandler, HTMLEventHandler), Nothing)

            If handler IsNot Nothing Then
                RemoveHandler handler.eventHandler, value
            End If
        End RemoveHandler
        RaiseEvent(ByVal e As mshtml.IHTMLEventObj)
           
        End RaiseEvent
    End Event
End Class

