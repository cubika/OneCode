'****************************** 模块头 *******************************
' 模块名:  Default.aspx.vb
' 项目名:      VBASPNETFormViewUpload
' Copyright (c) Microsoft Corporation.
' 
' 本页面填充了一个从SQL Server数据库中读取数据的FromView控件 
' 同时还提供了处理数据所需的UI.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************

#Region "Using directives"
Imports System.Data.SqlClient
#End Region

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' 检查页面是否初次被访问.
        If Not IsPostBack Then
            ' 启用FormView分页选项  
            ' 同时设定PageButton计数.
            fvPerson.AllowPaging = True
            fvPerson.PagerSettings.PageButtonCount = 15

            ' 填充FormView控件.
            BindFormView()
        End If
    End Sub

    Private Sub BindFormView()
        ' 从Web.config获取链接字符串.  
        ' 当我们使用Using语句时, 
        ' 不需要显式释放代码中的对象, 
        ' using语句会处理他们.
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' 新建一个DataSet对象.
            Dim dsPerson As New DataSet()

            ' 新建一个SELECT查询.
            Dim strSelectCmd As String = "SELECT PersonID,LastName,FirstName FROM Person"

            ' 新建一个SqlDataAdapter对象
            ' SqlDataAdapter表示一组数据命令和一个数据库链接 
            ' 用以填充DataSet与 
            ' 更新SQL Server数据库.
            Dim da As New SqlDataAdapter(strSelectCmd, conn)

            ' 打开数据链接
            conn.Open()

            ' 以查询的结果按行填充DataSet中名为Person的DataTable.
            ' 
            da.Fill(dsPerson, "Person")

            ' 绑定FormView控件.
            fvPerson.DataSource = dsPerson
            fvPerson.DataBind()
        End Using
    End Sub

    ' FormView.ItemDeleting 事件
    Protected Sub fvPerson_ItemDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewDeleteEventArgs) Handles fvPerson.ItemDeleting
        ' 从Web.config获取链接字符串.
        ' 当我们使用Using语句时, 
        ' 不需要显式释放代码中的对象, 
        ' using语句会处理他们.
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' 新建一个命令对象.
            Dim cmd As New SqlCommand()

            ' 将数据链接关联到命令.
            cmd.Connection = conn

            ' 设定命令文本
            ' SQL语句或者存储过程名字. 
            cmd.CommandText = "DELETE FROM Person WHERE PersonID = @PersonID"

            ' 设定命令类型
            ' CommandType.Text 表示原始SQL语句; 
            ' CommandType.StoredProcedure 表示存储过程.
            cmd.CommandType = CommandType.Text

            ' 从FormView控件的ItemTemplate中获取PersonID.
            ' 
            Dim strPersonID As String = DirectCast(fvPerson.Row.FindControl("lblPersonID"), Label).Text

            ' 向SqlCommand添加参数并设值.
            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID

            ' 打开数据链接.
            conn.Open()

            ' 执行命令.
            cmd.ExecuteNonQuery()
        End Using

        ' 重新绑定FormView控件以显示删除后的数据.
        BindFormView()
    End Sub

    ' FormView.ItemInserting 事件
    Protected Sub fvPerson_ItemInserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewInsertEventArgs) Handles fvPerson.ItemInserting
        ' 从Web.config获取链接字符串.
        ' 当我们使用Using语句时, 
        ' 不需要显式释放代码中的对象, 
        ' using语句会处理他们.
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' 新建一个命令对象.
            Dim cmd As New SqlCommand()

            ' 将数据链接关联到命令.
            cmd.Connection = conn

            ' 设定命令文本
            ' SQL语句或者存储过程名字. 
            cmd.CommandText = "INSERT INTO Person ( LastName, FirstName, Picture ) VALUES ( @LastName, @FirstName, @Picture )"

            ' 设定命令类型
            ' CommandType.Text 表示原始SQL语句; 
            ' CommandType.StoredProcedure 表示存储过程.
            cmd.CommandType = CommandType.Text

            ' 从FormView控件的InsertItemTemplate中获取名和姓.
            ' 
            Dim strLastName As String = DirectCast(fvPerson.Row.FindControl("tbLastName"), TextBox).Text
            Dim strFirstName As String = DirectCast(fvPerson.Row.FindControl("tbFirstName"), TextBox).Text

            ' 向SqlCommand添加参数并设值.
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = strLastName
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = strFirstName

            Dim uploadPicture As FileUpload = DirectCast(fvPerson.FindControl("uploadPicture"), FileUpload)

            If uploadPicture.HasFile Then
                ' 向SqlCommand添加图片参数.
                ' 如果已指定一张图片，将此参数设为此图片的按字节的值.
                ' 
                cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = uploadPicture.FileBytes
            Else
                ' 向SqlCommand添加图片参数.
                ' 如果未指定图片，将此参数的值设为 
                ' NULL.
                cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = DBNull.Value
            End If

            ' 打开数据链接.
            conn.Open()

            ' 执行命令.
            cmd.ExecuteNonQuery()
        End Using

        ' 将FormView控件切换到只读显示模式. 
        fvPerson.ChangeMode(FormViewMode.ReadOnly)

        ' 重新绑定FormView控件以显示插入后的数据.
        BindFormView()
    End Sub

    ' FormView.ItemUpdating 事件
    Protected Sub fvPerson_ItemUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewUpdateEventArgs) Handles fvPerson.ItemUpdating
        ' 从Web.config获取链接字符串.
        ' 当我们使用Using语句时, 
        ' 不需要显式释放代码中的对象, 
        ' using语句会处理他们.
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' 新建一个命令对象.
            Dim cmd As New SqlCommand()

            ' 将数据链接关联到命令.
            cmd.Connection = conn

            ' 设定命令文本
            ' SQL语句或者存储过程名字. 
            cmd.CommandText = "UPDATE Person SET LastName = @LastName, FirstName = @FirstName, Picture = ISNULL(@Picture,Picture) WHERE PersonID = @PersonID"

            ' 设定命令类型
            ' CommandType.Text 表示原始SQL语句; 
            ' CommandType.StoredProcedure 表示存储过程.
            cmd.CommandType = CommandType.Text

            ' 从FormView控件的EditItemTemplate中获取人物ID、名和姓.
            ' 
            Dim strPersonID As String = DirectCast(fvPerson.Row.FindControl("lblPersonID"), Label).Text
            Dim strLastName As String = DirectCast(fvPerson.Row.FindControl("tbLastName"), TextBox).Text
            Dim strFirstName As String = DirectCast(fvPerson.Row.FindControl("tbFirstName"), TextBox).Text

            ' 向SqlCommand添加参数并设值.
            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = strLastName
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = strFirstName

            ' 找到FormView控件的EditItemTemplate中的FileUpload控件. 
            ' 
            Dim uploadPicture As FileUpload = DirectCast(fvPerson.FindControl("uploadPicture"), FileUpload)

            If uploadPicture.HasFile Then
                ' 向SqlCommand添加图片参数.
                ' 如果已指定一张图片，将此参数设为此图片的按字节的值.
                ' 
                cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = uploadPicture.FileBytes
            Else
                ' 向SqlCommand添加图片参数.
                ' 如果未指定图片，将此参数的值设为 
                ' NULL.
                cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = DBNull.Value
            End If

            ' 打开数据链接.
            conn.Open()

            ' 执行命令.
            cmd.ExecuteNonQuery()
        End Using

        ' 将FormView控件切换到只读显示模式.
        fvPerson.ChangeMode(FormViewMode.ReadOnly)

        ' 重新绑定FormView控件以显示更新后的数据.
        BindFormView()
    End Sub

    ' FormView.PageIndexChanging 事件
    Protected Sub fvPerson_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewPageEventArgs) Handles fvPerson.PageIndexChanging
        ' 设定新页面的索引. 
        fvPerson.PageIndex = e.NewPageIndex

        ' 重新绑定FormView控件以显示新页面的数据.
        BindFormView()
    End Sub

    ' FormView.ModeChanging 事件
    Protected Sub fvPerson_ModeChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.FormViewModeEventArgs) Handles fvPerson.ModeChanging
        ' 将FormView control切换到新模式
        fvPerson.ChangeMode(e.NewMode)

        ' 重新绑定FormView控件以新模式显示数据.
        BindFormView()
    End Sub
End Class