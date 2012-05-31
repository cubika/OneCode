'*************************** 模块头 ******************************'
' 模块名:    HtmlInputElement.vb
' 项目名:	    VBWebBrowserAutomation
' 版权(c) Microsoft Corporation.
' 
' 这个抽象HtmlInputElement类代表着一个带有“输入”标签的HtmlElement。
'
' 当一个继承自HtmlInputElement的类序列化或反序列化一个对象时，XmlIncludeAttribute允许
' XmlSerializer来识别它。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Xml.Serialization

<XmlInclude(GetType(HtmlCheckBox)),
XmlInclude(GetType(HtmlPassword)),
XmlInclude(GetType(HtmlSubmit)),
XmlInclude(GetType(HtmlText))>
Public MustInherit Class HtmlInputElement
    Public Property ID() As String

    ''' <summary>
    ''' 这个无参数构造函数用于反序列化。
    ''' </summary>
    Protected Sub New()
    End Sub

    Protected Sub New(ByVal ID As String)
        Me.ID = ID
    End Sub

    ''' <summary>
    ''' 设置HtmlElement的值。
    ''' </summary>
    Public Overridable Sub SetValue(ByVal element As HtmlElement)
    End Sub

End Class
