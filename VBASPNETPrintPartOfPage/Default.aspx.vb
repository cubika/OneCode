'********************************** 模块头 **********************************\
' 模块名: Default.aspx.vb
' 项目名: VBASPNETPrintPartOfPage
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了如何用层叠样式表和JavaScript打印部分页面.
' 我们需要在部分网页上设置div层,并使用JavaScript代码来
' 打印它的有用部分.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/


Public Class _Default
    Inherits System.Web.UI.Page
    ' 定义一些字符串,用来添加div元素.
    Public PrintImageBegin As String
    Public PrintImageEnd As String
    Public PrintListBegin As String
    Public PrintListEnd As String
    Public PrintButtonBegin As String
    Public PrintButtonEnd As String
    Public PrintSearchBegin As String
    Public PrintSearchEnd As String
    Public Const EnablePirnt As String = "<div class=nonPrintable>"
    Public Const EndDiv As String = "</div>"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 检查文本框的状态,设置div元素.
        If chkDisplayImage.Checked Then
            PrintImageBegin = String.Empty
            PrintImageEnd = String.Empty
        Else
            PrintImageBegin = EnablePirnt
            PrintImageEnd = EndDiv
        End If
        If chkDisplayList.Checked Then
            PrintListBegin = String.Empty
            PrintListEnd = String.Empty
        Else
            PrintListBegin = EnablePirnt
            PrintListEnd = EndDiv
        End If
        If chkDisplayButton.Checked Then
            PrintButtonBegin = String.Empty
            PrintButtonEnd = String.Empty
        Else
            PrintButtonBegin = EnablePirnt
            PrintButtonEnd = EndDiv
        End If
        If chkDisplaySearch.Checked Then
            PrintSearchBegin = String.Empty
            PrintSearchEnd = String.Empty
        Else
            PrintSearchBegin = EnablePirnt
            PrintSearchEnd = EndDiv
        End If
    End Sub

    Protected Sub btnPrint_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrint.Click
        ClientScript.RegisterStartupScript(Me.[GetType](), "PrintOperation", "print()", True)
    End Sub
End Class