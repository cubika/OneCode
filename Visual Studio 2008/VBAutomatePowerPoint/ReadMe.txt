========================================================================
	控制台程序：VBAutomatePowerPoint项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
摘要：

VBAutomatePowerPoint案例演示了怎样使用VB.NET代码来创建一个Microsoft PowerPoint
实例，创建一个演示文稿，添加一个新的幻灯片，向幻灯片中加入一些文本，保存演示文
稿，退出Microsoft PowerPoint并进行非托管的COM资源的清理。

Office自动化是基于组件对象模型（COM）的。当你通过托管代码调用COM对象的时候，会
自动产生一个运行时可调用包装（RCW）对象。RCW封送了.NET应用程序和COM对象之间的
调用。RCW持有对COM对象的引用计数。如果RCW对象的所有引用没有被完全释放，则Office
的COM对象也不会退出，而可能致使Office应用程序在自动化结束后也不能退出。为了确认
Office应用程序完全退出，示例中演示了两种解决方案。

解决方案1.AutomatePowerPoint 演示了自动化的Microsoft PowerPoint应用程序通过使用
Microsoft PowerPoint主互操作程序集（PIA），和显式地将每一个COM存取器赋予一个新
的变量（这些变量需要在程序结束时显式的调用Marshal.FinalReleaseComObject方法来释
放它）。

解决方案2.AutomatePowerPoint 演示了自动化Microsoft PowerPoint应用程序通过使用
Microsoft PowerPoint PIA，并在自动化功能函数弹出栈后（在此时的RCW对象都是不可达
的）就开始强制一次垃圾回收来清理RCW对象和释放COM对象。


/////////////////////////////////////////////////////////////////////////////
先决条件：

运行此代码示例，计算机上必须安装有Microsoft PowerPoint 2007。


/////////////////////////////////////////////////////////////////////////////
演示：

通过了以下步骤来做一个PowerPoint自动化示例的演示：开始于一个Microsoft 
PowerPoint实例，添加一个新的演示文稿，插入一个幻灯片，在幻灯片上加入一些文本，
保存演示文稿，退出PowerPoint并清理非托管的COM资源。

步骤1.当您在Visual Studio 2008中成功创建了一个示例项目，您会得到一个应用程序：
VBAutomatePowerPoint.exe。

步骤2.打开Windows任务管理器(Ctrl+Shift+Esc)，确认没有正在运行的powerpnt.exe。 

步骤3.运行应用程序。如果没有错误被抛出，程序应该在控制台打印出以下内容。

  一个新的演示文稿被建立
  插入一个幻灯片
  添加一些文本
  保存并退出演示文稿
  退出PowerPoint应用程序

  一个新的演示文稿被建立
  插入一个幻灯片
  添加一些文本
  保存并退出演示文稿
  退出PowerPoint应用程序

之后，您会发现在应用程序的目录下出现了两个新的演示文稿：Sample1.pptx和
Sample2.pptx。这两个演示文稿都是只含有一个如下标题的幻灯片。

  一站式代码框架

步骤4.查看Windows任务管理器，确认powerpnt.exe进程已经不存在了，即Microsoft 
PowerPoint实例已经关闭并彻底的被清理了。


/////////////////////////////////////////////////////////////////////////////
相关示例：

VBAutomatePowerPoint - CSAutomatePowerPoint - CppAutomatePowerPoint

这些Microsoft PowerPoint自动化案例使用不同语言去做相同的事情。


/////////////////////////////////////////////////////////////////////////////
创建：

步骤1.创建一个控制台应用程序并引用PowerPoint主互操作程序集（PIA）和Office 12。
为了引用PowerPoint PIA和Office 12，可以右击项目文件，然后点击"Add Reference..."
按钮。在添加引用的对话框中，定位到.NET选项卡，找到
Microsoft.Office.Interop.PowerPoint 12.0.0.0和Office 12.0.0.0，点击OK。


步骤2.导入并重命名PowerPoint interop和Office命名空间：

	Imports Office = Microsoft.Office.Core
	Imports PowerPoint = Microsoft.Office.Interop.PowerPoint

步骤3.创建一个PowerPoint.Application对象来启动一个PowerPoint应用程序。

	oPowerPoint = New PowerPoint.Application()

默认情况下PowerPoint不可见，直到你使它可见。

	' 使PowerPoint实例不可见
	oPowerPoint.Visible = Office.MsoTriState.msoFalse
	' 或者使PowerPoint实例可见
	oPowerPoint.Visible = Office.MsoTriState.msoTrue

步骤4.从Application.Presentations获取演示文稿（Presentations）集合，并调用Add
函数添加一个新演示文稿。Add函数返回一个演示文稿对象。

	oPres = oPowerPoint.Presentations
	oPre = oPres.Add()

步骤5.调用Presentation.Slides集合的Add方法插入一个幻灯片，并为幻灯片加入一些文
本。

	oSlides = oPre.Slides
	oSlide = oSlides.Add(1, PowerPoint.PpSlideLayout.ppLayoutText)

	oShapes = oSlide.Shapes
	oShape = oShapes(1)
	oTxtFrame = oShape.TextFrame
	oTxtRange = oTxtFrame.TextRange
	oTxtRange.Text = "一站式代码框架"

步骤6.保存此演示文稿为pptx文件并将其关闭。

	Dim fileName As String = Path.GetDirectoryName( _
	Assembly.GetExecutingAssembly().Location) + "\\Sample1.pptx"
	oPre.SaveAs(fileName, PowerPoint.PpSaveAsFileType.ppSaveAsOpenXMLPresentation, _
			Office.MsoTriState.msoTriStateMixed)
	oPre.Close()

步骤7.退出PowerPoint应用程序。

	oPowerPoint.Quit()

步骤8.清除非托管的COM资源。在到达PowerPoint终结时刻，我们需要调用
Marshal.FinalReleaseComObject()方法访问每一个我们使用过的COM对象。我们也可以
显式地调用Marshal.FinalReleaseComObject访问所有的存取器对象。

	' 参见解决方案1.AutomatePowerPoint
	If Not oTxtRange Is Nothing Then
		Marshal.FinalReleaseComObject(oTxtRange)
		oTxtRange = Nothing
	End If
	If Not oTxtFrame Is Nothing Then
		Marshal.FinalReleaseComObject(oTxtFrame)
		oTxtFrame = Nothing
	End If
	If Not oShape Is Nothing Then
		Marshal.FinalReleaseComObject(oShape)
		oShape = Nothing
	End If
	If Not oShapes Is Nothing Then
		Marshal.FinalReleaseComObject(oShapes)
		oShapes = Nothing
	End If
	If Not oSlide Is Nothing Then
		Marshal.FinalReleaseComObject(oSlide)
		oSlide = Nothing
	End If
	If Not oSlides Is Nothing Then
		Marshal.FinalReleaseComObject(oSlides)
		oSlides = Nothing
	End If
	If Not oPre Is Nothing Then
		Marshal.FinalReleaseComObject(oPre)
		oPre = Nothing
	End If
	If Not oPres Is Nothing Then
		Marshal.FinalReleaseComObject(oPres)
		oPres = Nothing
	End If
	If Not oPowerPoint Is Nothing Then
		Marshal.FinalReleaseComObject(oPowerPoint)
		oPowerPoint = Nothing
	End If

并/或在调用的方法弹出栈时（此时的这些对象都不在可达）强制一次垃圾收集，然后调
用GC.WaitForPendingFinalizers方法。

	' 参见解决方案2.AutomatePowerPoint
	GC.Collect();
	GC.WaitForPendingFinalizers();
	' 为了使终结器（Finalizers）被调用，需要两次调用GC
	' 第一次调用，它只简单的列出需要被终结的对象， 
	' 第二次调用，已经终结化了。 
	' 只有在那时，对象才会自动执行它们的ReleaseComObject方法。
	GC.Collect();
	GC.WaitForPendingFinalizers();


/////////////////////////////////////////////////////////////////////////////
参考资料：

MSDN: PowerPoint 2007 Developer Reference
http://msdn.microsoft.com/en-us/library/bb265982.aspx


/////////////////////////////////////////////////////////////////////////////
