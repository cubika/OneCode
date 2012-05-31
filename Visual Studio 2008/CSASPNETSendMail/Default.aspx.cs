using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void SendEmail(string from, string to, string bcc, string cc, string subject, string body, string mAttachment)
    {

        // 创建 MailMessage类的一个实例.
        MailMessage NewEmail = new MailMessage();

        #region From
        // 设置邮件的发送方名称和地址.
        NewEmail.From = new MailAddress(from,from);
        #endregion

        #region To
        // 设置邮件接受方的地址.
        NewEmail.To.Add(new MailAddress(to));
        // You can also use NewEmail.To = new MailAddressCollection().Add(new MailAddress(to)); to set the collection
        #endregion

        #region BCC
        // 设置 Bcc 地址
        NewEmail.Bcc.Add(new MailAddress(bcc));
        #endregion
        
        #region CC
        // 检查cc的值是否为空或则是空字符串
        if ((cc != null) && (cc != string.Empty))
        {
            // 设置  CC 地址
            NewEmail.CC.Add(new MailAddress(cc));
        }
        #endregion

        #region Subject
        // 设置邮件的主题.
        NewEmail.Subject = subject;
        #endregion

        #region Body
        // 设置邮件的主体部分
        NewEmail.Body = body;
        #endregion

        #region Attachment
        Attachment MsgAttach = new Attachment((mAttachment));
        NewEmail.Attachments.Add(MsgAttach);
        #endregion

        #region Deployment
        // 指定邮件的格式化类型为HTML
        NewEmail.IsBodyHtml = true;
        // 设置邮件的优先级为普通
        NewEmail.Priority = MailPriority.Normal;
        #endregion


        // 实例化一个SmtpClient对象
        SmtpClient mSmtpClient = new SmtpClient();

        // 发送邮件
        mSmtpClient.Send(NewEmail);


    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string from = "SenderAddress";
        string to = "RecepientAddress";
        string bcc = "BCCAddress";
        string cc = "CCAddress";
        string subject = "TestMail";
        string body = "TestBody";
        string mAttachment = "FileURL";
        SendEmail(from, to, bcc, cc, subject, body, mAttachment);


    }
}
