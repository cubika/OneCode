'*************************** 模块头 ******************************'
' 模块名:  MainForm.vb
' 项目名:	    VBRichTextBoxSyntaxHighlighting
' 版权 (c) Microsoft Corporation.
' 
' 这是应用程序的Main窗口。他是用来初始化UI界面和处理事件。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Partial Public Class MainForm
    Inherits Form

    Public Sub New()
        InitializeComponent()

        ' 初始化 XMLViewerSettings.
        Dim viewerSetting As XMLViewerSettings =
            New XMLViewerSettings With {
                .AttributeKey = Color.Red,
                .AttributeValue = Color.Blue,
                .Tag = Color.Blue,
                .Element = Color.DarkRed,
                .Value = Color.Black}

        viewer.Settings = viewerSetting

    End Sub

    ''' <summary>
    ''' 处理按钮的 "btnProcess"单击事件.
    ''' </summary>
    Private Sub btnProcess_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnProcess.Click
        Try
            viewer.Process(True)
        Catch appException As ApplicationException
            MessageBox.Show(appException.Message, "应用程序异常")
        Catch ex As Exception
            MessageBox.Show(ex.Message, "异常")
        End Try

    End Sub

End Class
