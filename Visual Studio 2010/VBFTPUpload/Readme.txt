================================================================================ 
       Windows 应用程序 :    VBFTPUpload 项目概述          
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要：

这个例子展示怎样列出子目录和在FTP服务器上的文件夹中的文件和上传一个完整的文件夹

操作包括：
1.列出子目录和在FTP服务器上的文件夹中的文件
2.删除在FTP服务器上的一个文件或目录
3.上传一个文件到FTP服务器
4.在FTP服务器上创建一个目录
 
////////////////////////////////////////////////////////////////////////////////
演示：
第一步：在VS2010生成一个例子。
第二步：运行 VBFTPUpload.exe
第三步：在FTPServer textbox中输入一个有效的FTP服务器路径类型 ，路径开始以：ftp://localhost ，

*列出子目录和文件
第四步：按“连接”按钮，程序展示一个输入验证的对话框，如果FTP服务器支持匿名用户，你可以选中“匿名登录”。
                 第一次将花费几秒连接FTP服务器，然后你将会在“FTP 文件管理器”中看到文件和目录，在列表框中的选项如：
            2010-12-1 12:00 <DIR> FolderA

			2010-12-1 12:00 1024 FileB
            <DIR>文件夹的意思
第五步：在“FTP文件管理器”中双击一个包括<DIR>的选项，“FTP文件管理器”将导航到子目录和列出它的文件和子目录

第六步：单击“父文件夹”按钮，如果当前路径不是服务器的根路径，“FTP文件管理器”将导航到父路径和列出它的文件和子目录。

*删除一个在FTP服务器上的文件和目录
第七步：在“FTP文件管理器”中选择一个或多个选项，单击“删除”按钮

*上传一个文件到FTP服务器
第八步：单击“浏览”按钮在“上传文件”框中选择一个或多个文件。然后单击“上传文件”按钮，程序将上传选择的文件到FTP服务器上。

/////////////////////////////////////////////////////////////////////////////
代码逻辑:


1. 定义一个FTPClientManager类，这个类支持如下特性：

   1.1.核实一个文件或一个目录在FTP服务器上是否存在 
   1.2.删除在 FTP服务器上的文件和目录，删除一个文件，创建一个FtpWebRequest 然后设置WebRequestMethods.Ftp.DeleteFile.方法属性
          移除一个目录，创建一个FtpWebRequest 和设置WebRequestMethods.Ftp.RemoveDirectory. 方法属性。
   1.3. 在 FTP服务器上创建一个目录，创建一个目录，创建一个 FtpWebRequest和设置WebRequestMethods.Ftp.MakeDirectory. 方法属性
   1.4. 管理 FTPUploadClient 上传文件到FTP服务器上，上传一个文件，创建一个FtpWebRequest 和设置WebRequestMethods.Ftp.UploadFile.方法属性。
   1.5. 产生这些事件，UrlChanged, ErrorOccurred, StatusChanged, FileUploadCompleted
        NewMessageArrived.

2. 定义一个FTPUploadClient 类去上传文件从FTP 服务器上.

3. 定义类，ErrorEventArgs, FileUploadCompletedEventArgs, NewMessageEventArg
   and FTPClientManagerStatus 被 class FTPClientManager使用.

4. 定义一个FTPFileSystem类代表一个在远程FTP服务器文件，它也包括静态方法来解析 ListDirectoryDetails FtpWebRequest.响应 
    当设置一个FtpWebRequest方法属性到WebRequestMethods.Ftp.ListDirectoryDetailsFTP LIST 协议方法来得到一个在FTP服务器上的详细的 文件列表 ), 服务器响应将包括许多信息记录，每个记录代表一个文件或一个目录，基于服务器的 FTP Directory Listing Style，记录如下：
    a. MSDOS
       1.1. Directory
            12-13-10  12:41PM  <DIR>  Folder A
       1.2. File
            12-13-10  12:41PM  [Size] File B  
            
      注意: 如果在IIS或其他FTP服务器中的四位的年没选中，日期部分如"12-13-10" 替代 "12-13-2010"，
           
    b. UNIX
       2.1. Directory
            drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
       2.2. File
            -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
     
       注意：日期部分不包括年
	   
5. 在 MainForm, 注册FTPClientManager的UrlChanged事件，如果路径改变，列出在“FTP 文件管理器”中新路径文件和子目录 ，也注册 FTPClientManager的其他事件然后记录事件
 
/////////////////////////////////////////////////////////////////////////////
参考:

FtpWebRequest Class
http://msdn.microsoft.com/en-us/library/system.net.ftpwebrequest.aspx

WebRequestMethods.FTP Class
http://msdn.microsoft.com/en-us/library/system.net.webrequestmethods.ftp.aspx

Unix File Permissions
http://www.unix.com/tips-tutorials/19060-unix-file-permissions.html

/////////////////////////////////////////////////////////////////////////////

