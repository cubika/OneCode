'*************************** 模块头 ******************************'
' 模块名:  MainForum.vb
' 项目名:  VBWebDownloadProgress
' 版权(c)  Microsoft Corporation.
' 
' 这是这个应用程序的主要窗体. 它用来初始化界面和处理事件.
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

Partial Public Class MainForm
    Inherits Form

    Private _client As HttpDownloadClient = Nothing

    ' 指定下载是否已暂停.
    Private _isPaused As Boolean = False

    Private _lastNotificationTime As Date

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' 处理下载按钮点击事件.
    ''' </summary>
    Private Sub btnDownload_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnDownload.Click

        Try
            ' 构建临时文件路径.
            Dim tempPath As String = tbPath.Text.Trim() + ".tmp"

            ' 检查文件是否存在.
            If File.Exists(tempPath) Then
                Dim message As String = "已经存在重名文件， " _
                        + "你想要删除它吗？如果不，请更改本地路径."
                Dim result As DialogResult = MessageBox.Show(message, "文件名冲突",
                        MessageBoxButtons.OKCancel)

                If result = DialogResult.OK Then
                    File.Delete(tempPath)
                Else
                    Return
                End If

            End If

            ' 初始化一个HttpDownloadClient实例.
            ' 首先存储文件到一个临时文件.
            _client = New HttpDownloadClient(tbURL.Text, tbPath.Text.Trim() & ".tmp")

            '// 注册一个HttpDownloadClient事件.
            AddHandler _client.DownloadCompleted, AddressOf DownloadCompleted
            AddHandler _client.DownloadProgressChanged, AddressOf DownloadProgressChanged
            AddHandler _client.StatusChanged, AddressOf StatusChanged

            ' 开始下载文件.
            _client.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' 处理StatusChanged事件.
    ''' </summary>
    Private Sub StatusChanged(ByVal sender As Object, ByVal e As EventArgs)
        ' 刷新状态.
        lbStatus.Text = _client.Status.ToString()

        ' 刷新按钮.
        Select Case _client.Status
            Case HttpDownloadClientStatus.Idle,
                HttpDownloadClientStatus.Canceled,
                HttpDownloadClientStatus.Completed

                btnDownload.Enabled = True
                btnPause.Enabled = False
                btnCancel.Enabled = False
                tbPath.Enabled = True
                tbURL.Enabled = True

            Case HttpDownloadClientStatus.Downloading
                btnDownload.Enabled = False
                btnPause.Enabled = True
                btnCancel.Enabled = True
                tbPath.Enabled = False
                tbURL.Enabled = False

            Case HttpDownloadClientStatus.Pausing,
                HttpDownloadClientStatus.Canceling

                btnDownload.Enabled = False
                btnPause.Enabled = False
                btnCancel.Enabled = False
                tbPath.Enabled = False
                tbURL.Enabled = False

            Case HttpDownloadClientStatus.Paused
                btnDownload.Enabled = False
                btnPause.Enabled = True
                btnCancel.Enabled = False
                tbPath.Enabled = False
                tbURL.Enabled = False
        End Select

        If _client.Status = HttpDownloadClientStatus.Paused Then
            lbSummary.Text = String.Format("已接收: {0}KB, 总共: {1}KB, 时间: {2}:{3}:{4}",
                                           _client.DownloadedSize \ 1024,
                                           _client.TotalSize \ 1024,
                                           _client.TotalUsedTime.Hours,
                                           _client.TotalUsedTime.Minutes,
                                           _client.TotalUsedTime.Seconds)
        End If
    End Sub

    ''' <summary>
    ''' 处理DownloadProgressChanged事件.
    ''' </summary>
    Private Sub DownloadProgressChanged(ByVal sender As Object,
                                        ByVal e As HttpDownloadProgressChangedEventArgs)
        ' 每秒刷新摘要.
        If Date.Now > _lastNotificationTime.AddSeconds(1) Then
            lbSummary.Text = String.Format("已接收: {0}KB, 总共: {1}KB, 速度: {2}KB/s",
                                           e.ReceivedSize \ 1024, e.TotalSize \ 1024,
                                           e.DownloadSpeed \ 1024)
            prgDownload.Value = CInt(Fix(e.ReceivedSize * 100 \ e.TotalSize))
            _lastNotificationTime = Date.Now
        End If
    End Sub

    ''' <summary>
    ''' 处理DownloadCompleted事件
    ''' </summary>
    Private Sub DownloadCompleted(ByVal sender As Object,
                                  ByVal e As HttpDownloadCompletedEventArgs)
        If e.Error Is Nothing Then
            lbSummary.Text = String.Format("已接收: {0}KB, 总共: {1}KB, 时间: {2}:{3}:{4}",
                                           e.DownloadedSize \ 1024, e.TotalSize \ 1024,
                                           e.TotalTime.Hours, e.TotalTime.Minutes,
                                           e.TotalTime.Seconds)

            If File.Exists(tbPath.Text.Trim()) Then
                File.Delete(tbPath.Text.Trim())
            End If

            File.Move(tbPath.Text.Trim() & ".tmp", tbPath.Text.Trim())
            prgDownload.Value = 100
        Else
            lbSummary.Text = e.Error.Message
            If File.Exists(tbPath.Text.Trim() & ".tmp") Then
                File.Delete(tbPath.Text.Trim() & ".tmp")
            End If
            prgDownload.Value = 0
        End If

    End Sub

    ''' <summary>
    ''' 处理删除按钮点击事件.
    ''' </summary>
    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnCancel.Click
        _client.Cancel()
    End Sub

    ''' <summary>
    ''' 处理暂停按钮点击事件.
    ''' </summary>
    Private Sub btnPause_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnPause.Click
        If _isPaused Then
            _client.Resume()
            btnPause.Text = "暂停"
        Else
            _client.Pause()
            btnPause.Text = "重新开始"
        End If
        _isPaused = Not _isPaused
    End Sub
End Class
