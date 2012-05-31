========================================================================
    控制台应用程序：CppAutomatePowerPoint项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

CppAutomatePowerPoint案例演示了怎样使用VC++代码来创建一个Microsoft 
PowerPoint实例，创建一个演示文稿，添加一个新的幻灯片，向幻灯片中加入一些文本，
保存演示文稿，退出Microsoft PowerPoint并进行非托管的COM资源的清理。

你可以使用VC++代码通过这里提供的三种基本方法实现自动化应用:

1. 使用#import指令智能指针的自动化PowerPoint

Solution1.h/cpp中的代码演示了#import在自动化的PowerPoint中的使用。#import
(http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx),从Visual C++ 5.0开始使用
的一个新指令，用来从一个规定的类型库创建VC++“智能指针”。它很强大，但由于与
Microsoft Office应用程序协同操作时一般都有引用计数的问题，所以在此时经常不推荐使
用此功能。与Solution2.h/cpp中使用的直接调用API不同，智能指针使我们受益于早期后期
绑定对象的类型信息。#import将复杂的guid加入项目，并且COM API都被封装在#import指令
生成的自定义类中。


2. 使用C++和COM API的自动化PowerPoint

Solution2.h/cpp中的代码演示了使用C/C++和COM API自动化操作PowerPoint。原始的自动化
非常难操作，但是有时又需要避免使用MFC带来的开销和#import的问题。基本上，你在工作中
使用的是一些这样的API，如CoCreateInstance()和COM接口（如IDispatch和IUnknown）。

3. 使用MFC创建自动化的PowerPoint

通过MFC，Visual C++类向导会根据类库生成“封装类”。这些类使COM服务的操作简单化。
使用MFC的自动化操作PowerPoint在此示例中没有被包含。


/////////////////////////////////////////////////////////////////////////////
先决条件：

运行此代码示例，计算机上必须安装有Microsoft PowerPoint 2007。


/////////////////////////////////////////////////////////////////////////////
演示：

通过了以下步骤来做一个PowerPoint自动化示例的演示：开始于一个Microsoft 
PowerPoint实例，添加一个新的演示文稿，插入一个幻灯片，在幻灯片上加入一些文本，
保存演示文稿，退出PowerPoint并清理非托管的COM资源。

步骤1.当您在Visual Studio 2008中成功创建了一个示例项目，您会得到一个应用程序：
CppAutomatePowerPoint.exe。

步骤2.打开Windows任务管理器(Ctrl+Shift+Esc)，确认没有正在运行的powerpnt.exe。 

步骤3.运行应用程序。如果没有错误被抛出，程序应该在控制台打印出以下内容。

  PowerPoint.Application已经启动
  一个新的演示文稿被建立
  插入一个幻灯片
  添加一些文本
  保存并关闭演示文稿
  退出PowerPoint应用程序

  PowerPoint.Application已经启动
  一个新的演示文稿被建立
  演示文稿现在有0个幻灯片
  插入一个幻灯片
  添加一些文本
  保存并关闭演示文稿
  退出PowerPoint应用程序

之后，您会发现在应用程序的目录下出现了两个新的演示文稿：Sample1.pptx和
Sample2.pptx。这两个演示文稿都是只含有一个如下标题的幻灯片。

  一站式代码框架

步骤4.查看Windows任务管理器，确认powerpnt.exe进程已经不存在了，即Microsoft 
PowerPoint实例已经关闭并彻底的被清理了。


/////////////////////////////////////////////////////////////////////////////
项目关联:

CppAutomatePowerPoint - CSAutomatePowerPoint - VBAutomatePowerPoint

这些Microsoft PowerPoint自动化案例使用不同语言去做相同的事情。


/////////////////////////////////////////////////////////////////////////////
创建:

A. 使用#import指令智能指针的自动化PowerPoint 
(Solution1.h/cpp)

步骤1.使用#import指令导入目标COM服务的类型库。

	#import "libid:2DF8D04C-5BFA-101B-BDE5-00AA0044DE52" \
		rename("RGB", "MSORGB")
		rename("DocumentProperties", "MSODocumentProperties")
	// [-或-]
	//#import "C:\\Program Files\\Common Files\\Microsoft Shared\\OFFICE12\\MSO.DLL" \
	//	rename("RGB", "MSORGB")
	//	rename("DocumentProperties", "MSODocumentProperties")

	using namespace Office;

	#import "libid:0002E157-0000-0000-C000-000000000046"
	// [-或-]
	//#import "C:\\Program Files\\Common Files\\Microsoft Shared\\VBA\\VBA6\\VBE6EXT.OLB"

	using namespace VBIDE;

	#import "libid:91493440-5A91-11CF-8700-00AA0060263B"
	// [-或-]
	//#import "C:\\Program Files\\Microsoft Office\\Office12\\MSPPT.OLB"

步骤2.建立项目。如果创建成功，编译器会生成包含COM服务器基于#import指令中规定
的类型库的.tlh和.tli文件。它充当一个类封装器，我们现在可以使用它创建COM类并访
问其属性，方法，等等。

步骤3.初始化当前线程上的COM库并通过调用CoInitializeEx或CoInitialize标识并发模
型为但线程单元（STA）。

步骤4.使用智能指针创建PowerPoint.Application COM对象。类名是原始接口名
（如PowerPoint::_Application）后跟一个“Ptr”后缀。我们可以使用智能指针类构造器
或它的CreateInstance方法来创建它的COM对象。

步骤5.通过智能指针自动化这个PowerPoint COM对象。在此例中，你会接触到PowerPoint
自动化的基本操作，如

	创建一个新的演示文稿。 (即Application.Presentations.Add)
	插入一个幻灯片。
	添加一些文本,
	保存此演示文稿为pptx文件并关闭它。

步骤6.退出PowerPoint应用程序。(即Application.Quit())

步骤7.因为智能指针是被自动释放的，所以我们不需要手动释放这些COM对象。

步骤8.如果类型库被引用的同时而没有raw_interfaces_only和原始接口（如raw_Quit）
没有被使用时需要捕捉COM异常。比如：

	#import "XXXX.tlb"
	try
	{
		spPpApp->Quit();
	}
	catch (_com_error &err)
	{
	}

步骤9.调用CoUninitialize取消此线程上的COM初始化。

-----------------------------------------------------------------------------

B. 使用C++和COM API的自动化PowerPoint(Solution2.h/cpp)

步骤1.加入自动化帮助功能，自动封装。

步骤2.调用CoInitializeEx或CoInitialize初始化COM。

步骤3.通过API CLSIDFromProgID获得PowerPoint COM服务的CLSID。

步骤4.启动PowerPoint COM服务，通过API CoCreateInstance获得IDispatch接口。

步骤5.通过自动封装的帮助自动化PowerPoint COM对象。在此例中，你会接触到PowerPoint
自动化的基本操作，如

	创建一个新的演示文稿。 (即Application.Presentations.Add)
	插入一个幻灯片。
	添加一些文本,
	保存此演示文稿为pptx文件并关闭它。

步骤6.退出PowerPoint应用程序。(即Application.Quit())

步骤7.释放COM对象。

步骤8.调用CoUninitialize取消此线程上的COM初始化。


/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: PowerPoint 2007 Developer Reference
http://msdn.microsoft.com/en-us/library/bb265982.aspx


/////////////////////////////////////////////////////////////////////////////
