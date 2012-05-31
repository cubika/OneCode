
控制台应用程序:  控制台自动化Excel项目概述
/////////////////////////////////////////////////////////////////////////////

摘要：


CSAutomateExcel 案例阐述了通过Visual  C#代码生成Excel实例、填充数据到指定区域、创建，保存工作簿
以及关闭Excel应用程序并释放非托管的COM资源的相关过程。


Office 自动化建立在组件对象模型（COM）的基础上。当从托管代码中调用Office相关的COM对象时，
将自动生成一个运行库可调用包装（RCW）。RCW掌管.NET应用程序与COM对象间的调用，
它保存对COM对象进行引用的数量值。如果RCW上并非所有的引用都被释放，那么Office的COM对象将不会退出，
这也将导致Office应用程序在自动化运行后无法终止。为了确保Office应用程序完全退出，示例提供了两种解决方案。


解决方案1.  AutomateExcel 通过Microsoft Excel 主要的互用组件（PIA）自动化Excel应用程序，并且
将每个COM访问对象分配给一个新的变量，使用户最终可以通过调用Marshal.FinalReleaseComObject方法
释放这些变量。


解决方案2.  AutomateExcel 通过Microsoft Excel 主要的互用组件（PIA）自动化Excel应用程序，
在自动化方法退出堆栈后执行垃圾收集器（此时RCW对象不再被引用），从而清除RCW并释放COM对象。

/////////////////////////////////////////////////////////////////////////////

先决条件：

必须在装有Microsoft Excel 2007的计算机上运行该代码案例。
/////////////////////////////////////////////////////////////////////////////

演示：

下面的步骤阐述了Excel自动化样例的运行流程，即启动一个Excel 实例，生成工作簿，填充数据到指定区域，
保存工作簿并最终彻底退出Excel程序。

步骤1.  在Visual Studio 2008中成功建立示例项目后，将获得名为CSAutomateExcel.exe的应用程序。

步骤2. 打开Windows任务管理器（通过Ctrl+Shift+Esc）并确认没有正在执行的Excel程序。

步骤3. 运行程序。若无异常抛出，将在控制台窗口中打印如下内容。
  Excel.Application is started                                               // Excel.Application已启动
  A new workbook is created                                               //  新的Workbook被创建
  The active worksheet is renamed as Report                 //  处于活动状态的worksheet 被重命名为Report
  Filling data into the worksheet ...                                    //  将数据填充到worksheet
  Save and close the workbook                                          //  保存并关闭worksheet
  Quit the Excel application                                                //  退出Excel 应用程序

  Excel.Application is started                                            // Excel.Application已启动
  A new workbook is created                                             //  新的Workbook被创建
  The active worksheet is renamed as Report               //  处于活动状态的worksheet 被重命名为Report
  Filling data into the worksheet ...                                  //  将数据填充到worksheet
  Save and close the workbook                                        //  保存并关闭worksheet
  Quit the Excel application                                               //  退出Excel 应用程序

至此，在应用程序的目录中，你将看见两个分别名为Sample.xlsx和Sample2.xlsx的工作簿。
每个工作簿都各自包含一个名为”Report”的工作表。工作表内区域A1:C6间的数据如下。

  First Name   Last Name   Full Name                            //        姓                名               全名
  John         Smith       John Smith                                   //      John         Smith       John Smith
  Tom          Brown       Tom Brown                                 //      Tom          Brown      Tom Brown
  Sue          Thomas      Sue Thomas                              //       Sue          Thomas    Sue Thomas            
  Jane         Jones       Jane Jones                                 //       Jane         Jones        Jane Jones               
  Adam         Johnson     Adam Johnson                      //       Adam         Johnson     Adam Johnson                     

步骤4. 在Windows任务管理器中，确认Excel.exe进程不存在。例如，Excel实例已被正常关闭或移除。

/////////////////////////////////////////////////////////////////////////////
相关示例：

C#自动化Excel-VB自动化Excel-C++自动化Excel

这些示例采用不同的编程语言自动化Excel并完成相同的任务。

/////////////////////////////////////////////////////////////////////////////
创建过程：

步骤1. 创建一个控制台应用程序并引用Excel主要的互用组件（PIA）。
引用PIA时，右键点击工程文件选择“添加引用…”按钮。在添加引用对话框中切换到.NET 标签，
找到Microsoft.Office.Interop.Excel12.0.0.0后，点击OK。

步骤2. 导入并重命名Excel   interop命名空间：

	using Excel = Microsoft.Office.Interop.Excel;

步骤3. 通过创建一个Excel.Application 对象启动Excel应用程序。

	Excel.Application oXL = new Excel.Application();

步骤4. 通过Application.Workbooks获取工作簿集合对象，并通过该对象的Add方法生成一个新的工作簿。
这里Add方法将返回一个工作簿对象。
	oWBs = oXL.Workbooks;
	oWB = oWBs.Add(missing);

步骤5. 通过Workbook.ActiveSheet获取处于激活状态的工作表，并为其设置表名。
	oSheet = oWB.ActiveSheet as Excel.Worksheet;
	oSheet.Name = "Report";

步骤6. 构建包含一些姓名数据的二维数组saNames。并将该数组内容赋值给对象oRng1的Value2属性，
该对象代表工作表A2到B6间的区域。
	string[,] saNames = new string[,] {
		{"John", "Smith"}, 
		{"Tom", "Brown"}, 
		{"Sue", "Thomas"}, 
		{"Jane", "Jones"}, 
		{"Adam", "Johnson"}};

	oRng1 = oSheet.get_Range("A2", "B6");
	oRng1.Value2 = saNames;

步骤7. 通过设置正确的区域公式，组合姓与名的信息，从而生成姓名列。
	oRng2 = oSheet.get_Range("C2", "C6");
	oRng2.Formula = "=A2 & \" \" & B2";

步骤8. 调用方法workbook.Save将工作簿保存为一个本地文件。然后调用workbook.Close关闭工作簿，
并通过application.Quit退出应用程序。
	oWB.SaveAs(fileName, Excel.XlFileFormat.xlOpenXMLWorkbook,
		missing, missing, missing, missing,
		Excel.XlSaveAsAccessMode.xlNoChange,
		missing, missing, missing, missing, missing);
	oWB.Close(missing, missing, missing);

	oXL.UserControl = true;
	oXL.Quit();

步骤9. 释放非托管COM资源。 为了使Excel正常终止，必须在我们使用的每个COM对象上
调用Marshal.FinalReleaseComObject()方法。 我们可以直接在所有的访问对象上调用
Marshal.FinalReleaseComObject方法：
	// See Solution1.AutomateExcel
	if (oRng2 != null)
	{
		Marshal.FinalReleaseComObject(oRng2);
		oRng2 = null;
	}
	if (oRng1 != null)
	{
		Marshal.FinalReleaseComObject(oRng1);
		oRng1 = null;
	}
	if (oCells != null)
	{
		Marshal.FinalReleaseComObject(oCells);
		oCells = null;
	}
	if (oSheet != null)
	{
		Marshal.FinalReleaseComObject(oSheet);
		oSheet = null;
	}
	if (oWB != null)
	{
		Marshal.FinalReleaseComObject(oWB);
		oWB = null;
	}
	if (oWBs != null)
	{
		Marshal.FinalReleaseComObject(oWBs);
		oWBs = null;
	}
	if (oXL != null)
	{
		Marshal.FinalReleaseComObject(oXL);
		oXL = null;
	}

或者也可以在调用方法退出堆栈后（这时这些对象将不再被引用）启动垃圾收集器，
调用GC.WaitForPendingFinalizers方法。
	
                     //见Solution2.AutomateExcel

	GC.Collect();
	GC.WaitForPendingFinalizers();
	
                   //为了终止程序，垃圾收集器GC必须被调用两次。第一次调用将生成要终止项的相关列表，
                   //  第二次则是执行终止命令，此时对象将自动执行COM对象资源的释放。
	GC.Collect();
	GC.WaitForPendingFinalizers();


/////////////////////////////////////////////////////////////////////////////
参考资料：
MSDN: Excel 2007 Developer Reference
http://msdn.microsoft.com/en-us/library/bb149067.aspx

How to automate Microsoft Excel from Microsoft Visual C# .NET
http://support.microsoft.com/kb/302084

How to terminate Excel process after automation
http://blogs.msdn.com/geoffda/archive/2007/09/07/the-designer-process-that-would-not-terminate-part-2.aspx

How To Use Automation to Get and to Set Office Document Properties with 
Visual C# .NET
http://support.microsoft.com/kb/303296


/////////////////////////////////////////////////////////////////////////////