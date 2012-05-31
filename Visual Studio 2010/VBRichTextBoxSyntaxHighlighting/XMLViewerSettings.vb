'*************************** 模块头 ******************************'
' 模块名:  XMLViewerSettings.vb
' 项目名:	    VBRichTextBoxSyntaxHighlighting
' 版权(c) Microsoft Corporation.
' 
' 这个XMLViewerSettings类定义了一些在XmlViewer类要使用的颜色，也
' 定义了一些用来在RTF中指定颜色顺序的常量
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Text

Public Class XMLViewerSettings
    Public Const ElementID As Integer = 1
    Public Const ValueID As Integer = 2
    Public Const AttributeKeyID As Integer = 3
    Public Const AttributeValueID As Integer = 4
    Public Const TagID As Integer = 5

    ''' <summary>
    ''' 一个xml元素名称的颜色
    ''' </summary>
    Public Property Element() As Color

    ''' <summary>
    ''' 一个xml元素值的颜色
    ''' </summary>
    Public Property Value() As Color

    ''' <summary>
    ''' 在xml元素中，属性键的颜色
    ''' </summary>
    Public Property AttributeKey() As Color

    ''' <summary>
    ''' 在xml元素中，属性值的颜色
    ''' </summary>
    Public Property AttributeValue() As Color

    ''' <summary>
    ''' 标签和运算符的颜色，例如，  "<,/> 和 =".
    ''' </summary>
    Public Property Tag() As Color

    ''' <summary>
    ''' 转化设置给RTF颜色的定义
    ''' </summary>
    Public Function ToRtfFormatString() As String
        ' RTF颜色定义格式
        Dim format As String = "\red{0}\green{1}\blue{2};"

        Dim rtfFormatString As New StringBuilder()

        rtfFormatString.AppendFormat(format, Element.R, Element.G, Element.B)
        rtfFormatString.AppendFormat(format, Value.R, Value.G, Value.B)
        rtfFormatString.AppendFormat(format, AttributeKey.R, AttributeKey.G, AttributeKey.B)
        rtfFormatString.AppendFormat(format, AttributeValue.R, AttributeValue.G, AttributeValue.B)
        rtfFormatString.AppendFormat(format, Tag.R, Tag.G, Tag.B)

        Return rtfFormatString.ToString()

    End Function
End Class
