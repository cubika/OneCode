================================================================================
       Windows应用程序: CSFTPDownload 项目概述                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:
该示例演示了在FTP服务上如何列出一个子目录和文件的文件夹
和下载他们。


 这些操作包括：
1.  在FTP服务上列出所有的子目录和文件的文件夹。 
    当设置一个FtpWebRequest的Methods属性时WebRequestMethods.Ftp.ListDirectoryDetails(FTP LIST 协
	议方法在FTP服务上获得一个列表的详细清单)，服务的响应将
    包含多种信息记录，每个记录代表一个文件或目录.依靠FTP Directory Listing Style 的服务而定。
	如该记录： 
    1. MSDOS
       1.1. 目录
            12-13-10  12:41PM  <DIR>  Folder A
       1.2. 文件
            12-13-10  12:41PM  [Size] File B 
			 
        注意: 日期段，如“12-13-10“而不是”12-13-2010“，如果年是四位数在IIS或者FTP服务中
		      是不被检查的。

		
    2. UNIX
       2.1. 目录
            drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
       2.2.文件
            -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
     
      注意: 日期段不包含年.

2. 从FTP服务下载一个文件. 
  
   对于下载一个文件，
   创建一个FtpWebRequest和设置Method属性WebRequestMethods.Ftp.DownloadFile。


 
////////////////////////////////////////////////////////////////////////////////
演示:
 
步骤1. 在vs2010中创建这个事例.
 
步骤2. 运行CSFTPDownload.exe文件.
 
步骤3.在FTP服务文本框中输入一个有效的FTP服务的url， 这个url应当开始就支持ftp：// 像
ftp://localhost 一样
      
 
* 子目录和文件的列表。

步骤4. 按下“连接”按钮。应用程序将显示一个输入凭据的对话框，如果FTP服务支持匿名用户，
            你可以选择“匿名登录”

       对于第一次连接FTP服务这可能需要几分钟，然后再FTP File Explorer中你将看到这个文件和子目录，
	   该项目在列表框中就像：
       
            2010-12-1 12:00 <DIR> Folder A

			2010-12-1 12:00 1024 File B


	   <DIR> 意味着一个文件夹.
 
步骤5. 在“FTP文件资源管理器”中双击一个包括<DIR>的项目，这个“FTP文件资源管理器”将导航到该子目录下，
       并列出它的文件和子目录。

	   
步骤6. 单击“上一级文件夹”按钮，如果当前路径不是服务器的根路径，“FTP文件资源管理”将定位到
	   该父路径和文件的列表以及它的子目录。

*从FTP服务下载文件. 

步骤7.在“FTP文件资源管理”中选择一个或多个项目，点击“浏览”按钮选择下
       载路径，然后点击按钮下载。
 
       
	   
	   这个应用程序将选择的项目下载到所选择的下载路径下，如果所选项目包含一个
	   文件夹，这个应用程序也将下载这个文件和它的子目录。
	  

/////////////////////////////////////////////////////////////////////////////
代码逻辑:


1. 设计一个类FTPClientManager.这个类提供以下功能.

   1.1 检查URL是否是有效的链接。创建一个URL的FtpWebRequest和设置
       一个Method属性WebRequestMethods.Ftp.PrintWorkingDirectory。
       如果没有异常，则这个URL是有效的。
   
   1.2在FTP服务的文件夹的文件列表和子目录，创建一个URL的 FtpWebRequest和
       设置Method属性WebRequestMethods.Ftp.ListDirectoryDetails。
       服务器将返回许多信息记录，并且每一个记录代表一个文件或目录。
       
	   
   1.3 管理FTPDownloadClient下载文件。  

   1.4 提供一些事件，如UrlChanged, ErrorOccurred, StatusChanged, FileDownloadCompleted
       NewMessageArrived.

   
2. 在FTP服务下载文件中设计一个类FTPDownloadClient.当下载启动时，在
   后台的线程中将会有一个下载文件，下载的数据储存在第一个MemoryStream中，
   然后再写入本地文件。

3. 通过使用FTPClientManager类设计一些类，如ErrorEventArgs, FileDownloadCompletedEventArgs, 
   NewMessageEventArgand FTPClientManagerStatus。


4.设计一个FTPFileSystem类，这个类表示远程FTP服务上的一个文件，它
  还包括静态的方法为了分析一个 ListDirectoryDetails FtpWebRequest的响应。
 
	   
5. 在MainForm中，注册一个FTPClientManager的UrlChanged事件，如果URL改变了，
   在“FTP文件资源管理”中有新的URL的文件列表和子目录。它也注册了FTPClientManager的
   其它事件，并记录这个事件。


/////////////////////////////////////////////////////////////////////////////
参考资料:

FtpWebRequest类
http://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx

WebRequestMethods.FTP类
http://msdn.microsoft.com/en-us/library/system.net.webrequestmethods.ftp.aspx

Unix文件权限
http://www.unix.com/tips-tutorials/19060-unix-file-permissions.html

/////////////////////////////////////////////////////////////////////////////

