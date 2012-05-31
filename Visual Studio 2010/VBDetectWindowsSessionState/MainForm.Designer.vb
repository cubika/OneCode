Imports System.Security.Permissions
Partial Public Class MainForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso _session IsNot Nothing Then
            _session.Dispose()
        End If

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
        Me.panel1 = New System.Windows.Forms.Panel()
        Me.lbState = New System.Windows.Forms.Label()
        Me.panel2 = New System.Windows.Forms.Panel()
        Me.lstRecord = New System.Windows.Forms.ListBox()
        Me.chkEnableTimer = New System.Windows.Forms.CheckBox()
        Me.panel1.SuspendLayout()
        Me.panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'panel1
        '
        Me.panel1.Controls.Add(Me.chkEnableTimer)
        Me.panel1.Controls.Add(Me.lbState)
        Me.panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.panel1.Location = New System.Drawing.Point(0, 0)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(717, 29)
        Me.panel1.TabIndex = 0
        '
        'lbState
        '
        Me.lbState.AutoSize = True
        Me.lbState.Location = New System.Drawing.Point(13, 8)
        Me.lbState.Name = "lbState"
        Me.lbState.Size = New System.Drawing.Size(69, 13)
        Me.lbState.TabIndex = 0
        Me.lbState.Text = "当前状态"
        '
        'panel2
        '
        Me.panel2.Controls.Add(Me.lstRecord)
        Me.panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panel2.Location = New System.Drawing.Point(0, 29)
        Me.panel2.Name = "panel2"
        Me.panel2.Size = New System.Drawing.Size(717, 149)
        Me.panel2.TabIndex = 1
        '
        'lstRecord
        '
        Me.lstRecord.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstRecord.FormattingEnabled = True
        Me.lstRecord.Location = New System.Drawing.Point(0, 0)
        Me.lstRecord.Name = "lstRecord"
        Me.lstRecord.Size = New System.Drawing.Size(717, 149)
        Me.lstRecord.TabIndex = 0
        '
        'chkEnableTimer
        '
        Me.chkEnableTimer.AutoSize = True
        Me.chkEnableTimer.Location = New System.Drawing.Point(410, 6)
        Me.chkEnableTimer.Name = "chkEnableTimer"
        Me.chkEnableTimer.Size = New System.Drawing.Size(304, 17)
        Me.chkEnableTimer.TabIndex = 2
        Me.chkEnableTimer.Text = "启动定时器每5秒检测会话状态"
        Me.chkEnableTimer.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(717, 178)
        Me.Controls.Add(Me.panel2)
        Me.Controls.Add(Me.panel1)
        Me.Name = "MainForm"
        Me.Text = "检测Windows会话状态"
        Me.panel1.ResumeLayout(False)
        Me.panel1.PerformLayout()
        Me.panel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private panel1 As Panel
    Private lbState As Label
    Private panel2 As Panel
    Private lstRecord As ListBox
    Private WithEvents chkEnableTimer As System.Windows.Forms.CheckBox
End Class

