'************************************* 模块头 **************************************'
' 模块名:  MainForm.vb
' 项目:      VBWinFormControls
' Copyright (c) Microsoft Corporation.
' 
' 这个示例阐述了怎样自定义Windows Forms控件。
'
' 本示例中，有4个小例子：
'
' 1. 拥有多列的组合框。
'    展示了怎样在组合框的下拉列表中显示多列数据。
' 
' 2. 每个列表项拥有不同提示的列表框。
'    展示了怎样为列表框中的每个列表项显示不同的提示。     
'
' 3. 只能输入数字的文本框。
'    展示了怎样使文本框只允许输入数字。
'
' 4. 一个椭圆形的按钮。
'    展示了怎样创建一个不规则形状的按钮。 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
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
        MultipleColumnComboBox()
        ListBoxWithDiffToolTips()
    End Sub

#Region "示例1 -- 拥有多列的组合框"
    Private Sub MultipleColumnComboBox()
        Dim dtTest As DataTable = New DataTable()
        dtTest.Columns.Add("ID", GetType(Integer))
        dtTest.Columns.Add("Name", GetType(String))

        dtTest.Rows.Add(1, "John")
        dtTest.Rows.Add(2, "Amy")
        dtTest.Rows.Add(3, "Tony")
        dtTest.Rows.Add(4, "Bruce")
        dtTest.Rows.Add(5, "Allen")

        ' 将组合框的数据源设置为DataTable。
        Me.comboBox1.DataSource = dtTest
        Me.comboBox1.DisplayMember = "Name"
        Me.comboBox1.ValueMember = "ID"

        ' 将组合框的 DrawMode 设置为OwnerDrawFixed。
        Me.comboBox1.DrawMode = DrawMode.OwnerDrawFixed
        ' 在DrawItem事件中绘制子项。
    End Sub

    Private Sub comboBox1_DrawItem(ByVal sender As System.Object, _
                                   ByVal e As System.Windows.Forms.DrawItemEventArgs) _
                                   Handles comboBox1.DrawItem
        ' 绘制默认的背景
        e.DrawBackground()

        ' 因为组合框被绑定到DataTable，所以组合框的子项是DataRowView对象。
        Dim drv As DataRowView = CType(comboBox1.Items(e.Index), DataRowView)

        ' 取出每一列的值。
        Dim id As Integer = drv("ID").ToString()
        Dim name As String = drv("Name").ToString()

        ' 获得第一列的边界。
        Dim r1 As Rectangle = e.Bounds
        r1.Width = r1.Width / 2

        ' 绘制第一列的文本。
        Using sb As SolidBrush = New SolidBrush(e.ForeColor)
            e.Graphics.DrawString(id, e.Font, sb, r1)
        End Using

        ' 绘制一条分割线分割不同的列。 
        Using p As Pen = New Pen(Color.Black)
            e.Graphics.DrawLine(p, r1.Right, 0, r1.Right, r1.Bottom)
        End Using

        ' 获取第二列的边界。
        Dim r2 As Rectangle = e.Bounds
        r2.X = e.Bounds.Width / 2
        r2.Width = r2.Width / 2

        ' 绘制第二列的文本。
        Using sb As SolidBrush = New SolidBrush(e.ForeColor)
            e.Graphics.DrawString(name, e.Font, sb, r2)
        End Using
    End Sub
#End Region

#Region "示例 2 -- 每个列表项拥有不同提示的列表框"
    Private Sub ListBoxWithDiffToolTips()
        ' 初始化列表项。
        Me.listBox1.Items.Add("Item1")
        Me.listBox1.Items.Add("Item2")
        Me.listBox1.Items.Add("Item3")
        Me.listBox1.Items.Add("Item4")
        Me.listBox1.Items.Add("Item5")
    End Sub

    Private Sub listBox1_MouseMove(ByVal sender As System.Object, _
                    ByVal e As System.Windows.Forms.MouseEventArgs) _
                                   Handles listBox1.MouseMove
        Dim hoverIndex As Integer = Me.listBox1.IndexFromPoint(e.Location)

        If hoverIndex >= 0 And hoverIndex < listBox1.Items.Count Then
            Me.toolTip1.SetToolTip(listBox1, listBox1.Items(hoverIndex).ToString())
        End If
    End Sub
#End Region

#Region "示例 3 -- 只能输入数字的文本框"
    Private Sub textBox1_KeyPress(ByVal sender As System.Object, _
                    ByVal e As System.Windows.Forms.KeyPressEventArgs) _
                                  Handles textBox1.KeyPress
        If Not (Char.IsNumber(e.KeyChar) Or e.KeyChar = vbBack) Then
            e.Handled = True
        End If
    End Sub
#End Region

#Region "示例 4 -- 一个椭圆形的按钮"
    Private Sub roundButton1_Click(ByVal sender As System.Object, _
                                   ByVal e As System.EventArgs) _
                                   Handles roundButton1.Click
        MessageBox.Show("点击了!")
    End Sub
#End Region

End Class

#Region "RoundButton 类"
Public Class RoundButton
    Inherits Button

    Protected Overrides Sub OnPaint(ByVal pevent As System.Windows.Forms.PaintEventArgs)
        Dim path As GraphicsPath = New GraphicsPath()
        path.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height)
        Me.Region = New Region(path)
        MyBase.OnPaint(pevent)
    End Sub
End Class

#End Region
