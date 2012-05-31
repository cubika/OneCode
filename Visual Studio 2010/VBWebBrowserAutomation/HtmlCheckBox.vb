'*************************** 模块头 ******************************'
' 模块名:    HtmlCheckBox.vb
' 项目名:	    VBWebBrowserAutomation
' 版权(c) Microsoft Corporation.
' 
' 这个HtmlCheckBox类代表带有一个“checkbox”类型的“输入标签”的HtmlElement。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************'

Imports System.Security.Permissions

Public Class HtmlCheckBox
    Inherits HtmlInputElement

    Public Property Checked() As Boolean

    ''' <summary>
    ''' 这个无参数构造函数用于反序列化。
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 初始化一个HtmlCheckBox的实例。 使用构造函数HtmlInputElementFactory。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New(ByVal element As HtmlElement)
        MyBase.New(element.Id)

        ' 如果checkbox的有属性是“checked”它将被检查。
        Dim chekced As String = element.GetAttribute("checked")
        Checked = Not String.IsNullOrEmpty(chekced)
    End Sub

    ''' <summary>
    ''' 设置HtmlElement的值.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Overrides Sub SetValue(ByVal element As HtmlElement)
        '  如果checkbox的有属性是“checked”它将被检查。
        If Checked Then
            element.SetAttribute("checked", "checked")
        Else
            element.SetAttribute("checked", Nothing)
        End If
    End Sub

End Class
