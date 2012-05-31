'******************************** 模块头 *********************************\
'* 模块名:   TableStoragePagingUtility.vb
'* 项目名:   AzureTableStoragePaging
'* 版权 (c) Microsoft Corporation.
'* 
'* 这个类能被其它应用程序重复利用.如果你想再利用这些代码,你需要做的是实现自定
'* 义的ICachedDataProvider<T>类来存储TableStoragePagingUtility<T>所需要的数据.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\**************************************************************************/
Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.StorageClient

Namespace Utilities
    Public Class TableStoragePagingUtility(Of T)
        Private _cloudStorageAccount As CloudStorageAccount
        Private _provider As ICachedDataProvider(Of T)
        Private _tableServiceContext As TableServiceContext
        Private Property RC() As ResultContinuation
            Get
                Return _provider.GetResultContinuation()
            End Get
            Set(ByVal value As ResultContinuation)
                _provider.SetResultContinuation(value)
            End Set
        End Property
        Private _entitySetName As String
        Private _pageSize As Integer
        Public Property CurrentPageIndex() As Integer
            Get
                Return _provider.GetCurrentIndex()
            End Get
            Private Set(ByVal value As Integer)
                _provider.SetCurrentIndex(value)
            End Set
        End Property
        Public Property PageSize() As Integer
            Get
                Return _pageSize
            End Get
            Private Set(ByVal value As Integer)
                _pageSize = value
            End Set
        End Property

        Public Sub New(ByVal provider As ICachedDataProvider(Of T), ByVal cloudStorageAccount As CloudStorageAccount, ByVal tableServiceContext As TableServiceContext, ByVal pageSize As Integer, ByVal entitySetName As String)
            Me._provider = Provider
            Me._cloudStorageAccount = CloudStorageAccount
            Me._entitySetName = entitySetName
            Me._tableServiceContext = TableServiceContext
            If PageSize <= 0 Then
                Throw New IndexOutOfRangeException("pageSize out of range")
            End If
            Me.PageSize = PageSize
        End Sub

        ''' <summary>
        ''' 获得下一个页面
        ''' </summary>
        ''' <returns>下一个页面.如果当前页面是最后一页,返回最后一页.</returns>
        Public Function GetNextPage() As IEnumerable(Of T)
            ' 获取缓存数据
            Dim cachedData = Me._provider.GetCachedData()
            Dim pageCount As Integer = 0
            If Not cachedData Is Nothing Then
                pageCount = Convert.ToInt32(Math.Ceiling(CDbl(cachedData.Count()) / CDbl(Me.PageSize)))
            End If
            ' 如果存储表中仍然有实体可读并且当前页是最后一个页面,
            ' 请求表存储获取新数据.
            If (Not Me._provider.HasReachedEnd) AndAlso CurrentPageIndex = pageCount - 1 Then
                Dim q = Me._tableServiceContext.CreateQuery(Of T)(Me._entitySetName).Take(PageSize).AsTableServiceQuery()
                Dim r As IAsyncResult
                r = q.BeginExecuteSegmented(RC, Function(ar) 1 = 1, q)
                r.AsyncWaitHandle.WaitOne()
                Dim result As ResultSegment(Of T) = q.EndExecuteSegmented(r)
                Dim results = result.Results
                Me._provider.AddDataToCache(results)
                ' 如果有实体返回我们需要增加页面数
                If results.Count() > 0 Then
                    pageCount += 1
                End If
                RC = result.ContinuationToken
                ' 如果返回记号为空意味着表中不再有实体了
                If result.ContinuationToken Is Nothing Then
                    Me._provider.HasReachedEnd = True
                End If
            End If
            If CurrentPageIndex + 1 < pageCount Then
                CurrentPageIndex = CurrentPageIndex + 1
            Else
                CurrentPageIndex = pageCount - 1
            End If
            If cachedData Is Nothing Then
                cachedData = Me._provider.GetCachedData()
            End If
            Return cachedData.Skip((Me.CurrentPageIndex) * Me.PageSize).Take(Me.PageSize)
        End Function

        ''' <summary>
        ''' 获得前一个页面
        ''' </summary>
        ''' <returns>前一个页面.如果当前页是第一页，返回第一页.如果缓存中没有数据,返回空集/></returns>
        Public Function GetPreviousPage() As IEnumerable(Of T)

            Dim cachedData = Me._provider.GetCachedData()
            If Not cachedData Is Nothing AndAlso cachedData.Count() > 0 Then
                If CurrentPageIndex - 1 < 0 Then
                    CurrentPageIndex = 0
                Else
                    CurrentPageIndex = CurrentPageIndex - 1
                End If
                Return cachedData.Skip(Me.CurrentPageIndex * Me.PageSize).Take(Me.PageSize)
            Else
                Return New List(Of T)()
            End If
        End Function

        ''' <summary>
        ''' 如果有实体的缓存,返回当前页,否则检索表存储并返回第一个页面.
        ''' </summary>
        ''' <returns>如果有实体缓存，返回当前页.
        ''' 如果没有数据缓存，第一个页面将会从表存储中重新取回并被返回.</returns>
        Public Function GetCurrentOrFirstPage() As IEnumerable(Of T)
            Dim cachedData = Me._provider.GetCachedData()
            If Not cachedData Is Nothing AndAlso cachedData.Count() > 0 Then
                Return cachedData.Skip(Me.CurrentPageIndex * Me.PageSize).Take(Me.PageSize)
            Else
                Return GetNextPage()
            End If
        End Function
    End Class

    ''' <summary>
    ''' 这个类实现这个接口必须负责缓存存储，包括数据、ResultContinuation和HasReachedEnd标记.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Interface ICachedDataProvider(Of T)
        ''' <summary>
        ''' 返回所有缓存数据
        ''' </summary>
        ''' <returns></returns>
        Function GetCachedData() As IEnumerable(Of T)
        ''' <summary>
        ''' 把数据保存到缓存
        ''' </summary>
        ''' <param name="data">这个供应者的用户希望添加到缓存的数据</param>
        Sub AddDataToCache(ByVal data As IEnumerable(Of T))
        ''' <summary>
        ''' 保存当前索引
        ''' </summary>
        ''' <param name="index">这个供应者的用户发送的当前页面索引</param>
        Sub SetCurrentIndex(ByVal index As Integer)
        ''' <summary>
        ''' 获取当前索引
        ''' </summary>
        ''' <returns>保存在缓存的当前页面索引</returns>
        Function GetCurrentIndex() As Integer
        ''' <summary>
        ''' 设置连续记号
        ''' </summary>
        ''' <param name="rc">这个供应者的用户发送的ResultContinuation</param>
        Sub SetResultContinuation(ByVal rc As ResultContinuation)
        ''' <summary>
        ''' 获取连续记号
        ''' </summary>
        ''' <returns>缓存中保存的ResultContinuation</returns>
        Function GetResultContinuation() As ResultContinuation
        ''' <summary>
        ''' 一个标记告诉这个供应者的用户他是否可以充分信赖缓存而不用从表存储中获取新数据.
        ''' </summary>
        Property HasReachedEnd() As Boolean
    End Interface

    ''' <summary>
    ''' 实现ICachedDataProvider<T>的一个例子为MVC应用程序把数据缓存在Session中.
    ''' 因为它的实现使用MVC的Session,这个类的用户需要被告知不要在其它的Session变量中使用这个类的保留关键字
    ''' （例如一个以"currentindex"开始的变量.如果他们不得不使用它，他们可以定义一个特殊的ID来区分它们
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class MVCSessionCachedDataProvider(Of T)
        Implements ICachedDataProvider(Of T)
        Private _session As HttpSessionStateBase
        Private _id As String
        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="c">为这个参数大体地定义这个</param>
        ''' <param name="id">缓存提供者的id,你需要使用相同的id进入同一个的高速缓存</param>
        Public Sub New(ByVal c As Controller, ByVal id As String)
            _session = c.Session
            _id = id
            ' 初始化currentindex
            If _session("currentindex" & Me._id) Is Nothing Then
                _session("currentindex" & Me._id) = -1
            End If
            ' 初始化hasreachedend标记指出是否不需要检索新数据
            If _session("hasreachedend" & Me._id) Is Nothing Then
                _session("hasreachedend" & Me._id) = False
            End If
        End Sub
        Public Function GetCachedData() As IEnumerable(Of T) Implements ICachedDataProvider(Of T).GetCachedData
            Return TryCast(_session("inmemorycacheddata" & Me._id), List(Of T))
        End Function
        Public Sub AddDataToCache(ByVal data As IEnumerable(Of T)) Implements ICachedDataProvider(Of T).AddDataToCache
            Dim inmemorycacheddata = TryCast(_session("inmemorycacheddata" & Me._id), List(Of T))
            If inmemorycacheddata Is Nothing Then
                inmemorycacheddata = New List(Of T)()
            End If
            inmemorycacheddata.AddRange(data)
            _session("inmemorycacheddata" & Me._id) = inmemorycacheddata
        End Sub
        Public Sub SetCurrentIndex(ByVal index As Integer) Implements ICachedDataProvider(Of T).SetCurrentIndex
            _session("currentindex" & Me._id) = index
        End Sub
        Public Function GetCurrentIndex() As Integer Implements ICachedDataProvider(Of T).GetCurrentIndex
            Return Convert.ToInt32(_session("currentindex" & Me._id))
        End Function
        Public Function GetResultContinuation() As ResultContinuation Implements ICachedDataProvider(Of T).GetResultContinuation
            Return TryCast(_session("resultcontinuation" & Me._id), ResultContinuation)
        End Function
        Public Sub SetResultContinuation(ByVal rc As ResultContinuation) Implements ICachedDataProvider(Of T).SetResultContinuation
            _session("resultcontinuation" & Me._id) = rc
        End Sub

        Public Property HasReachedEnd() As Boolean Implements ICachedDataProvider(Of T).HasReachedEnd
            Get
                Return Convert.ToBoolean(_session("hasreachedend" & Me._id))
            End Get
            Set(ByVal value As Boolean)
                _session("hasreachedend" & Me._id) = value
            End Set
        End Property


    End Class
End Namespace

