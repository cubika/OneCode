'*************************** 模块头 ******************************'
' 模块名:    HtmlSubmit.vb
' 项目名:	    VBWebBrowserAutomation
' 版权 (c) Microsoft Corporation.
' 
' 这个HtmlSubmit类代表一个带有“submit”类型的“输入标签”的HtmlElement。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Security.Permissions

Public Class HtmlSubmit
    Inherits HtmlInputElement

    ''' <summary>
    ''' 这个无参数构造函数用于反序列化。
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' 初始化一个HtmlSubmit的实例。这个构造函数是用于 
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub New(ByVal element As HtmlElement)
        MyBase.New(element.Id)
    End Sub

End Class