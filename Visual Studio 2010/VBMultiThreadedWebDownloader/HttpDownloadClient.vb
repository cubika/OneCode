'****************************** 模块头 ******************************'
' 模块名称:  HttpDownloadClient.vb
' 项目名称:	    VBMultiThreadedWebDownloader
' 版权 (c) Microsoft Corporation.
' 
' 这个类是用来通过网络下载文件的。 它提供了一些公有的下载方法：Start, Pause, Resume and Cancel
'  一个客户端将使用一个线程来下载整个文件的一部分。
' 
' 下载的数据最先是存储为 MemoryStream,然后在转变成本地文件。
' 
' 当开始下载指定大小的数据文件时，触发DownloadProgressChanged 事件，
' 当完成或者取消下载的文件时，触发DownloadCompleted事件。
'
' 
' DownloadedSize 属性存储下载数据的大小（继续下载的时候使用）。
' 
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
Imports System.Net
Imports System.Threading

Public Class HttpDownloadClient
    ' 当创建或者写文件的时候使用。
    Private Shared _locker As New Object()

    ' 要下载文件的Url。
    Private _url As Uri
    Public Property Url() As Uri
        Get
            Return _url
        End Get
        Private Set(ByVal value As Uri)
            _url = value
        End Set
    End Property

    ' 存储文件的路径。
    ' 如果没有相同名字的文件，将创建一个新的文件。
    Private _downloadPath As String
    Public Property DownloadPath() As String
        Get
            Return _downloadPath
        End Get
        Private Set(ByVal value As String)
            _downloadPath = value
        End Set
    End Property

    ' StartPoint and EndPoint 属性用在多线程现在的情况下，每一个线程开始下载一个特定的部分。
    Private _startPoint As Long
    Public Property StartPoint() As Long
        Get
            Return _startPoint
        End Get
        Private Set(ByVal value As Long)
            _startPoint = value
        End Set
    End Property

    Private _endPoint As Long
    Public Property EndPoint() As Long
        Get
            Return _endPoint
        End Get
        Private Set(ByVal value As Long)
            _endPoint = value
        End Set
    End Property

    Public Property Proxy() As WebProxy

    '在响应流中当读取数据时，设置 BufferSize 的值。
    Private _bufferSize As Integer
    Public Property BufferSize() As Integer
        Get
            Return _bufferSize
        End Get
        Private Set(ByVal value As Integer)
            _bufferSize = value
        End Set
    End Property

    '  内存中缓冲区的大小。
    Private _maxCacheSize As Integer
    Public Property MaxCacheSize() As Integer
        Get
            Return _maxCacheSize
        End Get
        Private Set(ByVal value As Integer)
            _maxCacheSize = value
        End Set
    End Property



    ' 向服务器请求要下载文件的大小并存储。
    Public Property TotalSize() As Long


    ' 下载数据变成本地文件时的大小。
    Private _downloadedSize As Long
    Public Property DownloadedSize() As Long
        Get
            Return _downloadedSize
        End Get
        Private Set(ByVal value As Long)
            _downloadedSize = value
        End Set
    End Property

    Private _status As HttpDownloadClientStatus

    ' 如果状态改变，触发StatusChanged事件。
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

    Public Event DownloadProgressChanged _
        As EventHandler(Of HttpDownloadClientProgressChangedEventArgs)

    Public Event ErrorOccurred As EventHandler(Of ErrorEventArgs)

    Public Event StatusChanged As EventHandler

    ''' <summary>
    ''' 下载整个文件。
    ''' </summary>
    Public Sub New(ByVal url As Uri, ByVal downloadPath As String)
        Me.New(url, downloadPath, 0)
    End Sub

    ''' <summary>
    ''' 从一个指定开始点开始下载，到结束点。
    ''' </summary>
    Public Sub New(ByVal url As Uri, ByVal downloadPath As String,
                   ByVal startPoint As Long)
        Me.New(url, downloadPath, startPoint, Long.MaxValue)
    End Sub

    ''' <summary>
    ''' 下载文件的某一块.默认的缓冲区大小是1KB，内存缓存是1MB，每个通知缓冲区计数为64。
    ''' </summary>
    Public Sub New(ByVal url As Uri, ByVal downloadPath As String,
                   ByVal startPoint As Long, ByVal endPoint As Long)
        Me.New(url, downloadPath, startPoint, endPoint, 1024, 1048576)
    End Sub

    Public Sub New(ByVal url As Uri, ByVal downloadPath As String,
                   ByVal startPoint As Long, ByVal endPoint As Long,
                   ByVal bufferSize As Integer, ByVal cacheSize As Integer)
        If startPoint < 0 Then
            Throw New ArgumentOutOfRangeException(
                "StartPoint 不能小于 0. ")
        End If

        If endPoint < startPoint Then
            Throw New ArgumentOutOfRangeException(
                "EndPoint 不能小于 StartPoint ")
        End If

        If bufferSize < 0 Then
            Throw New ArgumentOutOfRangeException(
                "BufferSize不能小于 0. ")
        End If

        If cacheSize < bufferSize Then
            Throw New ArgumentOutOfRangeException(
                "MaxCacheSize 不能小于BufferSize ")
        End If


        Me.StartPoint = startPoint
        Me.EndPoint = endPoint
        Me.BufferSize = bufferSize
        Me.MaxCacheSize = cacheSize

        Me.Url = url
        Me.DownloadPath = downloadPath

        ' 设置闲置状态.
        Me._status = HttpDownloadClientStatus.Idle

    End Sub

    ''' <summary>
    ''' 开始下载.
    ''' </summary>
    Public Sub Start()

        '只有闲置的情况下才能开始。
        If Me.Status <> HttpDownloadClientStatus.Idle Then
            Throw New ApplicationException("只有闲置的情况下才能开始。")
        End If

        '  在主线程下开始下载。
        BeginDownload()
    End Sub

    ''' <summary>
    ''' 暂停下载.
    ''' </summary>
    Public Sub Pause()
        ' 只有正在下载的情况下才能暂停。
        If Me.Status <> HttpDownloadClientStatus.Downloading Then
            Throw New ApplicationException(
                "Only downloading client can be paused.")
        End If

        ' 主线程会检查状态。如果暂停了，下载将会被暂停，并且状态会变成暂停状态。
        Me.Status = HttpDownloadClientStatus.Pausing
    End Sub

    ''' <summary>
    '''继续下载。
    ''' </summary>
    Public Sub [Resume]()
        ' 只有暂停的情况下才能继续下载。
        If Me.Status <> HttpDownloadClientStatus.Paused Then
            Throw New ApplicationException("只有暂停的情况下才能继续下载。")
        End If

        ' 在主线程下开始下载。
        BeginDownload()
    End Sub

    ''' <summary>
    ''' 取消下载。
    ''' </summary>
    Public Sub Cancel()
        ' 只有正在下载或者暂停的情况下才能被取消。
        If Me.Status <> HttpDownloadClientStatus.Paused _
            AndAlso Me.Status <> HttpDownloadClientStatus.Downloading Then
            Throw New ApplicationException("只有正在下载或者暂停的情况下才能被取消。")
        End If

        ' 主线程会检查状态，如果是取消，下载将会取消，状态同时改变成取消状态。
        Me.Status = HttpDownloadClientStatus.Canceling
    End Sub

    ''' <summary>
    ''' 创建新的下载线程。
    ''' </summary>
    Private Sub BeginDownload()
        Dim threadStart_Renamed As New ThreadStart(AddressOf Download)
        Dim downloadThread As New Thread(threadStart_Renamed)
        downloadThread.IsBackground = True
        downloadThread.Start()
    End Sub

    ''' <summary>
    ''' 用 HttpWebRequest下载数据. 它从响应流中读取缓冲区的数据。
    ''' 并且先把数据存储在内存缓存中。
    ''' 如果缓存满了，或者下载被暂停，取消或者完成，就将缓存中的数据写成本地文件。
    ''' </summary>
    Private Sub Download()
        Dim request As HttpWebRequest = Nothing
        Dim response As HttpWebResponse = Nothing
        Dim responseStream As Stream = Nothing
        Dim downloadCache As MemoryStream = Nothing

        ' 设置状态。
        Me.Status = HttpDownloadClientStatus.Downloading

        Try

            '创建一个下载的请求。
            request = CType(WebRequest.Create(Url), HttpWebRequest)
            request.Method = "GET"
            request.Credentials = CredentialCache.DefaultCredentials


            ' 指定下载的模块。
            If EndPoint <> Long.MaxValue Then
                request.AddRange(StartPoint + DownloadedSize, EndPoint)
            Else
                request.AddRange(StartPoint + DownloadedSize)
            End If

            ' 设置 proxy.
            If Proxy IsNot Nothing Then
                request.Proxy = Proxy
            End If

            '得到从服务器中的响应和响应流。
            response = CType(request.GetResponse(), HttpWebResponse)

            responseStream = response.GetResponseStream()


            ' 在内存中缓存数据。
            downloadCache = New MemoryStream(Me.MaxCacheSize)

            Dim downloadBuffer(Me.BufferSize - 1) As Byte

            Dim bytesSize As Integer = 0
            Dim cachedSize As Integer = 0

            '下载文件，知道被取消，暂停或者完成。
            Do

                ' 从响应流中读取缓冲数据。
                bytesSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)

                ' 如果缓存满了，或者下载被暂停，取消，或者完成了，将缓存数据写成本地文件。
                If Me.Status <> HttpDownloadClientStatus.Downloading _
                    OrElse bytesSize = 0 _
                    OrElse Me.MaxCacheSize < cachedSize + bytesSize Then
                    Try
                        ' 将缓存数据写成本地文件。
                        WriteCacheToFile(downloadCache, cachedSize)

                        Me.DownloadedSize += cachedSize

                        '停止下载文件，如果下载被暂停，取消，或者完成。
                        If Me.Status <> HttpDownloadClientStatus.Downloading _
                            OrElse bytesSize = 0 Then
                            Exit Do
                        End If

                        ' 重新设置缓存。
                        downloadCache.Seek(0, SeekOrigin.Begin)
                        cachedSize = 0
                    Catch ex As Exception
                        '触发 DownloadCompleted事件。
                        Me.OnError(New ErrorEventArgs With {.Exception = ex})
                        Return
                    End Try
                End If

                '从缓冲区写入数据到内存中的缓存。
                downloadCache.Write(downloadBuffer, 0, bytesSize)

                cachedSize += bytesSize

                ' 触发DownloadProgressChanged事件。
                OnDownloadProgressChanged(
                    New HttpDownloadClientProgressChangedEventArgs With
                    {
                        .Size = bytesSize
                    })
            Loop

            '更新状态。当状态是暂停，取消，完成时，上面的循环将会停止。
            If Me.Status = HttpDownloadClientStatus.Pausing Then
                Me.Status = HttpDownloadClientStatus.Paused
            ElseIf Me.Status = HttpDownloadClientStatus.Canceling Then
                Me.Status = HttpDownloadClientStatus.Canceled

                Dim ex As New Exception("下载被用户取消 ")

                Me.OnError(New ErrorEventArgs With {.Exception = ex})
            Else
                Me.Status = HttpDownloadClientStatus.Completed
                Return
            End If

        Finally
            '  当以上代码执行结束时，关闭流。
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
    '''  将缓存数据写到本地文件
    ''' </summary>
    Private Sub WriteCacheToFile(ByVal downloadCache As MemoryStream,
                                 ByVal cachedSize As Integer)
        ' 锁定其他线程或者进程，去阻止向本地文件写数据。
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

    ''' <summary>
    ''' 引发ErrorOccurred 事件。
    ''' </summary>
    Protected Overridable Sub OnError(ByVal e As ErrorEventArgs)
        RaiseEvent ErrorOccurred(Me, e)
    End Sub

    ''' <summary>
    '''引发 DownloadProgressChanged 事件。
    ''' </summary>
    Protected Overridable Sub OnDownloadProgressChanged(ByVal e As HttpDownloadClientProgressChangedEventArgs)
        RaiseEvent DownloadProgressChanged(Me, e)
    End Sub

    ''' <summary>
    ''' 引发 StatusChanged事件。
    ''' </summary>
    Protected Overridable Sub OnStatusChanged(ByVal e As EventArgs)
        RaiseEvent StatusChanged(Me, e)
    End Sub
End Class

