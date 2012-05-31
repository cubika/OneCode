'*************************** 模块头******************************'
' 模块名称:  MainForm.vb
' 项目名称:	    VBMultiThreadedWebDownloader
' 版权 (c) Microsoft Corporation.
' 
' 这是本应用程序的主要窗体，它用来初始化UI界面和处理相关的事件。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************'

Imports System.Configuration
Imports System.IO
Imports System.Net

Partial Public Class MainForm
    Inherits Form
    Private _downloader As MultiThreadedWebDownloader = Nothing

    ' 表明下载是否暂停。
    Private _isPaused As Boolean = False

    Private _lastNotificationTime As Date

    Private _proxy As WebProxy = Nothing

    Public Sub New()
        InitializeComponent()

        ' 使用App.Config初始化proxy。
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("ProxyUrl")) Then
            _proxy = New WebProxy(
                System.Configuration.ConfigurationManager.AppSettings("ProxyUrl"))

            If (Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("ProxyUser"))) _
                AndAlso (Not String.IsNullOrEmpty(
                         ConfigurationManager.AppSettings("ProxyPwd"))) Then
                Dim credential As New NetworkCredential(
                    ConfigurationManager.AppSettings("ProxyUser"),
                    ConfigurationManager.AppSettings("ProxyPwd"))

                _proxy.Credentials = credential
            Else
                _proxy.UseDefaultCredentials = True
            End If
        End If
    End Sub

    ''' <summary>
    '''检查文件信息.
    ''' </summary>
    Private Sub btnCheck_Click(ByVal sender As Object,
                               ByVal e As EventArgs) Handles btnCheck.Click

        ' 初始化 MultiThreadedWebDownloader
        _downloader = New MultiThreadedWebDownloader(tbURL.Text)
        _downloader.Proxy = Me._proxy

        Try
            _downloader.CheckFile()

            ' 更新UI界面。
            tbURL.Enabled = False
            btnCheck.Enabled = False
            tbPath.Enabled = True
            btnDownload.Enabled = True
        Catch
            ' 如果有任何的异常，像System.Net.WebException 或者 
            ' System.Net.ProtocolViolationException, 就说明读取文件信息时有错误，
            '并且该文件是不能被下载的。
            MessageBox.Show("获取文件信息有错误. 请确保URL是有效的！")
        End Try
    End Sub

    ''' <summary>
    ''' 处理btnDownload 单击事件。
    ''' </summary>
    Private Sub btnDownload_Click(ByVal sender As Object,
                                  ByVal e As EventArgs) Handles btnDownload.Click
        Try

            ' 检查文件是否存在。
            If File.Exists(tbPath.Text.Trim()) Then
                Dim message As String = "已经存在一个同命名的文件，要覆盖它吗？ " _
                                        & "如果不，请改变存储路径。 "
                Dim result = MessageBox.Show(
                    message, "文件名冲突: " & tbPath.Text.Trim(),
                    MessageBoxButtons.OKCancel)

                If result = DialogResult.OK Then
                    File.Delete(tbPath.Text.Trim())
                Else
                    Return
                End If
            End If

            If File.Exists(tbPath.Text.Trim() & ".tmp") Then
                File.Delete(tbPath.Text.Trim() & ".tmp")
            End If

            ' 设置下载的路径。
            _downloader.DownloadPath = tbPath.Text.Trim() & ".tmp"


            ' 加载 HttpDownloadClient事件。
            AddHandler _downloader.DownloadCompleted, AddressOf DownloadCompleted
            AddHandler _downloader.DownloadProgressChanged,
                AddressOf DownloadProgressChanged
            AddHandler _downloader.StatusChanged, AddressOf StatusChanged
            AddHandler _downloader.ErrorOccurred, AddressOf ErrorOccurred
            ' 开始下载文件。
            _downloader.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    '''  处理StatusChanged 事件。
    ''' </summary>
    Private Sub StatusChanged(ByVal sender As Object, ByVal e As EventArgs)
        ' 刷新 进度条.
        lbStatus.Text = _downloader.Status.ToString()

        ' 更新UI界面。
        Select Case _downloader.Status

            Case MultiThreadedWebDownloaderStatus.Idle, _
                MultiThreadedWebDownloaderStatus.Canceled, _
                MultiThreadedWebDownloaderStatus.Completed
                btnCheck.Enabled = True
                btnDownload.Enabled = False
                btnPause.Enabled = False
                btnCancel.Enabled = False
                tbPath.Enabled = False
                tbURL.Enabled = True
            Case MultiThreadedWebDownloaderStatus.Checked
                btnCheck.Enabled = False
                btnDownload.Enabled = True
                btnPause.Enabled = False
                btnCancel.Enabled = False
                tbPath.Enabled = True
                tbURL.Enabled = False
            Case MultiThreadedWebDownloaderStatus.Downloading
                btnCheck.Enabled = False
                btnDownload.Enabled = False
                btnPause.Enabled = True
                btnCancel.Enabled = True
                tbPath.Enabled = False
                tbURL.Enabled = False
            Case MultiThreadedWebDownloaderStatus.Pausing, _
                MultiThreadedWebDownloaderStatus.Canceling
                btnCheck.Enabled = False
                btnDownload.Enabled = False
                btnPause.Enabled = False
                btnCancel.Enabled = False
                tbPath.Enabled = False
                tbURL.Enabled = False
            Case MultiThreadedWebDownloaderStatus.Paused
                btnCheck.Enabled = False
                btnDownload.Enabled = False
                btnPause.Enabled = True
                btnCancel.Enabled = False
                tbPath.Enabled = False
                tbURL.Enabled = False
        End Select

        If _downloader.Status = MultiThreadedWebDownloaderStatus.Paused Then
            lbSummary.Text = String.Format(
                "接收: {0}KB, 总共: {1}KB, 时间: {2}:{3}:{4}",
                _downloader.DownloadedSize / 1024,
                _downloader.TotalSize / 1024,
                _downloader.TotalUsedTime.Hours,
                _downloader.TotalUsedTime.Minutes,
                _downloader.TotalUsedTime.Seconds)
        End If
    End Sub

    ''' <summary>
    ''' 处理DownloadProgressChanged 事件。
    ''' </summary>
    Private Sub DownloadProgressChanged(ByVal sender As Object,
                                        ByVal e As MultiThreadedWebDownloaderProgressChangedEventArgs)
        ' 每隔一秒刷新一次主要信息 。
        If Date.Now > _lastNotificationTime.AddSeconds(1) Then
            lbSummary.Text = String.Format(
                "接收: {0}KB 总共: {1}KB 速度: {2}KB/s  线程个数: {3}",
                e.ReceivedSize \ 1024,
                e.TotalSize \ 1024,
                e.DownloadSpeed \ 1024,
                _downloader.DownloadThreadsCount)
            prgDownload.Value = CInt(Fix(e.ReceivedSize * 100 \ e.TotalSize))
            _lastNotificationTime = Date.Now
        End If
    End Sub

    ''' <summary>
    ''' 处理 DownloadCompleted事件。
    ''' </summary>
    Private Sub DownloadCompleted(ByVal sender As Object,
                                  ByVal e As MultiThreadedWebDownloaderCompletedEventArgs)
        lbSummary.Text = String.Format(
            "接收: {0}KB, 总共: {1}KB, 时间: {2}:{3}:{4}",
            e.DownloadedSize \ 1024,
            e.TotalSize \ 1024,
            e.TotalTime.Hours,
            e.TotalTime.Minutes,
            e.TotalTime.Seconds)

        File.Move(tbPath.Text.Trim() & ".tmp", tbPath.Text.Trim())

        prgDownload.Value = 100
    End Sub

    Private Sub ErrorOccurred(ByVal sender As Object, ByVal e As ErrorEventArgs)
        lbSummary.Text = e.Exception.Message
        prgDownload.Value = 0
    End Sub

    ''' <summary>
    '''处理 btnCancel 单击事件。
    ''' </summary>
    Private Sub btnCancel_Click(ByVal sender As Object,
                                ByVal e As EventArgs) Handles btnCancel.Click
        _downloader.Cancel()
    End Sub

    ''' <summary>
    ''' 处理 btnPause点击事件。
    ''' </summary>
    Private Sub btnPause_Click(ByVal sender As Object,
                               ByVal e As EventArgs) Handles btnPause.Click
        If _isPaused Then
            _downloader.Resume()
            btnPause.Text = "暂停"
        Else
            _downloader.Pause()
            btnPause.Text = "继续"
        End If
        _isPaused = Not _isPaused
    End Sub

End Class

