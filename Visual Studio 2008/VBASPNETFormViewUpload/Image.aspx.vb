'****************************** 模块头 *******************************
' 模块名:  Image.aspx.vb
' 项目名:      VBASPNETFormViewUpload
' Copyright (c) Microsoft Corporation.
' 
' 本页面从SQL Server数据库中获取一个图片 
' 并将其显示在网页上.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************

#Region "Using directives"
Imports System.Data.SqlClient
#End Region

Partial Public Class Image
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Request.QueryString("PersonID") IsNot Nothing Then
            ' 从Web.config获取链接字符串. 
            ' 当我们使用Using语句时, 
            ' 不需要显式释放代码中的对象, 
            ' using语句会处理他们.
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
                ' 新建一个命令对象.
                Dim cmd As New SqlCommand()

                ' 将数据链接关联到命令.
                cmd.Connection = conn

                ' 设定命令文本
                ' SQL语句或存储过程名. 
                cmd.CommandText = "SELECT Picture FROM Person WHERE PersonID = @PersonID AND Picture IS NOT NULL"

                ' 设定命令类型
                ' CommandType.Text代表原始SQL语句; 
                ' CommandType.StoredProcedure代表存储过程.
                cmd.CommandType = CommandType.Text

                ' 给SqlCommand添加参数并设定其值.
                cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = Request.QueryString("PersonID")

                ' 打开数据链接.
                conn.Open()

                ' 将返回值转换为byte数组.
                Dim bytes As Byte() = DirectCast(cmd.ExecuteScalar(), Byte())

                If bytes IsNot Nothing Then
                    ' 设定输出流HTTP MIME类型.
                    Response.ContentType = "image/jpeg"
                    ' 将一个二进制字符串写入HTTP输出流. 
                    ' 
                    Response.BinaryWrite(bytes)
                    ' 将所有已缓冲输出流发送到客户端. 
                    Response.End()
                End If
            End Using
        End If

    End Sub

End Class