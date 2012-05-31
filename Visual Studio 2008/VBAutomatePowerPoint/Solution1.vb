'******************************** 模块头 ********************************'
' 模块名:  Solution1.vb
' 项目名:  CSAutomatePowerPoint
' 版权 (c) Microsoft Corporation.
' 
' 解决方案1.AutomatePowerPoint 演示了自动化的Microsoft PowerPoint应用程序通过使用
' Microsoft PowerPoint主互操作程序集（PIA），和显式地将每一个COM存取器赋予一个新
' 的变量（这些变量需要在程序结束时显式的调用Marshal.FinalReleaseComObject方法来释
' 放它）。当您在使用此解决方案时，重要的是避免潜入对象式的调用，因为那会使运行时可
' 调用包装（RCW）孤立的存在于堆上，当调用Marshal.ReleaseComObject时，您将访问不到
' RCW。您需要非常小心。例如， 
' 
'   Dim oPre As PowerPoint.Presentation = oPowerPoint.Presentations.Add()
'  
' 调用oPowerPoint.Presentations.Add产生了一个对应于Presentations对象的RCW。如果您
' 如以上代码那样通过潜入访问这些存取器，对应于Presentations对象的RCW被创建在GC堆上，
' 但它的引用被建立在栈上并且之后便被丢弃。这样的话，便没有方法可以在
' Marshal.FinalReleaseComObject中访问到这个RCW对象了。为了使这些类的RCW被释放，您需
' 要在调用方法一被弹出栈便开始强制一次垃圾回收（参见解决方案2.AutomatePowerPoint），
' 或者您需要显示的将每一个存取器对象赋给一个变量，然后释放它。
' 
'   Dim oPres As PowerPoint.Presentations = oPowerPoint.Presentations
'   Dim oPre As PowerPoint.Presentation = oPres.Add()
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*******************************************************************************'

#Region "Imports directives"

Imports System.Reflection
Imports System.IO

Imports Office = Microsoft.Office.Core
Imports PowerPoint = Microsoft.Office.Interop.PowerPoint
Imports System.Runtime.InteropServices

#End Region


Class Solution1

    Public Shared Sub AutomatePowerPoint()

        Dim oPowerPoint As PowerPoint.Application = Nothing
        Dim oPres As PowerPoint.Presentations = Nothing
        Dim oPre As PowerPoint.Presentation = Nothing
        Dim oSlides As PowerPoint.Slides = Nothing
        Dim oSlide As PowerPoint.Slide = Nothing
        Dim oShapes As PowerPoint.Shapes = Nothing
        Dim oShape As PowerPoint.Shape = Nothing
        Dim oTxtFrame As PowerPoint.TextFrame = Nothing
        Dim oTxtRange As PowerPoint.TextRange = Nothing

        Try
            ' 创建一个Microsoft PowerPoint实例并使其不可见。

            oPowerPoint = New PowerPoint.Application
            ' 默认情况下PowerPoint不可见，直道你使它可见。
            'oPowerPoint.Visible = Office.MsoTriState.msoFalse

            ' 创建一个新的演示文稿。

            oPres = oPowerPoint.Presentations
            oPre = oPres.Add()
            Console.WriteLine("一个新的演示文稿被建立")

            ' 插入一个幻灯片，并为幻灯片加入一些文本。

            Console.WriteLine("插入一个幻灯片")
            oSlides = oPre.Slides
            oSlide = oSlides.Add(1, PowerPoint.PpSlideLayout.ppLayoutText)

            Console.WriteLine("添加一些文本")
            oShapes = oSlide.Shapes
            oShape = oShapes(1)
            oTxtFrame = oShape.TextFrame
            oTxtRange = oTxtFrame.TextRange
            oTxtRange.Text = "一站式代码框架"

            ' 保存此演示文稿为pptx文件并将其关闭。

            Console.WriteLine("保存并退出演示文稿")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) + "\\Sample1.pptx"
            oPre.SaveAs(fileName, PowerPoint.PpSaveAsFileType.ppSaveAsOpenXMLPresentation, _
                        Office.MsoTriState.msoTriStateMixed)
            oPre.Close()

            ' 退出PowerPoint应用程序。

            Console.WriteLine("退出PowerPoint应用程序")
            oPowerPoint.Quit()

        Catch ex As Exception
            Console.WriteLine("解决方案1.AutomatePowerPoint抛出的错误: {0}", _
                              ex.Message)
        Finally

            ' 显式地调用Marshal.FinalReleaseComObject访问所有的存取器对象清除 
            ' 非托管的COM资源。 
            ' 参见http://support.microsoft.com/kb/317109.

            If Not oTxtRange Is Nothing Then
                Marshal.FinalReleaseComObject(oTxtRange)
                oTxtRange = Nothing
            End If
            If Not oTxtFrame Is Nothing Then
                Marshal.FinalReleaseComObject(oTxtFrame)
                oTxtFrame = Nothing
            End If
            If Not oShape Is Nothing Then
                Marshal.FinalReleaseComObject(oShape)
                oShape = Nothing
            End If
            If Not oShapes Is Nothing Then
                Marshal.FinalReleaseComObject(oShapes)
                oShapes = Nothing
            End If
            If Not oSlide Is Nothing Then
                Marshal.FinalReleaseComObject(oSlide)
                oSlide = Nothing
            End If
            If Not oSlides Is Nothing Then
                Marshal.FinalReleaseComObject(oSlides)
                oSlides = Nothing
            End If
            If Not oPre Is Nothing Then
                Marshal.FinalReleaseComObject(oPre)
                oPre = Nothing
            End If
            If Not oPres Is Nothing Then
                Marshal.FinalReleaseComObject(oPres)
                oPres = Nothing
            End If
            If Not oPowerPoint Is Nothing Then
                Marshal.FinalReleaseComObject(oPowerPoint)
                oPowerPoint = Nothing
            End If

        End Try

    End Sub

End Class