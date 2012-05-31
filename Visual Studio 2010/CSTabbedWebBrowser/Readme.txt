=============================================================================
       Windows 应用程序: CSTabbedWebBrowser 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

该示例演示如何创建一个标签web浏览器。

在WebBorwser中“在新建选项卡中打开”默认状态下是被禁用的。
你可以向键值 HKCU\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING中，
添加一个*.exe=1(*表示该进程的名字)

当应用程序重新启动后，这个菜单才会生效。
参见： http://msdn.microsoft.com/en-us/library/ms537636(VS.85).aspx

DWebBrowserEvents2 接口 指定一个接口为事件接收器 ，应用程序必须从底层的 Activex控件接受事件通知，
并且在这个接口中有一个NewWindow3事件。

 参见：http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx


/////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在 VS2010中建立这个项目。

步骤2. 运行 CSTabbedWebBrowser.exe.

步骤3. 在地址栏中输入 http://1code.codeplex.com/ ,然后点击按钮“确定”.

步骤4. 在页头中右键点击“下载”, 然后点击“在新选项卡中打开”
       应用程序会在另一个新的选项卡中打开链接。
       

	   如果“在新选项卡中打开”被禁用, 选中 "启用选项卡" 然后重新启动应用程序。


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 设计一个WebBrowserEx类，并且继承System.Windows.Forms.WebBrowser类。
   这个类可以处理NewWindow3事件。
  

   DWebBrowserEvents2接口指定一个接口为事件接收器，应用程序必须实现从WebBrowser控件，
   或者Windows Internet Explorer应用程序中，接收事件通知。
   该事件通知包括 NewWindow3事件，NewWindow3在应用程序中将用到。
    

2. 设计一个WebBrowserTabPage类，并且继承System.Windows.Forms.TabPage类，同时包含一个WebBrowserEx属性。
   该类的一个实例可以直接添加到标签控件中。
       
3. 创建一个UserControl,并且包含一个System.Windows.Forms.TabControl实例。
   在TabControl中UserControl提供一个方法，进行创建/关闭WebBrowserTabPage。
   它还提供一个IsTabEnabled属性来获取或设置在上下文菜单“在新标签中打开“中 web浏览器是否已启用。


4. 在MainForm中，它提供控制启用/禁用标签，创建/关闭标签，同时可以使WebBrowser 后退，向前或刷新。



/////////////////////////////////////////////////////////////////////////////
参考资料:

http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx
http://msdn.microsoft.com/en-us/library/ms537636(VS.85).aspx
http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.createsink.aspx


/////////////////////////////////////////////////////////////////////////////
