'****************************** 模块头 *************************************'
' 模块名:   MainPage.xaml.vb
' 项目名:   VBSL4COMInterop
' 版权 (c) Microsoft Corporation.
' 
' Silverlight COM 后台代码文件交互操作.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.Runtime.InteropServices.Automation
Imports System.Threading

Partial Public Class MainPage
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, AddressOf MainPage_Loaded
    End Sub

    Private Sub MainPage_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 新建实体列表
        Dim list = New List(Of PersonEntity)()
        Dim rand = New Random()
        For i As Integer = 0 To 8
            Dim newentity = New PersonEntity()
            newentity.Name = "姓名:" + i.ToString()
            newentity.Age = rand.Next(100)
            If i Mod 2 = 0 Then
                newentity.Gender = "男"
            Else
                newentity.Gender = "女"
            End If
            list.Add(newentity)
        Next

        ' 将实体列表绑定到数据表格.
        dataGrid1.ItemsSource = list
    End Sub

    Private _isprint As Boolean
    ' 更新 "print directly" 状态.
    Private Sub CheckBox_StateChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim state = DirectCast(sender, CheckBox).IsChecked
        If state.HasValue AndAlso state.Value Then
            _isprint = True
        Else
            _isprint = False
        End If
    End Sub

    ' 将数据导出到记事本.
    Private Sub TextExport_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 检查是否允许使用AutomationFactory.
        If Not AutomationFactory.IsAvailable Then
            MessageBox.Show("这个函数要求silverlight应用程序在受信任的OOB模式下运行.")
        Else
            '使用shell打开记事本应用程序.
            Dim shell = AutomationFactory.CreateObject("WScript.Shell")
            shell.Run("%windir%\notepad", 5)
            Thread.Sleep(100)

            shell.SendKeys("Name{Tab}Age{Tab}Gender{Enter}")
            For Each item As PersonEntity In TryCast(dataGrid1.ItemsSource, List(Of PersonEntity))
                shell.SendKeys(item.Name & "{Tab}" & item.Age & "{Tab}" & item.Gender & "{Enter}")
            Next
        End If
    End Sub

    ' 将数据导出到Word.
    Private Sub WordExport_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        '检查是否允许使用AutomationFactory.
        If Not AutomationFactory.IsAvailable Then
            MessageBox.Show("这个函数要求silverlight应用程序在受信任的OOB模式下运行.")
        Else
            ' 创建Word自动控制对象.
            Dim word = AutomationFactory.CreateObject("Word.Application")
            word.Visible = True

            ' 新建一个Word文件.
            Dim doc = word.Documents.Add()

            ' 写标题
            Dim range1 = doc.Paragraphs(1).Range
            range1.Text = "Silverlight4 Word自动控制示例" & vbLf
            range1.Font.Size = 24
            range1.Font.Bold = True

            Dim list = TryCast(dataGrid1.ItemsSource, List(Of PersonEntity))

            Dim range2 = doc.Paragraphs(2).Range
            range2.Font.Size = 12
            range2.Font.Bold = False

            ' 创建表
            doc.Tables.Add(range2, list.Count + 1, 3, Nothing, Nothing)

            Dim cell = doc.Tables(1).Cell(1, 1)
            cell.Range.Text = "姓名"
            cell.Range.Font.Bold = True

            cell = doc.Tables(1).Cell(1, 2)
            cell.Range.Text = "年龄"
            cell.Range.Font.Bold = True

            cell = doc.Tables(1).Cell(1, 3)
            cell.Range.Text = "性别"
            cell.Range.Font.Bold = True

            ' 在表格中填写数据
            For i As Integer = 0 To list.Count - 1
                cell = doc.Tables(1).Cell(i + 2, 1)
                cell.Range.Text = list(i).Name

                cell = doc.Tables(1).Cell(i + 2, 2)
                cell.Range.Text = list(i).Age

                cell = doc.Tables(1).Cell(i + 2, 3)
                cell.Range.Text = list(i).Gender
            Next

            If _isprint Then
                '不需要预览，直接打印Word.
                doc.PrintOut()

            End If
        End If
    End Sub
End Class