========================================================================
            CSASPNETEmailAddressValidator 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

本项目阐述了如何发送一封确认邮件来检查邮箱地址的可用性.

/////////////////////////////////////////////////////////////////////////////
代码演示:

请根据以下步骤来测试本项目.

步骤 1: 打开CSASPNETEmailAddressValidator.sln.

步骤 2: 展开CSASPNETEmailAddressValidator网站的结点并按下 
        Ctrl + F5键浏览Default.aspx页面.

步骤 3: 我们将会看到一个Wizard控件. 第一步, 输入一个SMTP服务器名称或者是IP地址
        和邮箱地址和它的密码. 我们需要使用它来发送确认邮件.在这个示例中, 我们使
		用hotmail, 所以如果你有一个hotmail的邮箱, 你可以输入你的hotmail邮箱地
		址和密码然后点击下一步按钮.

步骤 4: 第二步，输入一个需要验证的邮箱地址. 当然, 你需要看到邮件处理到下一步.

步骤 5: 打开你所收到的邮件. 点击链接或者把它复制到浏览器的地址栏浏览, 你会看到
        一封“祝贺”的信息.

步骤 6: 验证结束.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤 1.  在Visual Studio 2010或者是Visual Web Developer 2010中新建一个C# 
         "ASP.NET空网络应用程序". 命名为"CSASPNETEmailAddressValidator".

步骤 2.  新建两个文件夹, "Handler"和"Module".

步骤 3.  在App_Data文件中创建一个数据库文件并且新建一个表,命名为"tblEmailValidation".
         按以下内容添加6列字段:
		 id: 表的自增列;
		 EmailAddress: 存储需要验证的邮箱地址;
		 IsValidated: 存储判断验证是否完成的一个字段.
		 IsSendCheckEmail: 存储判断是否发送过邮件的一个字段.
		 ValidatingStartTime: 存储验证开始的时间.
		 ValdateKey: 用来区分不同验证链接的唯一键.

步骤 4.  在Module文件夹中新建一个Linq to SQL类，命名为EmailAddressValidation.dbml.
         打开服务器管理选项卡中, 创建一个EmailValidationDB.mdf文件的链接，并且
		 把tblEmailValidation拖到视图中并且在EmailAddressValidation.dbml文件
		 桌面上删除. 然后生成项目.

步骤 5.  新建一个类文件, 命名为EmailValidation.cs. 按照示例写出代码, 我们可以在
         示例文件的注释中找到更多的细节.
         

步骤 6.  在Handler文件夹中新建一个HttpHandler. 按照示例写出代码, 我们可以在示例
         文件中的注释中找到更多的细节.
         
         
步骤 7.  把Webform1.aspx名称改为Default.aspx并且在上面新建一个Wizard控件.根据
         示例完成编码.
         
步骤 8.  打开Default.aspx.cs文件. 根据示例写出代码. 你可以可以在示例
         文件中的注释中找到更多的细节.

步骤 9.  打开web.config文件. 参考以下信息注册一个HttpHandler.
		 [Code]
		 <httpHandlers>
             <add path="mail.axd" verb="GET" validate="false" type="CSASPNETEmailAddresseValidator.Handler.EmailAvailableValidationHandler" />
         </httpHandlers>
         [/Code]  

步骤 10. 生成应用程序, 你可以开始调试了.


/////////////////////////////////////////////////////////////////////////////
参考信息:

MSDN: SmtpClient 类
http://msdn.microsoft.com/zh-cn/library/system.net.mail.smtpclient.aspx

MSDN: SQL-CLR 类型映射(LINQ to SQL)
http://msdn.microsoft.com/zh-cn/library/bb386947.aspx

MSDN: HttpHandlers
http://msdn.microsoft.com/zh-cn/library/5c67a8bd(VS.71).aspx

/////////////////////////////////////////////////////////////////////////////