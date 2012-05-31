================================================================================
	            Windows应用程序: CSMultiThreadedWebDownloader                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:
本程序演示怎么使用多线程下载文件。

System.Net.WebClient类有个 DownloadProgressChanged事件，可以通过这个事件实现下载过程。
但是这个类不支持暂停/继续下载。

例子中的MultiThreadedWebDownloader类是被用来通过网络下载文件，并且有以下功能：
1.使用多线程下载整个文件。
2.设置proxy。
3.设置缓冲区和缓存的大小。
4.开始，暂停，继续，取消下载。
5.提供文件的大小，下载速度和下载用时。
6.公开 StatusChanged、 DownloadProgressChanged 和 DownloadCompleted 的事件。

注意：

1.要暂停和取消下载，或者使用多线程下载，服务器必须支持"Accept-Ranges"头。以便我们能向webrequset
  添加下载文件的特定块的范围。
2.如果你想下载一个文件使用多线程或者多个HttpWebRequest，多请求（每一个请求都是不用的范围）都在一个
  连接中（并不是所有的）。你就必须考虑有多少线程在运行，要为ServicePointManager设置多少个max Connections。
  默认的情况下，ServicePointManager有2个max Connections，因此只有2个请求被同时激活并开始下载文件的一部分。
  所以，那样的做法会加快下载。但是，你还需要考虑到最大连接数。如果你增加可用的连接数，那么下载速度会更快。
  但是，不要增加到太大的数，否则，CPU会由于创建多个连接而增加开销，反而会降低下载速度。最好是把最大连接数限制在
   12*#CPU（ASP.NET中使用)。

   
////////////////////////////////////////////////////////////////////////////////
演示:
1.在Visual Studio 2010 创建一个工程。

2.运行CSMultiThreadedWebDownloader.exe. 

3.可以用下面这个连接下载一个测试文件。

http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe

4.单击“检查连接地址”按钮。然后“url”文本框和“检查连接地址”按钮就会不能使用，“本地路径”文本框和
  “下载”按钮将会被激活使用。

5.输入本地路径，像：D:\DotNetFx4.exe

6.单击“下载”按钮，你就会看到“Downloading”状态，和一些信息如：“接收：***KB，总共：***KB，速度：***KB/s
  线程数：*”，进度条也会改变。
  同时在D盘中，你会发现产生一个DotNetFx4.exe.tmp文件。

7.单击“暂停”按钮，你会看到“Paused”状态，主要信息如“接收：***KB，总共：***KB，用时：***”，进度条也停止。

8.单击“继续”按钮，你就会看到“Downloading”状态，和一些信息如：“接收：***KB，总共：***KB，速度：***KB/s
  线程数：*”，进度条也会改变。

9.当下载完成，有会看到“ompleted”状态，主要信息有：”接收：***KB，总共：***KB,用时：***”，进度条也停止。

   在D盘将会有一个DotNetFx4.exe文件。
	   


/////////////////////////////////////////////////////////////////////////////
代码逻辑：

1.当应用程序开始时，从app.config中设置proxy。

2.在下载文件之前，MultiThreadedWebDownloader要检查：
  2.1 文件是否存在。
  2.2远程服务器是否支持“Accept-Ranges”头。
  2.3文件大小是否大于0KB。

3.如果检查通过，然后用户就可以指定本地路径并开始下载。
   当下载开始，检查目标文件是否存在，如果不存在，就创一个新的和要下载文件一样大小的文件。
   如果远程服务器不支持“Accept-Ranges”头，下载只会创建一个HttpDownloadClient使用单线程下载。
   否则，下载会创建多个HttpDownloadClient，并使用多线程下载。

4.当创建一个HttpDownloadClient对象，就初始化一下字段或属性：
   StartPoint, EndPoint, BufferSize, MaxCacheSize, BufferCountPerNotification, Url
   DownloadPath 和 Status。

5.每个下载线程会从响应流中读取缓冲字节，并且先存储到内存缓存。如果缓存满了，或者下载被暂停了，
  取消或者完成了，就把缓存数据写到本地文件。

6.当读取缓冲数据时触发DownloadProgressChanged事件。

7.如果下载被暂停，每个HttpDownloadClient会存储已经下载的大小，当下载继续时，从新的开始点下载文件。

8.当当前的下载停止时，更新用时和状态。

9.当下载完成或者取消时，触发DownloadCompleted事件。
   

/////////////////////////////////////////////////////////////////////////////
相关资料:

system.net.webrequest类
http://msdn.microsoft.com/en-us/library/system.net.webrequest.aspx

怎样确定文件是否下载完成？ 
http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/e115d4a1-5800-4f2a-98d8-079de6cf8a1a
/////////////////////////////////////////////////////////////////////////////
