'* /****************************** 模块 标识 ******************************\
'* 模块名:    SchoolLinqToEntitiesUpdate.xaml.vb
'* 项目:      VBADONETDataServiceSL3Client
'* 版权       (c) Microsoft Corporation.
'* 
'* SchoolLinqToEntitiesUpdate.vb 展示如何在Silverlight中调用 ADO.NET Data Service
'* 来进行查询与更新。
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'\**************************************************************************

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.Data.Services.Client
Imports VBADONETDataServiceSL3Client.SchoolLinqToEntitiesService
Imports System.Windows.Browser
Imports System.Globalization
Imports System.IO
Imports System.Net.Browser

Partial Public Class SchoolLinqToEntitiesUpdate
    Inherits UserControl
    '  DataGrid 控件的数据源。
    Private _collection As New List(Of ScoreCardForSchoolLinqToEntities)()
    ' ADO.NET Data Service 的 URL。
    Private Const _schoolLinqToEntitiesUri As String = "http://localhost:8888/SchoolLinqToEntities.svc"
    ' collection => returnedCourseGrade => _entities ={via async REST call}=> ADO.NET Data Service
    Private _entities As SQLServer2005DBEntities

    Public Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, AddressOf MainPage_Loaded
    End Sub

    Private Sub MainPage_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)

        LoadData()
    End Sub

    ''' <summary>
    ''' 本方法中，发送一个异步的REST请求到ADO.NET Data Service来获得 CourseGrade 数据
    ''' 我们扩展了 Person 以及 Course，所以关联记录也会被返回到客户端。
    ''' </summary>
    Private Sub LoadData()
        _entities = New SQLServer2005DBEntities(New Uri(_schoolLinqToEntitiesUri))
        Dim query As DataServiceQuery(Of CourseGrade) = DirectCast((From c In _entities.CourseGrade.Expand("Person").Expand("Course") _
            Select c), DataServiceQuery(Of CourseGrade))

        query.BeginExecute(AddressOf OnCourseGradeQueryComplete, query)
    End Sub

    ''' <summary>
    ''' CourseGrade查询的回调方法。
    ''' </summary>
    ''' <param name="result"></param>
    Private Sub OnCourseGradeQueryComplete(ByVal result As IAsyncResult)
        Dispatcher.BeginInvoke(Function() CourseGradeQueryComplete(result))
    End Sub

    Private Function CourseGradeQueryComplete(ByVal result As IAsyncResult)

        Dim query As DataServiceQuery(Of CourseGrade) = TryCast(result.AsyncState, DataServiceQuery(Of CourseGrade))
        Try
            Dim returnedCourseGrade = query.EndExecute(result)

            If returnedCourseGrade IsNot Nothing Then
                ' 由于在服务器端有一个查询拦截器(如下)，所以服务器端只能返回ID大于4000的Course数据
                ' [QueryInterceptor("Course")]
                ' public Expression<Func<Course, bool>> QueryCourse()
                ' {
                ' // LINQ lambda expression to filter the course objects
                ' return c => c.CourseID > 4000;
                ' }

                _collection = (From c In returnedCourseGrade.ToList() _
                    Select New ScoreCardForSchoolLinqToEntities With {.CourseGrade = c, _
                                                                      .Course = If(c.Course Is Nothing, "只有 Course ID>4000 才会被显示", c.Course.Title), _
                                                                      .Grade = c.Grade, _
                                                                      .PersonName = String.Format("{0} {1}", c.Person.FirstName, c.Person.LastName) _
                                                                      }).ToList()

                Me.mainDataGrid.ItemsSource = _collection
            End If
        Catch ex As DataServiceQueryException
            Me.messageTextBlock.Text = String.Format("错误: {0} - {1}", ex.Response.StatusCode.ToString(), ex.Response.[Error].Message)
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' 本事件句柄处理的是DataGrid控件的RowEdited事件。
    ''' 在事件中调用ADO.NET Data Service，更新了被编辑的Grade数据。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mainDataGrid_RowEditEnded(ByVal sender As Object, ByVal e As DataGridRowEditEndedEventArgs)
        Dim s As ScoreCardForSchoolLinqToEntities = TryCast(e.Row.DataContext, ScoreCardForSchoolLinqToEntities)
        If s IsNot Nothing Then
            Dim recordforupdate As CourseGrade = s.CourseGrade
            _entities.UpdateObject(recordforupdate)

            _entities.BeginSaveChanges(SaveChangesOptions.ReplaceOnUpdate, AddressOf OnChangesSaved, _entities)

        End If
    End Sub

    ''' <summary>
    ''' 更新操作的回调方法。
    ''' </summary>
    ''' <param name="result"></param>
    Private Sub OnChangesSaved(ByVal result As IAsyncResult)
        Dispatcher.BeginInvoke(Function() ChangesSaved(result))
    End Sub

    Private Function ChangesSaved(ByVal result As IAsyncResult)

        Dim svcContext As SQLServer2005DBEntities = TryCast(result.AsyncState, SQLServer2005DBEntities)

        Try
            ' 完成保存改变操作并显示结果。
            WriteOperationResponse(svcContext.EndSaveChanges(result))
        Catch ex As DataServiceRequestException
            ' 显示错误信息。
            WriteOperationResponse(ex.Response)
        Catch ex As InvalidOperationException
            messageTextBlock.Text = ex.Message
        Finally
            ' 重新加载绑定的集合来刷新Grid，显示操作的结果。 
            ' 你也可以删除下边的代码行，并通过刷新本也或者直接查看数据库来看操作结果。
            LoadData()
        End Try
        Return Nothing
    End Function
    ''' <summary>
    ''' 本方法用以显示 ADO.NET Data Service 返回的详细信息。
    ''' </summary>
    ''' <param name="response"></param>
    Private Sub WriteOperationResponse(ByVal response As DataServiceResponse)
        messageTextBlock.Text = String.Empty
        Dim i As Integer = 1

        If response.IsBatchResponse Then
            messageTextBlock.Text = String.Format("批处理返回状态码: {0}" & vbLf, response.BatchStatusCode)
        End If
        For Each change As ChangeOperationResponse In response
            messageTextBlock.Text += String.Format(vbTab & "更改 {0} 状态码: {1}" & vbLf, i.ToString(), change.StatusCode.ToString())
            If change.[Error] IsNot Nothing Then
                String.Format(vbTab & "更改 {0} 错误: {1}" & vbLf, i.ToString(), change.[Error].Message)
            End If
            i += 1
        Next
    End Sub


End Class