================================================================================
          Windows应用程序: VBWebBrowserSuppressError 项目概述                       
================================================================================
////////////////////////////////////////////////////////////////////////////////
摘要:
VBWebBrowserSuppressError案例阐述了如何让Web浏览器忽略错误。这些错误包括：

1. 调用脚本JIT调试器。

   function CallDebugger() {
       debugger;
   }

   这个可以通过键值HKCU\Software\Microsoft\Internet Explorer\Main中的DisableJITDebugger
   的值来被禁用。

   注意如果要禁用JIT调试器，应用程序需要重启。

2. Html元素错误。

   function CreateScriptError() {
       throw ("Here is an error! ");
   }

   您可以注册Document.Window.Error事件并处理它。
   
   注意只有在JIT调试器被禁用时Document.Window.Error事件才会生效。

3. 链接错误。比如页面不存在(Http 404 错误)。

   接口DWebBrowserEvents2指明了一个应用程序必须实现的一个事件接收器接口，用来从底层ActiveX
   控件接收事件通知.这些接口含有一个链接错误事件.见下面链接：
   http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx

4. 其它错误，比如Javascript中用到剪贴板时需要权限允许。

   如果您想忽略所有错误，可以将ScriptErrorsSuppressed属性设置成true。当ScriptErrorsSuppressed
   为true时，Web浏览器控件会隐藏它所有源自底层ActiveX控件的对话框。并不仅仅是脚本错误。有时
   你可能需要在显示（比如用于浏览器安全设置或用户登录的）对话框时忽略脚本错误。在这种情况下
   请在HtmlWindow.Error事件的处理程序中，将ScriptErrorsSuppressed设定为false并忽略脚本错误。

////////////////////////////////////////////////////////////////////////////////
演示:

禁用JIT调试器

步骤1. 运行VBWebBrowserSuppressError.exe

步骤2. 不要勾选"忽略JIT调试器",然后重启应用程序。
	   如果复选框已经处于未选中状态则可以跳过此步。

步骤3. 将顶部的输入框置空,然后点击按钮"开始"。这一步操作会让浏览器链接到一个内建的错误html页。

步骤4. 在页面中点击"启动调试器",如果安装了VS您将会看到启动JIT调试器的一个对话框。

步骤5. 勾选"忽略JIT调试器",然后重启应用程序。

Step6. 将顶部的文本框置空,然后点击按钮"开始"。

Step7. 在页面中点击"启动调试器",您将不会看到启动JIT调试器对话框。


忽略html元素错误

步骤1. 运行VBWebBrowserSuppressError.exe

步骤2. 勾选"忽略JIT调试器",然后重启应用程序。
       如果复选框已经选中则可以跳过此步。

步骤3. 将顶部的文本框置空,然后点击按钮"开始"。这一步操作会让浏览器链接到一个内建的错误html页。

步骤4. 不要勾选"忽略html元素错误"。

步骤5. 在页面中点击"脚本错误",您将会看到一个Web页面错误对话框。

步骤6. 勾选"忽略html元素错误"。

步骤7. 在页面中点击"脚本错误",您将不会看到Web页面错误对话框.
	 
	 

处理链接错误

步骤1. 运行VBWebBrowserSuppressError.exe

步骤2. 不要勾选"忽略链接错误"。

步骤3. 在顶部的文本框中输入 http://www.microsoft.com/NotExist.html， 然后点击按钮"开始"。
       Microsoft.com将会告诉您此页面找不到。

步骤4. 勾选"忽略链接错误"。

步骤5. 在顶部的文本框中输入 http://www.microsoft.com/NotExist.html， 然后点击按钮"开始"。
       你将会看到一个内建的HTTP404错误页面。



忽略所有对话框	   	    

步骤1. 运行VBWebBrowserSuppressError.exe

步骤2. 将顶部的输入框置空,然后点击按钮"开始"。这一步操作会让浏览器链接到一个内建的错误html页。

步骤3. 不要勾选"忽略所有对话框"。

步骤4. 在页面中点击"安全对话框"，你将会看到一个"Windows安全警告"对话框。

步骤5. 将顶部的输入框置空,然后点击按钮"开始"。或者在快捷菜单中刷新页面。

步骤6. 勾选"忽略所有对话框"。

步骤7. 在页面中点击"安全对话框"，你将不会看到"Windows安全警告"对话框。


////////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 创建html文件Error.htm.这个Web页面可以生成交本错误,安全警告并能启动JIT调试器.

2. 设计一个WebBrowserEx类继承自WebBrowser类，这个类提供了一下特征：
   
   2.1. 禁用JIT调试器。
        静态属性DisableJITDebugger可以获取或设置键值HKCU\Software\Microsoft\Internet Explorer\Main
		中的"禁用脚本调试器"属性的值。

   2.2. 忽略了浏览器中载入的html文档对象的html元素错误。
        处理此浏览器中载入的html文档对象的window错误事件。

   2.3. 处理链接错误。
        接口DWebBrowserEvents2指明了一个应用程序必须实现的一个事件接收器接口，用来从Web浏
		览器控件或者Window IE浏览器应用程序接收事件通知.这些事件通知包括这个应用程序中会用
		到的链接错误事件。

   2.4. WebBrowser类自身也有个ScriptErrorsSuppressed属性,用来隐藏它所有源于底层ActiveX控
        件的对话框,不仅仅只是脚本错误。
	   
3. 在主窗口中处理复选框选中状态改变事件，来禁用JIT调试器，忽略html元素错误，忽略所有对话框。
   
   同时注册WebBrowserEx类的链接错误事件。如果"忽略链接错误"被选中并且http状态代码是404，则
   会跳转到内建的404错误页面。


/////////////////////////////////////////////////////////////////////////////
参考资料:
http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx
http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.scripterrorssuppressed.aspx
http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.createsink.aspx
/////////////////////////////////////////////////////////////////////////////
