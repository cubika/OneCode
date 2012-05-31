
/*****************************模块头****************************************\
 * 模块名：   Program.cs
 * 项目名：   CSAutomateExcel
 * 版权(c)    Microsoft Corporation.
 * 
 * CSAutomateExcel案例演示了通过Visual C#代码生成Excel实例、填充数据到指定区域、
 * 创建，保存工作簿以及关闭Excel应用程序并释放非托管的COM资源的相关过程。
 * 
 * Office 自动化建立在组件对象模型（COM）的基础上。当从托管代码中调用Office相关的
 * COM对象时，将自动生成一个运行库可调用包装（RCW）。RCW掌管.NET应用程序与COM对象
 * 间的调用，它保存对COM对象进行引用的数量值。如果RCW上并非所有的引用都被释放，
 * 那么Office的COM对象将不会退出，这也将导致Office应用程序在自动化运行后无法终止。
 * 为了确保Office应用程序完全退出，示例提供了两种解决方案。
 * 
 * 解决方案1.  AutomateExcel 通过Microsoft Excel 主要的互用组件（PIA）自动化
 * Excel应用程序，并且将每个COM访问对象分配给一个新的变量，使用户最终可以通过调用
 * Marshal.FinalReleaseComObject方法释放这些变量。
 * 
 * 解决方案2.  AutomateExcel 通过Microsoft Excel 主要的互用组件（PIA）自动化
 * Excel 应用程序，在自动化方法退出堆栈后执行垃圾收集器（此时RCW对象不再被引用），
 * 从而清除RCW并释放COM对象。
 * 
 * 该来源受微软授予的公共许可证约束。
 * 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * 保留其它所有权。
 * 
 * 该代码和资料不提供任何形式，明示或暗示的担保，包括但不限于适销特定用途的暗示性
 * 和适用性的保证。
 * *************************************************************************/
#region Using directives
using System;
#endregion


namespace CSAutomateExcel
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
           

            // AutomateExcel 通过Microsoft Excel 主要的互用组件（PIA）自动化
            // Excel应用程序，并且将每个COM访问对象分配给一个新的变量，使用户最终
            // 可以通过调用Marshal.FinalReleaseComObject方法释放这些变量。
            Solution1.AutomateExcel();

            // 输出空行
            Console.WriteLine();
            
            

            // AutomateExcel 通过Microsoft Excel 主要的互用组件（PIA）自动化
            // Excel 应用程序，在自动化方法退出堆栈后执行垃圾收集器
            //（此时RCW对象不再被引用），从而清除RCW并释放COM对象。
            Solution2.AutomateExcel();
        }
    }
}