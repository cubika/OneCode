
/********************************模块头****************************************\
 * 模块名： Solution1.cs
 * 项目名： CSAutomateExcel
 * 版权(c)  Microsoft Corporation.
 * Solution1.AutomateExcel演示了如何使用Microsoft Excel 主要的互用组件（PIA）自
 * 动化Excel应用程序，并且将每个COM访问对象分配给一个新的变量， 从而使用户最终可以
 * 通过调用Marshal.FinalReleaseComObject方法释放这些变量的过程。在使用该解决方案
 * 时，避免通过层级式回溯对象模型的方法进行对象调用是十分重要的，因为这样会使运行库
 * 可调用包装（RCW）被孤立于堆上，以至于将无法调用Marshal.ReleaseComObject 对其进
 * 行访问。这是需要注意的地方。例如，
 * 
 *      Excel.Workbook oWB = oXL.Workbooks.Add(missing);
 * 
 * 调用 oXL.Workbooks.Add为工作簿对象生成了一个RCW。 如果以代码所采用的方式引用这
 * 些访问对象，工作簿的RCW将被创建在GC堆上，而引用则创建在栈上，然后被丢弃。如此，
 * 则无法在RCW上调用 MarshalFinalReleaseComObject。为了获取这种类型的RCW，一种方
 * 法是在调用函数退出堆栈后立刻执行垃圾收集器GC（见 Solution2.AutomateExcel),另一
 * 种方法则是显示地将每个访问对象分配到一个变量，并释放变量。
 * 
 *      Excel.Workbooks oWBs = oXL.Workbooks;
 *      Excel.Workbook oWB = oWBs.Add(missing);
 * 
 * 
 * 该来源受微软授予的公共许可证约束。
 * 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * 保留其它所有权。
 * 
 * 该代码和资料不提供任何形式，明示或暗示的担保，包括但不限于适销特定用途的暗示性和
 * 适用性的保证。
 ***************************************************************************/
#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
#endregion


namespace CSAutomateExcel
{
    static class Solution1
    {
        public static void AutomateExcel()
        {
            object missing = Type.Missing;
            Excel.Application oXL = null;
            Excel.Workbooks oWBs = null;
            Excel.Workbook oWB = null;
            Excel.Worksheet oSheet = null;
            Excel.Range oCells = null;
            Excel.Range oRng1 = null;
            Excel.Range oRng2 = null;

            try
            {
                
                // 创建Microsoft Excel实例对象，并将其设置为不可见
                oXL = new Excel.Application();
                oXL.Visible = false;
                // 输出：Excel.Application已启动
                Console.WriteLine("Excel.Application is started");
                
               
                // 创建一个新的工作簿
                oWBs = oXL.Workbooks;
                oWB = oWBs.Add(missing);
                // 输出：新的workbook已创建
                Console.WriteLine("A new workbook is created");
               
                
                // 获取处于激活状态的工作表并设置名称
                oSheet = oWB.ActiveSheet as Excel.Worksheet;
                oSheet.Name = "Report";
                // 输出：处于活动状态的worksheet被重命名为Report
                Console.WriteLine("The active worksheet is renamed as Report");
                
                
                // 填充数据到工作表单元
                // 输出：填充数据到worksheet
                Console.WriteLine("Filling data into the worksheet ...");
                
                
                // 设置列标题
                oCells = oSheet.Cells;
                oCells[1, 1] = "First Name";
                oCells[1, 2] = "Last Name";
                oCells[1, 3] = "Full Name";

               
                // 构建用户名称数组
                string[,] saNames = new string[,] {
                {"John", "Smith"}, 
                {"Tom", "Brown"}, 
                {"Sue", "Thomas"}, 
                {"Jane", "Jones"}, 
                {"Adam", "Johnson"}};

                
                // 用数组值填充工作表中A2：B6区域（姓与名）
                oRng1 = oSheet.get_Range("A2", "B6");
                oRng1.Value2 = saNames;

               
                // 为工作表中C2:C6区域设置相关公式（=A2 & " " & B2）
                oRng2 = oSheet.get_Range("C2", "C6");
                oRng2.Formula = "=A2 & \" \" & B2";

               
                // 将工作簿保存为xlsx文件并关闭
                // 输出： 保存并关闭workbook
                Console.WriteLine("Save and close the workbook");
                string fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample1.xlsx";
                oWB.SaveAs(fileName, Excel.XlFileFormat.xlOpenXMLWorkbook,
                    missing, missing, missing, missing,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    missing, missing, missing, missing, missing);
                oWB.Close(missing, missing, missing);

               
                // 退出Excel应用程序
                // 输出：退出Excel应用程序
                Console.WriteLine("Quit the Excel application");
                
             
                // 如果不在用户控制下或者有未完成的引用,那么Excel在退出后将会继续
                // 驻留. 
                // 当启动Excel或者附加的编程，并且Application的Visible和
                // UserControl属性均为false,则可以显示地将UserControl属性设置为
                // True，使Quit 方法被调用时，能够强制应用程序终止，而不用考虑有
                // 未完成的引用

                oXL.UserControl = true;

                oXL.Quit();
            }
            catch (Exception ex)
            {
                 //输出： Solution1.AutomateExcel 抛出异常：异常信息
                Console.WriteLine("Solution1.AutomateExcel throws the error: {0}",
                    ex.Message);
               
            }
            finally
            {
               
                // 通过对所有访问对象显示调用Marshal.FinalReleaseComObject方法
                // 释放非托管Excel COM资源
                // 见http://support.microsoft.com/kb/317109.
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
            }
        }
    }
}
