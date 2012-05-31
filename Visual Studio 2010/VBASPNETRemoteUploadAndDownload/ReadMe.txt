========================================================================
  ASP.NET 应用程序 : VBASPNETRemoteUploadAndDownload 概述
========================================================================
/////////////////////////////////////////////////////////////////////////////
用法:

VBASPNETRemoteUploadAndDownload示例演示在ASP.NET中如何上传下载文件自远程服务器. 

本示例建立于使用VB语言中的WebClient和FtpWebRequest对象. 

WebClient和FtpWebRequest类都提供常用方法来发送数据到服务器URI.
同时接受来自由URI定义的资源的数据.

当上传和下载文件时, 这些类会提交webrequest到用户输入的url.

UploadData()方法通过HTTP或FTP发送一个数据缓冲(未编码)到以方法参数指定的资源,
然后返回服务器的web响应. 相应的, DownloadData()方法请求一个HTTP
或FTP下载方法到远程服务器来获得服务器的输出流.

/////////////////////////////////////////////////////////////////////////////

前提条件:

1. 远程服务器虚拟上传路径需要启用Asp.net 账户或者其他可访问用户账户的写权限,
 也就是说服务器对应目录可写.

否则远程服务器会返回403错误(关于服务器状态代码定义, 
 参照http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html).

2. 如果远程服务器(http或ftp)没有允许匿名访问. 
 则我们需要提供一个匿名帐号并设定WebClient.Credentials属性用来验证用户, 让服务器知道你是谁.
 (下列文档也会提及).

3. 注意:
	如果服务器运行于IIS7, 需要对用户权限做更多步设定过程.
    安装IIS7用WebDev扩展. 请根据下列文章说明.
	http://learn.iis.net/page.aspx/350/installing-and-configuring-webdav-on-iis-7/

	如果服务器没有 WebDev扩展程序, 我们也可以从下列连接下载
  	32bit: 
	http://go.microsoft.com/fwlink/?LinkID=141805
	
	64bit:
	http://go.microsoft.com/fwlink/?LinkID=141807

	有时如果我们未安装或启用这个扩展服务, IIS7 会禁止 
	RemoteUpload方法并返回404或403 http错误代码.

/////////////////////////////////////////////////////////////////////////////

开始演示:

1.打开VBRemoteUploadAndDownload.sln文件.

2.右击RemoteFileForm.aspx, 选择"在浏览器中查看"或者在浏览器中浏览"RemoteFileForm.aspx"
  以运行这个应用程序.

3.你可以看到RemoteFileForm.aspx页面两项功能.

  RemoteUpload:
    1) 输入你想上传的服务器url地址. 
		(e.g. http://www.example.com/upload/, ftp://ftpserver/)
    2) 使用UploadFile控件选择你要上传的本地文件.
    3) 单击上传按钮然后你可以在屏幕上看到结果.

  RemoteDownload:
    1) 输入你想下载的服务器url地址. 
		(e.g. http://www.example.com/download/test.txt, ftp://ftpserver/test.txt)
    2) 输入保存文件的目标本地路径或目录. (e.g. C:\Temp\)
    3) 单击下载按钮然后你可以在屏幕上看到结果.
	
/////////////////////////////////////////////////////////////////////////////

代码逻辑:

在.NET应用程序中使用WebClient和FtpWebRequest上传,下载数据.
注意在开头添加System.IO,System.Net的引用g.


上传:
RemoteUpload 类包含一些必要的属性和架构函数,将在UploadFile方法中使用.

UrlString: 用以接收数据的URI资源
FileNamePath : 上传文件的完整物理路径.
NewFileName: 远程服务器上传文件名.

1. 获得FileNamePath属性的文件名.

2. 获得上传文件数据的字节数组.

3. 获得正确的远程服务器url.

4. Http: 
   创建WebClient(导入命名空间System.Net的引用)实例,
   使用DefaultCredentials发送到主机作为网络身份验证并使用这个账户作为请求验证,
   即使用DefaultCredentials登入远程服务器(关于WebClient.Credentials属性.
   MSDN: http://msdn.microsoft.com/en-us/library/system.net.webclient.credentials.aspx 
         http://msdn.microsoft.com/en-us/library/system.net.networkcredential(VS.80).aspx)
   FTP:
   创建FtpWebRequest实例. 使用WebRequestMethods.Ftp.UploadFile方法上传文件.
   在本示例中,Ftp服务器可以被匿名用户访问,所以我们不需要指定用户名和密码.
   如果服务器在连接前需要验证用户.我们也需要提供用户名和密码. (关于FtpWebRequest.Credentials属性
   MSDN: http://msdn.microsoft.com/en-us/library/354e0act(VS.100).aspx)


5. 获得存储在内存中的上传文件字节(FileStream).

6. 通过使用HTTP方法"PUT"上传数据缓冲到远程服务器. 如果请求方法为空, 默认值是对http服务器为"POST"以及对ftp服务器为"STOR".

7. UploadData()方法返回上传是否成功的布尔值.

8. 关闭并释放WebClient或WebRequest资源.


下载:
 DownloadFile方法的主函数使用指定url地址通过的URI从远程服务器下载数据到本地应用程序.

1. 获得远程服务器url地址.

2. 获得目标文件路径.

3. 使用WebRequest对象检查文件是否存在于服务器端
 (导入命名空间System.Net的引用).

4. HTTP: 
   创建WebClient(System.Net,类似于上面提到的UploadFile方法)实例,
   访问你DownloadData() 方法通过指定URI下载文件缓冲资源到本地路径. 
   实际上,对于HTTP资源,使用"GET"方法.

   FTP:
   创建FtpWebRequest实例,通过使用WebRequestMethods.Ftp.DownloadFile方法, 
   我们可以接受来自服务器的资源流.此方法使用"RETR"命令下载FTP资源.

5. DownloadData方法会返回下载资源的字节数组.我们只需要从这个下载文件缓冲
使用FileStream(using System.IO)写一个字节段到本地服务器路径.

6. 关闭并释放FileStream资源.

/////////////////////////////////////////////////////////////////////////////

参考资料:

WebClient 类
http://msdn.microsoft.com/zh-cn/library/system.net.webclient.aspx

UploadData 方法
http://msdn.microsoft.com/zh-cn/library/system.net.webclient.uploaddata.aspx

DownloadData 方法
http://msdn.microsoft.com/zh-cn/library/system.net.webclient.downloaddata.aspx

FtpWebRequest 类
http://msdn.microsoft.com/zh-cn/library/system.net.ftpwebrequest.aspx

如何：使用 FTP 上载文件
http://msdn.microsoft.com/zh-cn/library/ms229715.aspx

/////////////////////////////////////////////////////////////////////////////

