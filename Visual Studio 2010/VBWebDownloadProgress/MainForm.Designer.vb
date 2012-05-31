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
        Me.prgDownload = New System.Windows.Forms.ProgressBar()
        Me.lbUrl = New System.Windows.Forms.Label()
        Me.tbURL = New System.Windows.Forms.TextBox()
        Me.btnDownload = New System.Windows.Forms.Button()
        Me.lbPath = New System.Windows.Forms.Label()
        Me.tbPath = New System.Windows.Forms.TextBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.btnPause = New System.Windows.Forms.Button()
        Me.lbSummary = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'prgDownload
        '
        Me.prgDownload.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.prgDownload.Location = New System.Drawing.Point(0, 147)
        Me.prgDownload.Name = "prgDownload"
        Me.prgDownload.Size = New System.Drawing.Size(704, 23)
        Me.prgDownload.TabIndex = 0
        '
        'lbUrl
        '
        Me.lbUrl.AutoSize = True
        Me.lbUrl.Location = New System.Drawing.Point(12, 40)
        Me.lbUrl.Name = "lbUrl"
        Me.lbUrl.Size = New System.Drawing.Size(31, 13)
        Me.lbUrl.TabIndex = 1
        Me.lbUrl.Text = "网址"
        '
        'tbURL
        '
        Me.tbURL.Location = New System.Drawing.Point(75, 36)
        Me.tbURL.Name = "tbURL"
        Me.tbURL.Size = New System.Drawing.Size(623, 20)
        Me.tbURL.TabIndex = 2
        Me.tbURL.Text = "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE" & _
            "/dotNetFx40_Full_x86_x64.exe"
        '
        'btnDownload
        '
        Me.btnDownload.Location = New System.Drawing.Point(449, 90)
        Me.btnDownload.Name = "btnDownload"
        Me.btnDownload.Size = New System.Drawing.Size(75, 23)
        Me.btnDownload.TabIndex = 3
        Me.btnDownload.Text = "下载"
        Me.btnDownload.UseVisualStyleBackColor = True
        '
        'lbPath
        '
        Me.lbPath.AutoSize = True
        Me.lbPath.Location = New System.Drawing.Point(12, 67)
        Me.lbPath.Name = "lbPath"
        Me.lbPath.Size = New System.Drawing.Size(55, 13)
        Me.lbPath.TabIndex = 1
        Me.lbPath.Text = "本地路径"
        '
        'tbPath
        '
        Me.tbPath.Location = New System.Drawing.Point(76, 63)
        Me.tbPath.Name = "tbPath"
        Me.tbPath.Size = New System.Drawing.Size(622, 20)
        Me.tbPath.TabIndex = 2
        Me.tbPath.Text = "D:\DotNetFx4.exe"
        '
        'btnCancel
        '
        Me.btnCancel.Enabled = False
        Me.btnCancel.Location = New System.Drawing.Point(623, 90)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "取消"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(12, 131)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(0, 13)
        Me.lbStatus.TabIndex = 1
        '
        'btnPause
        '
        Me.btnPause.Enabled = False
        Me.btnPause.Location = New System.Drawing.Point(538, 90)
        Me.btnPause.Name = "btnPause"
        Me.btnPause.Size = New System.Drawing.Size(75, 23)
        Me.btnPause.TabIndex = 4
        Me.btnPause.Text = "暂停"
        Me.btnPause.UseVisualStyleBackColor = True
        '
        'lbSummary
        '
        Me.lbSummary.AutoSize = True
        Me.lbSummary.Location = New System.Drawing.Point(114, 131)
        Me.lbSummary.Name = "lbSummary"
        Me.lbSummary.Size = New System.Drawing.Size(0, 13)
        Me.lbSummary.TabIndex = 1
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(704, 170)
        Me.Controls.Add(Me.btnPause)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnDownload)
        Me.Controls.Add(Me.tbPath)
        Me.Controls.Add(Me.tbURL)
        Me.Controls.Add(Me.lbSummary)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.lbPath)
        Me.Controls.Add(Me.lbUrl)
        Me.Controls.Add(Me.prgDownload)
        Me.Name = "MainForm"
        Me.Text = "网上下载进度应用程序"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private prgDownload As ProgressBar
    Private lbUrl As Label
    Private tbURL As TextBox
    Private WithEvents btnDownload As Button
    Private lbPath As Label
    Private tbPath As TextBox
    Private WithEvents btnCancel As Button
    Private lbStatus As Label
    Private WithEvents btnPause As Button
    Private lbSummary As Label
End Class


