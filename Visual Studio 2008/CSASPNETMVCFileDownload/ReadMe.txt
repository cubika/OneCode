========================================================================
       ASP.NET MVC 应用程序 : CSASPNETMVCFileDownload 项目 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

  此项目演示了如何在ASP.NET MVC web应用程序中表现下载文件. 
  
  具体的功能包括如何使用MVC路由来注册处理我们的文件下载的自定义
  url路由模式. 包括:
  
  * 如何显示一个下载文件列表
  * 如何在MVC页面中流输出文件内容
  
  示例应用程序同时包括一个自定义ActionResult类
  用来(二进制格式)输出文件内容.


/////////////////////////////////////////////////////////////////////////////
前提条件:

已安装Visual Studio 2008 SP1包括ASP.NET MVC 1.0 扩展. 

*ASP.NET MVC 1.0 RTM download:
http://www.microsoft.com/downloads/details.aspx?FamilyID=53289097-73ce-43bf-b6a6-35e00103cb4b&displaylang=en


/////////////////////////////////////////////////////////////////////////////
演示:
  
*打开项目

*选择default.aspx页面并在浏览器中查看

*在主页面UI中, 选择"文件下载"标签

*单击任意文件链接下载对应文件


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. 创建一个 Visual Studio 2008 SP1 ASP.NET MVC Web 应用程序项目

步骤2. 清除项目中不必要的部分, 包括: jquery脚本, 
       账户管理的控制器和视图...

步骤3: 添加我们的FileDownload控制器类并关联视图文件
       关于更多如何添加控制器类的信息, 参照:
       http://www.asp.net/learn/mvc/#MVC_Controllers
        
步骤4: 添加一个用来输出文件内容自定义ActionResult类

步骤5: 在global.asax文件事件中添加合适的URL路由规则. 路由规则对于链接请求
       到合适的MVC控制器和行为方法非常重要. 更多详细信息, 参照:
       http://weblogs.asp.net/scottgu/archive/2007/12/03/asp-net-mvc-framework-part-2-url-routing.aspx


关键组件:

* web.config 文件: 包含所有web应用程序必须的配置信息 

* global.asax: 包含所有URL路由规则

* HomeController 类: 包含主应用程序导航逻辑(比如默认页面和关于页面)

* FileController 类: 包含文件下载导航和处理逻辑(比如文件列表和文件下载)

* Home 视图: HomeController的页面UI元素

* File 视图: FileController的页面UI元素

* shared 视图 & Site.Master: 所有UI页面共享的UI元素

* BinaryContentResult: 用来(二进制格式)输出文件内容自定义ActionResult类


/////////////////////////////////////////////////////////////////////////////
参考资料:

#ASP.NET MVC Tutorials
http://www.asp.net/Learn/mvc/


/////////////////////////////////////////////////////////////////////////////