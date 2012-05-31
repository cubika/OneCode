'****************************** 模块头 ***********************************\
' 模块名:    Default.aspx.vb
' 项目名:    VBASPNETLimitDownloadSpeed
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了如何通过编程来限制下载速度. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.IO
Imports System.Threading

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 你可以增加文件的大小来得到一个更长时间的下载期.
        ' 1024 * 1024 * 1 = 1 Mb
        Dim length As Integer = 1024 * 1024 * 1
        Dim buffer As Byte() = New Byte(length - 1) {}

        Dim filepath As String = Server.MapPath("~/bigFileSample.dat")
        Using fs As New FileStream(filepath, FileMode.Create, FileAccess.Write)
            fs.Write(buffer, 0, length)
        End Using
    End Sub

    Protected Sub btnDownload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDownload.Click
        Dim outputFileName As String = "bigFileSample.dat"
        Dim filePath As String = Server.MapPath("~/bigFileSample.dat")

        Dim value As String = ddlDownloadSpeed.SelectedValue

        ' 1024 * 20 = 20 Kb/s.
        Dim downloadSpeed As Integer = 1024 * Integer.Parse(value)
        Response.Clear()

        ' 调用DownloadFileWithLimitedSpeed方法来下载文件.
        Try
            DownloadFileWithLimitedSpeed(outputFileName, filePath, downloadSpeed)
        Catch ex As Exception
            Response.Write("<p><font color=""red"">")
            Response.Write(ex.Message)
            Response.Write("</font></p>")
        End Try
        Response.End()
    End Sub

    Public Sub DownloadFileWithLimitedSpeed(ByVal fileName As String, ByVal filePath As String, ByVal downloadSpeed As Long)
        If Not File.Exists(filePath) Then
            Throw New Exception("Err: There is no such a file to download.")
        End If

        ' 获得文件的一个BinaryReader实例来下载. 
        Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            Using br As New BinaryReader(fs)
                Response.Buffer = False

                ' 文件长度.
                Dim fileLength As Long = fs.Length

                ' 包最小为 1024 = 1 Kb.
                Dim pack As Integer = 1024

                ' 初始的公式是: sleep = 1000 / (下载速度/ 包)
                ' 它等于1000.0 * 包 / 下载速度.
                ' 这里的1000.0表示1000毫秒 = 1秒. 
                Dim sleep As Integer = CInt(Math.Truncate(Math.Ceiling(1000.0 * pack / downloadSpeed)))

                ' 设置当前响应的头部.
                Response.AddHeader("Content-Length", fileLength.ToString())
                Response.ContentType = "application/octet-stream"

                Dim utf8EncodingFileName As String = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)
                Response.AddHeader("Content-Disposition", "attachment;filename=" & utf8EncodingFileName)

                ' maxCount表示线程发送的文件包的总数. 
                Dim maxCount As Integer = CInt(Math.Truncate(Math.Ceiling(Convert.ToDouble(fileLength) / pack)))

                For i As Integer = 0 To maxCount - 1
                    If Response.IsClientConnected Then
                        Response.BinaryWrite(br.ReadBytes(pack))

                        ' 在响应线程发送一个文件包以后让它进入休眠状态.
                        Thread.Sleep(sleep)
                    Else
                        Exit For
                    End If
                Next
            End Using
        End Using
    End Sub

End Class