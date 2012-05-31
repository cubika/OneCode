'/****************************** 模块 标识 ******************************\
'* 模块名:    SampleServiceInsert.xaml.vb
'* 项目:      VBADONETDataServiceSL3Client
'* 版权       (c) Microsoft Corporation.
'* 
'* SampleServiceInsert.vb 展示如何在Silverlight中调用 ASP.NET Data Service
'* 进行查询与增加。
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
Imports VBADONETDataServiceSL3Client.SampleService
Imports System.Data.Services.Client

Partial Public Class SampleServiceInsert
    Inherits UserControl
    ' ADO.NET Data Service 的 URL。
    Private Const _sampleUri As String = "http://localhost:8888/Samples.svc"
    ' returnedCategory => _context ={via async REST call}=> ADO.NET Data Service
    Private _context As SampleProjects
    Public Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, AddressOf SampleServiceInsert_Loaded
    End Sub

    Private Sub SampleServiceInsert_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        LoadData()
    End Sub

    ''' <summary>
    ''' 本方法中，我们向ADO.NET Data Service发送了一个异步的REST请求，来获得Category
    ''' 记录。. 
    ''' </summary>
    Private Sub LoadData()
        _context = New SampleProjects(New Uri(_sampleUri))
        Dim query As DataServiceQuery(Of Category) = DirectCast((From c In _context.Categories _
            Select c), DataServiceQuery(Of Category))

        query.BeginExecute(AddressOf OnCategoryQueryComplete, query)
    End Sub

    ''' <summary>
    ''' Category查询请求的回调方法。
    ''' </summary>
    ''' <param name="result"></param>
    Private Sub OnCategoryQueryComplete(ByVal result As IAsyncResult)
        Dispatcher.BeginInvoke(Function() CategoryQueryComplete(result))
    End Sub

    Private Function CategoryQueryComplete(ByVal result As IAsyncResult)

        Dim query As DataServiceQuery(Of Category) = TryCast(result.AsyncState, DataServiceQuery(Of Category))
        Try
            Dim returnedCategory = query.EndExecute(result)

            If returnedCategory IsNot Nothing Then

                Me.mainDataGrid.ItemsSource = returnedCategory.ToList()
            End If
        Catch ex As DataServiceQueryException
            Me.messageTextBlock.Text = String.Format("错误: {0} - {1}", ex.Response.StatusCode.ToString(), ex.Response.[Error].Message)
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' 本事件中，我们向 ADO.NET Data Service发送一个异步的REST请求
    ''' 来增加一条Category记录。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub InsertButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim categorynameforinsert As String = Me.categoryTextBox.Text
        _context.AddToCategories(New Category With {.CategoryName = categorynameforinsert})
        _context.BeginSaveChanges(AddressOf OnChangesSaved, _context)
    End Sub

    ''' <summary>
    '''  增加请求的回调方法。
    ''' </summary>
    ''' <param name="result"></param>
    Private Sub OnChangesSaved(ByVal result As IAsyncResult)
        Dispatcher.BeginInvoke(Function() ChangesSaved(result))
    End Sub

    Private Function ChangesSaved(ByVal result As IAsyncResult)
        Dim svcContext As SampleProjects = TryCast(result.AsyncState, SampleProjects)

        Try
            ' 完成保存改变操作显示结果。
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