'************************************* Module Header **************************************'
' 模块名称:	MainForm.vb
' 项目名:		VBWinFormMultipleColumnComboBox
' 版权		(c) Microsoft Corporation.
' 
' 
' 这个示例展示了如何在组合框的下拉框中显示多列数据.
' 
' This source is subject to the Microsoft Public License. 
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
' All other rights reserved. 
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
'******************************************************************************************'

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D

Public Class MainForm

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

		'  创建数据源:
        '  
		'  创建一个名为Test的表并且添加两个列 
        '  ID:     int
        '  Name:   string
        '
        Dim dtTest As DataTable = New DataTable()
        dtTest.Columns.Add("ID", GetType(Integer))
        dtTest.Columns.Add("Name", GetType(String))

        dtTest.Rows.Add(1, "John")
        dtTest.Rows.Add(2, "Amy")
        dtTest.Rows.Add(3, "Tony")
        dtTest.Rows.Add(4, "Bruce")
        dtTest.Rows.Add(5, "Allen")

        ' 将组合框绑定到数据表
        Me.comboBox1.DataSource = dtTest
        Me.comboBox1.DisplayMember = "Name"
        Me.comboBox1.ValueMember = "ID"

        ' 启用组合框的自我绘制功能
        Me.comboBox1.DrawMode = DrawMode.OwnerDrawFixed        
    End Sub

	' 通过处理DrawItem事件来绘制每一项
    Private Sub comboBox1_DrawItem(ByVal sender As System.Object, _
                                   ByVal e As System.Windows.Forms.DrawItemEventArgs) _
                                   Handles comboBox1.DrawItem
        ' 绘制默认背景
        e.DrawBackground()

        ' 组合框已经绑定到数据表,
        ' 所以每一项均为DataRowView对象.
        Dim drv As DataRowView = CType(comboBox1.Items(e.Index), DataRowView)

        ' 获取每一项中对应列的值.
        Dim id As Integer = drv("ID").ToString()
        Dim name As String = drv("Name").ToString()

        ' 获取第一列的边界
        Dim r1 As Rectangle = e.Bounds
        r1.Width = r1.Width / 2

        ' 在第一列上绘制文本
        Using sb As SolidBrush = New SolidBrush(e.ForeColor)
            e.Graphics.DrawString(id, e.Font, sb, r1)
        End Using

        ' 绘制线来分隔列 
        Using p As Pen = New Pen(Color.Black)
            e.Graphics.DrawLine(p, r1.Right, 0, r1.Right, r1.Bottom)
        End Using

        ' 获取第二列的边界
        Dim r2 As Rectangle = e.Bounds
        r2.X = e.Bounds.Width / 2
        r2.Width = r2.Width / 2

        ' 在第二列上绘制文本
        Using sb As SolidBrush = New SolidBrush(e.ForeColor)
            e.Graphics.DrawString(name, e.Font, sb, r2)
        End Using
    End Sub
End Class
