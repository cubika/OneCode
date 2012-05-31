'****************************** 模块头 ******************************'
' 模块名:  Solution2.vb
' 项目名:  VBAutomatePowerPoint
' 版权 (c) Microsoft Corporation.
' 
' 解决方案2.AutomatePowerPoint 演示了自动化Microsoft PowerPoint应用程序
' 通过使用Microsoft PowerPoint PIA，并在自动化功能函数弹出栈后（在此时的
' RCW对象都是不可达的）就开始强制一次垃圾回收来清理RCW对象和释放COM对象。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Import directives"

Imports System.Reflection
Imports System.IO

Imports Office = Microsoft.Office.Core
Imports PowerPoint = Microsoft.Office.Interop.PowerPoint

#End Region


Class Solution2

    Public Shared Sub AutomatePowerPoint()

        AutomatePowerPointImpl()


        ' 在自动化功能函数弹出栈后（在此时的RCW对象都是不可达的）就开始
        ' 强制一次垃圾回收来清理RCW对象和释放COM对象。

        GC.Collect()
        GC.WaitForPendingFinalizers()
        ' 为了使终结器（Finalizers）被调用，需要两次调用GC
        ' 第一次调用，它只简单的列出需要被终结的对象， 
        ' 第二次调用，已经终结化了。
        ' 只有在那时，对象才会自动执行它们的ReleaseComObject方法。
        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub


    Private Shared Sub AutomatePowerPointImpl()

        Try
            ' 创建一个Microsoft PowerPoint实例并使其不可见。

            Dim oPowerPoint As New PowerPoint.Application
            ' 默认情况下PowerPoint不可见，直到你使它可见。
            'oPowerPoint.Visible = Office.MsoTriState.msoFalse

            ' 创建一个新的演示文稿。

            Dim oPre As PowerPoint.Presentation = oPowerPoint.Presentations.Add()
            Console.WriteLine("一个新的演示文稿被建立")

            ' 插入一个幻灯片，并为幻灯片加入一些文本。

            Console.WriteLine("插入一个幻灯片")
            Dim oSlide As PowerPoint.Slide = oPre.Slides.Add( _
            1, PowerPoint.PpSlideLayout.ppLayoutText)

            Console.WriteLine("添加一些文本")
            oSlide.Shapes(1).TextFrame.TextRange.Text = "一站式代码框架"

            ' 保存此演示文稿为pptx文件并将其关闭。

            Console.WriteLine("保存并退出演示文稿")

            Dim fileName As String = Path.GetDirectoryName( _
                Assembly.GetExecutingAssembly().Location) + "\\Sample2.pptx"
            oPre.SaveAs(fileName, _
                PowerPoint.PpSaveAsFileType.ppSaveAsOpenXMLPresentation, _
                Office.MsoTriState.msoTriStateMixed)
            oPre.Close()

            ' 退出PowerPoint应用程序。
            Console.WriteLine("退出PowerPoint应用程序")
            oPowerPoint.Quit()

        Catch ex As Exception
            Console.WriteLine("解决方案2.AutomatePowerPoint抛出的错误: {0}", _
                              ex.Message)
        End Try

    End Sub

End Class