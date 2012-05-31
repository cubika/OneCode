'****************************** 模块头***************************************'
' 模块名:	Solution1.vb
' 项目名:	VBAutomateWord
' 版权(c)   Microsoft Corporation.
' 
' Solution1.AutomateWord 阐述了通过Microsoft Word主要的互用组件（PIA）自动化Word
' 应用程序, 并且将每个COM访问对象分配给一个新的变量, 使用户最终可以通过调用
' Marshal.FinalReleaseComObject方法释放这些变量的过程. 使用该解决方案时,避免通过
' 层级式回溯对象模型的方法进行对象调用是十分重要的，因为这样会使运行库可调用包装
'（RCW）被孤立于堆上,以至于将无法调用Marshal.ReleaseComObject 对其进行访问. 
' 这是需要注意的地方. 例如, 
' 
'   Dim oDoc As Word.Document = oWord.Documents.Add()
'  
' 调用 oWord.Documents.Add将为Documents对象创建RCW. 如果以代码所采用的层级式回溯
' 方式引用这些访问对象,文档的RCW将被创建在GC堆上,而引用则创建在栈上,然后被丢弃.
' 这样，将无法在RCW上调用MarshalFinalReleaseComObject.为了使该类型的RCW得以释放,
' 一种方法是在调用函数退出堆栈后立刻执行垃圾收集器GC（见Solution2.AutomateWord）,
' 另一种方法则是显式地将每个访问对象分配到一个变量并释放.
' 
'   Dim oDocs As Word.Documents = oWord.Documents
'   Dim oDoc As Word.Document = oDocs.Add()
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directives"

Imports System.Reflection
Imports System.IO

Imports Word = Microsoft.Office.Interop.Word
Imports System.Runtime.InteropServices

#End Region


Class Solution1

    Public Shared Sub AutomateWord()

        Dim oWord As Word.Application = Nothing
        Dim oDocs As Word.Documents = Nothing
        Dim oDoc As Word.Document = Nothing
        Dim oParas As Word.Paragraphs = Nothing
        Dim oPara As Word.Paragraph = Nothing
        Dim oParaRng As Word.Range = Nothing
        Dim oFont As Word.Font = Nothing

        Try
            ' 创建一个Microsoft Word实例并令其不可见

            oWord = New Word.Application()
            oWord.Visible = False
            Console.WriteLine("Word.Application is started")

            ' 创建一个新的文档

            oDocs = oWord.Documents
            oDoc = oDocs.Add()
            Console.WriteLine("A new document is created")

            ' 插入段落

            Console.WriteLine("Insert a paragraph")

            oParas = oDoc.Paragraphs
            oPara = oParas.Add()
            oParaRng = oPara.Range
            oParaRng.Text = "Heading 1"
            oFont = oParaRng.Font
            oFont.Bold = 1
            oParaRng.InsertParagraphAfter()

            ' 将文档保存为.docx文件并关闭

            Console.WriteLine("Save and close the document")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) & "\Sample1.docx"
            oDoc.SaveAs(fileName, Word.WdSaveFormat.wdFormatXMLDocument)
            oDoc.Close()

            ' 退出Word应用程序

            Console.WriteLine("Quit the Word application")
            oWord.Quit(False)

        Catch ex As Exception
            Console.WriteLine("Solution1.AutomateWord throws the error: {0}", _
                              ex.Message)
        Finally

            ' 通过在所有访问对象上显示调用Marshal.FinalReleaseComObject方法
            ' 释放非托管Word COM资源
            ' 见 http://support.microsoft.com/kb/317109.

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

        End Try

    End Sub

End Class