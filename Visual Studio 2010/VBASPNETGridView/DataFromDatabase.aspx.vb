'/****************************** 模块头 ******************************\
' 模块名:  DataFromDatabase.aspx.vb
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
Imports System.Data.SqlClient

Public Class DataFromDatabase
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 这个页面时首次被访问.
        If Not IsPostBack Then
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
    Private Sub BindGridView()
        ' 从 Web.config 获得连接字符串. 
        ' 当我们使用using语句时, 
        ' 我们不需要显式的调用 dispose方法, 
        ' using语句会做好这个.
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' 创建一个新的DataSet对象.
            Dim dsPerson As New DataSet()

            ' 创建一个查询
            Dim strSelectCmd As String = "SELECT PersonID,LastName,FirstName FROM Person"

            ' 创建一个新的 SqlDataAdapter 对象
            ' SqlDataAdapter 描述 数据命令的集合 和 一个数据库连接
            ' 用它们填充DataSet,和更新数据库
            Dim da As New SqlDataAdapter(strSelectCmd, conn)

            ' 打开数据库连接
            conn.Open()

            ' 填充名称为"Person" 的 DataTable 
            da.Fill(dsPerson, "Person")

            ' 在 Person DataTable 中获得 DataView.
            Dim dvPerson As DataView = dsPerson.Tables("Person").DefaultView

            ' 设置排序列和排序顺序
            dvPerson.Sort = ViewState("SortExpression").ToString()

            ' 绑定 GridView 控件
            gvPerson.DataSource = dvPerson
            gvPerson.DataBind()
        End Using
    End Sub

    ' GridView.RowDataBound 事件       
    Protected Sub gvPerson_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        ' 确定当前的 GridViewRow 是一个 DataRow.
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' 确定当前的 GridViewRow 是在正常状态或交替状态
            If e.Row.RowState = DataControlRowState.Normal OrElse e.Row.RowState = DataControlRowState.Alternate Then
                ' 添加客户端删除时确认脚本
                DirectCast(e.Row.Cells(1).Controls(0), LinkButton).Attributes("onclick") = "if(!confirm('Are you certain you want to delete this person ?')) return false;"
            End If
        End If
    End Sub

    ' GridView.PageIndexChanging 事件
    Protected Sub gvPerson_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        ' 设置新的显示页的index 
        gvPerson.PageIndex = e.NewPageIndex

        ' 重新绑定数据到新页面
        BindGridView()
    End Sub

    ' GridView.RowEditing 事件
    Protected Sub gvPerson_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs)
        ' 设置编辑模式到选中的行
        gvPerson.EditIndex = e.NewEditIndex

        ' 重新绑定数据
        BindGridView()

        ' 隐藏添加链接
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
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' 创建一个 command 对象.
            Dim cmd As New SqlCommand()

            ' 为这个 command 分配一个数据库连接.
            cmd.Connection = conn

            ' 设置 command 的 text属性
            ' SQL 语句 or 或者存储过称的名字
            cmd.CommandText = "UPDATE Person SET LastName = @LastName, FirstName = @FirstName WHERE PersonID = @PersonID"

            ' 设置 command 的类型
            ' CommandType.Text 是一般的 SQL 语句; 
            ' CommandType.StoredProcedure 是 存储过程.
            cmd.CommandType = CommandType.Text

            ' 在选中的行中取得 PersonID .
            Dim strPersonID As String = gvPerson.Rows(e.RowIndex).Cells(2).Text
            Dim strLastName As String = DirectCast(gvPerson.Rows(e.RowIndex).FindControl("TextBox1"), TextBox).Text
            Dim strFirstName As String = DirectCast(gvPerson.Rows(e.RowIndex).FindControl("TextBox2"), TextBox).Text

            ' 附加参数.
            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = strLastName
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = strFirstName

            ' 打开连接.
            conn.Open()

            ' 执行命令.
            cmd.ExecuteNonQuery()
        End Using

        ' 退出编辑模式.
        gvPerson.EditIndex = -1

        ' 在更新数据之后重新绑定数据.
        BindGridView()

        ' 显示添加按钮.
        lbtnAdd.Visible = True
    End Sub

    ' GridView.RowDeleting 事件
    Protected Sub gvPerson_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' 创建一个 command 对象.
            Dim cmd As New SqlCommand()

            ' 为这个 command 分配一个数据库连接.
            cmd.Connection = conn

            ' 设置 command 的 text属性
            ' SQL 语句 or 或者存储过称的名字
            cmd.CommandText = "DELETE FROM Person WHERE PersonID = @PersonID"

            ' 设置 command 的类型
            ' CommandType.Text 是一般的 SQL 语句; 
            ' CommandType.StoredProcedure 是 存储过程.
            cmd.CommandType = CommandType.Text

            ' 在选中的行中取得 PersonID.
            Dim strPersonID As String = gvPerson.Rows(e.RowIndex).Cells(2).Text

            ' 附加参数.
            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID

            ' 打开连接.
            conn.Open()

            ' 执行命令.
            cmd.ExecuteNonQuery()
        End Using

        ' 在删除数据之后重新绑定数据.
        BindGridView()
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
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' 创建一个 command 对象.
            Dim cmd As New SqlCommand()

            ' 为这个 command 分配一个数据库连接.
            cmd.Connection = conn

            ' 设置 command 的 text属性
            ' SQL 语句 or 或者存储过称的名字
            cmd.CommandText = "INSERT INTO Person ( LastName, FirstName ) VALUES ( @LastName, @FirstName )"

            ' 设置 command 的类型
            ' CommandType.Text 是一般的 SQL 语句; 
            ' CommandType.StoredProcedure 是 存储过程.
            cmd.CommandType = CommandType.Text

            ' 附加参数.
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = tbLastName.Text
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = tbFirstName.Text

            ' 打开连接.
            conn.Open()

            ' 执行命令.
            cmd.ExecuteNonQuery()
        End Using

        ' 重新绑定 GridView control 显示插入的数据.
        BindGridView()

        ' 清空 TextBox 控件.
        tbLastName.Text = ""
        tbFirstName.Text = ""

        ' 显示添加按钮和添加面板.
        lbtnAdd.Visible = True
        pnlAdd.Visible = False
    End Sub

    Protected Sub lbtnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' 清空 TextBox 控件.
        tbLastName.Text = ""
        tbFirstName.Text = ""

        ' 隐藏添加面板, 显示添加按钮.
        lbtnAdd.Visible = True
        pnlAdd.Visible = False
    End Sub

End Class