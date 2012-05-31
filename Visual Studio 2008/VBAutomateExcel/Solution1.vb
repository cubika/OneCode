'****************************** 模块头 *************************************'
' 模块名:  Solution1.vb
' 项目名:  VBAutomateExcel
' 版权(c)  Microsoft Corporation.
' 
' Solution1.AutomateExcel演示了如何使用Microsoft Excel主要的互用组件(PIA)自动化
' Excel应用程序,并且将每个COM访问对象分配给一个新的变量,从而使用户最终可以通过调用
' Marshal.FinalReleaseComObject方法释放这些变量的过程。使用该解决方案时,避免通过
' 层级式回溯对象模型的方法进行对象调用是十分重要的,因为这样会使运行库可调用包装
' (RCW)被孤立于堆上，以至于将无法调用Marshal.ReleaseComObject对其进行访问。这是需
' 要注意的地方。例如，
'
'   Dim oWB As Excel.Workbook = oXL.Workbooks.Add()
' 
' 调用 oXL.Workbooks.Add为工作簿对象生成了一个RCW。如果以代码所采用的方式引用这些
' 访问对象,工作簿的RCW将被创建在GC堆上,而引用则创建在栈上,然后被丢弃。如此, 则无法
' 在RCW上调用 MarshalFinalReleaseComObject。为了获取这种类型的RCW,一种方法是在调
' 用函数退出堆栈后立刻执行垃圾收集器GC(见 Solution2.AutomateExcel),另一种方法则是
' 显示地将每个访问对象分配到一个变量，并释放变量。 
' 
'   Dim oWBs As Excel.Workbooks = oXL.Workbooks
'   Dim oWB As Excel.Workbook = oWBs.Add()
' 
' 该来源受微软授予的公共许可证约束。
' 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' 保留其它所有权。
' 
' 该代码和资料不提供任何形式, 明示或暗示的担保, 包括但不限于适销特定用途的暗示性和
' 适用性的保证。
'***************************************************************************'

#Region "Imports directives"

Imports System.Reflection
Imports System.IO

Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices

#End Region


Class Solution1

    Public Shared Sub AutomateExcel()

        Dim oXL As Excel.Application = Nothing
        Dim oWBs As Excel.Workbooks = Nothing
        Dim oWB As Excel.Workbook = Nothing
        Dim oSheet As Excel.Worksheet = Nothing
        Dim oCells As Excel.Range = Nothing
        Dim oRng1 As Excel.Range = Nothing
        Dim oRng2 As Excel.Range = Nothing

        Try
            ' 创建Microsoft Excel实例对象，并将其设置为不可见

            oXL = New Excel.Application
            oXL.Visible = False
            ' 输出：Excel.Application已启动
            Console.WriteLine("Excel.Application is started")

            ' 创建一个新的工作簿

            oWBs = oXL.Workbooks
            oWB = oWBs.Add()
            ' 输出：新的工作簿已创建
            Console.WriteLine("A new workbook is created")

            ' 获取处于激活状态的工作表并设置名称

            oSheet = oWB.ActiveSheet
            oSheet.Name = "Report"
            ' 输出：处于活动状态的工作表被重命名为Report
            Console.WriteLine("The active worksheet is renamed as Report")

            ' 填充数据到工作表单元
            ' 输出：填充数据到工作表
            Console.WriteLine("Filling data into the worksheet ...")

            ' 设置列标题
            oCells = oSheet.Cells
            oCells(1, 1) = "First Name"
            oCells(1, 2) = "Last Name"
            oCells(1, 3) = "Full Name"

            ' 构建用户名称数组
            Dim saNames(,) As String = {{"John", "Smith"}, _
                                        {"Tom", "Brown"}, _
                                        {"Sue", "Thomas"}, _
                                        {"Jane", "Jones"}, _
                                        {"Adam", "Johnson"}}

            '  用数组值填充工作表中A2：B6区域（姓与名）
            oRng1 = oSheet.Range("A2", "B6")
            oRng1.Value2 = saNames

            ' 为工作表中C2:C6区域设置相关公式（=A2 & " " & B2）
            oRng2 = oSheet.Range("C2", "C6")
            oRng2.Formula = "=A2 & "" "" & B2"

            ' 将工作簿保存为xlsx文件并关闭
            ' 输出：保存并关闭工作簿
            Console.WriteLine("Save and close the workbook")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) & "\Sample1.xlsx"
            oWB.SaveAs(fileName, Excel.XlFileFormat.xlOpenXMLWorkbook)
            oWB.Close()

            ' 退出Excel应用程序
            ' 输出： 退出Excel应用程序
            Console.WriteLine("Quit the Excel application")

            '如果不在用户控制下或者有未完成的引用，那么Excel在退出后将会继续驻留. 
            ' 当启动Excel或者附加的编程，并且Application的Visible和UserControl
            ' 属性均为false,则可以显示地将UserControl属性设置为True，使Quit 方法
            ' 被调用时，能够强制应用程序终止，而不用考虑有未完成的引用
            oXL.UserControl = True

            oXL.Quit()

        Catch ex As Exception
            ' 输出：Solution1.AutomateExcel抛出错误
            Console.WriteLine("Solution1.AutomateExcel throws the error: {0}", _
                              ex.Message)
        Finally

            ' 通过对所有访问对象显示调用Marshal.FinalReleaseComObject方法
            ' 释放非托管Excel COM资源
            ' 见http://support.microsoft.com/kb/317109.

            If Not oRng2 Is Nothing Then
                Marshal.FinalReleaseComObject(oRng2)
                oRng2 = Nothing
            End If
            If Not oRng1 Is Nothing Then
                Marshal.FinalReleaseComObject(oRng1)
                oRng1 = Nothing
            End If
            If Not oCells Is Nothing Then
                Marshal.FinalReleaseComObject(oCells)
                oCells = Nothing
            End If
            If Not oSheet Is Nothing Then
                Marshal.FinalReleaseComObject(oSheet)
                oSheet = Nothing
            End If
            If Not oWB Is Nothing Then
                Marshal.FinalReleaseComObject(oWB)
                oWB = Nothing
            End If
            If Not oWBs Is Nothing Then
                Marshal.FinalReleaseComObject(oWBs)
                oWBs = Nothing
            End If
            If Not oXL Is Nothing Then
                Marshal.FinalReleaseComObject(oXL)
                oXL = Nothing
            End If

        End Try

    End Sub

End Class
