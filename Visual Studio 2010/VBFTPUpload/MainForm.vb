'*************************** 模块头 ******************************'
' 模块名:  MainForm.vb
' 项目:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
' 应用程序的主窗体，用来初始化UI和处理事件。
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
Imports System.Net
Imports System.Text
Imports System.IO

Partial Public Class MainForm
    Inherits Form

    Private _client As FTPClientManager = Nothing

    Private _currentCredentials As NetworkCredential = Nothing

    Public Sub New()
        InitializeComponent()

        RefreshUI()
    End Sub

#Region "URL navigation"

    ''' <summary>
    ''' 处理btnConnect单击事件
    ''' </summary>
    Private Sub btnConnect_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnConnect.Click

        ' 通过tbFTPServer.Text连接服务器.
        Connect(Me.tbFTPServer.Text.Trim())

    End Sub

    Private Sub Connect(ByVal urlStr As String)
        Try
            Dim url As New Uri(urlStr)

            '地址必须是ftp格式。 
            If Not url.Scheme.Equals("ftp", StringComparison.OrdinalIgnoreCase) Then
                Throw New ApplicationException("地址格式必须为 ftp. ")
            End If

            '设置包含文件的文件夹地址. 
            If url.IsFile Then
                url = New Uri(url, "..")
            End If

            ' 显示UICredentialsProvider窗体得到新认证。
            Using provider As New UICredentialsProvider(Me._currentCredentials)

                ' 显示UICredentialsProvider窗体为一个对话框.
                Dim result = provider.ShowDialog()

                ' 假如用户输入认证 ，按“OK”按钮
                If result = System.Windows.Forms.DialogResult.OK Then

                    ' 重设当前认证。
                    Me._currentCredentials = provider.Credentials

                Else
                    Return
                End If
            End Using

            ' 初始化FTP客户端实例
            _client = New FTPClientManager(url, _currentCredentials)

            AddHandler _client.UrlChanged, AddressOf client_UrlChanged
            AddHandler _client.StatusChanged, AddressOf client_StatusChanged
            AddHandler _client.ErrorOccurred, AddressOf client_ErrorOccurred
            AddHandler _client.FileUploadCompleted, AddressOf client_FileUploadCompleted
            AddHandler _client.NewMessageArrived, AddressOf client_NewMessageArrived

            ' 刷新UI和列出子目录和文件
            RefreshUI()


        Catch webEx As System.Net.WebException
            If (TryCast(webEx.Response, FtpWebResponse)).StatusCode = FtpStatusCode.NotLoggedIn Then
                '重新连接服务器
                Connect(urlStr)

                Return
            Else
                MessageBox.Show(webEx.Message)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 记录FTPClient信息
    ''' </summary>
    Private Sub client_NewMessageArrived(ByVal sender As Object,
                                         ByVal e As NewMessageEventArg)
        Dim log As String = String.Format("{0} {1}", Date.Now, e.NewMessage)
        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' 当一个文件被上传记录FileUploadCompleted事件
    ''' </summary>
    Private Sub client_FileUploadCompleted(ByVal sender As Object,
                                             ByVal e As FileUploadCompletedEventArgs)
        Dim log As String = String.Format(
            "{0} Upload from {1} to {2} is completed. Length: {3}. ",
            Date.Now, e.LocalFile.FullName, e.ServerPath, e.LocalFile.Length)

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' 假如有一个错误记录ErrorOccurred事件。
    ''' </summary>
    Private Sub client_ErrorOccurred(ByVal sender As Object, ByVal e As ErrorEventArgs)
        Me.lstLog.Items.Add(String.Format("{0} {1} ", Date.Now, e.ErrorException.Message))

        Dim innerException = e.ErrorException.InnerException

        ' 记录所有的innerException
        Do While innerException IsNot Nothing
            Me.lstLog.Items.Add(String.Format(vbTab & vbTab & vbTab & " {0} ",
                                              innerException.Message))
            innerException = innerException.InnerException
        Loop

        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' 加入FTPClient状态改变刷新UI.
    ''' </summary>
    Private Sub client_StatusChanged(ByVal sender As Object, ByVal e As EventArgs)
        RefreshUI()

        Dim log As String = String.Format("{0} FTPClient 状态改变到 {1}. ",
                                          Date.Now, _client.Status.ToString())

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    Private Sub RefreshUI()
        ' 假如客户端正上传文件，所有按钮为不好用。
        If _client Is Nothing OrElse _client.Status <> FTPClientManagerStatus.Idle Then

            btnBrowseLocalFolder.Enabled = False
            btnUploadFolder.Enabled = False

            btnBrowseLocalFile.Enabled = False
            btnUploadFile.Enabled = False

            btnDelete.Enabled = False

            btnNavigateParentFolder.Enabled = False
            lstFileExplorer.Enabled = False
        Else

            btnBrowseLocalFolder.Enabled = True
            btnUploadFolder.Enabled = True

            btnBrowseLocalFile.Enabled = True
            btnUploadFile.Enabled = True

            btnDelete.Enabled = True

            btnNavigateParentFolder.Enabled = True
            lstFileExplorer.Enabled = True
        End If

        btnConnect.Enabled = _client Is Nothing _
            OrElse _client.Status = FTPClientManagerStatus.Idle

        RefreshSubDirectoriesAndFiles()

    End Sub

    ''' <summary>
    ''' 处理FTPClient的UrlChanged事件
    ''' </summary>
    Private Sub client_UrlChanged(ByVal sender As Object, ByVal e As EventArgs)
        RefreshSubDirectoriesAndFiles()

        Dim log As String = String.Format("{0} The current url changed to {1}. ",
                                          Date.Now, _client.Url)

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' 处理1stFileExplorer双击事件
    ''' </summary>
    Private Sub lstFileExplorer_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) _
        Handles lstFileExplorer.DoubleClick
        ' 假如一个选项被选择，这个选项代表一个文件夹，导航到一个子目录
        If lstFileExplorer.SelectedItems.Count = 1 _
            AndAlso (TryCast(lstFileExplorer.SelectedItem, FTPFileSystem)).IsDirectory Then
            Me._client.Naviagte((TryCast(lstFileExplorer.SelectedItem, FTPFileSystem)).Url)
        End If
    End Sub

    ''' <summary>
    ''' 处理btnNavigateParentFolder单击事件。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnNavigateParentFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNavigateParentFolder.Click

        ' 导航到父文件夹.
        Me._client.NavigateParent()
    End Sub

    ''' <summary>
    ''' 列出子目录和文件
    ''' </summary>
    Private Sub RefreshSubDirectoriesAndFiles()
        If _client Is Nothing Then
            Return
        End If

        lbCurrentUrl.Text = String.Format("Current Path: {0}", _client.Url)

        Dim subDirs = _client.GetSubDirectoriesAndFiles()

        ' 排序列表
        Dim orderedDirs = From dir In subDirs
                          Order By dir.IsDirectory Descending, dir.Name
                          Select dir

        lstFileExplorer.Items.Clear()
        For Each subdir In orderedDirs
            lstFileExplorer.Items.Add(subdir)
        Next subdir
    End Sub


#End Region

#Region "Upload a Folder"

    ''' <summary>
    ''' 处理btnBrowseLocalFolder单击事件
    ''' </summary>
    Private Sub btnBrowseLocalFolder_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBrowseLocalFolder.Click
        BrowserLocalFolder()
    End Sub

    ''' <summary>
    '''  处理btnUploadFolder单击事件
    ''' </summary>
    Private Sub btnUploadFolder_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnUploadFolder.Click

        ' 如果tbLocalFolder.Text 为空，显示一个 FolderBrowserDialog
        If String.IsNullOrWhiteSpace(tbLocalFolder.Text) _
            AndAlso BrowserLocalFolder() <> DialogResult.OK Then
            Return
        End If

        Try
            Dim dir As New DirectoryInfo(tbLocalFolder.Text)

            If Not dir.Exists Then
                Throw New ApplicationException(
                    String.Format(" The folder {0} does not exist!", dir.FullName))
            End If

            ' 上传选项
            _client.UploadFolder(dir, _client.Url, chkCreateFolder.Checked)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    '''  显示一个 FolderBrowserDialog.
    ''' </summary>
    Private Function BrowserLocalFolder() As DialogResult
        Using folderBrowser As New FolderBrowserDialog()
            If Not String.IsNullOrWhiteSpace(tbLocalFolder.Text) Then
                folderBrowser.SelectedPath = tbLocalFolder.Text
            End If
            Dim result = folderBrowser.ShowDialog()
            If result = DialogResult.OK Then
                tbLocalFolder.Text = folderBrowser.SelectedPath
            End If
            Return result
        End Using
    End Function

#End Region


#Region "Upload files"

    Private Sub btnBrowseLocalFile_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBrowseLocalFile.Click
        BrowserLocalFiles()
    End Sub

    Private Sub btnUploadFile_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnUploadFile.Click
        If tbLocalFile.Tag Is Nothing AndAlso BrowserLocalFiles() <> DialogResult.OK Then
            Return
        End If

        Try
            Dim files As New List(Of FileInfo)()
            Dim selectedFiles() As String = TryCast(tbLocalFile.Tag, String())

            For Each selectedFile In selectedFiles
                Dim fileInfo_Renamed As New FileInfo(selectedFile)
                If Not fileInfo_Renamed.Exists Then
                    Throw New ApplicationException(
                        String.Format("  文件 {0} 不存在!", selectedFile))
                Else
                    files.Add(fileInfo_Renamed)
                End If
            Next selectedFile

            If files.Count > 0 Then
                _client.UploadFoldersAndFiles(files, _client.Url)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 展示一个FolderBrowserDialog
    ''' </summary>
    Private Function BrowserLocalFiles() As DialogResult
        Using fileBrowser As New OpenFileDialog()
            fileBrowser.Multiselect = True
            Dim result = fileBrowser.ShowDialog()
            If result = DialogResult.OK Then
                tbLocalFile.Tag = fileBrowser.FileNames

                Dim filesText As New StringBuilder()
                For Each file In fileBrowser.FileNames
                    filesText.Append(file & ";")
                Next file
                tbLocalFile.Text = filesText.ToString()
            End If
            Return result
        End Using
    End Function


#End Region

#Region "Delete files"

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnDelete.Click
        If lstFileExplorer.SelectedItems.Count = 0 Then
            MessageBox.Show("请在FTP文件管理器中选择删除项")
        End If

        Dim itemsToDelete = lstFileExplorer.SelectedItems.Cast(Of FTPFileSystem)()

        Me._client.DeleteItemsOnFTPServer(itemsToDelete)

        RefreshUI()
    End Sub

#End Region

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class
