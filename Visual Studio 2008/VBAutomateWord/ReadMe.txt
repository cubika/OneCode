========================================================================
                控制台应用程序 : VBAutomateWord 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

VBAutomateWord案例阐述了如何使用Visual Basic.NET代码生成Microsoft Word 实例，构建新的
Word文档，插入段落、表格，保存文档，关闭Word应用程序及释放非托管COM资源的过程。 

Office 自动化建立在组件对象模型（COM）的基础上。当从托管代码中调用Office相关的COM对象时，将自动生成
一个运行库可调用包装（RCW）。RCW掌管.NET应用程序与COM对象间的调用，它保存对COM对象进行引用的数量值。
如果RCW上并非所有的引用都被释放，那么Office的COM对象将不会退出，这也将导致Office应用程序在自动化
运行后无法终止。为了确保Office应用程序完全退出，示例提供了两种解决方案。


解决方案1. AutomateWord 通过Microsoft Word主要的互用组件（PIA）自动化Word应用程序，并且将每个COM
访问对象分配给一个新的变量，使用户最终可以通过调用Marshal.FinalReleaseComObject方法释放这些变量。


解决方案2. AutomateWord 通过Microsoft Word主要的互用组件（PIA）自动化Word应用程序，在自动化方法
退出堆栈后执行垃圾收集器（此时RCW对象不再被引用），从而清除RCW并释放COM对象。

/////////////////////////////////////////////////////////////////////////////
先决条件：

必须在装有Microsoft Word 2007的计算机上运行该代码案例。


/////////////////////////////////////////////////////////////////////////////
演示：

下面的步骤阐述了Word自动化样例的运行流程，即启动一个Word 实例，创建新的Word文档，插入段落及表格，
保存文档并完全退出Word应用程序。

步骤1. 在Visual Studio 2008中成功建立示例项目后，将获得名为 VBAutomateWord.exe 的应用程序。

步骤2. 打开Windows任务管理器（通过Ctrl+Shift+Esc）并确认没有正在执行的Winword.exe程序。

步骤3. 运行程序。若无异常抛出，将在控制台窗口中打印如下内容。


  Word.Application is started                   // Word.Application已启动
  A new document is created                     // 新的Word文档已被创建
  Insert a paragraph                            // 插入一个段落
  Save and close the document                   // 保存并关闭Word文档
  Quit the Word application                     // 退出Word应用程序

  Word.Application is started                   // Word.Application已启动
  Insert a paragraph                            // 插入一个段落
  Insert a table                                // 插入表格
  Save and close the document                   // 保存并关闭Word文档
  Quit the Word application                     // 退出Word应用程序

至此，在应用程序的目录中，你将看见两个分别名为Sample1.docx和Sample2.docx的新文档。它们均包含如下内容：

  Heading 1

Sample2.docx 另外还包含下面的表格

  r1c1	r1c2
  r2c1	r2c2
  r3c1	r3c2
  r4c1	r4c2
  r5c1	r5c2

步骤4. 在Windows任务管理器中，确认winword.exe进程不存在。例如，Microsoft Word 实例已被正常关闭或移除。


/////////////////////////////////////////////////////////////////////////////
相关示例：

C#自动化Word-C++自动化Word-VB自动化Word

这些示例采用不同的编程语言自动化Microsoft Word并完成相同的任务。

/////////////////////////////////////////////////////////////////////////////
创建过程：

步骤1. 创建一个控制台应用程序并引用Word主要的互用组件（PIA）。为了引用PIA，右键点击工程文件选择“添加引用…”按钮。
在添加引用对话框中切换到.NET 标签，找到Microsoft.Office.Interop.Word 12.0.0.0后，点击OK。 


步骤2. 导入并重命名Word互用组件命名空间：

	Imports Word = Microsoft.Office.Interop.Word

步骤3. 通过创建一个Word.Application 对象启动Word应用程序。

	oWord = New Word.Application()

步骤4. 通过Application.Documents获取文档集合并调用其上的Add方法生成一个新的文档。 Add方法将
返回一个文档对象。

步骤5. 插入段落。

	oParas = oDoc.Paragraphs
	oPara = oParas.Add()
	oParaRng = oPara.Range
	oParaRng.Text = "Heading 1"
	oFont = oParaRng.Font
	oFont.Bold = 1
	oParaRng.InsertParagraphAfter()

步骤6. 插入表格。

下面的代码存在的问题是当执行对象访问时将创建RCW并引用它们。例如，调用Document.Bookmarks.Item 为
Bookmarks对象创建了RCW。 如果以代码所采用的层级式回溯方式引用这些访问对象,RCW将被创建在GC堆上,而
引用则创建在栈上,然后被丢弃。 这样，将无法在RCW上调用MarshalFinalReleaseComObject。为了使该类型
的RCW得以释放,一种方法是在调用函数退出堆栈后立刻执行垃圾收集器GC（此时RCW对象将不再被引用）,另一种方法
则是显式地将每个访问对象分配到一个变量,再通过调用Marshal.FinalReleaseComObject释放变量。

	oBookmarkRng = oDoc.Bookmarks.Item("\endofdoc").Range

	oTable = oDoc.Tables.Add(oBookmarkRng, 5, 2)
	oTable.Range.ParagraphFormat.SpaceAfter = 6

	For r As Integer = 1 To 5
		For c As Integer = 1 To 2
			oTable.Cell(r, c).Range.Text = "r" & r & "c" & c
		Next
	Next

	' 改变列1和列2的宽度
	oTable.Columns(1).Width = oWord.InchesToPoints(2)
	oTable.Columns(2).Width = oWord.InchesToPoints(3)

步骤7. 将文档保存为.docx文件并关闭。

	Dim fileName As String = Path.GetDirectoryName( _
	Assembly.GetExecutingAssembly().Location) & "\Sample1.docx"
	oDoc.SaveAs(fileName, Word.WdSaveFormat.wdFormatXMLDocument)
	oDoc.Close()

步骤8. 退出Word应用程序。

	oWord.Quit(False)

步骤9. 释放非托管COM资源。 为了使Word正常终止，必须在我们使用的每个COM对象上调用
Marshal.FinalReleaseComObject()方法。 我们也可以直接在所有的访问对象上调用
Marshal.FinalReleaseComObject方法：


	' 见 Solution1.AutomateWord
	If Not oFont Is Nothing Then
		Marshal.FinalReleaseComObject(oFont)
		oFont = Nothing
	End If
	If Not oParaRng Is Nothing Then
		Marshal.FinalReleaseComObject(oParaRng)
		oParaRng = Nothing
	End If
	If Not oPara Is Nothing Then
		Marshal.FinalReleaseComObject(oPara)
		oPara = Nothing
	End If
	If Not oParas Is Nothing Then
		Marshal.FinalReleaseComObject(oParas)
		oParas = Nothing
	End If
	If Not oDoc Is Nothing Then
		Marshal.FinalReleaseComObject(oDoc)
		oDoc = Nothing
	End If
	If Not oDocs Is Nothing Then
		Marshal.FinalReleaseComObject(oDocs)
		oDocs = Nothing
	End If
	If Not oWord Is Nothing Then
		Marshal.FinalReleaseComObject(oWord)
		oWord = Nothing
	End If

或者也可以在调用方法退出堆栈后（这时这些对象将不再被引用）启动垃圾收集器，调用GC.WaitForPendingFinalizers方法。

	' 见 Solution2.AutomateWord
	GC.Collect()
	GC.WaitForPendingFinalizers()

	' 为了终止程序，垃圾收集器GC必须被调用两次。第一次调用将生成要终止项的相关列表，
	' 第二次则是执行终止命令，此时对象将自动执行COM对象资源的释放。
	GC.Collect()
	GC.WaitForPendingFinalizers()


/////////////////////////////////////////////////////////////////////////////
参考资料：

MSDN: Word 2007 Developer Reference
http://msdn.microsoft.com/en-us/library/bb244391.aspx

How to automate Word from Visual Basic .NET to create a new document
http://support.microsoft.com/kb/316383/


/////////////////////////////////////////////////////////////////////////////