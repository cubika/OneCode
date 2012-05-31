'/****************************** 模块 标识 ******************************\
'* 模块名:    SchoolLinqToSQLServiceDelete.xaml.vb
'* 项目:      VBADONETDataServiceSL3Client
'* 版权       (c) Microsoft Corporation.
'* 
'* SchoolLinqToSQLServiceDelete.vb 展示如何在Silverlight中调用 ASP.NET Data Service
'* 进行查询与删除操作。
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'\***************************************************************************/

Imports VBADONETDataServiceSL3Client.SchoolLinqToSQLService
Imports System.Data.Services.Client

Partial Public Class SchoolLinqToSQLServiceDelete
    Inherits UserControl
    ' DataGrid 控件的数据源.
    Private _collection As New List(Of ScoreCardForSchoolLinqToSQL)()
    ' ADO.NET Data Service 的 URL
    Private Const _schoolLinqToSQLUri As String = "http://localhost:8888/SchoolLinqToSQL.svc"
    ' collection => returnedCourseGrade => _context ={via async REST call}=> ADO.NET Data Service
    Private _context As SchoolLinqToSQLDataContext

    Public Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, AddressOf SchoolLinqToSQLServiceDelete_Loaded
    End Sub

    Private Sub SchoolLinqToSQLServiceDelete_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        LoadData()
    End Sub

    ''' <summary>
    ''' 本方法中，发送一个异步的REST请求到ADO.NET Data Service来获得 CourseGrade 数据
    ''' 我们扩展了 Person 以及 Course，所以关联记录也会被返回到客户端。
    ''' </summary>
    Private Sub LoadData()
        _context = New SchoolLinqToSQLDataContext(New Uri(_schoolLinqToSQLUri))
        Dim query As DataServiceQuery(Of CourseGrade) = DirectCast((From c In _context.CourseGrades.Expand("Person").Expand("Course") _
            Select c), DataServiceQuery(Of CourseGrade))

        query.BeginExecute(AddressOf OnCourseGradeQueryComplete, query)
    End Sub

    ''' <summary>
    '''CourseGrade查询的回调方法。
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
                _collection = (From c In returnedCourseGrade.ToList() _
                    Select New ScoreCardForSchoolLinqToSQL With {.CourseGrade = c, _
                                                                .Course = c.Course.Title, _
                                                                .Grade = c.Grade, _
                                                                .PersonName = String.Format("{0} {1}", _
                                                                                            c.Person.FirstName, c.Person.LastName)}).ToList()

                Me.mainDataGrid.ItemsSource = _collection
            End If
        Catch ex As DataServiceQueryException
            Me.messageTextBlock.Text = String.Format("Error: {0} - {1}", ex.Response.StatusCode.ToString(), ex.Response.[Error].Message)

        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' 本事件句柄处理的是删除按钮控件的点击事件。
    ''' 事件句柄中通过调用 ADO.NET Data Service 删除与删除按钮在同一行的数据。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DeleteButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim b As Button = DirectCast(sender, Button)
        Dim s As ScoreCardForSchoolLinqToSQL = TryCast(b.DataContext, ScoreCardForSchoolLinqToSQL)
        If s IsNot Nothing Then
            Dim recordfordelete As CourseGrade = s.CourseGrade
            _context.DeleteObject(recordfordelete)
            _context.BeginSaveChanges(SaveChangesOptions.None, AddressOf OnChangesSaved, _context)
        End If
    End Sub

    ''' <summary>
    ''' 删除操作的回调事件。
    ''' </summary>
    ''' <param name="result"></param>
    Private Sub OnChangesSaved(ByVal result As IAsyncResult)
        Dispatcher.BeginInvoke(Function() ChangesSaved(result))
    End Sub

    Private Function ChangesSaved(ByVal result As IAsyncResult)
        Dim svcContext As SchoolLinqToSQLDataContext = TryCast(result.AsyncState, SchoolLinqToSQLDataContext)

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
            ' 你也可以删除下边的代码行，并通过刷新本页或者直接查看数据库来看操作结果。
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
            messageTextBlock.Text = String.Format("Batch operation response code: {0}" & vbLf, response.BatchStatusCode)
        End If
        For Each change As ChangeOperationResponse In response
            messageTextBlock.Text += String.Format(vbTab & "Change {0} code: {1}" & vbLf, i.ToString(), change.StatusCode.ToString())
            If change.[Error] IsNot Nothing Then
                String.Format(vbTab & "Change {0} error: {1}" & vbLf, i.ToString(), change.[Error].Message)
            End If
            i += 1
        Next
    End Sub
End Class
