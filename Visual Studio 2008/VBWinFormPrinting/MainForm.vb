'************************************* 模块头 **************************************\
' 模块名:  MainForm.vb
' 项目名:      VBWinFormPrinting
' 版权 (c) Microsoft Corporation.
' 
' 这个打印实例演示如何在Windows窗体应用程序中创建标准的打印任务.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'******************************************************************************************/

Public Class MainForm
    Inherits Form

    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' 这个实例假设你的窗体中有一个按钮控件, 
        ' 一个命名为myDocument的PrintDocument组件, 
        ' 和一个PrintPreviewDialog控件. 

        ' 处理PrintPage事件并在里面写打印逻辑.
        AddHandler PrintDocument1.PrintPage, AddressOf Me.printDocument1_PrintPage

        ' 给PrintPreviewDialog组件制定一个PrintDocument实例.
        Me.PrintPreviewDialog1.Document = Me.PrintDocument1
    End Sub

    Private Sub printDocument1_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs)
        ' 在该事件中明确指定打印什么和如何打印.
        ' 以下代码指定了打印一个矩形框和一个字符串. 

        Dim f As Font = New Font("Vanada", 12)
        Dim br As SolidBrush = New SolidBrush(Color.Black)
        Dim p As Pen = New Pen(Color.Black)
        e.Graphics.DrawString("这个一个文本.", f, br, 50, 50)

        e.Graphics.DrawRectangle(p, 50, 100, 300, 150)
    End Sub

    Private Sub btnPrint_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrint.Click
        Me.PrintPreviewDialog1.ShowDialog()
    End Sub
    
End Class
