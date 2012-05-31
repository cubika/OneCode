================================================================================
       Windows 应用: VBWebBrowserWithProxy 概述                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:
这是个用来展示如何使用代理来进行网页浏览的例子。

浏览器控件是一个对浏览器ActiveX控件的可管理的封装，不管用户的计算机安装使用何种
版本的控件它都能使用。这就意味着这个浏览器只是IE的一个实例，所以它能使用IE关于
代理的设置。

从Internet探索者（就是IE了）5.0开始的以后的版本，都能设定Internet的连接选项，例如
局域网连接或ASDL连接。Winnet.dll包含了2个扩充方法（InternetSetOption和
InternetQueryOption）用来设置和恢复internet设置。
////////////////////////////////////////////////////////////////////////////////
示范:

第一步：建立一个Visual Studio 2010工程 

第二步：在PorxyList.xml中设置代理服务器
		下面是个样板： 
	   <ProxyList>
			<Proxy>
				<ProxyName>Proxy Name</ProxyName>
				<Address>Proxy url</Address>
				<UserName></UserName>
				<Password></Password>
			</Proxy> 
		</ProxyList>
		如果代理服务器需要授权认证则用户名和密码就必须填写
		
第三步：运行VBWebBrowserWithProxy.exe。

第四步：在界面顶部的文本框中输入 http://www.whatsmyip.us/ 或其他在你的IP地址上有效的网址

第五步：使用“无代理服务器”单击浏览按钮，浏览器会显示你的真实地址。

第六步：使用“有代理服务器”，在组合列表框中选择一个服务器，然后单击浏览按钮
		浏览器会显示通过代理的IP地址。


/////////////////////////////////////////////////////////////////////////////
代码设计思路:

1. 封装2个wininet.dll的扩充方法（InternetSetOption和InternetQueryOption）,设计好数据结构
   并用它们初始化常量。

2. 用WinINet类来设置、激活、备份、恢复internet选项。

3. WebBrowserControl继承了WebBrowser类并且具有设置代理的功能。
   原始的internet 设置将被备份，被指定的代理将在浏览过程中被
   使用，并且，在浏览过程被重置时，原始的internet 设置将被恢复。

4. 在应用程序开始执行时根据ProxyList.xml初始化代理服务器。 

5. 设置代理并通过单击浏览按钮去浏览你的链接。


/////////////////////////////////////////////////////////////////////////////
参考资料:
http://msdn.microsoft.com/en-us/library/aa385114(VS.85).aspx
http://msdn.microsoft.com/en-us/library/aa385101(VS.85).aspx
http://msdn.microsoft.com/en-us/library/aa385384(VS.85).aspx
http://msdn.microsoft.com/en-us/library/aa385145(VS.85).aspx

/////////////////////////////////////////////////////////////////////////////
