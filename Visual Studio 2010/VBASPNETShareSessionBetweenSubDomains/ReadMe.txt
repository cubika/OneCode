==============================================================================
 ASP.NET 应用程序 : VBASPNETShareSessionBetweenSubDomains 项目 概述
==============================================================================

//////////////////////////////////////////////////////////////////////////////
总结:

会话可以设置为不同的模式(InProc,SqlServer和StateServer).
当使用SqlServer/SateServer模式时，会话将存储在一个特定的SQL Server/SateServer.
如果两个ASP.NET Web应用程序指定同一SQL Server作为会话服务器,  
所有会话保存在同一服务器中. 总之, 如果使用SQL Server 会话, 
可以在不同ASP.NET应用程序的之间共享会话. 因为ASP.NET保存会话Id到cookie来指定当前会话, 
所以为了共享会话, 必须在cookie中共享会话Id.

VBASPNETShareSessionBetweenSubDomains示例演示了如何配置
一个SessionState服务器,然后创建一个SharedSessionModule模块实现
子域之间ASP.NET Web应用程序的交流.

两个ASP.NET Web应用程序必须运行在同一根域内(可以使用不同的端口).

//////////////////////////////////////////////////////////////////////////////
步骤:

1. 配置SQL Server保存ASP.NET会话状态.

   运行"C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssadd"
   向Sql Server添加会话状态支持[1].

2. 配置ASP.NET Web应用程序使用SQL Server保存Session同时使用特定的decryptionKey和validationKey.

   增加这项配置到web.config以使用SQL Server会话状态:
   <configuration>
    <system.web>
     <sessionState 
         mode="SQLServer" 
         sqlConnectionString="Data Source=localhost\sqlexpress;Integrated Security=True" />
    </system.web>
   </configuration>

   增加这项配置到web.config以使用指定的decryptionKey和validationKey:
   <configuration>
    <system.web>
     <machineKey 
         decryptionKey="EDCDA6DF458176504BBCC720A4E29348E252E652591179E2" 
         validationKey="CC482ED6B5D3569819B3C8F07AC3FA855B2FED7F0130F55D8405597C796457A2F5162D35C69B61F257DB5EFE6BC4F6CEBDD23A4118C4519F55185CB5EB3DFE61"/>
    </system.web>
   </configuration>

3. 编写SharedSessionModule模块实现共享会话的逻辑 

   a. 实现Init() 方法配置应用程序Id自web.config读取.

   b. 实现PostRequestHandlerExecute事件来存储会话ID到同一域和根路径的Cookie.

4. 配置ASP.NET Web应用程序以使用SharedSessionModule模块.
   
   增加这项配置到web.config以使用SharedSessionModule模块:
   <configuration>
    <system.web>
     <httpModules>
       <add 
         name="SharedSessionModule" 
         type="CSASPNETShareSessionBetweenSubDomainsModule.SharedSessionModule, CSASPNETShareSessionBetweenSubDomainsModule, Version=1.0.0.0, Culture=neutral"/>
     </httpModules>
    </system.web>
    <appSettings>
      <add key="ApplicationName" value="MySampleWebSite"/>
      <add key="RootDomain" value="localhost"/>
    </appSettings>
   </configuration>

5. 运行和测试
   
   a. 添加一个新的Web页面.
   b. 添加两个按钮(用于刷新页面,设置会话)和一个用于显示会话值的Label.
   c. 在Page_PreRender()方法中, 读取会话并显示到Label. 在Button单击事件中,设定值到会话.
   d. 使用与Web站点1同样的设置创建一个新的Web站点, 但在会话中设定不同值
   e. 在两个标签中打开两个站点. 现在如果你设定会话值为站点1,
      你可以在站点2获得同样的值. 因为他们使用同样的会话.

[1] 从Sql Server移除会话支持.
    运行"C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssremove"
    从Sql Server移除会话状态支持.

//////////////////////////////////////////////////////////////////////////////
参考资料:

ASP.NET Session State
http://msdn.microsoft.com/zh-cn/library/ms972429.aspx

ASP.NET SQL Server 注册工具 (Aspnet_regsql.exe) 
http://msdn.microsoft.com/zh-cn/library/ms229862(VS.80).aspx

ASP.NET Cookies 概述
http://msdn.microsoft.com/zh-cn/library/ms178194.aspx

HOW TO：使用 Visual Basic .NET 创建 ASP.NET HTTP 模块
http://support.microsoft.com/kb/308000

Application Pool Identities
http://learn.iis.net/page.aspx/624/application-pool-identities/