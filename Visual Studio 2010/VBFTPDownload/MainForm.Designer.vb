
Partial Public Class MainForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.pnlFTPServer = New System.Windows.Forms.Panel()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.tbFTPServer = New System.Windows.Forms.TextBox()
        Me.lbFTPServer = New System.Windows.Forms.Label()
        Me.lbCurrentUrl = New System.Windows.Forms.Label()
        Me.grpDownload = New System.Windows.Forms.GroupBox()
        Me.btnDownload = New System.Windows.Forms.Button()
        Me.btnBrowseDownloadPath = New System.Windows.Forms.Button()
        Me.tbDownloadPath = New System.Windows.Forms.TextBox()
        Me.lbDownloadPath = New System.Windows.Forms.Label()
        Me.pnlStatus = New System.Windows.Forms.Panel()
        Me.grpLog = New System.Windows.Forms.GroupBox()
        Me.lstLog = New System.Windows.Forms.ListBox()
        Me.grpFileExplorer = New System.Windows.Forms.GroupBox()
        Me.lstFileExplorer = New System.Windows.Forms.ListBox()
        Me.pnlCurrentPath = New System.Windows.Forms.Panel()
        Me.btnNavigateParentFolder = New System.Windows.Forms.Button()
        Me.pnlFTPServer.SuspendLayout()
        Me.grpDownload.SuspendLayout()
        Me.pnlStatus.SuspendLayout()
        Me.grpLog.SuspendLayout()
        Me.grpFileExplorer.SuspendLayout()
        Me.pnlCurrentPath.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlFTPServer
        '
        Me.pnlFTPServer.Controls.Add(Me.btnConnect)
        Me.pnlFTPServer.Controls.Add(Me.tbFTPServer)
        Me.pnlFTPServer.Controls.Add(Me.lbFTPServer)
        Me.pnlFTPServer.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlFTPServer.Location = New System.Drawing.Point(0, 0)
        Me.pnlFTPServer.Name = "pnlFTPServer"
        Me.pnlFTPServer.Size = New System.Drawing.Size(1077, 33)
        Me.pnlFTPServer.TabIndex = 0
        '
        'btnConnect
        '
        Me.btnConnect.Location = New System.Drawing.Point(440, 4)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(75, 23)
        Me.btnConnect.TabIndex = 2
        Me.btnConnect.Text = "连接"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'tbFTPServer
        '
        Me.tbFTPServer.Location = New System.Drawing.Point(78, 8)
        Me.tbFTPServer.Name = "tbFTPServer"
        Me.tbFTPServer.Size = New System.Drawing.Size(355, 20)
        Me.tbFTPServer.TabIndex = 1
        Me.tbFTPServer.Text = "ftp://localhost"
        '
        'lbFTPServer
        '
        Me.lbFTPServer.AutoSize = True
        Me.lbFTPServer.Location = New System.Drawing.Point(13, 13)
        Me.lbFTPServer.Name = "lbFTPServer"
        Me.lbFTPServer.Size = New System.Drawing.Size(51, 13)
        Me.lbFTPServer.TabIndex = 0
        Me.lbFTPServer.Text = "FTP服务"
        '
        'lbCurrentUrl
        '
        Me.lbCurrentUrl.AutoSize = True
        Me.lbCurrentUrl.Location = New System.Drawing.Point(106, 10)
        Me.lbCurrentUrl.Name = "lbCurrentUrl"
        Me.lbCurrentUrl.Size = New System.Drawing.Size(55, 13)
        Me.lbCurrentUrl.TabIndex = 4
        Me.lbCurrentUrl.Text = "当前路径"
        '
        'grpDownload
        '
        Me.grpDownload.Controls.Add(Me.btnDownload)
        Me.grpDownload.Controls.Add(Me.btnBrowseDownloadPath)
        Me.grpDownload.Controls.Add(Me.tbDownloadPath)
        Me.grpDownload.Controls.Add(Me.lbDownloadPath)
        Me.grpDownload.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpDownload.Location = New System.Drawing.Point(524, 33)
        Me.grpDownload.Name = "grpDownload"
        Me.grpDownload.Size = New System.Drawing.Size(553, 341)
        Me.grpDownload.TabIndex = 0
        Me.grpDownload.TabStop = False
        Me.grpDownload.Text = "下载"
        '
        'btnDownload
        '
        Me.btnDownload.Location = New System.Drawing.Point(91, 64)
        Me.btnDownload.Name = "btnDownload"
        Me.btnDownload.Size = New System.Drawing.Size(75, 23)
        Me.btnDownload.TabIndex = 3
        Me.btnDownload.Text = "下载"
        Me.btnDownload.UseVisualStyleBackColor = True
        '
        'btnBrowseDownloadPath
        '
        Me.btnBrowseDownloadPath.Location = New System.Drawing.Point(427, 25)
        Me.btnBrowseDownloadPath.Name = "btnBrowseDownloadPath"
        Me.btnBrowseDownloadPath.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseDownloadPath.TabIndex = 2
        Me.btnBrowseDownloadPath.Text = "Browse"
        Me.btnBrowseDownloadPath.UseVisualStyleBackColor = True
        '
        'tbDownloadPath
        '
        Me.tbDownloadPath.Location = New System.Drawing.Point(91, 26)
        Me.tbDownloadPath.Name = "tbDownloadPath"
        Me.tbDownloadPath.ReadOnly = True
        Me.tbDownloadPath.Size = New System.Drawing.Size(329, 20)
        Me.tbDownloadPath.TabIndex = 1
        '
        'lbDownloadPath
        '
        Me.lbDownloadPath.AutoSize = True
        Me.lbDownloadPath.Location = New System.Drawing.Point(7, 30)
        Me.lbDownloadPath.Name = "lbDownloadPath"
        Me.lbDownloadPath.Size = New System.Drawing.Size(55, 13)
        Me.lbDownloadPath.TabIndex = 0
        Me.lbDownloadPath.Text = "下载路径"
        '
        'pnlStatus
        '
        Me.pnlStatus.Controls.Add(Me.grpLog)
        Me.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlStatus.Location = New System.Drawing.Point(0, 374)
        Me.pnlStatus.Name = "pnlStatus"
        Me.pnlStatus.Size = New System.Drawing.Size(1077, 199)
        Me.pnlStatus.TabIndex = 1
        '
        'grpLog
        '
        Me.grpLog.Controls.Add(Me.lstLog)
        Me.grpLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpLog.Location = New System.Drawing.Point(0, 0)
        Me.grpLog.Name = "grpLog"
        Me.grpLog.Size = New System.Drawing.Size(1077, 199)
        Me.grpLog.TabIndex = 0
        Me.grpLog.TabStop = False
        Me.grpLog.Text = "日志"
        '
        'lstLog
        '
        Me.lstLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstLog.FormattingEnabled = True
        Me.lstLog.Location = New System.Drawing.Point(3, 16)
        Me.lstLog.Name = "lstLog"
        Me.lstLog.Size = New System.Drawing.Size(1071, 180)
        Me.lstLog.TabIndex = 0
        '
        'grpFileExplorer
        '
        Me.grpFileExplorer.Controls.Add(Me.lstFileExplorer)
        Me.grpFileExplorer.Controls.Add(Me.pnlCurrentPath)
        Me.grpFileExplorer.Dock = System.Windows.Forms.DockStyle.Left
        Me.grpFileExplorer.Location = New System.Drawing.Point(0, 33)
        Me.grpFileExplorer.Name = "grpFileExplorer"
        Me.grpFileExplorer.Size = New System.Drawing.Size(524, 341)
        Me.grpFileExplorer.TabIndex = 2
        Me.grpFileExplorer.TabStop = False
        Me.grpFileExplorer.Text = "FTP文件资源管理器"
        '
        'lstFileExplorer
        '
        Me.lstFileExplorer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstFileExplorer.FormattingEnabled = True
        Me.lstFileExplorer.Location = New System.Drawing.Point(3, 48)
        Me.lstFileExplorer.Name = "lstFileExplorer"
        Me.lstFileExplorer.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstFileExplorer.Size = New System.Drawing.Size(518, 290)
        Me.lstFileExplorer.TabIndex = 0
        '
        'pnlCurrentPath
        '
        Me.pnlCurrentPath.Controls.Add(Me.lbCurrentUrl)
        Me.pnlCurrentPath.Controls.Add(Me.btnNavigateParentFolder)
        Me.pnlCurrentPath.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlCurrentPath.Location = New System.Drawing.Point(3, 16)
        Me.pnlCurrentPath.Name = "pnlCurrentPath"
        Me.pnlCurrentPath.Size = New System.Drawing.Size(518, 32)
        Me.pnlCurrentPath.TabIndex = 6
        '
        'btnNavigateParentFolder
        '
        Me.btnNavigateParentFolder.Location = New System.Drawing.Point(9, 4)
        Me.btnNavigateParentFolder.Name = "btnNavigateParentFolder"
        Me.btnNavigateParentFolder.Size = New System.Drawing.Size(91, 23)
        Me.btnNavigateParentFolder.TabIndex = 5
        Me.btnNavigateParentFolder.Text = "上一级目录"
        Me.btnNavigateParentFolder.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(1077, 573)
        Me.Controls.Add(Me.grpDownload)
        Me.Controls.Add(Me.grpFileExplorer)
        Me.Controls.Add(Me.pnlStatus)
        Me.Controls.Add(Me.pnlFTPServer)
        Me.Name = "MainForm"
        Me.Text = "VBFTPDownload"
        Me.pnlFTPServer.ResumeLayout(False)
        Me.pnlFTPServer.PerformLayout()
        Me.grpDownload.ResumeLayout(False)
        Me.grpDownload.PerformLayout()
        Me.pnlStatus.ResumeLayout(False)
        Me.grpLog.ResumeLayout(False)
        Me.grpFileExplorer.ResumeLayout(False)
        Me.pnlCurrentPath.ResumeLayout(False)
        Me.pnlCurrentPath.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private pnlFTPServer As Panel
    Private WithEvents btnConnect As Button
    Private tbFTPServer As TextBox
    Private lbFTPServer As Label
    Private grpDownload As GroupBox
    Private pnlStatus As Panel
    Private grpFileExplorer As GroupBox
    Private WithEvents lstFileExplorer As ListBox
    Private WithEvents btnDownload As Button
    Private WithEvents btnBrowseDownloadPath As Button
    Private tbDownloadPath As TextBox
    Private lbDownloadPath As Label
    Private lbCurrentUrl As Label
    Private pnlCurrentPath As Panel
    Private WithEvents btnNavigateParentFolder As Button
    Private grpLog As GroupBox
    Private lstLog As ListBox
End Class

