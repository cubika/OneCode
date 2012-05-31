'****************************** 模块头******************************'
' 模块名称:  MultiThreadedWebDownloader.vb
' 项目名称:	    VBMultiThreadedWebDownloader
' 版权 (c) Microsoft Corporation.
' 
'  这个类是采用多线程通过网络下载文件，它提供了一些公有的方法： 
' Start, Pause, Resume and Cancel 
' 
' 在开始下载之前，远程服务器会检查它是否支持"Accept-Ranges" .
' 
' 当下载开始，它将检查文件是否存在。如果不存在，则创建一个新的和要下载一样大小的文件。
'  然后创建多个HttpDownloadClients开始下载文件
'
' 当下载一个指定大小的数据时 ，它将触发 DownloadProgressChanged事件。
' 当下载结束或者被取消时，会触发 DownloadCompleted事件。
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.IO
Imports System.Linq
Imports System.Net

Public Class MultiThreadedWebDownloader
    '用于计算下载速度
    Private Shared _locker As New Object()


    ''' <summary>
    ''' 文件下载的地址. 
    ''' </summary>
    Private _url As Uri
    Public Property Url() As Uri
        Get
            Return _url
        End Get
        Private Set(ByVal value As Uri)
            _url = value
        End Set
    End Property

    ''' <summary>
    ''' 判断远程是否支持"Accept-Ranges"头.
    ''' </summary>
    Private _isRangeSupported As Boolean
    Public Property IsRangeSupported() As Boolean
        Get
            Return _isRangeSupported
        End Get
        Private Set(ByVal value As Boolean)
            _isRangeSupported = value
        End Set
    End Property

    ''' <summary>
    ''' 文件的大小
    ''' </summary>
    Private _totalSize As Long
    Public Property TotalSize() As Long
        Get
            Return _totalSize
        End Get
        Private Set(ByVal value As Long)
            _totalSize = value
        End Set
    End Property

    Private _downloadPath As String

    ''' <summary>
    ''' 文件的存储路径。
    ''' 如果不存在相同名字的文件，则创建一个新的。
    ''' </summary>
    Public Property DownloadPath() As String
        Get
            Return _downloadPath
        End Get
        Set(ByVal value As String)
            If Me.Status <> MultiThreadedWebDownloaderStatus.Checked Then
                Throw New ApplicationException(
                    "当状态是Checked时，才可以设置路径.")
            End If

            _downloadPath = value
        End Set
    End Property

    ''' <summary>
    ''' 所有下载端的Proxy情况。
    ''' </summary>
    Public Property Proxy() As WebProxy

    ''' <summary>
    ''' 下载文件的大小。
    ''' </summary>
    Private _downloadedSize As Long
    Public Property DownloadedSize() As Long
        Get
            Return _downloadedSize
        End Get
        Private Set(ByVal value As Long)
            _downloadedSize = value
        End Set
    End Property


    ' 存储下载时已用多长时间.不包含暂停的时间。 
    '当下载状态是暂停，取消，或者完成 时，它才会更新。
    Private _usedTime As New TimeSpan()

    Private _lastStartTime As Date

    ''' <summary>
    ''' 如果下载状态是Downloading,总共用的时间就是usedTime. 否则，总时间应该包括 
    '''当前现在线程所用的时间。
    ''' </summary>
    Public ReadOnly Property TotalUsedTime() As TimeSpan
        Get
            If Me.Status <> MultiThreadedWebDownloaderStatus.Downloading Then
                Return _usedTime
            Else
                Return _usedTime.Add(Date.Now.Subtract(_lastStartTime))
            End If
        End Get
    End Property

    ' DownloadProgressChanged时间中的时间和大小。
    '下面的2个是用来计算下载速度的。
    Private _lastNotificationTime As Date

    Private _lastNotificationDownloadedSize As Long

    Private _bufferCount As Integer = 0

    ''' <summary>
    ''' 如果得到缓冲数，则触发DownloadProgressChanged事件。
    ''' </summary>
    Private _bufferCountPerNotification As Integer
    Public Property BufferCountPerNotification() As Integer
        Get
            Return _bufferCountPerNotification
        End Get
        Private Set(ByVal value As Integer)
            _bufferCountPerNotification = value
        End Set
    End Property

    ''' <summary>
    ''' 当在响应流中读取数据时设置 BufferSize。
    ''' </summary>
    Private _bufferSize As Integer
    Public Property BufferSize() As Integer
        Get
            Return _bufferSize
        End Get
        Private Set(ByVal value As Integer)
            _bufferSize = value
        End Set
    End Property

    ''' <summary>
    ''' 缓存大小
    ''' </summary>
    Private _maxCacheSize As Integer
    Public Property MaxCacheSize() As Integer
        Get
            Return _maxCacheSize
        End Get
        Private Set(ByVal value As Integer)
            _maxCacheSize = value
        End Set
    End Property

    Private _status As MultiThreadedWebDownloaderStatus

    ''' <summary>
    ''' 如果状态改变，触发StatusChanged事件。
    ''' </summary>
    Public Property Status() As MultiThreadedWebDownloaderStatus
        Get
            Return _status
        End Get

        Private Set(ByVal value As MultiThreadedWebDownloaderStatus)
            If _status <> value Then
                _status = value
                Me.OnStatusChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    ''' <summary>
    ''' 最大线程数。线程数的计算=最小值+TotalSize / MaxCacheSize。
    ''' </summary>
    Private _maxThreadCount As Integer
    Public Property MaxThreadCount() As Integer
        Get
            Return _maxThreadCount
        End Get
        Private Set(ByVal value As Integer)
            _maxThreadCount = value
        End Set
    End Property

    ' HttpDownloadClients 用来下载文件。每一个它的实例使用一个线程，下载文件的一部分。
    Private _downloadClients As List(Of HttpDownloadClient) = Nothing

    Public ReadOnly Property DownloadThreadsCount() As Integer
        Get
            If _downloadClients IsNot Nothing Then
                Return _downloadClients.Count
            Else
                Return 0
            End If
        End Get
    End Property

    Public Event DownloadProgressChanged _
        As EventHandler(Of MultiThreadedWebDownloaderProgressChangedEventArgs)

    Public Event DownloadCompleted _
        As EventHandler(Of MultiThreadedWebDownloaderCompletedEventArgs)

    Public Event StatusChanged As EventHandler

    Public Event ErrorOccurred As EventHandler(Of ErrorEventArgs)


    ''' <summary>
    '''下载整个文件。默认的缓冲区大小是1KB，内存缓存是1MB，每个通知缓冲区计数为64。线程个数是逻辑处理器数量的一倍。
    ''' </summary>
    Public Sub New(ByVal url As String)
        Me.New(url, 1024, 1048576, 512, Environment.ProcessorCount * 2)
    End Sub

    Public Sub New(ByVal url As String,
                   ByVal bufferSize As Integer,
                   ByVal cacheSize As Integer,
                   ByVal bufferCountPerNotification As Integer,
                   ByVal maxThreadCount As Integer)

        If bufferSize < 0 Then
            Throw New ArgumentOutOfRangeException(
                "BufferSize 不能小于  0. ")
        End If

        If cacheSize < bufferSize Then
            Throw New ArgumentOutOfRangeException(
                "MaxCacheSize 不能小于  BufferSize ")
        End If

        If bufferCountPerNotification <= 0 Then
            Throw New ArgumentOutOfRangeException(
                "BufferCountPerNotification 不能小于  0. ")
        End If

        If maxThreadCount < 1 Then
            Throw New ArgumentOutOfRangeException(
                "maxThreadCount 不能小于  1. ")
        End If

        Me.Url = New Uri(url)
        Me.BufferSize = bufferSize
        Me.MaxCacheSize = cacheSize
        Me.BufferCountPerNotification = bufferCountPerNotification

        Me.MaxThreadCount = maxThreadCount

        ' 设置最大连接数（被ServicePoint允许的范围下）
        ServicePointManager.DefaultConnectionLimit = maxThreadCount

        ' 初始化HttpDownloadClient。
        _downloadClients = New List(Of HttpDownloadClient)()

        ' 设置闲置状态。
        Me.Status = MultiThreadedWebDownloaderStatus.Idle
    End Sub

    ''' <summary>
    ''' 检查总共大小和文件的IsRangeSupported。
    ''' 如果没有任何异常，文件就可以被下载 。 
    ''' </summary>
    Public Sub CheckFile()

        ' 文件只会在闲置的状态下检查。
        If Me.Status <> MultiThreadedWebDownloaderStatus.Idle Then
            Throw New ApplicationException(
                "文件只会在闲置的状态下检查。")
        End If

        ' 在远程服务器上检查文件的信息。
        Dim request = CType(WebRequest.Create(Url), HttpWebRequest)

        ' 设置 proxy.
        If Proxy IsNot Nothing Then
            request.Proxy = Proxy
        End If

        Using response = request.GetResponse()
            Me.IsRangeSupported = response.Headers.AllKeys.Contains("Accept-Ranges")
            Me.TotalSize = response.ContentLength

            If TotalSize <= 0 Then
                Throw New ApplicationException("要下载的文件不存在!")
            End If
        End Using

        ' 设置为checked状态。
        Me.Status = MultiThreadedWebDownloaderStatus.Checked
    End Sub


    ''' <summary>
    ''' 检查目标文件是否存在。如果不存在，则创建一个新的和药下载的一样大小的文件。
    ''' </summary>
    Private Sub CheckFileOrCreateFile()
        '  锁定其他线程或者进程，去阻止创建本地文件。
        SyncLock _locker
            Dim fileToDownload As New FileInfo(DownloadPath)
            If fileToDownload.Exists Then

                ' 目标文件应该和下载的文件一样大小。
                If fileToDownload.Length <> Me.TotalSize Then
                    Throw New ApplicationException(
                        "路径下有同名的文件")
                End If

                ' 创建一个文件.
            Else
                Using fileStream_Renamed As FileStream = File.Create(Me.DownloadPath)
                    Dim createdSize As Long = 0
                    Dim buffer(4095) As Byte
                    Do While createdSize < TotalSize
                        Dim bufferSize As Integer =
                            If((TotalSize - createdSize) < 4096,
                               CInt(Fix(TotalSize - createdSize)),
                               4096)
                        fileStream_Renamed.Write(buffer, 0, bufferSize)
                        createdSize += bufferSize
                    Loop
                End Using
            End If
        End SyncLock
    End Sub

    ''' <summary>
    '''开始下载。
    ''' </summary>
    Public Sub Start()
        ' 检查文件是否存在
        CheckFileOrCreateFile()

        ' 只有状态是Checked 时，才会开始下载 。
        If Me.Status <> MultiThreadedWebDownloaderStatus.Checked Then
            Throw New ApplicationException("只有状态是Checked 时，才会开始下载 。")
        End If

        ' 如果文件不支持"Accept-Ranges",然后创建一个 HttpDownloadClient
        ' 使用单线程下载文件。 否则就创建多个HttpDownloadClients下载文件。
        If Not IsRangeSupported Then
            Dim client As New HttpDownloadClient(Me.Url,
                                                 Me.DownloadPath,
                                                 0,
                                                 Long.MaxValue,
                                                 Me.BufferSize,
                                                 Me.BufferCountPerNotification)
            client.TotalSize = Me.TotalSize
            Me._downloadClients.Add(client)
        Else
            ' 计算每个client要下载的大小。
            Dim maxSizePerThread As Integer =
                CInt(Fix(Math.Ceiling(CDbl(Me.TotalSize) / Me.MaxThreadCount)))
            If maxSizePerThread < Me.MaxCacheSize Then
                maxSizePerThread = Me.MaxCacheSize
            End If

            Dim leftSizeToDownload As Long = Me.TotalSize

            ' 线程的个数为：  MaxThreadCount 的最小值+ TotalSize / MaxCacheSize.           
            Dim threadsCount As Integer =
                CInt(Fix(Math.Ceiling(CDbl(Me.TotalSize) / maxSizePerThread)))

            For i As Integer = 0 To threadsCount - 1
                Dim endPoint As Long = maxSizePerThread * (i + 1) - 1
                Dim sizeToDownload As Long = maxSizePerThread

                If endPoint > Me.TotalSize Then
                    endPoint = Me.TotalSize - 1
                    sizeToDownload = endPoint - maxSizePerThread * i
                End If

                ' 下载整个文件的一部分。
                Dim client As New HttpDownloadClient(Me.Url,
                                                     Me.DownloadPath,
                                                     maxSizePerThread * i,
                                                     endPoint)

                client.TotalSize = sizeToDownload
                Me._downloadClients.Add(client)
            Next i
        End If

        ' 设置 lastStartTime ，用于计算用时多少。
        _lastStartTime = Date.Now

        '设置状态为：downloading 
        Me.Status = MultiThreadedWebDownloaderStatus.Downloading

        '开始所有的HttpDownloadClients.
        For Each client In Me._downloadClients
            If Me.Proxy IsNot Nothing Then
                client.Proxy = Me.Proxy
            End If

            ' 加载 HttpDownloadClients事件。
            AddHandler client.DownloadProgressChanged,
                AddressOf client_DownloadProgressChanged

            AddHandler client.StatusChanged, AddressOf client_StatusChanged

            AddHandler client.ErrorOccurred, AddressOf client_ErrorOccurred
            client.Start()
        Next client


    End Sub

    ''' <summary>
    ''' 暂停下载
    ''' </summary>
    Public Sub Pause()
        '只有在 downloading状态下才能暂停。
        If Me.Status <> MultiThreadedWebDownloaderStatus.Downloading Then
            Throw New ApplicationException(
                "只有在 downloading状态下才能暂停。")
        End If

        Me.Status = MultiThreadedWebDownloaderStatus.Pausing

        ' 暂停所有的 HttpDownloadClients. 只有所有client被暂停 ，
        '下载状态才会改成Paused.
        For Each client In Me._downloadClients
            If client.Status <> HttpDownloadClientStatus.Completed Then
                client.Pause()
            End If
        Next client
    End Sub

    ''' <summary>
    ''' 继续下载.
    ''' </summary>
    Public Sub [Resume]()
        ' Only paused downloader can be paused.
        If Me.Status <> MultiThreadedWebDownloaderStatus.Paused Then
            Throw New ApplicationException(
                "只有paused状态才能暂停  ")
        End If

        ' 设置lastStartTime 
        _lastStartTime = Date.Now

        ' 设置状态为： downloading .
        Me.Status = MultiThreadedWebDownloaderStatus.Downloading

        ' 继续所有的HttpDownloadClients.
        For Each client In Me._downloadClients
            If client.Status <> HttpDownloadClientStatus.Completed Then
                client.Resume()
            End If
        Next client

    End Sub

    ''' <summary>
    ''' 取消下载
    ''' </summary>
    Public Sub Cancel()
        ' 只有downloading状态才能取消。
        If Me.Status <> MultiThreadedWebDownloaderStatus.Downloading Then
            Throw New ApplicationException(
                "只有downloading状态才能取消。")
        End If

        Me.Status = MultiThreadedWebDownloaderStatus.Canceling

        Me.OnError(New ErrorEventArgs With
                   {
                       .Exception = New Exception("下载被取消.")
                   })

        ' 取消所有的 HttpDownloadClients.
        For Each client In Me._downloadClients
            If client.Status <> HttpDownloadClientStatus.Completed Then
                client.Cancel()
            End If
        Next client

    End Sub

    ''' <summary>
    '''  处理所有的 HttpDownloadClients的StatusChanged 事件。
    ''' </summary>
    Private Sub client_StatusChanged(ByVal sender As Object, ByVal e As EventArgs)

        ' 如果所有的client都完成了,状态才改成 completed.
        If Me._downloadClients.All(Function(client) client.Status =
                                       HttpDownloadClientStatus.Completed) Then
            Me.Status = MultiThreadedWebDownloaderStatus.Completed
        Else

            ' 已完成的client不用考虑。 
            Dim nonCompletedClients =
                Me._downloadClients.Where(
                    Function(client) client.Status <> HttpDownloadClientStatus.Completed)

            ' 如果所有的 nonCompletedClients是Paused,状态才能是Paused.
            If nonCompletedClients.All(
                Function(client) client.Status = HttpDownloadClientStatus.Paused) Then
                Me.Status = MultiThreadedWebDownloaderStatus.Paused
            End If

            ' 如果所有的nonCompletedClients是Canceled,状态才是Canceled.
            If nonCompletedClients.All(
                Function(client) client.Status = HttpDownloadClientStatus.Canceled) Then
                Me.Status = MultiThreadedWebDownloaderStatus.Canceled
            End If
        End If

    End Sub

    ''' <summary>
    ''' 处理所有 HttpDownloadClients的  DownloadProgressChanged事件，并计算速度
    ''' </summary>
    Private Sub client_DownloadProgressChanged(ByVal sender As Object,
                                               ByVal e As HttpDownloadClientProgressChangedEventArgs)
        SyncLock _locker
            DownloadedSize += e.Size
            _bufferCount += 1

            If _bufferCount = BufferCountPerNotification Then

                Dim speed As Integer = 0
                Dim current As Date = Date.Now
                Dim interval As TimeSpan = current.Subtract(_lastNotificationTime)

                If interval.TotalSeconds < 60 Then
                    speed = CInt(Fix(Math.Floor((Me.DownloadedSize - Me._lastNotificationDownloadedSize) / interval.TotalSeconds)))
                End If

                _lastNotificationTime = current
                _lastNotificationDownloadedSize = Me.DownloadedSize

                Dim downloadProgressChangedEventArgs =
                    New MultiThreadedWebDownloaderProgressChangedEventArgs(
                    DownloadedSize, TotalSize, speed)
                Me.OnDownloadProgressChanged(downloadProgressChangedEventArgs)



                ' 重新设置bufferCount.
                _bufferCount = 0
            End If

        End SyncLock
    End Sub

    ''' <summary>
    ''' 处理所有HttpDownloadClients的  ErrorOccurred 事件.
    ''' </summary>
    Private Sub client_ErrorOccurred(ByVal sender As Object, ByVal e As ErrorEventArgs)
        If Me.Status <> MultiThreadedWebDownloaderStatus.Canceling _
            AndAlso Me.Status <> MultiThreadedWebDownloaderStatus.Canceled Then
            Me.Cancel()

            '  引发 ErrorOccurred 事件
            Me.OnError(e)

        End If

    End Sub

    ''' <summary>
    ''' 引发 DownloadProgressChanged 事件.如果状态是Completed,则引发 DownloadCompleted事件。
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overridable Sub OnDownloadProgressChanged(ByVal e As MultiThreadedWebDownloaderProgressChangedEventArgs)
        RaiseEvent DownloadProgressChanged(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 StatusChanged 事件
    ''' </summary>
    Protected Overridable Sub OnStatusChanged(ByVal e As EventArgs)
        If Me.Status = MultiThreadedWebDownloaderStatus.Paused _
            OrElse Me.Status = MultiThreadedWebDownloaderStatus.Canceled _
            OrElse Me.Status = MultiThreadedWebDownloaderStatus.Completed Then
            ' 当暂停时，更新时间 .
            _usedTime = _usedTime.Add(Date.Now.Subtract(_lastStartTime))
        End If

        RaiseEvent StatusChanged(Me, e)

        If Me.Status = MultiThreadedWebDownloaderStatus.Completed Then
            Dim downloadCompletedEventArgs As _
                New MultiThreadedWebDownloaderCompletedEventArgs(
                    Me.DownloadedSize, Me.TotalSize, Me.TotalUsedTime)
            Me.OnDownloadCompleted(downloadCompletedEventArgs)
        End If

    End Sub

    ''' <summary>
    '''引发   DownloadCompleted事件。
    ''' </summary>
    Protected Overridable Sub OnDownloadCompleted(ByVal e As MultiThreadedWebDownloaderCompletedEventArgs)
        RaiseEvent DownloadCompleted(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 ErrorOccurred事件。
    ''' </summary>
    Protected Overridable Sub OnError(ByVal e As ErrorEventArgs)
        RaiseEvent ErrorOccurred(Me, e)
    End Sub
End Class

