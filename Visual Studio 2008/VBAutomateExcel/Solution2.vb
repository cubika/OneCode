'******************************模块头 **************************************'
' 模块名:  Solution2.vb
' 项目名:  VBAutomateExcel
' 版权(c)  Microsoft Corporation.
' 
' Solution2.AutomateExcel 演示了通过Microsoft Excel 主要的互用组件（PIA）自动化
' Excel 应用程序，并在自动化方法退出堆栈后执行垃圾收集器 (此时RCW对象不再被引用) ,
' 从而清除RCW并释放COM对象的过程。
'
' 该来源受微软授予的公共许可证约束。
' 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' 保留其它所有权。
' 
' 该代码和资料不提供任何形式，明示或暗示的担保，包括但不限于适销特定用途的暗示性和
' 适用性的保证。
'***************************************************************************'

#Region "Import directives"

Imports System.IO
Imports System.Reflection

Imports Excel = Microsoft.Office.Interop.Excel

#End Region


Class Solution2

    Public Shared Sub AutomateExcel()

        AutomateExcelImpl()


        ' 当调用方法退出堆栈时（此时这些对象将不再被引用）执行垃圾收集器
        ' 释放非托管COM资源

        GC.Collect()
        GC.WaitForPendingFinalizers()
        ' 为了终止程序，垃圾收集器GC必须被调用两次。第一次调用将生成要终止项的
        ' 相关列表，第二次则是执行终止命令，此时对象将自动执行COM对象资源的释放。
        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub


    Private Shared Sub AutomateExcelImpl()

        Try
            ' 创建一个Microsoft Excel实例并设置其为不可见

            Dim oXL As New Excel.Application
            oXL.Visible = False
            ' 输出：Excel.Application已启动
            Console.WriteLine("Excel.Application is started")

            ' 创建一个新的工作簿对象

            Dim oWB As Excel.Workbook = oXL.Workbooks.Add()
            ' 输出：新的工作簿已创建
            Console.WriteLine("A new workbook is created")

            ' 获取处于激活状态的工作表并设置名称

            Dim oSheet As Excel.Worksheet = oWB.ActiveSheet
            oSheet.Name = "Report"
            ' 输出：处于活动状态的工作表被重命名为Report
            Console.WriteLine("The active worksheet is renamed as Report")

            ' 填充数据到工作表单元
            ' 输出：填充数据到工作表
            Console.WriteLine("Filling data into the worksheet ...")

            ' 设置列标题
            oSheet.Cells(1, 1) = "First Name"
            oSheet.Cells(1, 2) = "Last Name"
            oSheet.Cells(1, 3) = "Full Name"

            ' 构建用户名称数组
            Dim saNames(,) As String = {{"John", "Smith"}, _
                                        {"Tom", "Brown"}, _
                                        {"Sue", "Thomas"}, _
                                        {"Jane", "Jones"}, _
                                        {"Adam", "Johnson"}}

            ' 用数组值填充工作表中A2：B6区域（姓与名）
            oSheet.Range("A2", "B6").Value2 = saNames

            ' 为工作表中C2:C6区域设置相关公式（=A2 & " " & B2）
            oSheet.Range("C2", "C6").Formula = "=A2 & "" "" & B2"

            ' 将工作簿保存为xlsx文件并关闭
            ' 输出：保存并关闭工作簿
            Console.WriteLine("Save and close the workbook")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) & "\Sample2.xlsx"
            oWB.SaveAs(fileName, Excel.XlFileFormat.xlOpenXMLWorkbook)
            oWB.Close()

            ' 退出Excel应用程序
            ' 输出： 退出Excel应用程序
            Console.WriteLine("Quit the Excel application")

            ' 如果不在用户控制下或者有未完成的引用,那么Excel在退出后将会继续驻留. 
            ' 当启动Excel或者附加的编程，并且Application的Visible和UserControl
            ' 属性均为false,则可以显示地将UserControl属性设置为True，使Quit 方法
            ' 被调用时，能够强制应用程序终止，而不用考虑有未完成的引用
            oXL.UserControl = True

            oXL.Quit()

        Catch ex As Exception
            Console.WriteLine("Solution2.AutomateExcel throws the error: {0}", _
                              ex.Message)
        End Try

    End Sub

End Class
