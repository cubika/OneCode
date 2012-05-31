====================================================================================
     Silverlight 应用程序 : VBADONETDataServiceSL3Client 项目概述
====================================================================================

/////////////////////////////////////////////////////////////////////////////
用途:

本示例展示了如何在Silverlight中访问Ado.Net Data Services。示例中创建了3个代理对应了服务器端的3个不同的服务。
分别使用了Ado.Net Entity Data Model，LinqToSQL Data Classes 以及非关系型数据库的普通类型作为数据源

本示例展示如何通过访问REST类型服务(Ado.Net Data Services)来进行增删改查。使用的客户端代码都会由代理自动创建，
我们也可以自己写客户端代码，但是自动创建更方便快捷。


/////////////////////////////////////////////////////////////////////////////
先决条件:

Silverlight 3 Tools for Visual Studio 2008 SP1
http://www.microsoft.com/downloads/details.aspx?familyid=9442b0f2-7465-417a-88f3-5e7b5409e9dd&displaylang=en

Silverilght 3 runtime:
http://silverlight.net/getstarted/


/////////////////////////////////////////////////////////////////////////////
项目 相关:

VBADONETDataServiceSL3Client -> VBADONETDataService -> SQLServer2005DB
VBADONETDataServiceSL3Client 发送异步的REST请求 VBADONETDataService 来获得数据.
VBADONETDataService 读取SQLServer2005DB数据库.

/////////////////////////////////////////////////////////////////////////////
注意:

在运行本示例前，请确认你已经部署了VBADONETDataService并且对服务的地址(比如，
在SchoolLinqToEntitiesUpdate.xaml.vb中的http://localhost/SchoolLinqToEntities.svc)进行了正确的更改。
同时检查Silverlight程序的宿主页面与ADO.NET Data Service处在同一个域下。
关于更多的详细信息可以在本文档的"已知问题"部分找到。


/////////////////////////////////////////////////////////////////////////////
代码 逻辑:

1. 查询.
   (1) 新建一个 DataServiceContext 对象。
   
   (2) 创建一个 DataServiceQuery(Of T) 对象。
   
   (3) 调用 DataServiceQuery(Of T).BeginExecute() 方法来开始一个异步的REST请求
	   注册一个回调事件，用以在请求结束后执行。
	   
   (4) 调用 DataServiceQuery(Of T).EndExecute() 方法 来结束查询并获得数据。

       
2. 更新.
   
   (1) 维持并使用一个在查询时创建的 DataServiceContext 对象。
	   
   (2) 调用 DataServiceContext.UpdateObject() 方法。
   
   (3) 调用 DataServiceContext.BeginSaveChanges() 方法来开始一个异步的REST请求
       注册一个回调事件，用以在请求返回后执行。
   
   (4) 调用 DataServiceContext.EndSaveChanges() 方法来解释请求。
   
3. 删除.

   (1) 维持并使用一个在查询时创建的 DataServiceContext 对象。
	   
   (2) 调用 DataServiceContext.DeleteObject() 方法.
   
   (3) 调用 DataServiceContext.BeginSaveChanges() 方法来开始一个异步的REST请求
       注册一个回调事件，用以在请求返回后执行。
   
   (4) 调用 DataServiceContext.EndSaveChanges() 方法结束请求。
   
4. 增加.

   (1) 维持并使用一个在查询时创建的 DataServiceContext 对象。
	   
   (2) 调用 DataServiceContext.AddObject() 方法(或者本例中更友好的方法比如 AddToCategories())。
   
   (3) 调用 DataServiceContext.BeginSaveChanges() 方法开始一个异步的REST请求
       注册一个回调事件，用以在请求返回后执行。
   
   (4) 调用 DataServiceContext.EndSaveChanges() 方法来结束请求。

   
/////////////////////////////////////////////////////////////////////////////
已知 问题:

如果Silverlight的宿主页面与ADO.NET Data Services处于不同的域中，
本例在FireFox中无法正常运行。因为IE8允许你设置是否允许XmlHttpRequest进行跨域的请求操作。
而FireFox在进行跨域请求时，会直接失败。

要解决这个问题，有2个方法：

1. 使用 WebClient 或者 HttpWebRequest 而不是客户端的生成代码。

详情请查看 REST HTTP messages:
http://msdn.microsoft.com/en-us/library/dd672595(VS.100).aspx

或者你可以创建一个测试ADO.NET Data Service客户端并且使用Fiddler工具来直接观察正确的消息。  

这个方法需要我们做更多来请求/获得/解析信息。 


2. 在Silverlight程序的宿主页同一个域中添加一个Web Serivce。这个Web Service被当作一个代理来使用。
Silverlight程序访问这个Web Service然后这个Web Service代替Silverlight程序去取得ADO.NET Data Service。

For more details please check out:
http://blogs.msdn.com/phaniraj/archive/2008/10/21/accessing-cross-domain-ado-net-data-services-from-the-silverlight-client-library.aspx
http://blogs.msdn.com/tom_laird-mcconnell/archive/2009/03/25/creating-an-ado-net-data-service-proxy-as-workaround-for-silverlight-ado-net-cross-domain-issue.aspx


/////////////////////////////////////////////////////////////////////////////
参考文档:

ADO.NET Data Services (Silverlight)
http://msdn.microsoft.com/en-us/library/cc838234(VS.95).aspx

How to: Specify Browser or Client HTTP Handling
http://blogs.msdn.com/silverlight_sdk/archive/2009/08/12/new-networking-stack-in-silverlight-3.aspx