'**************************************** 模块头 ***************************************
'* 模块名:  MainForm.vb
'* 项目名:	 VBWinFormTreeViewLoad
'* 版权(c) Microsoft Corporation.
'* 
'* 该示例展示了如何从一个数据表(DataTable)载入数据，
'* 并将数据显示到一个树形视图控件(TreeView)中。
'* 
'* 更多关于 TreeView 控件的信息，请参考:
'* 
'*  Windows Forms TreeView 控件
'*  http://msdn.microsoft.com/zh-cn/library/ch6etkw4.aspx
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************************

Imports System
Imports System.Data
Imports System.Windows.Forms

Public Class MainForm
    ' 用于填充 TreeView 的 DataTable 对象
    Private _dtEmployees As DataTable

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me._dtEmployees = Me.CreateDataTable()

        ' 调用 BuildTree 方法来创建 TreeView
        CreateTreeViewFromDataTable.BuildTree(Me._dtEmployees, Me.TreeView1, True, "名字", "员工标识", "上级")
    End Sub

    Public Function CreateDataTable() As DataTable
        Dim dataTable As New DataTable()

        ' 这一列的值用来标识每个树节点 TreeNode
        dataTable.Columns.Add("员工标识")

        ' 这一列的值用来显示在树节点中
        dataTable.Columns.Add("名字")

        ' 这一列的值用来标识树节点的父节点
        dataTable.Columns.Add("上级")

        ' 填充并且构建数据表 DataTable
        dataTable.Rows.Add(1, "小王", 2)
        dataTable.Rows.Add(2, "小李", DBNull.Value)
        dataTable.Rows.Add(3, "小孙", 2)
        dataTable.Rows.Add(4, "小张", 2)
        dataTable.Rows.Add(5, "小刘", 2)
        dataTable.Rows.Add(6, "小田", 5)
        dataTable.Rows.Add(7, "小金", 5)
        dataTable.Rows.Add(8, "小赵", 2)
        dataTable.Rows.Add(9, "小钱", 5)
        Return dataTable
    End Function
End Class
