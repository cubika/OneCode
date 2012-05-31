'*********************************** 模块头 **************************************\
' 模块名:	MainForm.vb
' 项目名:		VBWinFormBindToNestedProperties
' 版权 (c) Microsoft Corporation.
' 
' 这个实例演示了如何将数据源中的嵌套的属性绑定到DataGridView控件中的列上.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************************/

Imports System.ComponentModel

Public Class MainForm
    Inherits Form

    Private mylist As New BindingList(Of Person)

    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' 绑定一个列表到DataGridView控件
        Me.DataGridView1.AutoGenerateColumns = False
        Me.DataGridView1.DataSource = mylist

        Me.DataGridView1.Columns.Add("ID", "编码")
        Me.DataGridView1.Columns.Add("Name", "姓名")
        Me.DataGridView1.Columns.Add("CityName", "城市名称")
        Me.DataGridView1.Columns.Add("PostCode", "邮政编码")

        CType(Me.DataGridView1.Columns("ID"), DataGridViewTextBoxColumn).DataPropertyName = "ID"
        CType(Me.DataGridView1.Columns("Name"), DataGridViewTextBoxColumn).DataPropertyName = "Name"
        CType(Me.DataGridView1.Columns("CityName"), DataGridViewTextBoxColumn).DataPropertyName = "HomeAddr_CityName"
        CType(Me.DataGridView1.Columns("PostCode"), DataGridViewTextBoxColumn).DataPropertyName = "HomeAddr_PostCode"
    End Sub

    Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        ' 增加Persion类的实例到列表中            
        Dim p As New Person
        p.ID = "1"
        p.Name = "某某1"

        Dim addr As New Address
        addr.Cityname = "城市1"
        addr.Postcode = "邮政编码1"
        p.HomeAddr = addr

        mylist.Add(p)

        p = New Person
        p.ID = "2"
        p.Name = "某某2"

        addr = New Address
        addr.Cityname = "城市2"
        addr.Postcode = "邮政编码2"
        p.HomeAddr = addr

        mylist.Add(p)
    End Sub
End Class


