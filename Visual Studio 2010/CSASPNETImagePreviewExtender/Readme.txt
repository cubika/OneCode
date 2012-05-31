========================================================================
              CSASPNETImagePreviewExtender 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

本项目演示了如何设计一个 AJAX 扩展程序控件. 
在此示例中, 这是个关于图片的扩展控件.
使用这个扩展控件的图片最初被显示为一张缩略图,如果用户单击图片,将弹出以原始尺寸显示的大图.

/////////////////////////////////////////////////////////////////////////////
示例演示. 

直接打开CSASPNETImagePreviewExtender.sln然后直接打开
ImagePreviewExtenderTest测试页面扩展控件.

如果你想要进一步测试, 请遵照下列演示步骤.

步骤 1: 在浏览器中查看Default.aspx. 你将看到一些图片.

步骤 2: 当页面载入时,图片都被显示为缩略图模式,看上去较小.单击第一张图片.

步骤 3:当单击使用扩展控件的图片时, 你将看到全尺寸的大图探出显示并自适应图片位置.

步骤 4: 单击大图左上的CLOSE按钮.

步骤 5: 如果我们把图片放入一个Panel中且Panel已使用ImagePreviewExtender扩展, 
你会发现所有该panel中的图片都拥有同样的特性.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤 1. 在 Visual Studio 2010 或 Visual Web Developer 2010中创建一个
	C# "ASP.NET AJAX 扩展程序控件" 项目.
	将其命名为ImagePreviewExtender.

步骤 2. 我们可以看到有三个文件被创建了. 一个.JS文件, 一个.CS文件和 
	一个.RESX 文件. 删除.RESX文件, 在此示例中我们不需要它.
	重命名js文件为ImagePreviewBehavior.js.
	重命名cs文件为ImagePreviewControl.cs.

步骤 3.  打开ImagePreviewControl.cs.
	增加一个名为"ThumbnailCssClass"的属性.
	在GetScriptDescriptors的重载函数中, 使用类似下列代码
	添加属性到描述符.

	[Code]
	ScriptBehaviorDescriptor descriptor = new ScriptBehaviorDescriptor(
		"ImagePreviewExtender.ImagePreviewBehavior", 
		targetControl.ClientID);
        descriptor.AddProperty("ThumbnailCssClass", ThumbnailCssClass);
	[/Code]

	在GetScriptReferences函数中设定ScriptReference名称.

步骤 4.  打开ImagePreviewBehavior.js. 依据示例的描述用同样的文件名编写javascript.

步骤 5.  打开"Properties"文件夹中的AssemblyInfo.cs.在文件底部,创建两个web资源定义. 
我们可以下载一个关闭图标并复制到根目录.

	[Code]
	[Assembly: WebResource("ImagePreviewExtender.ImagePreviewBehavior.js",
			 "text/javascript")] 
	[Assembly: WebResource("ImagePreviewExtender.Close.png", 
			"image/png")]
	[/Code]

步骤 6.  扩展已经完成. 创建一个ASP.NET WebSite或 ASP.NET Web应用程序来测试扩展. 
	首先引用项目然后就像使用AJAXControlToolKit中的一样使用控件. 



/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: Microsoft Ajax 扩展程序控件概述
http://msdn.microsoft.com/zh-cn/library/bb470384.aspx

MSDN: 演练：Microsoft Ajax 扩展程序控件
http://msdn.microsoft.com/zh-cn/library/bb470455.aspx

MSDN: 创建扩展程序控件以将客户端行为与 Web 服务器控件关联
http://msdn.microsoft.com/zh-cn/library/bb386403.aspx

MSDN: ExtenderControl 成员
http://msdn.microsoft.com/zh-cn/library/system.web.ui.extendercontrol_members.aspx

/////////////////////////////////////////////////////////////////////////////