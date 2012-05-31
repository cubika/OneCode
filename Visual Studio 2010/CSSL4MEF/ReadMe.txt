=============================================================================
         SILVERLIGHT 应用程序 : CSSL4MEF Project 概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

Managed Extensibility Framework (MEF)是一款可以帮助开发者设计可扩展程序的一个框架.
Silverlight 4支持这一框架. 本示例使用 MEF 建立了一个简单的文本编辑器. 通过使用预先
定义好的条约, 用户可以建立组件增强编辑器的功能. 这个组件在运行时加载.


/////////////////////////////////////////////////////////////////////////////
演示:

按以下步骤测试本示例：
1. 打开 CSSL4MEF 解决方案并生成解决方案.
2. 右键点击 CSSL4MEFTestPage.aspx 文件, 选择"在浏览器中查看".
3. 在打开的页面中, 你可以看到一个 Silverlight 程序.
	a. 在 Silverlight 的右边区域有一些控件, 这些控件可以改变左边显示的文本的状态.
	右边的所有控件, 都是由 MEF 建立的组件.
	b. 点击"点击加载颜色配置控件"按钮, MEF 将加载扩展组件, 并动态重新配置面板.


/////////////////////////////////////////////////////////////////////////////
先决条件:

Silverlight 4 Tools for Visual Studio 2010
http://www.silverlight.net/getstarted/

Silverilght 4 runtime
http://www.silverlight.net/getstarted/


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 如何使用 MEF 建立扩展 Silverlight 程序？
	说来话长, 我推荐阅读 CodeBetter 的博客中的 MEF 系列
	http://codebetter.com/blogs/glenn.block/archive/2009/11/30/building-the-hello-mef-dashboard-in-silverlight-4-part-i.aspx

2. 解决方案的结构是什么？
	CSSL4MEF 项目利用 MEF 实现 ConfigPanel 控件, 该控件可以在运行时下载插件并扩展功能.

	CSSL4MEF.Web 项目是 CSSL4MEF silverlight 程序的托管 web 程序.

	ConfigControl.Contract 项目定义了 ConfigPanel 的扩展接口协议.

	ConfigControl.Extension 项目实现了 configpanel 的扩展接口, 建立了"ColorPicker"组件.

3. MEF 如何在本项目中运行？
	运行程序时, 你将会发现有两块区域. 左边的区域显示了一个简短的文本, 而右边的
	区域有一些可以改变文本样式的控件组成. 实际上, 这些控件绑定到了预先定义的 DataModel 上.
	这些样式的文本没有什么魔力, 仅仅是将 UI 属性绑定到 datamodel 并且利用
	INotifiyPropertyChanged 适时的更新 UI. 右边部分, 是一个 Silverlight 控件“ConfigPanel".
	它可以将"ConfigData"属性绑定到 datamodel, 并自动生成配置控件.
	
	因为 datamodel 属性可以是任何类型并且拥有多种需求, 为了使 ConfigPanel 能够
	自动生成与 datamodel 对应的控件, 我们需要使 ConfigPanel具有可扩展性. 在本次
	方案中, MEF 可以帮助我们实现这一设计任务.

	我们定义了一个"IConfigControl"接口, 能够返回一个绑定到给定属性的编辑控件.
	ConfigPanel 利用 MEF 控制一系列 IConfigControl, 通过调用IConfigControl.MatchTest方法,
	configPanel 可以找到每个属性最合适的 configControl 并将控件添加到 configPanel 中.

	假设我们拥有一个 Color 类型属性的 datamodel, 扩展 ConfigPanel, 使其支持 Color 类型,
	我们需要建立一个新的 silverlight 项目, 实现 IConfigControl 并标记 Export 特性, 使其
	易于发现. 然后, 利用"DeploymentCatalogService", 我们可以动态加载扩展配置控件, 一旦
	类型发生改变, configPanel 获得通知并为 datamodel 重写 UI.
    

/////////////////////////////////////////////////////////////////////////////
参考资料:

MEF community site
http://mef.codeplex.com/


/////////////////////////////////////////////////////////////////////////////