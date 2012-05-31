
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

        If disposing AndAlso (documentManipulator IsNot Nothing) Then
            documentManipulator.Dispose()
        End If

        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.pnlBrowse = New System.Windows.Forms.Panel()
        Me.lbFileName = New System.Windows.Forms.Label()
        Me.btnOpenFile = New System.Windows.Forms.Button()
        Me.pnlImageList = New System.Windows.Forms.Panel()
        Me.lstImage = New System.Windows.Forms.ListBox()
        Me.pnlOperation = New System.Windows.Forms.Panel()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.btnReplace = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.picView = New System.Windows.Forms.PictureBox()
        Me.pnlBrowse.SuspendLayout()
        Me.pnlImageList.SuspendLayout()
        Me.pnlOperation.SuspendLayout()
        CType(Me.picView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlBrowse
        '
        Me.pnlBrowse.Controls.Add(Me.lbFileName)
        Me.pnlBrowse.Controls.Add(Me.btnOpenFile)
        Me.pnlBrowse.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlBrowse.Location = New System.Drawing.Point(0, 0)
        Me.pnlBrowse.Name = "pnlBrowse"
        Me.pnlBrowse.Size = New System.Drawing.Size(783, 37)
        Me.pnlBrowse.TabIndex = 0
        '
        'lbFileName
        '
        Me.lbFileName.AutoSize = True
        Me.lbFileName.Location = New System.Drawing.Point(143, 13)
        Me.lbFileName.Name = "lbFileName"
        Me.lbFileName.Size = New System.Drawing.Size(0, 13)
        Me.lbFileName.TabIndex = 2
        '
        'btnOpenFile
        '
        Me.btnOpenFile.Location = New System.Drawing.Point(12, 8)
        Me.btnOpenFile.Name = "btnOpenFile"
        Me.btnOpenFile.Size = New System.Drawing.Size(124, 23)
        Me.btnOpenFile.TabIndex = 1
        Me.btnOpenFile.Text = "打开Word文档"
        Me.btnOpenFile.UseVisualStyleBackColor = True
        '
        'pnlImageList
        '
        Me.pnlImageList.Controls.Add(Me.lstImage)
        Me.pnlImageList.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlImageList.Location = New System.Drawing.Point(0, 37)
        Me.pnlImageList.Name = "pnlImageList"
        Me.pnlImageList.Size = New System.Drawing.Size(269, 431)
        Me.pnlImageList.TabIndex = 1
        '
        'lstImage
        '
        Me.lstImage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstImage.FormattingEnabled = True
        Me.lstImage.Location = New System.Drawing.Point(0, 0)
        Me.lstImage.Name = "lstImage"
        Me.lstImage.Size = New System.Drawing.Size(269, 431)
        Me.lstImage.TabIndex = 0
        '
        'pnlOperation
        '
        Me.pnlOperation.Controls.Add(Me.btnExport)
        Me.pnlOperation.Controls.Add(Me.btnReplace)
        Me.pnlOperation.Controls.Add(Me.btnDelete)
        Me.pnlOperation.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlOperation.Location = New System.Drawing.Point(269, 37)
        Me.pnlOperation.Name = "pnlOperation"
        Me.pnlOperation.Size = New System.Drawing.Size(514, 35)
        Me.pnlOperation.TabIndex = 2
        '
        'btnExport
        '
        Me.btnExport.Location = New System.Drawing.Point(33, 5)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(75, 23)
        Me.btnExport.TabIndex = 2
        Me.btnExport.Text = "导出"
        Me.btnExport.UseVisualStyleBackColor = True
        '
        'btnReplace
        '
        Me.btnReplace.Location = New System.Drawing.Point(218, 5)
        Me.btnReplace.Name = "btnReplace"
        Me.btnReplace.Size = New System.Drawing.Size(75, 23)
        Me.btnReplace.TabIndex = 1
        Me.btnReplace.Text = "替换"
        Me.btnReplace.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(125, 5)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(75, 23)
        Me.btnDelete.TabIndex = 0
        Me.btnDelete.Text = "删除"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'picView
        '
        Me.picView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picView.Location = New System.Drawing.Point(269, 72)
        Me.picView.Name = "picView"
        Me.picView.Size = New System.Drawing.Size(514, 396)
        Me.picView.TabIndex = 3
        Me.picView.TabStop = False
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(783, 468)
        Me.Controls.Add(Me.picView)
        Me.Controls.Add(Me.pnlOperation)
        Me.Controls.Add(Me.pnlImageList)
        Me.Controls.Add(Me.pnlBrowse)
        Me.Name = "MainForm"
        Me.Text = "操作Word文档中的图片"
        Me.pnlBrowse.ResumeLayout(False)
        Me.pnlBrowse.PerformLayout()
        Me.pnlImageList.ResumeLayout(False)
        Me.pnlOperation.ResumeLayout(False)
        CType(Me.picView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private pnlBrowse As Panel
    Private lbFileName As Label
    Private WithEvents btnOpenFile As Button
    Private pnlImageList As Panel
    Private WithEvents lstImage As ListBox
    Private pnlOperation As Panel
    Private WithEvents btnReplace As Button
    Private WithEvents btnDelete As Button
    Private picView As PictureBox
    Private WithEvents btnExport As Button
End Class

