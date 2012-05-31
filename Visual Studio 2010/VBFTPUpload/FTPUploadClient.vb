'*************************** M模块头 ******************************'
' 模块名:  FTPUploadClient.vb
' 项目:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
' 这个类被使用上传一个文件到FTP服务器，当上传开始，将要在一个后台线程上上传文件
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

Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Net

Partial Public Class FTPClientManager

    Public Class FTPUploadClient
       
        ' 2K bytes.
        Public Const BufferSize As Integer = 2048

        Private _manager As FTPClientManager

        Public Event FileUploadCompleted As EventHandler(Of FileUploadCompletedEventArgs)

        Public Event AllFilesUploadCompleted As EventHandler

        Public Sub New(ByVal manager As FTPClientManager)
            If manager Is Nothing Then
                Throw New ArgumentNullException("FTPClientManager cannot be null.")
            End If

            Me._manager = manager
        End Sub

        ''' <summary>
        ''' 上传文件，目录和其子目录
        ''' </summary>
        Public Sub UploadDirectoriesAndFiles(ByVal fileSysInfos As IEnumerable(Of FileSystemInfo),
                                             ByVal serverPath As Uri)
            If fileSysInfos Is Nothing Then
                Throw New ArgumentNullException("The files to upload cannot be null.")
            End If

            ' 创建一个线程上传数据。
            Dim threadStart As New ParameterizedThreadStart(
                AddressOf StartUploadDirectoriesAndFiles)

            Dim uploadThread As New Thread(threadStart)
            uploadThread.IsBackground = True
            uploadThread.Start(New Object() {fileSysInfos, serverPath})

        End Sub

        ''' <summary>
        '''上传文件，目录和其子目录
        ''' </summary>
        Private Sub StartUploadDirectoriesAndFiles(ByVal state As Object)
            Dim paras = TryCast(state, Object())

            Dim fileSysInfos As IEnumerable(Of FileSystemInfo) = TryCast(paras(0), 
                IEnumerable(Of FileSystemInfo))

            Dim serverPath As Uri = TryCast(paras(1), Uri)

            For Each fileSys In fileSysInfos
                UploadDirectoryOrFile(fileSys, serverPath)
            Next fileSys

            Me.OnAllFilesUploadCompleted(EventArgs.Empty)
        End Sub

        ''' <summary>
        '''  上传一个单独的文件和目录
        ''' </summary>
        Private Sub UploadDirectoryOrFile(ByVal fileSystem As FileSystemInfo,
                                          ByVal serverPath As Uri)

            ' 直接上传文件
            If TypeOf fileSystem Is FileInfo Then
                UploadFile(TryCast(fileSystem, FileInfo), serverPath)

                '上传一个目录
            Else

                ' 构造子目录路径
                Dim subDirectoryPath As New Uri(serverPath, fileSystem.Name & "/")

                Me._manager.CreateDirectoryOnFTPServer(serverPath, fileSystem.Name & "/")

                ' 得到子目录和文件
                Dim subDirectoriesAndFiles =
                    (TryCast(fileSystem, DirectoryInfo)).GetFileSystemInfos()

                '上传文件夹中的文件和子目录.
                For Each subFile In subDirectoriesAndFiles
                    UploadDirectoryOrFile(subFile, subDirectoryPath)
                Next subFile
            End If
        End Sub

        ''' <summary>
        ''' 直接上传一个单独文件
        ''' </summary>
        Private Sub UploadFile(ByVal file As FileInfo, ByVal serverPath As Uri)
            If file Is Nothing Then
                Throw New ArgumentNullException(" The file to upload is null. ")
            End If

            Dim destPath As New Uri(serverPath, file.Name)

            ' 创建一个上传文件的请求
            Dim request As FtpWebRequest =
                TryCast(WebRequest.Create(destPath), FtpWebRequest)

            request.Credentials = Me._manager.Credentials

            ' 上传文件.
            request.Method = WebRequestMethods.Ftp.UploadFile

            Dim response As FtpWebResponse = Nothing
            Dim requestStream As Stream = Nothing
            Dim localFileStream As FileStream = Nothing

            Try

                ' 从服务器检索响应
                response = TryCast(request.GetResponse(), FtpWebResponse)

                ' 打开读取本地文件
                localFileStream = file.OpenRead()

                ' 检索要求流
                requestStream = request.GetRequestStream()

                Dim bytesSize As Integer = 0
                Dim uploadBuffer(FTPUploadClient.BufferSize - 1) As Byte

                Do

                    ' 读取本地文件的缓冲
                    bytesSize = localFileStream.Read(uploadBuffer, 0, uploadBuffer.Length)

                    If bytesSize = 0 Then
                        Exit Do
                    Else

                        '在请求流中写缓冲。
                        requestStream.Write(uploadBuffer, 0, bytesSize)

                    End If
                Loop

                Dim fileUploadCompletedEventArgs = New FileUploadCompletedEventArgs With {.LocalFile = file, .ServerPath = destPath}

                Me.OnFileUploadCompleted(fileUploadCompletedEventArgs)

            Catch webEx As System.Net.WebException
                Dim ftpResponse As FtpWebResponse = TryCast(webEx.Response, FtpWebResponse)

                Dim msg As String = String.Format(
                    "当上传{0}有个错误. " _
                    & " StatusCode: {1}  StatusDescription: {2} ",
                    file.FullName,
                    ftpResponse.StatusCode.ToString(),
                    ftpResponse.StatusDescription)
                Dim errorException As New ApplicationException(msg, webEx)

                ' Fire the ErrorOccurred event with the error.
                Dim e As ErrorEventArgs = New ErrorEventArgs _
                                          With {.ErrorException = errorException}

                Me._manager.OnErrorOccurred(e)
            Catch ex As Exception
                Dim msg As String = String.Format(
                    "当上传 {0}有个错误. " _
                    & " See InnerException for detailed error. ",
                    file.FullName)
                Dim errorException As New ApplicationException(msg, ex)

                ' 激发ErrorOccurred事件
                Dim e As ErrorEventArgs = New ErrorEventArgs _
                                          With {.ErrorException = errorException}

                Me._manager.OnErrorOccurred(e)
            Finally
                If response IsNot Nothing Then
                    response.Close()
                End If

                If requestStream IsNot Nothing Then
                    requestStream.Close()
                End If

                If localFileStream IsNot Nothing Then
                    localFileStream.Close()
                End If
            End Try
        End Sub

        Protected Overridable Sub OnFileUploadCompleted(ByVal e As FileUploadCompletedEventArgs)

            RaiseEvent FileUploadCompleted(Me, e)
        End Sub

        Protected Overridable Sub OnAllFilesUploadCompleted(ByVal e As EventArgs)
            RaiseEvent AllFilesUploadCompleted(Me, e)
        End Sub
    End Class
End Class

