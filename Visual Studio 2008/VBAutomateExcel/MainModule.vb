'******************************模块头***************************************'
' 模块名:  MainModule.vb
' 项目名:  VBAutomateExcel
' 版权(c)  Microsoft Corporation.
' 
' VBAutomateExcel案例阐述了通过Visual Basic.NET代码生成Excel实例、填充数据到指定
' 区域、创建，保存工作簿以及关闭Excel应用程序并释放非托管的COM资源的相关过程。
'
' Office 自动化建立在组件对象模型（COM）的基础上。当从托管代码中调用Office 相关的
' COM对象时，将自动生成一个运行库可调用包装（RCW）。RCW掌管.NET应用程序与COM对象间
' 的调用，它保存对COM对象进行引用的数量值。如果RCW上并非所有的引用都被释放，那么
' Office的COM对象将不会退出，这也将导致Office应用程序在自动化运行后无法终止。为了 
' 确保Office应用程序完全退出，示例提供了两种解决方案。
'
' 解决方案1. AutomateExcel 通过Microsoft Excel 主要的互用组件（PIA）自动化Excel
' 应用程序，并且将每个COM访问对象分配给一个新的变量，使用户最终可以通过调用
' Marshal.FinalReleaseComObject方法释放这些变量。
' 
' 解决方案2.  AutomateExcel 通过Microsoft Excel 主要的互用组件（PIA）自动化Excel
' 应用程序，在自动化方法退出堆栈后执行垃圾收集器（此时RCW对象不再被引用），从而清除
' RCW并释放COM对象。
' 
' 该来源受微软授予的公共许可证约束。
' 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' 保留其它所有权。
' 
' 该代码和资料不提供任何形式，明示或暗示的担保，包括但不限于适销特定用途的暗示性和
' 适用性的保证。
'***************************************************************************'


Module MainModule

    <STAThread()> _
    Sub Main()

        ' Solution1.AutomateExcel演示了如何使用Microsoft Excel 主要的互用组件
        '（PIA）自动化Excel应用程序，并且将每个COM访问对象分配给一个新的变量，
        ' 从而使用户最终可以通过调用Marshal.FinalReleaseComObject方法释放这些
        ' 变量的过程。
        Solution1.AutomateExcel()

        Console.WriteLine()

        ' Solution2.AutomateExcel 演示了通过Microsoft Excel 主要的互用组件
        '（PIA）自动化Excel 应用程序，并在自动化方法退出堆栈后执行垃圾收集器
        '（此时RCW对象不再被引用），从而清除RCW并释放COM对象的过程。
        Solution2.AutomateExcel()

    End Sub

End Module
