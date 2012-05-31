'/****************************** 模块头 ******************************\
' 模块名:    EmailValidation.vb
' 项目名:    VBASPNETEmailAddressValidator
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了如何发送一封确认的邮件去检查一个邮箱地址的可用性.
' 
' 在本文件中， 我们创建一个类，主要的逻辑是在确认邮件中验证邮箱地址的可用性.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'\*****************************************************************************/


Imports System.Linq
Imports System.Web
Imports System.Net.Mail
Imports System.Net


' 创建一个枚举来返回确认邮件的发送后的状态.
Public Enum ValidateEmailResult
    EmailValidated
    EmailValidating
    EmailNotValidate
    EmailStartToValidate
End Enum

Public Class EmailValidation

    ' 一些存放在SMTP主机的只读字段.
    ' 我们知道，如果我们想发送一封邮件，我们需要一个可以发送到另一个
    ' 邮件服务器上的邮件服务器，它可以被称为SMTP服务器.
    ' 反之, 可以接收到信息的邮件服务器，我们称之为POP服务器.
    ' 这里的"邮件主机"是指SMTP服务器的名称或者是IP地址，“发送邮箱地址” 
    ' 是指用来发送确认邮箱的主机地址，“发送邮箱密码”是指邮箱账号的密码，
    ' “需要SSL”是指使用一些SMTP需要通过SSL来处理邮件之间交流的邮箱.
    ' 例如Hotmail.
    Private ReadOnly mailHost As String
    Private ReadOnly sendMailAddress As String
    Private ReadOnly sendMailPassword As String
    Private ReadOnly needSSL As Boolean

    Public Sub New(ByVal sendAddress As String, _
                   ByVal password As String, _
                   ByVal hostserver As String, _
                   ByVal enableSSL As Boolean)

        mailHost = hostserver
        sendMailAddress = sendAddress
        sendMailPassword = password
        needSSL = enableSSL
    End Sub

    ' 这个方法用于发送确认邮件.
    ' 我们使用一个简单的数据库表来存放待验证的邮箱地址信息.
    Public Function StartToValidateEmail(ByVal emailaddress As String) _
                                                As ValidateEmailResult

        ' 使用Linq to SQL来访问数据库.
        Using context As New EmailAddressValidationDataContext

            ' 检查邮箱地址在数据库是否已经有相同的.
            Dim eval As tblEmailValidation =
                context.tblEmailValidations.Where(Function(t) _
                  t.EmailAddress = emailaddress).FirstOrDefault()

            If eval IsNot Nothing Then

                ' 如果是，返回验证处理的结果.
                If eval.IsValidated Then
                    Return ValidateEmailResult.EmailValidated
                Else
                    Return ValidateEmailResult.EmailValidating
                End If
            End If

            ' 生成一个唯一键来验证地址.
            Dim querykey As String =
                Guid.NewGuid().ToString().Replace("-", "")

            ' 发送确认邮件.
            SendValidationEmail(emailaddress, querykey)

            ' 如果不是，在数据库中创建一个新的记录.
            context.tblEmailValidations.InsertOnSubmit(
                        New tblEmailValidation() With { _
                           .EmailAddress = emailaddress, _
                           .IsValidated = False, _
                           .IsSendCheckEmail = False, _
                           .ValidatingStartTime = DateTime.Now, _
                           .ValidateKey = querykey _
                          })
            context.SubmitChanges()


            Return ValidateEmailResult.EmailStartToValidate
        End Using
    End Function

    ' 这个方法用于取得当前域名，该域名将被拼接为一个网址发送到地址.
    ' 用户从确认邮件中点击地址将会触发这个句柄，EmailAvailableValidationHandler, 
    ' 最后验证结束时将更新数据库中的记录.
    Private Function GetDomainURI() As String
        If HttpContext.Current Is Nothing Then
            Throw New NullReferenceException("需要网络上下文内容")
        End If
        Dim request As HttpRequest = HttpContext.Current.Request
        Dim rsl As String = ""
        rsl += If(request.ServerVariables("HTTPS") = "on", "https://", "http://")
        rsl += request.ServerVariables("SERVER_NAME")
        rsl += If((request.ServerVariables("SERVER_PORT") <> "80"),
                  (":" & request.ServerVariables("SERVER_PORT")), "")
        Return rsl
    End Function

    ' 这里我们使用SmtpClient和MaillMessage类发送确认邮件.
    Private Sub SendValidationEmail(ByVal address As String, ByVal querykey As String)

        Using smtp As New SmtpClient(mailHost)
            Dim from As New MailAddress(sendMailAddress, "Confirmation Email")
            Dim [to] As New MailAddress(address)

            Using message As New MailMessage(from, [to])
                message.IsBodyHtml = True
                message.Subject = "确认邮件"

                ' 在最后一步的验证中我们只发送了一个链接.
                ' 我们也可以创建自己的html样式让邮件看起来更好一些.
                message.Body = String.Format("<a href='{0}/mail.axd?k={1}'>" &
                     "请点击这里完成邮件验证.</a>",
                     GetDomainURI(),
                     querykey)

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network
                smtp.UseDefaultCredentials = False
                smtp.Credentials = New NetworkCredential(sendMailAddress,
                                                         sendMailPassword)
                smtp.Port = 25
                smtp.EnableSsl = needSSL
                smtp.Send(message)
            End Using
        End Using

    End Sub

    ' 此方法用来重发确认邮件.
    Public Sub ReSendValidationEmail(ByVal address As String)
        Using context As New EmailAddressValidationDataContext()

            Dim eval As tblEmailValidation =
                context.tblEmailValidations.Where( _
                    Function(t) t.EmailAddress = address).FirstOrDefault()
            If eval IsNot Nothing Then
                SendValidationEmail(address, eval.ValidateKey)
            End If
        End Using

    End Sub
End Class
