'******************************** 模块头 **************************************'
' 模块名:	    HttpDownloadClient.vb
' 项目名:    VBWebDownloadProgress
' 版权(c)   Microsoft Corporation.
' 
' 这个类被用来从网上下载文件. 它提供的公有方法来实现开始、暂停、重新开始和取消一次下载。
' 
' 下载开始时，会检查目标文件是否存在，如果不存在，就创建一个和将要下载的文件同样大小的本地文件，
' 然后开始在后台线程下载。 
' 
' 已下载的数据先储存在一个MemoryStream中，然后写入本地文件中。 
' 
' 当读指定数目的数据时，就会引发DownloadProgressChanged事件。
' 当下载任务完成或被取消时，也会引发DownloadCompleted事件。
' 
' DownloadedSize属性存储着已下载的数据的大小，这里的数据用来重新开始下载。
' 
' StartPoint属性可用于多线程并发的情况，并且每个线程分别下载整个文件的一个特定的数据块。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.IO
Imports System.Net
Imports System.Threading

Public Class HttpDownloadClient
    ' 在创建或写一个文件的时候用到.
    Private Shared _locker As New Object()

    ' 存储下载数据已用的时间. 
    ' 不包括暂停的时间并且只在下载任务被暂停、取消或完成的时候被更新.
    Private _usedTime As New TimeSpan()

    Private _lastStartTime As Date

    ''' <summary>
    ''' 如果状态时正在下载，那么总时间就是下载用的时间. 否则的话总时间应该包含在 
    ''' 当前下载线程所用的时间中.
    ''' </summary>
    Public ReadOnly Property TotalUsedTime() As TimeSpan
        Get
            If Me.Status <> HttpDownloadClientStatus.Downloading Then
                Return _usedTime
            Else
                Return _usedTime.Add(Date.Now.Subtract(_lastStartTime))
            End If
        End Get
    End Property

    ' 最后的DownloadProgressChanged事件中的时间和大小.
    ' 这两个字段是用来计算下载速度的.
    Private _lastNotificationTime As Date
    Private _lastNotificationDownloadedSize As Int64

    ' 当读指定数目的数据时，就会引发DownloadProgressChanged事件.
    Private _bufferCountPerNotification As Integer
    Public Property BufferCountPerNotification() As Integer
        Get
            Return _bufferCountPerNotification
        End Get
        Private Set(ByVal value As Integer)
            _bufferCountPerNotification = value
        End Set
    End Property

    ' 下载文件的地址
    Private _url As Uri
    Public Property Url() As Uri
        Get
            Return _url
        End Get
        Private Set(ByVal value As Uri)
            _url = value
        End Set
    End Property

    ' 存储文件的本地路径.
    ' 如果没有相同名字的文件，就创建一个新的.
    Private _downloadPath As String
    Public Property DownloadPath() As String
        Get
            Return _downloadPath
        End Get
        Private Set(ByVal value As String)
            _downloadPath = value
        End Set
    End Property

    ' StartPoint属性和EndPoint属性可用于多线程并发的情况，
    ' 并且每个线程分别下载整个文件的一个特定的数据块。
    Private _startPoint As Integer
    Public Property StartPoint() As Integer
        Get
            Return _startPoint
        End Get
        Private Set(ByVal value As Integer)
            _startPoint = value
        End Set
    End Property

    Private _endPoint As Integer
    Public Property EndPoint() As Integer
        Get
            Return _endPoint
        End Get
        Private Set(ByVal value As Integer)
            _endPoint = value
        End Set
    End Property

    ' 当读取响应流的数据时设定缓冲区的大小.
    Private _bufferSize As Integer
    Public Property BufferSize() As Integer
        Get
            Return _bufferSize
        End Get
        Private Set(ByVal value As Integer)
            _bufferSize = value
        End Set
    End Property

    ' 内存中缓存的大小.
    Private _maxCacheSize As Integer
    Public Property MaxCacheSize() As Integer
        Get
            Return _maxCacheSize
        End Get
        Private Set(ByVal value As Integer)
            _maxCacheSize = value
        End Set
    End Property

    Private _totalSize As Int64 = 0

    ' 向服务器请求文件的大小并保存它.
    Public ReadOnly Property TotalSize() As Int64
        Get
            If _totalSize = 0 Then
                Dim request = CType(WebRequest.Create(Url), HttpWebRequest)
                If EndPoint <> Integer.MaxValue Then
                    request.AddRange(StartPoint, EndPoint)
                Else
                    request.AddRange(StartPoint)
                End If

                ' 处理Web响应.
                Using response = request.GetResponse()
                    _totalSize = response.ContentLength
                End Using

            End If
            Return _totalSize
        End Get
    End Property

    ' 已下载的数据的大小已写入本地文件.
    Private _downloadedSize As Int64
    Public Property DownloadedSize() As Int64
        Get
            Return _downloadedSize
        End Get
        Private Set(ByVal value As Int64)
            _downloadedSize = value
        End Set
    End Property

    Private _status As HttpDownloadClientStatus

    ' 如果状态改变，触发StatusChanged事件.
    Public Property Status() As HttpDownloadClientStatus
        Get
            Return _status
        End Get

        Private Set(ByVal value As HttpDownloadClientStatus)
            If _status <> value Then
                _status = value
                Me.OnStatusChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    Public Event DownloadProgressChanged As EventHandler(Of HttpDownloadProgressChangedEventArgs)

    Public Event DownloadCompleted As EventHandler(Of HttpDownloadCompletedEventArgs)

    Public Event StatusChanged As EventHandler

    ''' <summary>
    ''' 下载整个文件.
    ''' </summary>
    Public Sub New(ByVal url As String, ByVal downloadPath As String)
        Me.New(url, downloadPath, 0)
    End Sub

    ''' <summary>
    ''' 从一个开始点下载文件到最后.
    ''' </summary>
    Public Sub New(ByVal url As String, ByVal downloadPath As String,
                   ByVal startPoint As Integer)
        Me.New(url, downloadPath, startPoint, Integer.MaxValue)
    End Sub

    ''' <summary>
    ''' 下载文件的一个数据块. 默认的缓冲区大小是1KB，内存缓存是2MB，
    ''' BufferCountPerNotification是512个
    ''' </summary>
    Public Sub New(ByVal url As String, ByVal downloadPath As String,
                   ByVal startPoint As Integer, ByVal endPoint As Integer)
        Me.New(url, downloadPath, startPoint, endPoint, 1024, 2097152, 512)
    End Sub

    Public Sub New(ByVal url As String, ByVal downloadPath As String,
                   ByVal startPoint As Integer, ByVal endPoint As Integer,
                   ByVal bufferSize As Integer, ByVal cacheSize As Integer,
                   ByVal bufferCountPerNotification As Integer)
        If startPoint < 0 Then
            Throw New ArgumentOutOfRangeException("StartPoint不能小于0. ")
        End If

        If endPoint < startPoint Then
            Throw New ArgumentOutOfRangeException(
                "EndPoint不能小于StartPoint. ")
        End If

        If bufferSize < 0 Then
            Throw New ArgumentOutOfRangeException("BufferSize不能小于0. ")
        End If

        If cacheSize < bufferSize Then
            Throw New ArgumentOutOfRangeException(
                "MaxCacheSize不能小于BufferSize. ")
        End If

        If bufferCountPerNotification <= 0 Then
            Throw New ArgumentOutOfRangeException(
                "BufferCountPerNotification不能小于0.")
        End If

        Me.StartPoint = startPoint
        Me.EndPoint = endPoint
        Me.BufferSize = bufferSize
        Me.MaxCacheSize = cacheSize
        Me._bufferCountPerNotification = bufferCountPerNotification

        Me.Url = New Uri(url, UriKind.Absolute)
        Me.DownloadPath = downloadPath

        ' 设定空闲的状态.
        Me._status = HttpDownloadClientStatus.Idle

    End Sub

    ''' <summary>
    ''' 开始下载.
    ''' </summary>
    Public Sub Start()
        ' 检查目标文件是否存在.
        CheckFileOrCreateFile()

        ' 只有空闲的下载客户端才能开始.
        If Me.Status <> HttpDownloadClientStatus.Idle Then
            Throw New ApplicationException("Only idle download client can be started.")
        End If

        ' 开始在后台线程下载.
        BeginDownload()
    End Sub

    ''' <summary>
    ''' 暂停下载.
    ''' </summary>
    Public Sub Pause()
        ' 只有正在下载的客户端才能暂停.
        If Me.Status <> HttpDownloadClientStatus.Downloading Then
            Throw New ApplicationException("只有正在下载的客户端才能暂停.")
        End If

        ' 后台线程会查看状态，如果状态时暂停的，
        ' 下载将会被暂停并且状态将随之改为暂停.
        Me.Status = HttpDownloadClientStatus.Pausing
    End Sub

    ''' <summary>
    ''' 重新开始下载.
    ''' </summary>
    Public Sub [Resume]()
        ' 只有暂停的客户端才能重新下载.
        If Me.Status <> HttpDownloadClientStatus.Paused Then
            Throw New ApplicationException("Only paused client can be resumed.")
        End If

        ' 开始在后台线程进行下载.
        BeginDownload()
    End Sub

    ''' <summary>
    ''' 取消下载
    ''' </summary>
    Public Sub Cancel()
        ' 只有正在下载的或者是暂停的客户端才能被取消.
        If Me.Status <> HttpDownloadClientStatus.Paused _
            AndAlso Me.Status <> HttpDownloadClientStatus.Downloading Then
            Throw New ApplicationException("只有正在下载的或者是暂停的客户端" _
                                           & " 才能被取消.")
        End If

        ' 后台线程将查看状态.如果是正在取消，
        ' 那么下载将被取消并且状态将改成已取消.
        Me.Status = HttpDownloadClientStatus.Canceling
    End Sub

    ''' <summary>
    ''' 创建一个线程下载数据.
    ''' </summary>
    Private Sub BeginDownload()
        Dim threadStart_Renamed As New ThreadStart(AddressOf Download)
        Dim downloadThread As New Thread(threadStart_Renamed)
        downloadThread.IsBackground = True
        downloadThread.Start()
    End Sub

    ''' <summary>
    ''' 通过HttpWebRequest线程来下载数据.它会从响应流中读取一个字节的缓冲区，
    ''' 并把它先储存在MemoryStream的缓存中。
    ''' 如果缓存没有空间或者下载暂停、取消或完成了，就将缓存中的数据写入本地文件中。
    ''' </summary>
    Private Sub Download()
        Dim request As HttpWebRequest = Nothing
        Dim response As HttpWebResponse = Nothing
        Dim responseStream As Stream = Nothing
        Dim downloadCache As MemoryStream = Nothing
        _lastStartTime = Date.Now

        ' 设定状态.
        Me.Status = HttpDownloadClientStatus.Downloading

        Try

            ' 为需要下载的文件创建一个请求.
            request = CType(WebRequest.Create(Url), HttpWebRequest)
            request.Method = "GET"
            request.Credentials = CredentialCache.DefaultCredentials


            ' 指定块下载.
            If EndPoint <> Integer.MaxValue Then
                request.AddRange(StartPoint + DownloadedSize, EndPoint)
            Else
                request.AddRange(StartPoint + DownloadedSize)
            End If

            ' 接受服务器端的响应并得到响应流.
            response = CType(request.GetResponse(), HttpWebResponse)

            responseStream = response.GetResponseStream()


            ' 内存中的缓存数据.
            downloadCache = New MemoryStream(Me.MaxCacheSize)

            Dim downloadBuffer(Me.BufferSize - 1) As Byte

            Dim bytesSize As Integer = 0
            Dim cachedSize As Integer = 0
            Dim receivedBufferCount As Integer = 0

            ' 下载文件直到下载被暂停、取消或完成.
            Do

                ' 从流中读取缓冲区的数据.
                bytesSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)

                ' 如果缓存是满的，或者下载被暂停、取消或完成，
                ' 就把缓存中的数据写入本地文件.
                If Me.Status <> HttpDownloadClientStatus.Downloading OrElse bytesSize = 0 _
                    OrElse Me.MaxCacheSize < cachedSize + bytesSize Then

                    Try
                        ' 把缓存中的数据写入本地文件.
                        WriteCacheToFile(downloadCache, cachedSize)

                        Me.DownloadedSize += cachedSize

                        ' 如果下载被暂停、取消或完成了，
                        ' 就停止下载文件.
                        If Me.Status <> HttpDownloadClientStatus.Downloading _
                            OrElse bytesSize = 0 Then
                            Exit Do
                        End If

                        ' 读取缓存. 
                        downloadCache.Seek(0, SeekOrigin.Begin)
                        cachedSize = 0
                    Catch ex As Exception
                        ' 如果错误，触发DownloadCompleted事件.
                        Me.OnDownloadCompleted(
                            New HttpDownloadCompletedEventArgs(Me.DownloadedSize,
                                                               Me.TotalSize,
                                                               Me.TotalUsedTime,
                                                               ex))
                        Return
                    End Try

                End If


                ' 将数据从缓冲区写入内存的缓存中.
                downloadCache.Write(downloadBuffer, 0, bytesSize)

                cachedSize += bytesSize

                receivedBufferCount += 1

                ' 触发DownloadProgressChanged事件.
                If receivedBufferCount = Me._bufferCountPerNotification Then
                    InternalDownloadProgressChanged(cachedSize)
                    receivedBufferCount = 0
                End If
            Loop


            ' 如果当前下载被停止了，更新所用的时间.
            _usedTime = _usedTime.Add(Date.Now.Subtract(_lastStartTime))

            ' 更新客户端的状态. 在客户端的状态为暂停、取消或完成时，
            ' 以上的循环将被终止.
            If Me.Status = HttpDownloadClientStatus.Pausing Then
                Me.Status = HttpDownloadClientStatus.Paused
            ElseIf Me.Status = HttpDownloadClientStatus.Canceling Then
                Me.Status = HttpDownloadClientStatus.Canceled

                Dim ex As New Exception("由于用户的需求下载已被取消.")

                Me.OnDownloadCompleted(
                    New HttpDownloadCompletedEventArgs(Me.DownloadedSize, Me.TotalSize,
                                                       Me.TotalUsedTime, ex))
            Else
                Me.Status = HttpDownloadClientStatus.Completed
                Me.OnDownloadCompleted(
                    New HttpDownloadCompletedEventArgs(Me.DownloadedSize, Me.TotalSize,
                                                       Me.TotalUsedTime, Nothing))
                Return
            End If

        Finally
            ' 当以上的代码执行完毕，关闭流.
            If responseStream IsNot Nothing Then
                responseStream.Close()
            End If
            If response IsNot Nothing Then
                response.Close()
            End If
            If downloadCache IsNot Nothing Then
                downloadCache.Close()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' 检查目标文件是否存在，如果不存在，
    ''' 创建一个与将要下载的文件一样大小的文件.
    ''' </summary>
    Private Sub CheckFileOrCreateFile()
        ' 锁定其他的线程或者进程来避免创建文件.
        SyncLock _locker
            Dim fileToDownload As New FileInfo(DownloadPath)
            If fileToDownload.Exists Then

                ' 目标文件应该与将要被下载的文件大小一样.
                If fileToDownload.Length <> Me.TotalSize Then
                    Throw New ApplicationException(
                        "下载路径已经存在一个文件不匹配" _
                        & " 要下载的文件.")
                End If

                ' 创建一个文件.
            Else
                If TotalSize = 0 Then
                    Throw New ApplicationException("要下载的文件不存在！")
                End If

                Using fileStream_Renamed As FileStream = File.Create(Me.DownloadPath)
                    Dim createdSize As Long = 0
                    Dim buffer(4095) As Byte
                    Do While createdSize < TotalSize
                        Dim bufferSize As Integer = If((TotalSize - createdSize) < 4096,
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
    ''' 将缓存中的数据写入本地文件.
    ''' </summary>
    Private Sub WriteCacheToFile(ByVal downloadCache As MemoryStream,
                                 ByVal cachedSize As Integer)
        ' 锁定其他线程或进程避免数据写入文件.
        SyncLock _locker
            Using fileStream_Renamed As New FileStream(DownloadPath, FileMode.Open)
                Dim cacheContent(cachedSize - 1) As Byte
                downloadCache.Seek(0, SeekOrigin.Begin)
                downloadCache.Read(cacheContent, 0, cachedSize)
                fileStream_Renamed.Seek(DownloadedSize + StartPoint, SeekOrigin.Begin)
                fileStream_Renamed.Write(cacheContent, 0, cachedSize)
            End Using
        End SyncLock
    End Sub


    Protected Overridable Sub OnDownloadCompleted(ByVal e As HttpDownloadCompletedEventArgs)
        RaiseEvent DownloadCompleted(Me, e)
    End Sub

    ''' <summary>
    ''' 计算下载速度并触发DownloadProgressChanged事件.
    ''' </summary>
    ''' <param name="cachedSize"></param>
    Private Sub InternalDownloadProgressChanged(ByVal cachedSize As Integer)
        Dim speed As Integer = 0
        Dim current As Date = Date.Now
        Dim interval As TimeSpan = current.Subtract(_lastNotificationTime)

        If interval.TotalSeconds < 60 Then
            speed =
                CInt(Fix(Math.Floor((Me.DownloadedSize + cachedSize -
                                     Me._lastNotificationDownloadedSize) / interval.TotalSeconds)))
        End If
        _lastNotificationTime = current
        _lastNotificationDownloadedSize = Me.DownloadedSize + cachedSize

        Me.OnDownloadProgressChanged(
            New HttpDownloadProgressChangedEventArgs(Me.DownloadedSize + cachedSize,
                                                    Me.TotalSize, speed))


    End Sub

    Protected Overridable Sub OnDownloadProgressChanged(ByVal e As HttpDownloadProgressChangedEventArgs)
        RaiseEvent DownloadProgressChanged(Me, e)
    End Sub

    Protected Overridable Sub OnStatusChanged(ByVal e As EventArgs)
        RaiseEvent StatusChanged(Me, e)
    End Sub
End Class

