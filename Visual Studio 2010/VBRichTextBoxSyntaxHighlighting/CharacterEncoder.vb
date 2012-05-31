'*************************** 模块头 ******************************'
' 模块名:  XmlTextEncoder.vb
' 项目名:	    VBRichTextBoxSyntaxHighlighting
' 版权 (c) Microsoft Corporation.
' 
' CharacterEncoder类提供一个Static方法，去编码在XML和RTF中的一个特殊的字符，例如 '<', '>', '"', '&', ''', '\',
'  '{' and '}' 
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

Public Class CharacterEncoder
    Public Shared Function Encode(ByVal originalText As String) As String
        If String.IsNullOrWhiteSpace(originalText) Then
            Return String.Empty
        End If

        Dim encodedText As New StringBuilder()
        For i As Integer = 0 To originalText.Length - 1
            Select Case originalText.Chars(i)
                Case """"c
                    encodedText.Append("&quot;")
                Case "&"c
                    encodedText.Append("&amp;")
                Case "'"c
                    encodedText.Append("&apos;")
                Case "<"c
                    encodedText.Append("&lt;")
                Case ">"c
                    encodedText.Append("&gt;")

                    ' 字符 '\' 应该被转换为 "\\" 
                Case "\"c
                    encodedText.Append("\\")

                    ' 字符 '{' 应该被转换为 "\{" 
                Case "{"c
                    encodedText.Append("\{")

                    ' 字符 '}' 应该被转换为 "\}" 
                Case "}"c
                    encodedText.Append("\}")

                Case Else
                    encodedText.Append(originalText.Chars(i))
            End Select

        Next i
        Return encodedText.ToString()
    End Function
End Class
