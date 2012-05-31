'*************************** Module Header ******************************'
' 模块名:  FTPDownloadClient.vb
' 项目名:	    VBFTPDownload
' 版权(c)  Microsoft Corporation.
' 
' 这个类被经常用来从FTP下载文件。当下载启动时，他将下载的文件加入到后台的线程
' 中，下载的数据储存在第一个MemoryStream中，然后再写入本地文件。.
' 
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

Partial Public Class FTPClientManager

    Public Class FTPDownloadClient
        ' 2M bytes.
        Public Const MaxCacheSize As Integer = 2097152

        ' 2K bytes.
        Public Const BufferSize As Integer = 2048

        Private _manager As FTPClientManager

        Public Event FileDownloadCompleted As EventHandler(Of FileDownloadCompletedEventArgs)

        Public Event AllFilesDownloadCompleted As EventHandler

        Public Sub New(ByVal manager As FTPClientManager)
            If manager Is Nothing Then
                Throw New ArgumentNullException("FTPClientManager cannot be null.")
            End If

            Me._manager = manager
        End Sub

        ''' <summary>
        '''下载文件，目录和子目录.
        ''' </summary>
        Public Sub DownloadDirectoriesAndFiles(ByVal files As IEnumerable(Of FTPFileSystem),
                                               ByVal localPath As String)
            If files Is Nothing Then
                Throw New ArgumentNullException("The files to download cannot be null.")
            End If

            ' 创建一个下载数据的线程.
            Dim threadStart As New ParameterizedThreadStart(
                AddressOf StartDownloadDirectoriesAndFiles)
            Dim downloadThread As New Thread(threadStart)
            downloadThread.IsBackground = True
            downloadThread.Start(New Object() {files, localPath})
        End Sub

        ''' <summary>
        '''下载文件，目录和它的子目录.
        ''' </summary>
        Private Sub StartDownloadDirectoriesAndFiles(ByVal state As Object)
            Dim paras = TryCast(state, Object())

            Dim files As IEnumerable(Of FTPFileSystem) = TryCast(paras(0), 
                IEnumerable(Of FTPFileSystem))
            Dim localPath As String = TryCast(paras(1), String)

            For Each file In files
                DownloadDirectoryOrFile(file, localPath)
            Next file

            Me.OnAllFilesDownloadCompleted(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' 下载一个单一的文件和目录.
        ''' </summary>
        Private Sub DownloadDirectoryOrFile(ByVal fileSystem As FTPFileSystem,
                                            ByVal localPath As String)

            '下载文件目录.
            If Not fileSystem.IsDirectory Then
                DownloadFile(fileSystem, localPath)

                ' 下载一个目录.
            Else

                ' 目录路径的结构.
                Dim directoryPath As String = localPath & "\" & fileSystem.Name

                If Not Directory.Exists(directoryPath) Then
                    Directory.CreateDirectory(directoryPath)
                End If

                ' 获得子目录和文件.
                Dim subDirectoriesAndFiles =
                    Me._manager.GetSubDirectoriesAndFiles(fileSystem.Url)

                '下载文件夹的文件和子目录.
                For Each subFile In subDirectoriesAndFiles
                    DownloadDirectoryOrFile(subFile, directoryPath)
                Next subFile
            End If
        End Sub

        ''' <summary>
        ''' 下载一个单一的文件目录.
        ''' </summary>
        Private Sub DownloadFile(ByVal file As FTPFileSystem, ByVal localPath As String)
            If file.IsDirectory Then
                Throw New ArgumentException(
                    "The FTPFileSystem to download is a directory in fact")
            End If

            Dim destPath As String = localPath & "\" & file.Name

            ' 创建一个要下载的文件的请求.
            Dim request As FtpWebRequest =
                TryCast(WebRequest.Create(file.Url), FtpWebRequest)

            request.Credentials = Me._manager.Credentials

            '下载文件.
            request.Method = WebRequestMethods.Ftp.DownloadFile

            Dim response As FtpWebResponse = Nothing
            Dim responseStream As Stream = Nothing
            Dim downloadCache As MemoryStream = Nothing


            Try

                ' 检索来自服务器的响应，并得到响应流.
                response = TryCast(request.GetResponse(), FtpWebResponse)

                Me._manager.OnNewMessageArrived(New NewMessageEventArg _
                                                With {.NewMessage = response.StatusDescription})

                responseStream = response.GetResponseStream()

                ' 内存中的缓存数据.
                downloadCache = New MemoryStream(FTPDownloadClient.MaxCacheSize)
                Dim downloadBuffer(FTPDownloadClient.BufferSize - 1) As Byte

                Dim bytesSize As Integer = 0
                Dim cachedSize As Integer = 0

                ' 下载这个文件直到下载完成.
                Do

                    '从流中读取数据的缓冲区.
                    bytesSize =
                        responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)

                    '如果这个缓存区是空的，或者下载完成，把在缓存中的数据写到本地文件中.
                    If bytesSize = 0 OrElse MaxCacheSize < cachedSize + bytesSize Then
                        Try
                            ' 把在缓存中的数据写到本地文件中.
                            WriteCacheToFile(downloadCache, destPath, cachedSize)

                            '如果被暂停下载将停止正在下载的文件，取消或者完成. 
                            If bytesSize = 0 Then
                                Exit Do
                            End If

                            ' 重置缓存.
                            downloadCache.Seek(0, SeekOrigin.Begin)
                            cachedSize = 0
                        Catch ex As Exception
                            Dim msg As String =
                                String.Format("There is an error while downloading {0}. " _
                                              & " See InnerException for detailed error. ",
                                              file.Url)
                            Dim errorException As New ApplicationException(msg, ex)

                            ' 这个错误给了DownloadCompleted事件.
                            Dim e As ErrorEventArgs = New ErrorEventArgs _
                                                      With {.ErrorException = errorException}

                            Me._manager.OnErrorOccurred(e)

                            Return
                        End Try

                    End If

                    ' 把缓冲区的数据写到内存中的缓存.
                    downloadCache.Write(downloadBuffer, 0, bytesSize)
                    cachedSize += bytesSize
                Loop

                Dim fileDownloadCompletedEventArgs_Renamed =
                    New FileDownloadCompletedEventArgs _
                    With {.LocalFile = New FileInfo(destPath), .ServerPath = file.Url}

                Me.OnFileDownloadCompleted(fileDownloadCompletedEventArgs_Renamed)
            Finally
                If response IsNot Nothing Then
                    response.Close()
                End If

                If responseStream IsNot Nothing Then
                    responseStream.Close()
                End If

                If downloadCache IsNot Nothing Then
                    downloadCache.Close()
                End If
            End Try
        End Sub

        ''' <summary>
        '''写缓存中的数据到本地文件.
        ''' </summary>
        Private Sub WriteCacheToFile(ByVal downloadCache As MemoryStream,
                                     ByVal downloadPath As String,
                                     ByVal cachedSize As Integer)
            Using fileStream_Renamed As New FileStream(downloadPath, FileMode.Append)
                Dim cacheContent(cachedSize - 1) As Byte
                downloadCache.Seek(0, SeekOrigin.Begin)
                downloadCache.Read(cacheContent, 0, cachedSize)
                fileStream_Renamed.Write(cacheContent, 0, cachedSize)
            End Using
        End Sub

        Protected Overridable Sub OnFileDownloadCompleted(ByVal e As FileDownloadCompletedEventArgs)

            RaiseEvent FileDownloadCompleted(Me, e)
        End Sub

        Protected Overridable Sub OnAllFilesDownloadCompleted(ByVal e As EventArgs)
            RaiseEvent AllFilesDownloadCompleted(Me, e)
        End Sub
    End Class
End Class