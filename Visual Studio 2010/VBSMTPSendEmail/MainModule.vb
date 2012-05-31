'****************************** 模块头 ************************************\
'模块:	    MainModule.vb
'项目:		VBSMTPSendEmail
'版权 (c) Microsoft Corporation.
' 
' VBSMTPSendEmail利用VB.NET演示了如何通过SMTP服务器，发送包含有附件和图片的邮件。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

#Region "Imports directives"
Imports System.Net.Mail
Imports System.Net
#End Region


Module MainModule

    Sub Main()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 创建邮件对象。
        ' 

        Console.WriteLine("创建邮件对象")
        Dim mail As New MailMessage
        mail.To.Add("anyreceiver@anydomain.com")
        mail.From = New MailAddress("anyaddress@anydomain.com")
        mail.Subject = "Test email of All-In-One Code Framework - VBSMTPSendEmail"
        mail.Body = "Welcome to <a href='http://cfx.codeplex.com'>All-In-One Code Framework</a>!"
        mail.IsBodyHtml = True

        ' 附件
        Console.WriteLine("添加附件")
        Dim attachedFile As String = "<attached file path>"
        mail.Attachments.Add(New Attachment(attachedFile))

        ' 在消息体中嵌入图片。
        Console.WriteLine("嵌入图片")
        mail.Body += "<br/><img alt="""" src=""cid:image1"">"

        Dim imgFile As String = "<image file path>"
        Dim htmlView As AlternateView = _
        AlternateView.CreateAlternateViewFromString(mail.Body, Nothing, _
                                                    "text/html")
        Dim imgLink As LinkedResource = New LinkedResource(imgFile, _
                                                           "image/jpg")
        imgLink.ContentId = "image1"
        imgLink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64
        htmlView.LinkedResources.Add(imgLink)
        mail.AlternateViews.Add(htmlView)


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 配置SMTP客户端并发送邮件。
        ' 

        ' 配置SMTP客户端
        Dim smtp As New SmtpClient
        smtp.Host = "smtp.live.com"
        smtp.Credentials = New NetworkCredential( _
        "myaccount@live.com", "mypassword")
        smtp.EnableSsl = True

        ' 发送邮件
        Console.WriteLine("发送邮件...")
        smtp.Send(mail)

    End Sub

End Module
