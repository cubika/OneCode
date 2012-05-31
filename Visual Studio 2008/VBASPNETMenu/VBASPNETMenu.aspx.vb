'****************************** 模块头 ***********************************\
' 模块名:  VBASPNETMenu.aspx.vb
' 项目名:      VBASPNETMenu
' 版权 (c) Microsoft Corporation.
'
' 这个示例展示了如何绑定 ASP.NET 菜单控件到数据库.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Partial Public Class VBASPNETMenu
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            GenerateMenuItem()
        End If
    End Sub
    Public Sub GenerateMenuItem()
        ' 从数据库中获得数据.
        Dim ds As DataSet = GetData()

        For Each mainRow As DataRow In ds.Tables(0).Rows
            ' 从主表中加载记录到菜单中..
            Dim masterItem As New MenuItem(mainRow("mainName").ToString())
            masterItem.NavigateUrl = mainRow("mainUrl").ToString()
            Menu1.Items.Add(masterItem)

            For Each childRow As DataRow In mainRow.GetChildRows("Child")
                ' 根据主表和子表的关系, 加载子表的数据.
                Dim childItem As New MenuItem(DirectCast(childRow("childName"), String))
                childItem.NavigateUrl = childRow("childUrl").ToString()
                masterItem.ChildItems.Add(childItem)
            Next
        Next
    End Sub

    Public Function GetData() As DataSet
        ' 为了测试, 我们使用内存的DataTable代替数据库.
        Dim mainTB As New DataTable()
        Dim mainIdCol As New DataColumn("mainId")
        Dim mainNameCol As New DataColumn("mainName")
        Dim mainUrlCol As New DataColumn("mainUrl")
        mainTB.Columns.Add(mainIdCol)
        mainTB.Columns.Add(mainNameCol)
        mainTB.Columns.Add(mainUrlCol)

        Dim childTB As New DataTable()
        Dim childIdCol As New DataColumn("childId")
        Dim childNameCol As New DataColumn("childName")

        ' 子表中的 MainId 列是连接到主表的外键.
        Dim childMainIdCol As New DataColumn("MainId")
        Dim childUrlCol As New DataColumn("childUrl")

        childTB.Columns.Add(childIdCol)
        childTB.Columns.Add(childNameCol)
        childTB.Columns.Add(childMainIdCol)
        childTB.Columns.Add(childUrlCol)


        ' 添加一些记录到主表.
        Dim dr As DataRow = mainTB.NewRow()
        dr(0) = "1"
        dr(1) = "Home"
        dr(2) = "test.aspx"
        mainTB.Rows.Add(dr)
        Dim dr1 As DataRow = mainTB.NewRow()
        dr1(0) = "2"
        dr1(1) = "Articles"
        dr1(2) = "test.aspx"
        mainTB.Rows.Add(dr1)
        Dim dr2 As DataRow = mainTB.NewRow()
        dr2(0) = "3"
        dr2(1) = "Help"
        dr2(2) = "test.aspx"
        mainTB.Rows.Add(dr2)
        Dim dr3 As DataRow = mainTB.NewRow()
        dr3(0) = "4"
        dr3(1) = "DownLoad"
        dr3(2) = "test.aspx"
        mainTB.Rows.Add(dr3)


        ' 添加一些记录到子表
        Dim dr5 As DataRow = childTB.NewRow()
        dr5(0) = "1"
        dr5(1) = "ASP.NET"
        dr5(2) = "2"
        dr5(3) = "test.aspx"
        childTB.Rows.Add(dr5)
        Dim dr6 As DataRow = childTB.NewRow()
        dr6(0) = "2"
        dr6(1) = "SQL Server"
        dr6(2) = "2"
        dr6(3) = "test.aspx"
        childTB.Rows.Add(dr6)
        Dim dr7 As DataRow = childTB.NewRow()
        dr7(0) = "3"
        dr7(1) = "JavaScript"
        dr7(2) = "2"
        dr7(3) = "test.aspx"
        childTB.Rows.Add(dr7)

        ' 使用一个 DataSet 包含这两个表.
        Dim ds As New DataSet()
        ds.Tables.Add(mainTB)
        ds.Tables.Add(childTB)

        ' 绑定主表和子表的关系.
        ds.Relations.Add("Child", ds.Tables(0).Columns("mainId"), ds.Tables(1).Columns("MainId"))


        Return ds
    End Function
End Class