'/**************************************** 模块头 *****************************************\
'* 模块名:    ImageHandler.ashx
'* 项目名:    VBASPNETImagEditUpload
'* 版权 (c) Microsoft Corporation
'*
'* 这是一个常用从特定数据库中根据指定Id记录读取字节集合的http-handler.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\*****************************************************************************************/

Imports System.Web
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging


Public Class ImageHandler
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest


        Using cmd As New SqlCommand()

            cmd.Connection = New SqlConnection(
                ConfigurationManager.ConnectionStrings("db_PersonsConnectionString") _
                .ConnectionString)
            cmd.Connection.Open()
            cmd.CommandText = "select PersonImage,PersonImageType from tb_personInfo" _
               + " where id=" + context.Request.QueryString("id")

            Dim reader As SqlDataReader = cmd.ExecuteReader(
                CommandBehavior.CloseConnection Or CommandBehavior.SingleRow)
            If (reader.Read) Then
                Dim imgbytes() As Byte = Nothing
                Dim imgtype As String = Nothing

                If (reader.GetValue(0) IsNot DBNull.Value) Then
                    imgbytes = CType(reader.GetValue(0), Byte())
                    imgtype = reader.GetString(1)

                    ' 如果是bmp,因为格式不同先转换成jpg然后显示.

                    If (imgtype.Equals("image/bmp", StringComparison.OrdinalIgnoreCase)) Then
                        Using ms As New MemoryStream(imgbytes)
                            Using bt As New Bitmap(ms)
                                bt.Save(context.Response.OutputStream, ImageFormat.Jpeg)
                            End Using
                        End Using
                    Else
                        context.Response.ContentType = imgtype
                        context.Response.BinaryWrite(imgbytes)
                    End If

                Else
                    imgbytes = File.ReadAllBytes(
                        context.Server.MapPath("~/DefaultImage/DefaultImage.JPG"))
                    imgtype = "image/pjpeg"
                End If
                context.Response.ContentType = imgtype
                context.Response.BinaryWrite(imgbytes)
            End If

            reader.Close()
            context.Response.End()
        End Using


    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class