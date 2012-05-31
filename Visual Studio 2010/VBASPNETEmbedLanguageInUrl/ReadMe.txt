=============================================================================
                  VBASPNETEmbedLanguageInUrl 项目概述
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

步骤1: 打开VBASPNETEmbedLanguageInUrl.sln.

步骤2: 展开VBASPNETEmbedLanguageInUrl应用程序并且按Ctrl + F5来
        启动应用程序. 

步骤3: 我们将会看到正常的英文网页,我们试着修改浏览器地址栏中的值,
        例如,把"en-us"改为"zh-cn".

步骤4: 如果你正确的修改了url,你将会看到这个页面的中文版.        . 

步骤5: 很好,如果你输入了一个在这个应用程序中没有的语言,
        你将会看到一个默认的英文版网页.

步骤6: 验证结束.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. 在Visual Studio 2010或Visual Web Developer 2010中创建一个VB语言 
        的"ASP.NET空应用程序",命名为"VBASPNETEmbedLanguageInUrl". 

步骤2. 添加一个文件夹,"XmlFolder".为了在这个页面中演示不同语言的内容 
        我们需要一个数据库或一个xml文件来存储我们的数据,在这个代码
		示例中我们需要添加一个Language.xml的文件.

步骤3. 在应用程序的根目录下添加三个web窗体,"ShowMe.aspx"页面用来向用户 
        显示,"InvalidPage.aspx"用来处理错误http请求,"Default.aspx"用来启动
		url地址.

步骤4. 添加三个类文件,"BasePage.vb"类用来检查请求的url的语言部分和名称部分  
        并且设置页面的Culture和UICultrue属性.
        "UrlRoutingHandler.vb"类用来检查文件名并且如果它们不存在要把 
		它们传送到InvalidPage.aspx页面."XmlLoad.vb"类用来加载xml数据 
		到ShowMe.aspx页面中显示它们.
		[注意]
		如果你要创建更多的网页包括不同的语言,请继承BasePage.vb类
        来设置页面的Cultrue和UICulture.

步骤5. 在Global.asax文件中注册url路径.
        [代码]
             Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
                 RegisterRoutes(RouteTable.Routes)
             End Sub
             ''' <summary>
             ''' Url路径
             ''' </summary>
             ''' <param name="routes"></param>
             Public Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
                 routes.Add("Page", New Route("{language}/{pageName}", New UrlRoutingHandlers()))
             End Sub
		[/代码]
		注册url地址之后,我们需要创建UrlRoutingHandlers来检查请求的url.
		 UrlRoutingHanders代码例如： 
		[代码]
             ''' <summary>
             ''' 创建RoutingHandler来检查HttpRequest 
             ''' 和返回正确的url地址.
             ''' </summary>
             ''' <param name="context"></param>
             ''' <returns></returns>
             Public Function GetHttpHandler1(ByVal context As System.Web.Routing.RequestContext) 
			     As System.Web.IHttpHandler Implements System.Web.Routing.IRouteHandler.GetHttpHandler
                 Dim language As String = context.RouteData.Values("language").ToString().ToLower()
                 Dim pageName As String = context.RouteData.Values("pageName").ToString()
                 If pageName = "ShowMe.aspx" Then
                     Return TryCast(BuildManager.CreateInstanceFromVirtualPath("~/ShowMe.aspx", GetType(Page)), Page)
                 Else
                    Return BuildManager.CreateInstanceFromVirtualPath("~/InvalidPage.aspx", GetType(Page))
                 End If
             End Function
		[/代码] 

步骤6. 创建两个源文件来支持多语言的网页,命名为： 
        "Resource.resx", "Resource.zh-cn.resx".

步骤7. 在BasePage.cs中添加一些代码,设置page.Culture和page.UICulture.
        [代码]
		     ''' <summary>
             ''' BasePage类用来设置Page.Culture和Page.UICulture.
             ''' </summary>
             Protected Overrides Sub InitializeCulture()
                 Try
                     Dim language As String = RouteData.Values("language").ToString().ToLower()
                     Dim pageName As String = RouteData.Values("pageName").ToString()
                     Session("info") = language & "," & pageName
                     Page.Culture = language
                     Page.UICulture = language
                 Catch generatedExceptionName As Exception
				     Session("info") = "error,error"
                 End Try

             End Sub
		[/代码]

步骤8. 在ShowMe.aspx页面我们需要添加一些xml数据和源文件数据.
        [代码]
		     lbTitleContent.Text = strTitle

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
http://msdn.microsoft.com/zh-cn/library/ccec7sz1(VS.80).aspx


/////////////////////////////////////////////////////////////////////////////