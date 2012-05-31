/****************************** 模块头 ************************************\
* 模块:	    Program.cs
* 项目:		CSSMTPSendEmail
* 版权 (c) Microsoft Corporation.
* 
* CSSMTPSendEmail利用C#演示了如何通过SMTP服务器，发送包含有附件和图片的邮件。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Net.Mail;
using System.Net;
#endregion


class Program
{
    static void Main(string[] args)
    {
        /////////////////////////////////////////////////////////////////////
        // 创建邮件对象。
        // 

        Console.WriteLine("创建邮件对象。");

        MailMessage mail = new MailMessage();
        mail.To.Add("anyreceiver@anydomain.com");
        mail.From = new MailAddress("anyaddress@anydomain.com");
        mail.Subject = "Test email of All-In-One Code Framework - CSSMTPSendEmail";
        mail.Body = "Welcome to <a href='http://cfx.codeplex.com'>All-In-One Code Framework</a>!";
        mail.IsBodyHtml = true;

        // 附件
        Console.WriteLine("添加附件");
        string attachedFile = "<attached file path>";
        mail.Attachments.Add(new Attachment(attachedFile));

        // 在消息体中嵌入图片。
        Console.WriteLine("嵌入图片");
        mail.Body += "<br/><img alt=\"\" src=\"cid:image1\">";

        string imgFile = "<image file path>";
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
            mail.Body, null, "text/html");
        LinkedResource imgLink = new LinkedResource(imgFile, "image/jpg");
        imgLink.ContentId = "image1";
        imgLink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
        htmlView.LinkedResources.Add(imgLink);
        mail.AlternateViews.Add(htmlView);


        /////////////////////////////////////////////////////////////////////
        // 配置SMTP客户端并发送邮件.
        // 

        // 配置SMTP客户端
        SmtpClient smtp = new SmtpClient();
        smtp.Host = "smtp.live.com";
        smtp.Credentials = new NetworkCredential(
            "myaccount@live.com", "mypassword");
        smtp.EnableSsl = true;

        // 发送邮件
        Console.WriteLine("发送邮件...");
        smtp.Send(mail);
    }
}