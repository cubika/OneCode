
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
        Me.grpUploadFolder = New System.Windows.Forms.GroupBox()
        Me.chkCreateFolder = New System.Windows.Forms.CheckBox()
        Me.btnUploadFolder = New System.Windows.Forms.Button()
        Me.btnBrowseLocalFolder = New System.Windows.Forms.Button()
        Me.tbLocalFolder = New System.Windows.Forms.TextBox()
        Me.lbLocalFolder = New System.Windows.Forms.Label()
        Me.pnlStatus = New System.Windows.Forms.Panel()
        Me.grpLog = New System.Windows.Forms.GroupBox()
        Me.lstLog = New System.Windows.Forms.ListBox()
        Me.grpFileExplorer = New System.Windows.Forms.GroupBox()
        Me.lstFileExplorer = New System.Windows.Forms.ListBox()
        Me.pnlCurrentPath = New System.Windows.Forms.Panel()
        Me.btnNavigateParentFolder = New System.Windows.Forms.Button()
        Me.grpUploadFile = New System.Windows.Forms.GroupBox()
        Me.btnUploadFile = New System.Windows.Forms.Button()
        Me.btnBrowseLocalFile = New System.Windows.Forms.Button()
        Me.tbLocalFile = New System.Windows.Forms.TextBox()
        Me.lbLocalFile = New System.Windows.Forms.Label()
        Me.groupBox1 = New System.Windows.Forms.GroupBox()
        Me.lbDelete = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.pnlFTPServer.SuspendLayout()
        Me.grpUploadFolder.SuspendLayout()
        Me.pnlStatus.SuspendLayout()
        Me.grpLog.SuspendLayout()
        Me.grpFileExplorer.SuspendLayout()
        Me.pnlCurrentPath.SuspendLayout()
        Me.grpUploadFile.SuspendLayout()
        Me.groupBox1.SuspendLayout()
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
        Me.tbFTPServer.Size = New System.Drawing.Size(355, 21)
        Me.tbFTPServer.TabIndex = 1
        Me.tbFTPServer.Text = "ftp://localhost"
        '
        'lbFTPServer
        '
        Me.lbFTPServer.AutoSize = True
        Me.lbFTPServer.Location = New System.Drawing.Point(13, 13)
        Me.lbFTPServer.Name = "lbFTPServer"
        Me.lbFTPServer.Size = New System.Drawing.Size(59, 12)
        Me.lbFTPServer.TabIndex = 0
        Me.lbFTPServer.Text = "FTP服务器"
        '
        'lbCurrentUrl
        '
        Me.lbCurrentUrl.AutoSize = True
        Me.lbCurrentUrl.Location = New System.Drawing.Point(106, 10)
        Me.lbCurrentUrl.Name = "lbCurrentUrl"
        Me.lbCurrentUrl.Size = New System.Drawing.Size(53, 12)
        Me.lbCurrentUrl.TabIndex = 4
        Me.lbCurrentUrl.Text = "当前路径"
        '
        'grpUploadFolder
        '
        Me.grpUploadFolder.Controls.Add(Me.chkCreateFolder)
        Me.grpUploadFolder.Controls.Add(Me.btnUploadFolder)
        Me.grpUploadFolder.Controls.Add(Me.btnBrowseLocalFolder)
        Me.grpUploadFolder.Controls.Add(Me.tbLocalFolder)
        Me.grpUploadFolder.Controls.Add(Me.lbLocalFolder)
        Me.grpUploadFolder.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpUploadFolder.Location = New System.Drawing.Point(524, 33)
        Me.grpUploadFolder.Name = "grpUploadFolder"
        Me.grpUploadFolder.Size = New System.Drawing.Size(553, 111)
        Me.grpUploadFolder.TabIndex = 0
        Me.grpUploadFolder.TabStop = False
        Me.grpUploadFolder.Text = "上传文件夹"
        '
        'chkCreateFolder
        '
        Me.chkCreateFolder.AutoSize = True
        Me.chkCreateFolder.Location = New System.Drawing.Point(91, 54)
        Me.chkCreateFolder.Name = "chkCreateFolder"
        Me.chkCreateFolder.Size = New System.Drawing.Size(186, 16)
        Me.chkCreateFolder.TabIndex = 4
        Me.chkCreateFolder.Text = "在FTP服务器上创建一个文件夹"
        Me.chkCreateFolder.UseVisualStyleBackColor = True
        '
        'btnUploadFolder
        '
        Me.btnUploadFolder.Enabled = False
        Me.btnUploadFolder.Location = New System.Drawing.Point(91, 77)
        Me.btnUploadFolder.Name = "btnUploadFolder"
        Me.btnUploadFolder.Size = New System.Drawing.Size(98, 23)
        Me.btnUploadFolder.TabIndex = 3
        Me.btnUploadFolder.Text = "上传文件夹"
        Me.btnUploadFolder.UseVisualStyleBackColor = True
        '
        'btnBrowseLocalFolder
        '
        Me.btnBrowseLocalFolder.Location = New System.Drawing.Point(427, 25)
        Me.btnBrowseLocalFolder.Name = "btnBrowseLocalFolder"
        Me.btnBrowseLocalFolder.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseLocalFolder.TabIndex = 2
        Me.btnBrowseLocalFolder.Text = "浏览"
        Me.btnBrowseLocalFolder.UseVisualStyleBackColor = True
        '
        'tbLocalFolder
        '
        Me.tbLocalFolder.Location = New System.Drawing.Point(91, 26)
        Me.tbLocalFolder.Name = "tbLocalFolder"
        Me.tbLocalFolder.ReadOnly = True
        Me.tbLocalFolder.Size = New System.Drawing.Size(329, 21)
        Me.tbLocalFolder.TabIndex = 1
        '
        'lbLocalFolder
        '
        Me.lbLocalFolder.AutoSize = True
        Me.lbLocalFolder.Location = New System.Drawing.Point(7, 30)
        Me.lbLocalFolder.Name = "lbLocalFolder"
        Me.lbLocalFolder.Size = New System.Drawing.Size(65, 12)
        Me.lbLocalFolder.TabIndex = 0
        Me.lbLocalFolder.Text = "本地文件夹"
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
        Me.grpLog.Text = "记录"
        '
        'lstLog
        '
        Me.lstLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstLog.FormattingEnabled = True
        Me.lstLog.ItemHeight = 12
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
        Me.grpFileExplorer.Text = "FTP文件管理器"
        '
        'lstFileExplorer
        '
        Me.lstFileExplorer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstFileExplorer.FormattingEnabled = True
        Me.lstFileExplorer.ItemHeight = 12
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
        Me.btnNavigateParentFolder.Text = "父文件夹"
        Me.btnNavigateParentFolder.UseVisualStyleBackColor = True
        '
        'grpUploadFile
        '
        Me.grpUploadFile.Controls.Add(Me.btnUploadFile)
        Me.grpUploadFile.Controls.Add(Me.btnBrowseLocalFile)
        Me.grpUploadFile.Controls.Add(Me.tbLocalFile)
        Me.grpUploadFile.Controls.Add(Me.lbLocalFile)
        Me.grpUploadFile.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpUploadFile.Location = New System.Drawing.Point(524, 144)
        Me.grpUploadFile.Name = "grpUploadFile"
        Me.grpUploadFile.Size = New System.Drawing.Size(553, 96)
        Me.grpUploadFile.TabIndex = 3
        Me.grpUploadFile.TabStop = False
        Me.grpUploadFile.Text = "上传文件"
        '
        'btnUploadFile
        '
        Me.btnUploadFile.Enabled = False
        Me.btnUploadFile.Location = New System.Drawing.Point(91, 52)
        Me.btnUploadFile.Name = "btnUploadFile"
        Me.btnUploadFile.Size = New System.Drawing.Size(98, 23)
        Me.btnUploadFile.TabIndex = 3
        Me.btnUploadFile.Text = "上传文件"
        Me.btnUploadFile.UseVisualStyleBackColor = True
        '
        'btnBrowseLocalFile
        '
        Me.btnBrowseLocalFile.Location = New System.Drawing.Point(427, 25)
        Me.btnBrowseLocalFile.Name = "btnBrowseLocalFile"
        Me.btnBrowseLocalFile.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseLocalFile.TabIndex = 2
        Me.btnBrowseLocalFile.Text = "浏览"
        Me.btnBrowseLocalFile.UseVisualStyleBackColor = True
        '
        'tbLocalFile
        '
        Me.tbLocalFile.Location = New System.Drawing.Point(91, 26)
        Me.tbLocalFile.Name = "tbLocalFile"
        Me.tbLocalFile.ReadOnly = True
        Me.tbLocalFile.Size = New System.Drawing.Size(329, 21)
        Me.tbLocalFile.TabIndex = 1
        '
        'lbLocalFile
        '
        Me.lbLocalFile.AutoSize = True
        Me.lbLocalFile.Location = New System.Drawing.Point(7, 30)
        Me.lbLocalFile.Name = "lbLocalFile"
        Me.lbLocalFile.Size = New System.Drawing.Size(53, 12)
        Me.lbLocalFile.TabIndex = 0
        Me.lbLocalFile.Text = "本地文件"
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.lbDelete)
        Me.groupBox1.Controls.Add(Me.btnDelete)
        Me.groupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.groupBox1.Location = New System.Drawing.Point(524, 240)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(553, 96)
        Me.groupBox1.TabIndex = 4
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "删除文件夹/文件"
        '
        'lbDelete
        '
        Me.lbDelete.AutoSize = True
        Me.lbDelete.Location = New System.Drawing.Point(88, 56)
        Me.lbDelete.Name = "lbDelete"
        Me.lbDelete.Size = New System.Drawing.Size(179, 12)
        Me.lbDelete.TabIndex = 4
        Me.lbDelete.Text = "删除在FTP文件管理器中的选中项"
        '
        'btnDelete
        '
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New System.Drawing.Point(91, 19)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(98, 23)
        Me.btnDelete.TabIndex = 3
        Me.btnDelete.Text = "删除"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(1077, 573)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.grpUploadFile)
        Me.Controls.Add(Me.grpUploadFolder)
        Me.Controls.Add(Me.grpFileExplorer)
        Me.Controls.Add(Me.pnlStatus)
        Me.Controls.Add(Me.pnlFTPServer)
        Me.Name = "MainForm"
        Me.Text = "VBFTPUpload"
        Me.pnlFTPServer.ResumeLayout(False)
        Me.pnlFTPServer.PerformLayout()
        Me.grpUploadFolder.ResumeLayout(False)
        Me.grpUploadFolder.PerformLayout()
        Me.pnlStatus.ResumeLayout(False)
        Me.grpLog.ResumeLayout(False)
        Me.grpFileExplorer.ResumeLayout(False)
        Me.pnlCurrentPath.ResumeLayout(False)
        Me.pnlCurrentPath.PerformLayout()
        Me.grpUploadFile.ResumeLayout(False)
        Me.grpUploadFile.PerformLayout()
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private pnlFTPServer As Panel
    Private WithEvents btnConnect As Button
    Private tbFTPServer As TextBox
    Private lbFTPServer As Label
    Private grpUploadFolder As GroupBox
    Private pnlStatus As Panel
    Private grpFileExplorer As GroupBox
    Private WithEvents lstFileExplorer As ListBox
    Private WithEvents btnUploadFolder As Button
    Private WithEvents btnBrowseLocalFolder As Button
    Private tbLocalFolder As TextBox
    Private lbLocalFolder As Label
    Private lbCurrentUrl As Label
    Private pnlCurrentPath As Panel
    Private WithEvents btnNavigateParentFolder As Button
    Private grpLog As GroupBox
    Private lstLog As ListBox
    Private chkCreateFolder As CheckBox
    Private grpUploadFile As GroupBox
    Private WithEvents btnUploadFile As Button
    Private WithEvents btnBrowseLocalFile As Button
    Private tbLocalFile As TextBox
    Private lbLocalFile As Label
    Private groupBox1 As GroupBox
    Private lbDelete As Label
    Private WithEvents btnDelete As Button
End Class

