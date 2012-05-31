'****************************** 模块头 ***************************************'
' 模块名:	Solution2.vb
' 项目名:	VBAutomateWord
' 版权(c)   Microsoft Corporation.
' 
' Solution2.AutomateWord阐述了通过Microsoft Word主要的互用组件（PIA）自动化Word
' 应用程序,在自动化方法退出堆栈后执行垃圾收集器（此时RCW对象不再被引用）,从而
' 清除RCW并释放COM对象的过程.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************'

#Region "Import directives"

Imports System.Reflection
Imports System.IO

Imports Word = Microsoft.Office.Interop.Word

#End Region


Class Solution2

    Public Shared Sub AutomateWord()

        AutomateWordImpl()


        ' 在自动化方法退出堆栈后执行垃圾收集器（此时RCW对象不再被引用）
        ' 从而清除RCW并释放COM对象


        GC.Collect()
        GC.WaitForPendingFinalizers()
        ' 为了终止程序,垃圾收集器GC必须被调用两次
        ' 第一次调用将生成要终止项的相关列表
        ' 第二次则是执行终止命令,此时对象将自动执行COM对象资源的释放

        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub


    Private Shared Sub AutomateWordImpl()

        Try
            ' 创建一个Microsoft Word实例并令其不可见

            Dim oWord As New Word.Application
            oWord.Visible = False
            Console.WriteLine("Word.Application is started")

            ' 创建一个新的文档
            Dim oDoc As Word.Document = oWord.Documents.Add()
            Console.WriteLine("A new document is created")

            ' 插入段落

            Console.WriteLine("Insert a paragraph")

            Dim oPara As Word.Paragraph = oDoc.Paragraphs.Add()
            oPara.Range.Text = "Heading 1"
            oPara.Range.Font.Bold = 1
            oPara.Range.InsertParagraphAfter()

            ' 插入表格

            Console.WriteLine("Insert a table")

            Dim oBookmarkRng As Word.Range = oDoc.Bookmarks.Item("\endofdoc").Range

            Dim oTable As Word.Table = oDoc.Tables.Add(oBookmarkRng, 5, 2)
            oTable.Range.ParagraphFormat.SpaceAfter = 6

            For r As Integer = 1 To 5
                For c As Integer = 1 To 2
                    oTable.Cell(r, c).Range.Text = "r" & r & "c" & c
                Next
            Next

            ' 改变列1和列2的宽度
            oTable.Columns(1).Width = oWord.InchesToPoints(2)
            oTable.Columns(2).Width = oWord.InchesToPoints(3)

            ' 将文档保存为.docx文件并关闭

            Console.WriteLine("Save and close the document")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) & "\Sample2.docx"
            oDoc.SaveAs(fileName, Word.WdSaveFormat.wdFormatXMLDocument)
            oDoc.Close()

            ' 退出Word应用程序

            Console.WriteLine("Quit the Word application")
            oWord.Quit(False)

        Catch ex As Exception
            Console.WriteLine("Solution2.AutomateWord throws the error: {0}", _
                              ex.Message)
        End Try

    End Sub

End Class
