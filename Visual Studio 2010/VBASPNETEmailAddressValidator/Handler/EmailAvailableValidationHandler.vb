'/****************************** 模块头 ******************************\
' 模块名:    EmailAvailableValidationHandler.vb
' 项目名:    VBASPNETEmailAddressValidator
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了如何发送一封确认的邮件去检查一个邮箱地址的可用性.
' 
' 在本文件，我们创建一个HttpHandler用来更新数据库中的记录并且完成验证.
' 我们需要在Web.config文件中注册这个HttpHandler.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'\****************************************************************************

Imports System.Web
Imports System.Web.Configuration
Imports System.Configuration
Imports System.Linq


Public Class EmailAvailableValidationHandler
    Implements IHttpHandler
    Public ReadOnly Property IsReusable() As Boolean _
                            Implements IHttpHandler.IsReusable
        Get
            Return True
        End Get
    End Property


    Public Sub ProcessRequest(ByVal context As HttpContext) _
                            Implements IHttpHandler.ProcessRequest

        ' 取得唯一键与数据库中的所存储的键相比较.
        Dim key As String = context.Request.QueryString("k")

        If Not String.IsNullOrEmpty(key) Then
            Using service As New EmailAddressValidationDataContext
                Dim EValidation As tblEmailValidation =
                    service.tblEmailValidations.Where(Function(t) _
                    t.ValidateKey.Trim() = key).FirstOrDefault()
                If EValidation IsNot Nothing Then

                    ' 通过验证后更新记录.
                    EValidation.IsValidated = True
                    service.SubmitChanges()

                    ' 我们可以定制返回的值并输出.
                    ' 这里只是简单的输出信息.
                    context.Response.Write("恭喜! 你的邮箱地址: " _
                    + EValidation.EmailAddress & "已经验证成功!")
                Else
                    context.Response.Write("请先提交你的邮箱地址.")
                End If
            End Using
        End If
    End Sub



End Class
