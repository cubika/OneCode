=============================================================================
                     CSASPNETAutoLogin 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
总结:

本项目阐述了怎样用HttpWebRequest类使用户不用填写用户名和密码来登录 
网站.


/////////////////////////////////////////////////////////////////////////////
演示

直接单击CSASPNETAutoLogin.sln并且打开CSASPNETAutoLogin网站来直  
接测试页面.

如果你还想更进一步的测试，请参考下面的演示步骤.

步骤1: 在Login.aspx页面，你填写用户名和密码，并且单击登录按钮.
页面将会提示你已经登录这个页面，并且显示你的名字和密码,这证明
你已经成功登录这个页面，这是我们登录一个站点的常用方法.

步骤2: 在AutoLogin.aspx页面，你也要填写你的用户名和密码，并且 
单击自动登录按钮，页面将会显示你的用户名和密码.但是在后台代码
中，我们使用httpwebrequest向login.aspx而不是当前的页面来提交
当前的用户名和密码. 它在login.aspx页面显示相同信息，它证明了
我们成功向login.aspx提交autologin.aspx页面的当前的用户名和
密码.


/////////////////////////////////////////////////////////////////////////////
代码演示:

步骤1. 我们通过使用HttpWebRequest请求login.aspx获取方法和返回
登录页面的html代码. 

步骤2. 我们使用登录页面的html代码子串来获取__VIEWSTATE,__EVENTVALIDATION
参数.

步骤3. 我们把__VIEWSTATE,__EVENTVALIDATION,用户名,密码和登录按 
钮合并成完整的字符串并把它转换成字节数组.

步骤4. 提交请求数据获取返回值并且显示它.

 

/////////////////////////////////////////////////////////////////////////////
参考文献:

MSDN: HttpWebRequest and HttpWebResponse Class
http://msdn.microsoft.com/zh-cn/library/system.net.httpwebrequest.aspx
http://msdn.microsoft.com/zh-cn/library/system.net.httpwebresponse.aspx

MSDN: http://msdn.microsoft.com/zh-cn/library/debx8sh9.aspx
http://msdn.microsoft.com/zh-cn/library/debx8sh9.aspx

Automatic Login into a website in C#.net
http://forums.asp.net/t/1507150.aspx

How to use HttpWebRequest to send POST request to another web server
http://www.netomatix.com/httppostdata.aspx


/////////////////////////////////////////////////////////////////////////////