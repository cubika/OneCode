'*************************** Module Header ******************************'
' 模块名:  MainForm.vb
' 项目名:	    VBFTPDownload
' 版权(c)  Microsoft Corporation.
' 
' 这是这个应用程序的主窗体.它是用来初始化界面并处理事件的.
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

Partial Public Class MainForm
    Inherits Form

    Private _client As FTPClientManager = Nothing

    Private _currentCredentials As NetworkCredential = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "URL navigation"

    ''' <summary>
    ''' 处理btnConnect按钮的单击事件.
    ''' </summary>
    Private Sub btnConnect_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnConnect.Click

        ' 通过tbFTPServer.Text指定一个连接服务.
        Connect(Me.tbFTPServer.Text.Trim())

    End Sub

    Private Sub Connect(ByVal urlStr As String)
        Try
            Dim url As New Uri(urlStr)

            '  URL的模式必须是FTP的. 
            If Not url.Scheme.Equals("ftp", StringComparison.OrdinalIgnoreCase) Then
                Throw New ApplicationException("The schema of url must be ftp. ")
            End If

            '把这个url转到这个文件夹并且这个文件夹包括这个文件. 
            If url.IsFile Then
                url = New Uri(url, "..")
            End If

            ' 显示窗体UICredentialsProvider获得新的凭据.
            Using provider As New UICredentialsProvider(Me._currentCredentials)

                ' 显示UICredentialsProvider作为一个对话框.
                Dim result = provider.ShowDialog()

                ' 如果用户输入了凭据并且按下了 "确定" 按钮.
                If result = System.Windows.Forms.DialogResult.OK Then

                    ' 重置当前的凭据.
                    Me._currentCredentials = provider.Credentials

                Else
                    Return
                End If
            End Using

            ' 初始化FTPClient实例.
            _client = New FTPClientManager(url, _currentCredentials)

            AddHandler _client.UrlChanged, AddressOf client_UrlChanged
            AddHandler _client.StatusChanged, AddressOf client_StatusChanged
            AddHandler _client.ErrorOccurred, AddressOf client_ErrorOccurred
            AddHandler _client.FileDownloadCompleted, AddressOf client_FileDownloadCompleted
            AddHandler _client.NewMessageArrived, AddressOf client_NewMessageArrived

            ' 子目录和文件的列表.
            RefreshSubDirectoriesAndFiles()


        Catch webEx As System.Net.WebException
            If (TryCast(webEx.Response, FtpWebResponse)).StatusCode = FtpStatusCode.NotLoggedIn Then
                '重新连接服务器.
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
    '''记录 FTPClient的消息.
    ''' </summary>
    Private Sub client_NewMessageArrived(ByVal sender As Object,
                                         ByVal e As NewMessageEventArg)
        Dim log As String = String.Format("{0} {1}", Date.Now, e.NewMessage)
        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    '''当一个文件被下载时记录FileDownloadCompleted事件.
    ''' </summary>
    Private Sub client_FileDownloadCompleted(ByVal sender As Object,
                                             ByVal e As FileDownloadCompletedEventArgs)
        Dim log As String =
            String.Format("{0} Download from {1} to {2} is completed. Length: {3}. ",
                          Date.Now, e.ServerPath,
                          e.LocalFile.FullName,
                          e.LocalFile.Length)

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' 如果是一个错误将记录ErrorOccurred事件.
    ''' </summary>
    Private Sub client_ErrorOccurred(ByVal sender As Object, ByVal e As ErrorEventArgs)
        Me.lstLog.Items.Add(String.Format("{0} {1} ", Date.Now, e.ErrorException.Message))

        Dim innerException = e.ErrorException.InnerException

        '记录所有的innerException.
        Do While innerException IsNot Nothing
            Me.lstLog.Items.Add(String.Format(vbTab & vbTab & vbTab & " {0} ",
                                              innerException.Message))
            innerException = innerException.InnerException
        Loop

        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' 如果 FTPClient的状态改变了，刷新UI.
    ''' </summary>
    Private Sub client_StatusChanged(ByVal sender As Object, ByVal e As EventArgs)

        ' 如果客户端正在下载文件将禁用所有的按钮 .
        If _client.Status = FTPClientManagerStatus.Downloading Then
            btnBrowseDownloadPath.Enabled = False
            btnConnect.Enabled = False
            btnDownload.Enabled = False
            btnNavigateParentFolder.Enabled = False
            lstFileExplorer.Enabled = False
        Else
            btnBrowseDownloadPath.Enabled = True
            btnConnect.Enabled = True
            btnDownload.Enabled = True
            btnNavigateParentFolder.Enabled = True
            lstFileExplorer.Enabled = True
        End If

        Dim log As String = String.Format("{0} FTPClient status changed to {1}. ",
                                          Date.Now, _client.Status.ToString())

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' 处理FTPClient的UrlChanged事件.
    ''' </summary>
    Private Sub client_UrlChanged(ByVal sender As Object, ByVal e As EventArgs)
        RefreshSubDirectoriesAndFiles()

        Dim log As String = String.Format("{0} The current url changed to {1}. ",
                                          Date.Now, _client.Url)

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    '''处理lstFileExplorer的 DoubleClick事件.
    ''' </summary>
    Private Sub lstFileExplorer_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) _
        Handles lstFileExplorer.DoubleClick
        ' 如果只选择了一个项目并且这个项目代表了一个文件夹，然后导航到了一个子目录

        If lstFileExplorer.SelectedItems.Count = 1 _
            AndAlso (TryCast(lstFileExplorer.SelectedItem, FTPFileSystem)).IsDirectory Then
            Me._client.Naviagte((TryCast(lstFileExplorer.SelectedItem, FTPFileSystem)).Url)
        End If
    End Sub

    ''' <summary>
    ''' 处理btnNavigateParentFolder的事件.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnNavigateParentFolder_Click(ByVal sender As Object,
                                              ByVal e As EventArgs) _
                                          Handles btnNavigateParentFolder.Click

        ' 导航到上一级目录.
        Me._client.NavigateParent()
    End Sub

    ''' <summary>
    ''' 子目录和文件的列表.
    ''' </summary>
    Private Sub RefreshSubDirectoriesAndFiles()
        lbCurrentUrl.Text = String.Format("Current Path: {0}", _client.Url)

        Dim subDirs = _client.GetSubDirectoriesAndFiles()

        ' 列表的排序.
        Dim orderedDirs = From dir In subDirs
                          Order By dir.IsDirectory Descending, dir.Name
                          Select dir

        lstFileExplorer.Items.Clear()
        For Each subdir In orderedDirs
            lstFileExplorer.Items.Add(subdir)
        Next subdir
    End Sub


#End Region

#Region "Download File/Folders"

    ''' <summary>
    ''' 处理 btnBrowseDownloadPath的单击事件.
    ''' </summary>
    Private Sub btnBrowseDownloadPath_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBrowseDownloadPath.Click
        BrowserDownloadPath()
    End Sub

    ''' <summary>
    '''处理btnDownload的单击事件.
    ''' </summary>
    Private Sub btnDownload_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnDownload.Click

        '在资源管理器中一个或多个文件/文件夹应该被选中。
        If lstFileExplorer.SelectedItems.Count = 0 Then
            MessageBox.Show(
                "在文件资源管理器中请选择一个或多个文件/文件夹",
                "没有被选择的文件")
            Return
        End If

        ' 如果这个tbDownloadPath.Text文本是空的时，就显示一个文件夹浏览对话框.
        If String.IsNullOrWhiteSpace(tbDownloadPath.Text) _
            AndAlso BrowserDownloadPath() <> DialogResult.OK Then
            Return
        End If


        Dim directoriesAndFiles = lstFileExplorer.SelectedItems.Cast(Of FTPFileSystem)()

        ' 下载这个选中的项目.
        _client.DownloadDirectoriesAndFiles(directoriesAndFiles, tbDownloadPath.Text)

    End Sub

    ''' <summary>
    '''显示一个文件夹浏览对话框.
    ''' </summary>
    Private Function BrowserDownloadPath() As DialogResult
        Using folderBrowser As New FolderBrowserDialog()
            If Not String.IsNullOrWhiteSpace(tbDownloadPath.Text) Then
                folderBrowser.SelectedPath = tbDownloadPath.Text
            End If
            Dim result = folderBrowser.ShowDialog()
            If result = DialogResult.OK Then
                tbDownloadPath.Text = folderBrowser.SelectedPath
            End If
            Return result
        End Using
    End Function
#End Region

    Private Sub grpDownload_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
End Class