'****************************** 模块头 **************************************\
'* 模块名:             HighlightCodePage.aspx.vb
'* 项目名:        VBASPNETHighlightCodeInPage
'* 版权(c) Microsoft Corporation
'*
'* 这个页面用以使用户高亮它的代码. 
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
'* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
'* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************


Public Class HighlightCodePage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Me.Load

        Me.lbError.Visible = False
        Me.lbResult.Visible = False
    End Sub

    Protected Sub btnHighLight_Click(ByVal sender As Object, _
                                     ByVal e As EventArgs)
        Dim _error As String = String.Empty

        ' 检查用户输入的数据.
        If CheckControlValue(Me.ddlLanguage.SelectedValue,
                             Me.tbCode.Text, _error) Then
            ' 初始化根据匹配选项用来保存不同语言代码
            ' 及其相关正则表达式的散列表变量.
            Dim _htb As Hashtable = CodeManager.Init()

            ' 初始化合适的集合对象.
            Dim _rg As New RegExp()
            _rg = DirectCast(_htb(Me.ddlLanguage.SelectedValue), RegExp)
            Me.lbResult.Visible = True
            If Me.ddlLanguage.SelectedValue <> "html" Then
                ' 在标签控件中显示高亮的代码.
                Me.lbResult.Text = CodeManager.Encode(
                    CodeManager.HighlightCode(
                        Server.HtmlEncode(
                            Me.tbCode.Text).
                        Replace("&quot;", """").
                        Replace("&#39;", "'"),
                        Me.ddlLanguage.SelectedValue, _rg))
            Else
                ' 在标签控件中显示高亮的代码.
                Me.lbResult.Text = CodeManager.Encode(
                    CodeManager.HighlightHTMLCode(Me.tbCode.Text, _htb))
            End If
        Else
            Me.lbError.Visible = True
            Me.lbError.Text = _error
        End If
    End Sub
    Public Function CheckControlValue(ByVal selectValue As String, _
                                      ByVal inputValue As String, _
                                      ByRef [error] As String) As Boolean
        [error] = String.Empty
        If selectValue = "-1" Then
            [error] = "请选择语言."
            Return False
        End If
        If String.IsNullOrEmpty(inputValue) Then
            [error] = "请粘贴您的代码到文本框控件."
            Return False
        End If
        Return True
    End Function


End Class