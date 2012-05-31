=============================================================================
                  CSASPNETEmbedLanguageInUrl 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
总结:

本项目阐述了如何在URL中插入语言的编码例如：
 http://domain/en-us/ShowMe.aspx. 页面会根据语言的
编码呈现不同的内容,在例子中使用url-地址和源文件来本 
地化页面的内容.


/////////////////////////////////////////////////////////////////////////////
代码演示：

请参考下面的示范步骤.

步骤1: 打开CSASPNETEmbedLanguageInUrl.sln.

步骤2: 展开CSASPNETEmbedLanguageInUrl应用程序并且按Ctrl + F5来  
        启动应用程序.

步骤3: 我们将会看到正常的英文网页,我们试着修改浏览器地址栏中的值,
        例如,把"en-us"改为"zh-cn".

步骤4: 如果你正确的修改了url,你将会看到这个页面的中文版.        . 

步骤5: 很好,如果你输入了一个在这个应用程序中没有的语言,
        你将会看到一个默认的英文版网页.

步骤6: 验证结束.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. 在Visual Studio 2010或Visual Web Developer 2010中创建一个C#语言 
        的"ASP.NET空应用程序",命名为"CSASPNETEmbedLanguageInUrl". 

步骤2. 添加一个文件夹,"XmlFolder".为了在这个页面中演示不同语言的内容 
        我们需要一个数据库或一个xml文件来存储我们的数据,在这个代码
		示例中我们需要添加一个Language.xml的文件.

步骤3. 在应用程序的根目录下添加三个web窗体,"ShowMe.aspx"页面用来向用户 
        显示,"InvalidPage.aspx"用来处理错误http请求,"Default.aspx"用来启动
		url路径.

步骤4. 添加三个类文件,"BasePage.cs"类用来检查请求的url的语言部分和名称部分  
        并且设置页面的Culture和UICultrue属性.
        "UrlRoutingHandler.cs"类用来检查文件名并且如果它们不存在要把  
		它们传送到InvalidPage.aspx页面."XmlLoad.cs"类用来加载xml数据
		到ShowMe.aspx页面中显示它们.
		[注意]
		如果你要创建更多的网页包括不同的语言,请继承BasePage.cs类
        来设置页面的Cultrue和UICulture.

步骤5. 在Global.asax文件中注册url地址. 
		[代码]
             protected void Application_Start(object sender, EventArgs e)
             {
                 RegisterRoutes(RouteTable.Routes);
             }

             public static void RegisterRoutes(RouteCollection routes)
             {
                 routes.Add("Page", new Route("{language}/{pageName}", new UrlRoutingHandlers()));
             }
		[/代码]
		注册url地址之后,我们需要创建UrlRoutingHandlers来检查请求的url.
		 UrlRoutingHanders代码例如： 
		[代码]
              /// <summary>
              /// 创建RoutingHandler来检查HttpRequest 
              /// 和返回正确的url地址.
              /// </summary>
              /// <param name="context"></param>
              /// <returns></returns>
              public IHttpHandler GetHttpHandler(RequestContext context)
              {
                  string language = context.RouteData.Values["language"].ToString().ToLower();
                  string pageName = context.RouteData.Values["pageName"].ToString();
                  if (pageName == "ShowMe.aspx")
                  {
                      return BuildManager.CreateInstanceFromVirtualPath("~/ShowMe.aspx", typeof(Page)) as Page;
                  }
                  else
                  {
                      return BuildManager.CreateInstanceFromVirtualPath("~/InvalidPage.aspx", typeof(Page)) as Page;
                  }
              }
		[/代码] 

步骤6. 创建两个源文件来支持多语言的网页,命名为： 
        "Resource.resx", "Resource.zh-cn.resx".

步骤7. 在BasePage.cs中添加一些代码,设置page.Culture和page.UICulture. 
        [代码]
		     /// <summary>
             /// BasePage类用来设置Page.Culture和Page.UICulture.
             /// </summary>
             protected override void InitializeCulture()
             {
                 try
                 {
                     string language = RouteData.Values["language"].ToString().ToLower();
                     string pageName = RouteData.Values["pageName"].ToString();
                     Session["info"] = language + "," + pageName;
                     Page.Culture = language;
                     Page.UICulture = language;
                 }
                 catch (Exception)
                 {
				      Session["info"] = "error,error"; 
				 }
             }
		[/代码]

步骤8. 在ShowMe.aspx页面我们需要添加一些xml数据和源文件数据.
        [代码]
		     lbTitleContent.Text = strTitle;

			  <asp:Literal ID="litTitle" runat="server" Text='<%$ Resources:Resource,Title %>'></asp:Literal>
		[/代码] 

步骤9. 构建应用程序并且你可以调试它.


/////////////////////////////////////////////////////////////////////////////
参考文献:

MSDN: Url Routing
http://msdn.microsoft.com/zh-cn/magazine/dd347546.aspx

MSDN: UrlRoutingHandler Class	
http://msdn.microsoft.com/zh-cn/library/system.web.routing.urlroutinghandler.aspx

MSDN: Resourse File
http://msdn.microsoft.com/zh-cn/library/ccec7sz1.aspx


/////////////////////////////////////////////////////////////////////////////