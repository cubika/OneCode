========================================================================
    ASP.Net 应用程序 : CSASPNETSendMail 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

CSASPNETSendMail示例如何通过System.Net.Mail收发邮件.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

System.Web.Mail 和 System.Net.Mail 都是微软.NET框架自身提供出来的类型. System.Web.Mail 类所有的
.NET框架 都支持. 然而, System.Web.Mail 会在.NET 2.0 之后的版本中被废弃. 在 .Net 2.0中, 
新的名字空间 ‘System.Net.Mail’ 将被引入. 如果你开发的应用是.NET 2.0版本或者之后的版本, 推荐使用
‘System.Web.Mail’.

1. 创建 MailMessage类的一个实例.
	MailMessage NewEmail = new MailMessage();
2. 设置邮件的发送方名称和地址.
	NewEmail.From = new MailAddress(address,displayname);
3. 设置邮件接受方的地址.
   你可以使用NewEmail.To.Add(new MailAddress(to)) 或 
   NewEmail.To = new MailAddressCollection().Add(new MailAddress(to)); 
   可以定义一个收件人,也可以是一个集合.
4. 设置 Bcc 地址 and CC 地址
	NewEmail.Bcc.Add(new MailAddress(bcc));
	NewEmail.CC.Add(new MailAddress(cc));
5. 设置邮件的主题.
	NewEmail.Subject = subject;
6. 设置邮件的主体部分
	NewEmail.Body = body;
7. 设置邮件的附件
	Attachment MsgAttach = new Attachment((mAttachment));
        NewEmail.Attachments.Add(MsgAttach);
8.设置 主体内容是否为HTML
	NewEmail.IsBodyHtml = true;
9. 设置邮件的优先级
	NewEmail.Priority = MailPriority.Normal;
10. 实例化一个SmtpClient对象
        SmtpClient mSmtpClient = new SmtpClient();
11. 发送邮件
        mSmtpClient.Send(NewEmail);
12. 在 web.config 部署邮件的设置 
  <system.net>
    <mailSettings>
      <smtp>
        <network host="smtp host" port="25" userName="username" password="password"/>
      </smtp>
    </mailSettings>

  </system.net>

/////////////////////////////////////////////////////////////////////////////
参考:

Send Mail in ASP.Net Fourm FAQ
http://forums.asp.net/t/1352706.aspx#_What_are_the_2

/////////////////////////////////////////////////////////////////////////////


