'**************************************** 模块头 *****************************************'
'模块名:      RunCmd.aspx.vb
'项目名:      VBASPNETCMD
'版权 (c) Microsoft Corporation.
'
'RunCmd类反应了用户输入的处理和结果的输出.
'
'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'All other rights reserved.

'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************************'

Imports System.IO

Partial Public Class RunCmd
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

    End Sub


    Protected Sub btnRunBatch_Click(ByVal sender As Object, ByVal e As EventArgs)

        ' 上传文件
        Dim fileName As String = HttpContext.Current.Server.MapPath(
            "Batchs\" & System.Guid.NewGuid().ToString() & "_" & fileUpload.FileName)
        fileUpload.SaveAs(fileName)

        ' 运行这个批处理文件
        Dim output As String = RunBatch(fileName)

        ' 设定结果
        tbResult.Text = output

        ' 删除临时文件
        System.IO.File.Delete(fileName)
    End Sub

    Protected Sub btnRunCmd_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' 根据命令行代码创建批处理文件
        Dim commandLine As String = Me.tbCommand.Text
        Dim fileName As String = HttpContext.Current.Server.MapPath(
            "Batchs\" & System.Guid.NewGuid().ToString() & ".bat")
        Using sw As StreamWriter = System.IO.File.CreateText(fileName)
            sw.Write(commandLine)
            sw.Flush()
        End Using

        ' 运行这个批处理文件
        Dim output As String = RunBatch(fileName)

        ' 设定结果
        tbResult.Text = output

        ' 删除临时文件
        System.IO.File.Delete(fileName)
    End Sub

    ''' <summary>
    ''' 使用BatchRunner类运行批处理文件
    ''' </summary>
    ''' <param name="fileName">批处理文件的全名</param>
    ''' <returns></returns>
    Private Function RunBatch(ByVal fileName As String) As String

        ' 设定批处理执行信息
        Dim batchRunner As New BatchRunner()
        batchRunner.DomainName = tbDomainName.Text.Trim()
        batchRunner.UserName = tbUserName.Text.Trim()
        batchRunner.Password = tbPassword.Text.Trim()

        ' 运行批处理文件
        Dim output As String = batchRunner.ExecuteBatch(fileName)

        Return output
    End Function
End Class