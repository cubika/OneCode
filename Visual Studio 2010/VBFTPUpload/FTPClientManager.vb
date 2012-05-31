'*************************** 模块头 ******************************'
'模块名:  FTPClientManager.vb
' 项目:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
' FTPClientManager类提供如下特性：
'1.查证是否一个文件或一个目录在FTP服务器上存在。
'2.列出在FTP服务器子目录和文件夹中的文件
'3.删除FTP服务器上的文件和目录
' 4.在FTP服务器上创建一个目录
'5.管理FTPUploadClient上传文件件到FTP服务器上
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
Imports System.Linq

Partial Public Class FTPClientManager

    ''' <summary>
    ''' 连接认证到FTP服务器
    ''' </summary>
    Public Property Credentials() As ICredentials

    ''' <summary>
    ''' FTPClient当前路径
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

    Private _status As FTPClientManagerStatus

    ''' <summary>
    '''得到或设置FTPClient状态.
    ''' </summary>
    Public Property Status() As FTPClientManagerStatus
        Get
            Return _status
        End Get

        Private Set(ByVal value As FTPClientManagerStatus)
            If _status <> value Then
                _status = value

                ' 执行OnStatusChanged事件
                Me.OnStatusChanged(EventArgs.Empty)

            End If
        End Set
    End Property

    Public Event UrlChanged As EventHandler

    Public Event ErrorOccurred As EventHandler(Of ErrorEventArgs)

    Public Event StatusChanged As EventHandler

    Public Event FileUploadCompleted As EventHandler(Of FileUploadCompletedEventArgs)

    Public Event NewMessageArrived As EventHandler(Of NewMessageEventArg)

    ''' <summary>
    ''' 初始化一个FTPClient事例
    ''' </summary>
    Public Sub New(ByVal url As Uri, ByVal credentials As ICredentials)
        Me.Credentials = credentials

        '检查路径是否存在并且认证是否正确 假如有错误，抛出异常
        CheckFTPUrlExist(url)

        Me.Url = url

        ' 设置 Status.
        Me.Status = FTPClientManagerStatus.Idle

    End Sub

    ''' <summary>
    '''  导航到父文件夹
    ''' </summary>
    Public Sub NavigateParent()
        If Url.AbsolutePath <> "/" Then

            '得到父路径
            Dim newUrl As New Uri(Me.Url, "..")

            ' Check whether the Url exists.
            CheckFTPUrlExist(newUrl)

            Me.Url = newUrl
            Me.OnUrlChanged(EventArgs.Empty)
        End If
    End Sub

    ''' <summary>
    ''' 导航目录
    ''' </summary>
    Public Sub Naviagte(ByVal newUrl As Uri)
        ' 检查路径是否存在
        Dim urlExist As Boolean = VerifyFTPUrlExist(newUrl)

        Me.Url = newUrl
        Me.OnUrlChanged(EventArgs.Empty)
    End Sub

    ''' <summary>
    ''' 假如有路径不存在，抛出异常
    ''' </summary>
    Private Sub CheckFTPUrlExist(ByVal url As Uri)
        Dim urlExist As Boolean = VerifyFTPUrlExist(url)

        If Not urlExist Then
            Throw New ApplicationException("The url does not exist")
        End If
    End Sub

    ''' <summary>
    ''' 核实路径是否存在
    ''' </summary>
    Private Function VerifyFTPUrlExist(ByVal url As Uri) As Boolean
        Dim urlExist As Boolean = False

        If url.IsFile Then
            urlExist = VerifyFileExist(url)
        Else
            urlExist = VerifyDirectoryExist(url)
        End If

        Return urlExist
    End Function

    ''' <summary>
    ''' 核实目录是否存在
    ''' </summary>
    Private Function VerifyDirectoryExist(ByVal url As Uri) As Boolean
        Dim request As FtpWebRequest = TryCast(WebRequest.Create(url), FtpWebRequest)
        request.Credentials = Me.Credentials
        request.Method = WebRequestMethods.Ftp.ListDirectory

        Dim response As FtpWebResponse = Nothing

        Try
            response = TryCast(request.GetResponse(), FtpWebResponse)

            Return response.StatusCode = FtpStatusCode.DataAlreadyOpen
        Catch webEx As System.Net.WebException
            Dim ftpResponse As FtpWebResponse = TryCast(webEx.Response, FtpWebResponse)

            If ftpResponse.StatusCode = FtpStatusCode.ActionNotTakenFileUnavailable Then
                Return False
            End If

            Throw
        Finally
            If response IsNot Nothing Then
                response.Close()
            End If
        End Try
    End Function

    ''' <summary>
    ''' 核实文件是否存在
    ''' </summary>
    Private Function VerifyFileExist(ByVal url As Uri) As Boolean
        Dim request As FtpWebRequest = TryCast(WebRequest.Create(url), FtpWebRequest)
        request.Credentials = Me.Credentials
        request.Method = WebRequestMethods.Ftp.GetFileSize

        Dim response As FtpWebResponse = Nothing

        Try
            response = TryCast(request.GetResponse(), FtpWebResponse)

            Return response.StatusCode = FtpStatusCode.FileStatus
        Catch webEx As System.Net.WebException
            Dim ftpResponse As FtpWebResponse = TryCast(webEx.Response, FtpWebResponse)

            If ftpResponse.StatusCode = FtpStatusCode.ActionNotTakenFileUnavailable Then
                Return False
            End If

            Throw
        Finally
            If response IsNot Nothing Then
                response.Close()
            End If
        End Try
    End Function

    ''' <summary>
    ''' 得到子目录和默认的当前路径文件
    ''' </summary>
    Public Function GetSubDirectoriesAndFiles() As IEnumerable(Of FTPFileSystem)
        Return GetSubDirectoriesAndFiles(Me.Url)
    End Function

    ''' <summary>
    ''' 得到子目录和路径中文件，在枚举中使用所有文件夹
    ''' 当运行FTP LIST 协议方法得到详细的文件列表。
    ''' 在一个FTP服务器上，服务器响应许多信息记录，每个记录代表一个文件
    ''' </summary>
    Public Function GetSubDirectoriesAndFiles(ByVal url As Uri) _
        As IEnumerable(Of FTPFileSystem)
        Dim request As FtpWebRequest = TryCast(WebRequest.Create(url), FtpWebRequest)
        request.Credentials = Me.Credentials
        request.Method = WebRequestMethods.Ftp.ListDirectoryDetails

        Dim response As FtpWebResponse = Nothing
        Dim responseStream As Stream = Nothing
        Dim reader As StreamReader = Nothing
        Try
            response = TryCast(request.GetResponse(), FtpWebResponse)

            Me.OnNewMessageArrived(New NewMessageEventArg _
                                   With {.NewMessage = response.StatusDescription})

                responseStream = response.GetResponseStream()
                reader = New StreamReader(responseStream)

                Dim subDirs As New List(Of FTPFileSystem)()

                Dim subDir As String = reader.ReadLine()

            '从记录字符串中找出FTP目录列表格式
                Dim style As FTPDirectoryListingStyle = FTPDirectoryListingStyle.MSDOS
                If Not String.IsNullOrEmpty(subDir) Then
                    style = FTPFileSystem.GetDirectoryListingStyle(subDir)
                End If
                Do While Not String.IsNullOrEmpty(subDir)
                    subDirs.Add(FTPFileSystem.ParseRecordString(url, subDir, style))

                    subDir = reader.ReadLine()
                Loop
                Return subDirs
        Finally
            If response IsNot Nothing Then
                response.Close()
            End If

            ' 关闭StreamReader对象和相关流，释放reader系统资源
            If reader IsNot Nothing Then
                reader.Close()
            End If
        End Try
    End Function

    ''' <summary>
    ''' 在远程FTP服务器上创建一个文件夹子目录
    ''' </summary>
    Public Sub CreateDirectoryOnFTPServer(ByVal serverPath As Uri,
                                          ByVal subDirectoryName As String)

        ' 创建路径为这个新子目录
        Dim subDirUrl As New Uri(serverPath, subDirectoryName)

        ' 检查子目录是否存在
        Dim urlExist As Boolean = VerifyFTPUrlExist(subDirUrl)

        If urlExist Then
            Return
        End If

        Try
            ' 创建一个FtpWebRequest来创建子目录
            Dim request As FtpWebRequest = TryCast(WebRequest.Create(subDirUrl), 
                FtpWebRequest)
            request.Credentials = Me.Credentials
            request.Method = WebRequestMethods.Ftp.MakeDirectory

            Using response As FtpWebResponse = TryCast(request.GetResponse(), 
                FtpWebResponse)
                Me.OnNewMessageArrived(New NewMessageEventArg _
                                       With {.NewMessage = response.StatusDescription})
            End Using

            ' 假如文件夹没存在，创建文件夹
        Catch webEx As System.Net.WebException

            Dim ftpResponse As FtpWebResponse = TryCast(webEx.Response, FtpWebResponse)

            Dim msg As String = String.Format(
                "There is an error while creating folder {0}. " _
                & " StatusCode: {1}  StatusDescription: {2} ",
                subDirUrl.ToString(),
                ftpResponse.StatusCode.ToString(),
                ftpResponse.StatusDescription)
            Dim errorException As New ApplicationException(msg, webEx)

            ' 执行带错误的ErrorOccurred事件
            Dim e As ErrorEventArgs = New ErrorEventArgs _
                                      With {.ErrorException = errorException}

            Me.OnErrorOccurred(e)
        End Try
    End Sub

    ''' <summary>
    '''  在FTP服务器上删除选项
    ''' </summary>
    Public Sub DeleteItemsOnFTPServer(ByVal fileSystems As IEnumerable(Of FTPFileSystem))
        If fileSystems Is Nothing Then
            Throw New ArgumentException("The item to delete is null!")
        End If

        For Each fileSystem In fileSystems
            DeleteItemOnFTPServer(fileSystem)
        Next fileSystem

    End Sub

    ''' <summary>
    ''' 在FTP服务器上删除一个选项
    ''' </summary>
    Public Sub DeleteItemOnFTPServer(ByVal fileSystem As FTPFileSystem)
        ' Check whether sub directory exist.
        Dim urlExist As Boolean = VerifyFTPUrlExist(fileSystem.Url)

        If Not urlExist Then
            Return
        End If

        Try

            '不为空的文件夹不能删除
            If fileSystem.IsDirectory Then
                Dim subFTPFiles = GetSubDirectoriesAndFiles(fileSystem.Url)

                DeleteItemsOnFTPServer(subFTPFiles)
            End If

            '创建一个FtpWebRequest创建子目录
            Dim request As FtpWebRequest = TryCast(WebRequest.Create(fileSystem.Url), 
                FtpWebRequest)
            request.Credentials = Me.Credentials

            request.Method = If(fileSystem.IsDirectory,
                                WebRequestMethods.Ftp.RemoveDirectory,
                                WebRequestMethods.Ftp.DeleteFile)

            Using response As FtpWebResponse = TryCast(request.GetResponse(), 
                FtpWebResponse)
                Me.OnNewMessageArrived(New NewMessageEventArg _
                                       With {.NewMessage = response.StatusDescription})
            End Using
        Catch webEx As System.Net.WebException
            Dim ftpResponse As FtpWebResponse = TryCast(webEx.Response, FtpWebResponse)

            Dim msg As String = String.Format(
                "There is an error while deleting {0}. " _
                & " StatusCode: {1}  StatusDescription: {2} ",
                fileSystem.Url.ToString(),
                ftpResponse.StatusCode.ToString(),
                ftpResponse.StatusDescription)
            Dim errorException As New ApplicationException(msg, webEx)

            ' 有错误时执行ErrorOccurred 事件
            Dim e As ErrorEventArgs = New ErrorEventArgs _
                                      With {.ErrorException = errorException}

            Me.OnErrorOccurred(e)
        End Try
    End Sub

    ''' <summary>
    ''' 上传一个全部的本地文件夹到FTP服务器
    ''' </summary>
    Public Sub UploadFolder(ByVal localFolder As DirectoryInfo,
                            ByVal serverPath As Uri, ByVal createFolderOnServer As Boolean)
        ' UploadFoldersAndFiles方法将创建或重写一个默认文件夹
        If createFolderOnServer Then
            UploadFoldersAndFiles(New FileSystemInfo() {localFolder}, serverPath)

            ' 上传文件和本地文件夹子目录
        Else
            UploadFoldersAndFiles(localFolder.GetFileSystemInfos(), serverPath)
        End If
    End Sub

    ''' <summary>
    '''上传本地文件夹和文件到FTP服务器
    ''' </summary>
    Public Sub UploadFoldersAndFiles(ByVal fileSystemInfos As IEnumerable(Of FileSystemInfo),
                                     ByVal serverPath As Uri)
        If Me._status <> FTPClientManagerStatus.Idle Then
            Throw New ApplicationException("This client is busy now.")
        End If

        Me.Status = FTPClientManagerStatus.Uploading

        Dim uploadClient As New FTPUploadClient(Me)

        '注册事件
        AddHandler uploadClient.AllFilesUploadCompleted,
            AddressOf uploadClient_AllFilesUploadCompleted
        AddHandler uploadClient.FileUploadCompleted,
            AddressOf uploadClient_FileUploadCompleted

        uploadClient.UploadDirectoriesAndFiles(fileSystemInfos, serverPath)
    End Sub


    Private Sub uploadClient_FileUploadCompleted(ByVal sender As Object,
                                                 ByVal e As FileUploadCompletedEventArgs)
        Me.OnFileUploadCompleted(e)
    End Sub

    Private Sub uploadClient_AllFilesUploadCompleted(ByVal sender As Object,
                                                     ByVal e As EventArgs)
        Me.Status = FTPClientManagerStatus.Idle
    End Sub

    Protected Overridable Sub OnUrlChanged(ByVal e As EventArgs)
        RaiseEvent UrlChanged(Me, e)
    End Sub

    Protected Overridable Sub OnStatusChanged(ByVal e As EventArgs)
        RaiseEvent StatusChanged(Me, e)
    End Sub

    Protected Overridable Sub OnFileUploadCompleted(ByVal e As FileUploadCompletedEventArgs)
        RaiseEvent FileUploadCompleted(Me, e)
    End Sub

    Protected Overridable Sub OnErrorOccurred(ByVal e As ErrorEventArgs)
        Me.Status = FTPClientManagerStatus.Idle

        RaiseEvent ErrorOccurred(Me, e)
    End Sub

    Protected Overridable Sub OnNewMessageArrived(ByVal e As NewMessageEventArg)
        RaiseEvent NewMessageArrived(Me, e)
    End Sub
End Class
