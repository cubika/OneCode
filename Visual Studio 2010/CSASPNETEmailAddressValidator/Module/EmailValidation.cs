/****************************** 模块头 ******************************\
* 模块名:    EmailValidation.cs
* 项目名:    CSASPNETEmailAddresseValidator
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何发送一封确认的邮件去检查一个邮箱地址的可用性.
* 
* 在本文件中， 我们创建一个类，主要的逻辑是在确认邮件中验证邮箱地址的可用性.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/


using System;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace CSASPNETEmailAddresseValidator.Module
{

    // 创建一个枚举来返回确认邮件的发送后的状态.
    public enum ValidateEmailResult
    {
        EmailValidated,
        EmailValidating,
        EmailNotValidate,
        EmailStartToValidate
    }

    public class EmailValidation
    {

        // 一些存放在SMTP主机的只读字段.
        // 我们知道，如果我们想发送一封邮件，我们需要一个可以发送到另一个
        // 邮件服务器上的邮件服务器，它可以被称为SMTP服务器.
        // 反之, 可以接收到信息的邮件服务器，我们称之为POP服务器.
        // 这里的"邮件主机"是指SMTP服务器的名称或者是IP地址，“发送邮箱地址” 
        // 是指用来发送确认邮箱的主机地址，“发送邮箱密码”是指邮箱账号的密码，
        // “需要SSL”是指使用一些SMTP需要通过SSL来处理邮件之间交流的邮箱.
        // 例如Hotmail.
        private readonly string mailHost;
        private readonly string sendMailAddress;
        private readonly string sendMailPassword;
        private readonly bool needSSL;

        public EmailValidation(string mailAddress, string password,
                                string hostserver, bool enableSSL)
        {
            mailHost = hostserver;
            sendMailAddress = mailAddress;
            sendMailPassword = password;
            needSSL = enableSSL;
        }

        // 这个方法用于发送确认邮件.
        // 我们使用一个简单的数据库表来存放待验证的邮箱地址信息.
        public ValidateEmailResult StartToValidateEmail(string emailaddress)
        {

            // 使用Linq to SQL来访问数据库.
            using (EmailAddressValidationDataContext context =
                new EmailAddressValidationDataContext())
            {

                // 检查邮箱地址在数据库是否已经有相同的.
                tblEmailValidation eval = context.tblEmailValidations.Where(
                    t => t.EmailAddress == emailaddress).FirstOrDefault();

                if (eval != null)
                {

                    // 如果是，返回验证处理的结果.
                    if (eval.IsValidated)
                    {
                        return ValidateEmailResult.EmailValidated;
                    }
                    else
                    {
                        return ValidateEmailResult.EmailValidating;
                    }
                }

                // 生成一个唯一键来验证地址.
                string querykey = Guid.NewGuid().ToString().Replace("-", "");

                // 发送确认邮件.
                SendValidationEmail(emailaddress, querykey);

                // 如果不是，在数据库中创建一个新的记录.
                context.tblEmailValidations.InsertOnSubmit(new tblEmailValidation()
                {
                    EmailAddress = emailaddress,
                    IsValidated = false,
                    IsSendCheckEmail = false,
                    ValidatingStartTime = DateTime.Now,
                    ValidateKey = querykey
                });
                context.SubmitChanges();

                return ValidateEmailResult.EmailStartToValidate;

            }
        }

        // 这个方法用于取得当前域名，该域名将被拼接为一个网址发送到地址.
        // 用户从确认邮件中点击地址将会触发这个句柄，EmailAvailableValidationHandler, 
        // 最后验证结束时将更新数据库中的记录.
        private string GetDomainURI()
        {
            if (HttpContext.Current == null)
            {
                throw new NullReferenceException("需要网络上下文内容");
            }
            HttpRequest request = HttpContext.Current.Request;
            string rsl = "";
            rsl += request.ServerVariables["HTTPS"] == "on" ? "https://" : "http://";
            rsl += request.ServerVariables["SERVER_NAME"];
            rsl += (request.ServerVariables["SERVER_PORT"] != "80") ?
                    (":" + request.ServerVariables["SERVER_PORT"]) : "";
            return rsl;
        }

        // 这里我们使用SmtpClient和MaillMessage类发送确认邮件.
        private void SendValidationEmail(string address, string querykey)
        {
            using (SmtpClient smtp = new SmtpClient(mailHost))
            {
                MailAddress from = new MailAddress(sendMailAddress, "Confirmation Email");
                MailAddress to = new MailAddress(address);

                using (MailMessage message = new MailMessage(from, to))
                {
                    message.IsBodyHtml = true;
                    message.Subject = "确认邮件";

                    // 在最后一步的验证中我们只发送了一个链接.
                    // 我们也可以创建自己的html样式让邮件看起来更好一些.
                    message.Body = string.Format("<a href='{0}/mail.axd?k={1}'>" +
                        "请点击这里完成邮件验证.</a>",
                        GetDomainURI(), querykey);

                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(sendMailAddress, sendMailPassword);
                    smtp.Port = 25;
                    smtp.EnableSsl = needSSL;
                    smtp.Send(message);
                }
            }
        }

        // 此方法用来重发确认邮件.
        public void ReSendValidationEmail(string address)
        {
            using (EmailAddressValidationDataContext context =
                    new EmailAddressValidationDataContext())
            {

                tblEmailValidation eval = context.tblEmailValidations.Where(
                            t => t.EmailAddress == address).FirstOrDefault();
                if (eval != null)
                {
                    SendValidationEmail(address, eval.ValidateKey);
                }
            }

        }
    }
}