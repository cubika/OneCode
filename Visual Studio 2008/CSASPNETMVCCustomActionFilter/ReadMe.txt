========================================================================
       ASP.NET MVC 应用程序 : CSASPNETMVCCustomActionFilter 项目 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

	CSASPNETMVCCustomActionFilter示例演示了如何使用C# 代码为ASP.NET MVC
	web应用程序创建自定义ActionFilters. 在本示例中,	
	有两个自定义ActionFilters, 一个用来在页面视图结果执行和渲染
	前自定义ViewData ; 另一个在ActionResult运行的各种事件中记录信息的
    自定义ActionFilter.


/////////////////////////////////////////////////////////////////////////////
前提条件:

已安装Visual Studio 2008 SP1包括ASP.NET MVC 1.0 扩展. 

*ASP.NET MVC 1.0 RTM 下载:
http://www.microsoft.com/downloads/details.aspx?FamilyID=53289097-73ce-43bf-b6a6-35e00103cb4b&displaylang=en


/////////////////////////////////////////////////////////////////////////////
如何运行:
  
*打开项目

*选择default.aspx页面并使用浏览器查看

*在主页面UI中, 会显示消息数据(由ActionFilter修改)

*单击"关于"选项卡, 将显示About页面, 这个行为将触发Logging ActionFilter 
记录事件到指定文件.
[备注]请先创建D:\temp\logs,或在对应文件中修改路径

/////////////////////////////////////////////////////////////////////////////
关键组件:

*web.config 文件:包含所有web应用程序必须的配置信息 

*global.asax: 包含所有URL路由规则

*HomeController 类: 包含主应用程序导航逻辑(比如默认页面和关于页面)

*Home 视图: HomeController的页面UI元素

*shared 视图 & Site.Master: 所有UI页面共享的UI元素

*MessageModifierActionFilter: 这是一个用来干涉ActionResult的运行
修改ViewData的自定义ActionFilters

*TextLogActionFilter: 这是另一个在ActionResult运行的各种事件中记录信息的
自定义ActionFilter.


/////////////////////////////////////////////////////////////////////////////
参考资料:

#ASP.NET MVC Tutorials
http://www.asp.net/Learn/mvc/


/////////////////////////////////////////////////////////////////////////////