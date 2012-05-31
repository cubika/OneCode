'/****************************** 模块头 ******************************\
' 模块名:  DataInMemory.aspx.vb
' 项目名:  VBSPNETGridView
' 版权 (c) Microsoft Corporation.
' 
' 这个 VBASPNETGridView 项目描述了如何填充 ASP.NET GridView 控件和如何在 ASP.NET
' GridView 空间中实现添加,编辑,更新,删除,分页,排序等功能.
' 这个 DataTable 被记录到 ViewState 中以持久化回传数据.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
' All other rights reserved.
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/
Public Class DataInMemory
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 这个页面时首次被访问.
        If Not IsPostBack Then
            ' 初始化 DataTable 并将其存储在 ViewState.
            InitializeDataSource()

            ' 开启分页功能并设置页的大小.
            gvPerson.AllowPaging = True
            gvPerson.PageSize = 5

            ' 开启排序.
            gvPerson.AllowSorting = True

            ' 初始化排序表达式.
            ViewState("SortExpression") = "PersonID ASC"

            ' 填充 GridView.
            BindGridView()
        End If

    End Sub
    ' 初始化 DataTable.
    Private Sub InitializeDataSource()
        ' 建立一个名为 dtPerson 的 DataTable.
        Dim dtPerson As New DataTable()

        ' 添加四个列到 DataTable.
        dtPerson.Columns.Add("PersonID")
        dtPerson.Columns.Add("LastName")
        dtPerson.Columns.Add("FirstName")

        ' 指定 PersonID 列为子增长列
        ' 并设置初值和增长值.
        dtPerson.Columns("PersonID").AutoIncrement = True
        dtPerson.Columns("PersonID").AutoIncrementSeed = 1
        dtPerson.Columns("PersonID").AutoIncrementStep = 1

        ' 设置 PersonID 列为主键.
        Dim dcKeys As DataColumn() = New DataColumn(0) {}
        dcKeys(0) = dtPerson.Columns("PersonID")
        dtPerson.PrimaryKey = dcKeys

        ' 添加新的行到DataTable中.
        dtPerson.Rows.Add(Nothing, "Davolio", "Nancy")
        dtPerson.Rows.Add(Nothing, "Fuller", "Andrew")
        dtPerson.Rows.Add(Nothing, "Leverling", "Janet")
        dtPerson.Rows.Add(Nothing, "Dodsworth", "Anne")
        dtPerson.Rows.Add(Nothing, "Buchanan", "Steven")
        dtPerson.Rows.Add(Nothing, "Suyama", "Michael")
        dtPerson.Rows.Add(Nothing, "Callahan", "Laura")

        ' 存储到 ViewState. 
        ViewState("dtPerson") = dtPerson
    End Sub

    Private Sub BindGridView()
        If ViewState("dtPerson") IsNot Nothing Then
            ' 从 ViewState 中得到 DataTable.
            Dim dtPerson As DataTable = DirectCast(ViewState("dtPerson"), DataTable)

            ' 转换 DataTable 到 DataView.
            Dim dvPerson As New DataView(dtPerson)

            ' 设置排序列和排序顺序.
            dvPerson.Sort = ViewState("SortExpression").ToString()

            ' 绑定 GridView 控件.
            gvPerson.DataSource = dvPerson
            gvPerson.DataBind()
        End If
    End Sub

    ' GridView.RowDataBound 事件
    Protected Sub gvPerson_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        ' 确定当前的 GridViewRow 是一个 DataRow.
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' 确定当前的 GridViewRow 是在正常状态或交替状态.
            If e.Row.RowState = DataControlRowState.Normal OrElse e.Row.RowState = DataControlRowState.Alternate Then
                ' 添加客户端删除时确认脚本.
                DirectCast(e.Row.Cells(1).Controls(0), LinkButton).Attributes("onclick") = "if(!confirm('Are you certain you want to delete this person ?')) return false;"
            End If
        End If
    End Sub

    ' GridView.PageIndexChanging 事件
    Protected Sub gvPerson_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        ' 设置新的显示页的index .  
        gvPerson.PageIndex = e.NewPageIndex

        ' 重新绑定数据到新页面.
        BindGridView()
    End Sub

    ' GridView.RowEditing 事件
    Protected Sub gvPerson_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs)
        ' 设置编辑模式到选中的行. 
        gvPerson.EditIndex = e.NewEditIndex

        ' 重新绑定数据.
        BindGridView()

        ' 隐藏添加链接.
        lbtnAdd.Visible = False
    End Sub

    ' GridView.RowCancelingEdit 事件
    Protected Sub gvPerson_RowCancelingEdit(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs)
        ' 退出编辑模式.
        gvPerson.EditIndex = -1

        ' 重新绑定数据.
        BindGridView()

        ' 显示添加按钮.
        lbtnAdd.Visible = True
    End Sub

    ' GridView.RowUpdating 事件
    Protected Sub gvPerson_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        If ViewState("dtPerson") IsNot Nothing Then
            ' 从 ViewState中获得DataTable.
            Dim dtPerson As DataTable = DirectCast(ViewState("dtPerson"), DataTable)

            ' 从选中的行获取PersonID.
            Dim strPersonID As String = gvPerson.Rows(e.RowIndex).Cells(2).Text

            ' 在 DateTable 查找到该行.
            Dim drPerson As DataRow = dtPerson.Rows.Find(strPersonID)

            ' 找到编辑的值并更新各个项.
            drPerson("LastName") = DirectCast(gvPerson.Rows(e.RowIndex).FindControl("TextBox1"), TextBox).Text
            drPerson("FirstName") = DirectCast(gvPerson.Rows(e.RowIndex).FindControl("TextBox2"), TextBox).Text

            ' 退出编辑模式e.
            gvPerson.EditIndex = -1

            ' 重新绑定数据,显示更新内容.
            BindGridView()

            ' 显示添加按钮.
            lbtnAdd.Visible = True
        End If
    End Sub

    ' GridView.RowDeleting 事件
    Protected Sub gvPerson_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        If ViewState("dtPerson") IsNot Nothing Then
            ' 从 ViewState 得到 DataTable.
            Dim dtPerson As DataTable = DirectCast(ViewState("dtPerson"), DataTable)

            ' 从选中的行中得到 PersonID.
            Dim strPersonID As String = gvPerson.Rows(e.RowIndex).Cells(2).Text

            ' 在 DateTable 找到该行.
            Dim drPerson As DataRow = dtPerson.Rows.Find(strPersonID)

            ' 在 DataTable 中删除该行.
            dtPerson.Rows.Remove(drPerson)

            ' 重新绑定 GridView 控件,显示删除后的数据.
            BindGridView()
        End If
    End Sub

    ' GridView.Sorting 事件
    Protected Sub gvPerson_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)
        Dim strSortExpression As String() = ViewState("SortExpression").ToString().Split(" "c)

        ' 如果排序列和之前的相同, 
        ' 那么改变排序的方向.
        If strSortExpression(0) = e.SortExpression Then
            If strSortExpression(1) = "ASC" Then
                ViewState("SortExpression") = Convert.ToString(e.SortExpression) & " " & "DESC"
            Else
                ViewState("SortExpression") = Convert.ToString(e.SortExpression) & " " & "ASC"
            End If
        Else
            ' 如果排序的列不同,
            ' 那么把排序的方向设置为 "Ascending".
            ViewState("SortExpression") = Convert.ToString(e.SortExpression) & " " & "ASC"
        End If

        ' 排序之后重新绑定数据.
        BindGridView()
    End Sub

    Protected Sub lbtnAdd_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' 隐藏添加按钮,显示添加面板.
        lbtnAdd.Visible = False
        pnlAdd.Visible = True
    End Sub

    Protected Sub lbtnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs)
        If ViewState("dtPerson") IsNot Nothing Then
            ' 从 ViewState 得到 DataTable, 并插入新的数据.
            Dim dtPerson As DataTable = DirectCast(ViewState("dtPerson"), DataTable)

            ' 添加一个新行.
            dtPerson.Rows.Add(Nothing, tbLastName.Text, tbFirstName.Text)

            ' 重新绑定数据显示插入的数据.
            BindGridView()
        End If

        ' 清空 TextBox 控件.
        tbLastName.Text = ""
        tbFirstName.Text = ""

        ' 显示添加按钮 ,隐藏添加面板.
        lbtnAdd.Visible = True
        pnlAdd.Visible = False
    End Sub

    Protected Sub lbtnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' 清空 TextBox 控件.
        tbLastName.Text = ""
        tbFirstName.Text = ""

        ' 显示添加按钮 ,隐藏添加面板.
        lbtnAdd.Visible = True
        pnlAdd.Visible = False
    End Sub

End Class