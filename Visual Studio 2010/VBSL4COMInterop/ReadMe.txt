========================================================================
          SILVERLIGHT 应用程序: VBSL4COMInterop 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用途:

本项目新建了一个简单的应用程序，它可以和COM组件进行交互操作，并且可以将数据导出
到记事本或者Microsoft Excel应用程序.

/////////////////////////////////////////////////////////////////////////////
代码测试:

测试silverlight4 COM相互交互特征，请按照以下步骤进行：
1.  使用评估许可在本地计算机安装silverlight应用程序.
    a. 打开VBSL4COMInterop解决方案并编译.
	b. 运行这个项目.
	c. 在浏览器中右击silverlight应用程序，选择“在本机安装VBSL4COMInterop应用程
	   序...”，然后在弹出的面板中单击“安装”按钮.
2.  在OOB模式下测试该应用程序
    a. 双击桌面快捷键“VBSL4COMInterop应用程序”启动这个应用程序.
	b. 通过操纵数据表格编辑数据.
	c. 点击按钮将数据导出到不同的应用程序.

/////////////////////////////////////////////////////////////////////////////
先决条件:

Visual Studio 2010 的Silverlight 4 工具RC2
http://www.microsoft.com/downloads/details.aspx?FamilyID=bf5ab940-c011-4bd1-ad98-da671e491009&displaylang=en

Silverilght 4 运行环境
http://silverlight.net/getstarted/

Microsoft Office 2007 更高版本
http://office.microsoft.com/zh-cn/default.aspx

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 在Silverlight里和COM交互操作的前提条件是什么？
    和COM交互操作时Silverlight4的新特征，为了开发COM交互操作Silverlight应用程序，
	我们需要使用Silverlight4 SDK.另外，为了和COM交互操作，Silverlight应用程序需要
	在受信任OOB模式下运行.
	关于受信任的应用程序，请参考
	http://msdn.microsoft.com/zh-cn/library/ee721083(VS.95).aspx

	为了确定COM交互操作是否可用，我们需要用到这个代码：
	System.Runtime.InteropServices.Automation.AutomationFactory.IsAvailable

2. 怎样在Silverlight里操纵Microsoft Word自动控制？
    1. 使用AutomationFactory新建Word应用程序自动控制对象.
	    Dim word = AutomationFactory.CreateObject("Word.Application")

	2. 新建Word文档，然后写入文本输入区并运用格式.
	    Dim doc = word.Documents.Add()
        Dim range1 = doc.Paragraphs[1].Range
        range1.Text = "Silverlight4 Word Automation Sample" & vbLf
        range1.Font.Size = 24
        range1.Font.Bold = true

	3. 打印文档
	    doc.PrintOut()

3. 怎样在Silverlight里操纵Windows Notepad？
    1.使用AutomationFactory新建WSHShell对象.
	    Dim shell = AutomationFactory.CreateObject("WScript.Shell")

	2. 运行Notepad.exe
	    shell.Run(@"%windir%\notepad", 5)

	3. 发送关键词到Notepad应用程序
	    shell.SendKeys("Name{Tab}Age{Tab}Gender{Enter}")
    
/////////////////////////////////////////////////////////////////////////////
相关资料:

MSDN：AutomationFactory类
http://msdn.microsoft.com/zh-cn/library/system.runtime.interopservices.automation.automationfactory(VS.95).aspx

MSDN：如何在受信任的应用程序中使用自动化
http://msdn.microsoft.com/zh-cn/library/ff457794(VS.95).aspx

/////////////////////////////////////////////////////////////////////////////
