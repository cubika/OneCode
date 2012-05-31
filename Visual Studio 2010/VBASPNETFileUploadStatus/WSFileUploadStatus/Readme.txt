========================================================================
                 VBASPNETFileUploadStatus 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

本项目使用AJAX阐述了在不使用第三方的组件时实现显示上传的状态和信息
第三方的组件像：ActiveX 控件, Flash 或者 Silverlight.
这也是一个大文件上传的解决方案.

/////////////////////////////////////////////////////////////////////////////
原理:

当文件上传时，服务器将会像下面所述获取请求数据.
(备注：当我们使用像Fiddler的工具上传文件时我们可以得到这部分.)

/*---------------------例子引用开始------------------*/
POST http://jerrywengserver/UploadControls.aspx HTTP/1.1
Accept: application/x-ms-application, image/jpeg, application/xaml+xml, 
        image/gif, image/pjpeg, application/x-ms-xbap,
        application/x-shockwave-flash, */*
Referer: http://jerrywengserver/UploadControls.aspx
Accept-Language: en-US
User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64;
            Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729;
            .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; MS-RTC LM 8;
            .NET4.0C; .NET4.0E)
Content-Type: multipart/form-data; boundary=---------------------------7da106f207ba
Accept-Encoding: gzip, deflate
Host: jerrywengserver
Content-Length: 1488
Connection: Keep-Alive
Pragma: no-cache

-----------------------------7da106f207ba
Content-Disposition: form-data; name="__VIEWSTATE"

/wEPDwUKMTI3MTMxMTcxNw9kFgICAw8WAh4HZW5jdHlwZQUTbXVsdGlwYXJ0L2Zvcm0tZGF0YWRkcrWP136t6D4d+g8BDfyR5WF+aP/yi4YARRyuOuRsO1M=
-----------------------------7da106f207ba
Content-Disposition: form-data; name="__EVENTVALIDATION"

/wEWAgL5mtyRBALt3oXMA9W4TniGaEKs/xcWf28H93S+wRcfLHr35wNo+N1v9gQ5
-----------------------------7da106f207ba
Content-Disposition: form-data; name="fuFile"; filename="C:\******\FileUploadTest.txt"
Content-Type: text/plain

*****This part is the content of the uploaed file!*****
-----------------------------7da106f207ba
Content-Disposition: form-data; name="btnUpload"

Upload
-----------------------------7da106f207ba--
/*------------------例子引用结束------------------*/

在开头部分会有一些有用信息，例如，
  请求的Content-Type.
  分隔主体部分的边界.
  请求的content-length.
  一些请求的变量.
  上传文件的文件名和它的Content-Type.

如果我们分析数据，我们会发现一些如下的要点.
  1. 所有请求的数据被边界分隔，这个边界被定义在开头部分的Content-Type
     里.
  2. 一个参数的名字和值被一个新行分隔.
  3. 如果参数是一个文件，我们可以获取这个文件的文件名和Content-Type.
  4. 文件的数据放在文件的Content-Type后面.

所以如果服务器获取所有这些数据，上传将会被结束.
现在有个问题是我们是怎么知道服务器已经读取了多少数据，并且有没有
一种方法可以使我们能够控制服务器一次读取的长度. 

对于IIS和.NET框架，我们可以通过HTTP模块来控制它.在BeginRequest
事件中数据读取将会被启动.并且HttpWorkerRequest类可以控制读取
过程.

我们可以使用HttpWorkerRequest.GetPreloadedEntityBody()来获取
服务器读取的请求数据的第一部分. 
如果数据太大HttpWorkerRequest.IsEntireEntityBodyIsPreloaded   
将会返回false，我们可以使用HttpWorkerRequest.ReadEntityBody()来读取
剩下的数据.用这种方法,我们可以知道已经上传多少数据并且还剩下多少数
据.
最后，我们需要把状态发送回服务器,这里我们把状态存储在缓存中.

另一个重要的问题是客户端是如何在没有回发到服务器情况下从服务器
端获取状态的.
答案是使用AJAX的特性.这里我们使用ICallBackEventHandler, 
因为这样不仅容易操作而且对于我们理解这个过程也非常清楚.
我们可以在readme文件的底部参考文献中学会如何使用它.
我们可以使用JQuery AJAX来返回一个web服务或者类的句柄
来获取状态.


/////////////////////////////////////////////////////////////////////////////
代码演示：

步骤1.  在Visual Studio 2010或者Visual Web Developer 2010中创建一
个Visual Basic代码项目把它命名为FileUploadStatus.

步骤2.  在项目中添加两个引用, System.Web.Extension 和 System.Web.

步骤3.  在例子文件夹"FileUploadStatus"中复制UploadStatus.vb的代
码.

步骤4.  添加一个ASP.NET模块并且把它命名为UploadProcessModule.

步骤5.  在例子的文件夹"FileUploadStatus"中复制UploadProcessModule.vb
的代码.

步骤6.  还要创建文件: BinaryHelper.vb, UploadFile.vb, 
UploadFileCollection.vb 和 FileUploadDataManager.vb, 并且复制它
们的代码.

步骤7.  保存创建的项目.
[注意]确定这个项目已经发布在适合"任意 CPU".
我们不能添加把一个x86版本的网站部署在一个x64系统的平台上的
引用.

步骤8.  在解决方案中添加一个新的空ASP.NET的网站,把它命名为
WSFileUploadStatus.

步骤9.  添加一个项目引用"UploadStatus",我们在开始时已经创建.

步骤10.  创建一个新Web用户控件命名为UploadStatusWindow.ascx.
我们将会使用这个用户控件来支持显示状态信息的弹出窗口. 在这个
例子文件夹"WSFileUploadStatus"中复制UploadStatusWindow.ascx
标记.

步骤11. 创建一个ASP.NET的Web页面命名为UploadControls.aspx.在
页面中添加一个文件上传的Web控件和一个web按钮控件.在例子文件夹 
"WSFileUploadStatus"中从UploadControls.aspx复制标记.

步骤12. 创建一个ASP.NET的web页面命名为Deafult.aspx.在页面中添
加一个iframe,设置src为"UploadControls.aspx".在例子文件夹
"WSFileUploadStatus"中从Default.aspx复制javascript函数和标记.

步骤13. 在例子文件夹中复制"styles" and "scripts"这两个文件夹
到网站中.

步骤14. 修改web.config.注册HttpModule.设置maxRequestLength
为1048576, 意思是最大请求数据将会限制在1GB. 如果我们在IIS7中
部署网站, 我么你需要在system.webServer块中设置requestLimits.

步骤15. 我们要确保已经使页面和例子中的一致,构建解决方案.

步骤16. 测试这个网站.


/////////////////////////////////////////////////////////////////////////////
参考信息:

MSDN: HttpModules
http://msdn.microsoft.com/zh-cn/library/zec9k340(VS.71).aspx

MSDN: HttpWorkerRequest Class
http://msdn.microsoft.com/zh-cn/library/system.web.httpworkerrequest.aspx

MSDN: ICallBackEventHandler Interface
http://msdn.microsoft.com/zh-cn/library/system.web.ui.icallbackeventhandler.aspx

MSDN: An Introduction to JavaScript Object Notation (JSON) in JavaScript and .NET
http://msdn.microsoft.com/zh-cn/library/bb299886.aspx

MSDN: JavaScriptSerializer Class
http://msdn.microsoft.com/zh-cn/library/system.web.script.serialization.javascriptserializer.aspx

/////////////////////////////////////////////////////////////////////////////