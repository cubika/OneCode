'*************************** 模块头 ******************************'
' 模块名:    HtmlInputElementFactory.vb
' 项目名:	    VBWebBrowserAutomation
' 版权 (c) Microsoft Corporation.
' 
' 这个HtmlInputElementFactory类是用来在网页上的HtmlElement获得一个HtmlInputElement的。
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

Public NotInheritable Class HtmlInputElementFactory

    Private Sub New()
    End Sub

    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Shared Function GetInputElement(ByVal element As HtmlElement) As HtmlInputElement
        If Not element.TagName.Equals("input", StringComparison.OrdinalIgnoreCase) Then
            Return Nothing
        End If

        Dim input As HtmlInputElement = Nothing

        Dim type As String = element.GetAttribute("type").ToLower()

        Select Case type
            Case "checkbox"
                input = New HtmlCheckBox(element)
            Case "password"
                input = New HtmlPassword(element)
            Case "submit"
                input = New HtmlSubmit(element)
            Case "text"
                input = New HtmlText(element)
            Case Else

        End Select
        Return input
    End Function
End Class
