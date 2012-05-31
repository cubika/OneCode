================================================================================
	   Windows 应用程序: VBWebBrowserAutomation项目概述                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:
这个例子是演示怎么创建一个有以下功能的Web浏览器
1. 熟练控制HTMEL元素并且自动登录站点。
2. 打不开站点时指定一个站点。

////////////////////////////////////////////////////////////////////////////////
演示:

第一步. 在VS2010中打开项目,首先，在StoredSites\www.codeplex.com.xml中用你的用户名和密码替代默
认的用户名和密码登录到 http://www.codeplex.com 。 

第二步. 在VS2010中编译项目。 

第三步. 运行VBWebBrowserAutomation.exe. 按钮默认的“自动完成”是有缺陷的。

第四步. 在超链接文本框中键入 https://www.codeplex.com/site/login?RedirectUrl=https%3a%2f%2fwww.codeplex.com%2fsite%2fusers%2fupdate
	    并且按下Enter键。
		
	   这个超链接是www.codeplex.com的登陆页。 重定义超链接的意思是如果你
	   成功登录这个站点，这个页将被重定向到这个超链接。

第五步. 当这个网页被完全加载后，“自动完成”的按钮被激活。点击这个按钮，这个将
被重定向到 https://www.codeplex.com/site/users/update。

第六步. 当这个新的网页被加载后， 再次点击“自动完成”按钮，在网页中"新邮件地址" 
        这一栏将被填写。

第七步. 在超链接文本框中键入 http://www.contoso.com 并且按下Enter键。你将在一
        个页上看到一条信息“对不起，不能打开这个站点”。


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 设计一个能将一个对象序列化为XML文件或XML文件序列化为一个对象的类XMLSerialization。

2. 设计的类HtmlCheckBox，HtmlPassword，HtmlSubmit和HtmlText代表checkbox，密码文本框，
   提交按钮和正常的文本框。所有的类继承自代表一个HtmlElement有“输入”标签的HtmlInputElement。
   这个HtmlInputElementFactory类是用来在网页中从一个HtmlElement获得一个HtmlInputElement。

3. 设计一个StoredSite类来存储一个站点的html元素。一个站点是被当做一个XML文件被存储在StoredSites
   文件夹中，并且能被序列化。
   
   这个类支持一个FillWebPage方法来自动完成网页。如果能找到一个提交按钮，这个按钮将被自动点击。
  
4.  设计一个BlockSites类来包含不能不能访问的站点列表。这个\Resource\BlockList.xml
    文件能被序列化成一个BlockSites实例。
   
5. 设计一个WebBrowserEx类，这个类继承自System.Windows.Forms.WebBrowser类。

   重写OnNavigating方法用来检查这个超链接是否被包含在不能访问站点列表中。如果是，指向build-in 
   Block.htm。
   
   重写OnDocumentCompleted方法来检查加载也是否自动完成加载。如果站点和超链接是在存储中的，
   这时这方法将自动被用上。

/////////////////////////////////////////////////////////////////////////////
参考:
http://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx
http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.aspx
/////////////////////////////////////////////////////////////////////////////
