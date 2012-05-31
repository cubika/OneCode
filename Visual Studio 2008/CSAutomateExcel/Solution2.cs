
/*********************************模块头************************************\
 * 模块名： Solution2.cs
 * 项目名： CSAutomateExcel
 * 版权(c)  Microsoft Corporation.
 * 
 * Solution2.AutomateExcel 演示了通过Microsoft Excel 主要的互用组件(PIA)自动化
 * Excel 应用程序，并在自动化方法退出堆栈后执行垃圾收集器(此时RCW对象不再被引用),
 * 从而清除RCW并释放COM对象的过程。
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
#endregion


namespace CSAutomateExcel
{
    static class Solution2
    {
        public static void AutomateExcel()
        {
            AutomateExcelImpl();


           
            // 当调用方法退出堆栈时（此时这些对象将不再被引用）执行垃圾收集器
            // 释放非托管COM资源
            GC.Collect();
            GC.WaitForPendingFinalizers();

            
            // 为了终止程序，垃圾收集器GC必须被调用两次。第一次调用将生成要终止项
            // 的相关列表，第二次则是执行终止命令，此时对象将自动执行COM对象资源的
            // 释放。

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void AutomateExcelImpl()
        {
            object missing = Type.Missing;

            try
            {
                
                // 创建一个Microsoft Excel实例并设置其为不可见
                Excel.Application oXL = new Excel.Application();
                oXL.Visible = false;
                // 输出：Excel.Application已启动
                Console.WriteLine("Excel.Application is started");
               
                
                // 创建一个新的工作簿对象
                Excel.Workbook oWB = oXL.Workbooks.Add(missing);
                // 输出：新的workbook已创建
                Console.WriteLine("A new workbook is created");
                
               
                // 获取处于激活状态的worksheet并设置名称
                Excel.Worksheet oSheet = oWB.ActiveSheet as Excel.Worksheet;
                oSheet.Name = "Report";
                // 输出：处于活动状态的worksheet被重命名为Report 
                Console.WriteLine("The active worksheet is renamed as Report");
               
               
                // 填充数据到工作表单元
                // 输出：填充数据到工作表
                Console.WriteLine("Filling data into the worksheet ...");
               
               
                // 设置列标题
                oSheet.Cells[1, 1] = "First Name";
                oSheet.Cells[1, 2] = "Last Name";
                oSheet.Cells[1, 3] = "Full Name";

                
                // 构建用户名称数组
                string[,] saNames = new string[,] {
                {"John", "Smith"}, 
                {"Tom", "Brown"}, 
                {"Sue", "Thomas"}, 
                {"Jane", "Jones"}, 
                {"Adam", "Johnson"}};

              
                // 用数组值填充工作表中A2：B6区域（姓与名）
                oSheet.get_Range("A2", "B6").Value2 = saNames;

               
                // 为工作表中C2:C6区域设置相关公式（=A2 & " " & B2）
                oSheet.get_Range("C2", "C6").Formula = "=A2 & \" \" & B2";

               
                // 将工作簿保存为xlsx文件并关闭
                // 输出： 保存并关闭workbook
                Console.WriteLine("Save and close the workbook");
                

                string fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample2.xlsx";
                oWB.SaveAs(fileName, Excel.XlFileFormat.xlOpenXMLWorkbook,
                    missing, missing, missing, missing,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    missing, missing, missing, missing, missing);
                oWB.Close(missing, missing, missing);

                
                // 退出Excel应用程序
                // 输出： 退出Excel应用程序
                Console.WriteLine("Quit the Excel application");
                
               
                
                // 如果不在用户控制下或者有未完成的引用，那么Excel 在退出后将会继 
                // 续驻留.当启动Excel或者附加的编程，并且 Application的Visible和
                // UserControl属性均为false,则可以显示地将UserControl属性设置为
                // True，使Quit 方法被调用时，能够强制应用程序终止， 而不用考虑有
                // 未完成的引用
                oXL.UserControl = true;

                oXL.Quit();
            }
            catch (Exception ex)
            {
                // 输出：Solution2.AutomateExcel抛出异常：异常信息
                Console.WriteLine("Solution2.AutomateExcel throws the error: {0}",                       
                    ex.Message);
            }
        }
    }
}