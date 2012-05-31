Partial Public Class RunProcessAsUser
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
        Me.btnCommand = New System.Windows.Forms.Button()
        Me.tbCommand = New System.Windows.Forms.TextBox()
        Me.tbPassword = New System.Windows.Forms.TextBox()
        Me.tbDomain = New System.Windows.Forms.TextBox()
        Me.lbPassword = New System.Windows.Forms.Label()
        Me.lbDomain = New System.Windows.Forms.Label()
        Me.lbUserName = New System.Windows.Forms.Label()
        Me.btnRunCommand = New System.Windows.Forms.Button()
        Me.btnCredentialUIPrompt = New System.Windows.Forms.Button()
        Me.tbUserName = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'btnCommand
        '
        Me.btnCommand.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnCommand.Location = New System.Drawing.Point(23, 118)
        Me.btnCommand.Name = "btnCommand"
        Me.btnCommand.Size = New System.Drawing.Size(64, 23)
        Me.btnCommand.TabIndex = 38
        Me.btnCommand.Text = "命令..."
        '
        'tbCommand
        '
        Me.tbCommand.Location = New System.Drawing.Point(87, 118)
        Me.tbCommand.Name = "tbCommand"
        Me.tbCommand.Size = New System.Drawing.Size(184, 20)
        Me.tbCommand.TabIndex = 39
        '
        'tbPassword
        '
        Me.tbPassword.Location = New System.Drawing.Point(87, 86)
        Me.tbPassword.Name = "tbPassword"
        Me.tbPassword.Size = New System.Drawing.Size(184, 20)
        Me.tbPassword.TabIndex = 37
        '
        'tbDomain
        '
        Me.tbDomain.Location = New System.Drawing.Point(87, 51)
        Me.tbDomain.Name = "tbDomain"
        Me.tbDomain.Size = New System.Drawing.Size(184, 20)
        Me.tbDomain.TabIndex = 36
        '
        'lbPassword
        '
        Me.lbPassword.Location = New System.Drawing.Point(23, 86)
        Me.lbPassword.Name = "lbPassword"
        Me.lbPassword.Size = New System.Drawing.Size(64, 23)
        Me.lbPassword.TabIndex = 43
        Me.lbPassword.Text = "密码"
        '
        'lbDomain
        '
        Me.lbDomain.Location = New System.Drawing.Point(23, 54)
        Me.lbDomain.Name = "lbDomain"
        Me.lbDomain.Size = New System.Drawing.Size(56, 23)
        Me.lbDomain.TabIndex = 42
        Me.lbDomain.Text = "域"
        '
        'lbUserName
        '
        Me.lbUserName.Location = New System.Drawing.Point(23, 22)
        Me.lbUserName.Name = "lbUserName"
        Me.lbUserName.Size = New System.Drawing.Size(64, 23)
        Me.lbUserName.TabIndex = 40
        Me.lbUserName.Text = "用户名"
        '
        'btnRunCommand
        '
        Me.btnRunCommand.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnRunCommand.Location = New System.Drawing.Point(87, 144)
        Me.btnRunCommand.Name = "btnRunCommand"
        Me.btnRunCommand.Size = New System.Drawing.Size(128, 24)
        Me.btnRunCommand.TabIndex = 41
        Me.btnRunCommand.Text = "运行命令"
        '
        'btnCredentialUIPrompt
        '
        Me.btnCredentialUIPrompt.Location = New System.Drawing.Point(277, 17)
        Me.btnCredentialUIPrompt.Name = "btnCredentialUIPrompt"
        Me.btnCredentialUIPrompt.Size = New System.Drawing.Size(39, 23)
        Me.btnCredentialUIPrompt.TabIndex = 44
        Me.btnCredentialUIPrompt.Text = "..."
        Me.btnCredentialUIPrompt.UseVisualStyleBackColor = True
        '
        'tbUserName
        '
        Me.tbUserName.Location = New System.Drawing.Point(87, 19)
        Me.tbUserName.Name = "tbUserName"
        Me.tbUserName.Size = New System.Drawing.Size(184, 20)
        Me.tbUserName.TabIndex = 47
        '
        'RunProcessAsUser
        '
        Me.ClientSize = New System.Drawing.Size(330, 198)
        Me.Controls.Add(Me.tbUserName)
        Me.Controls.Add(Me.btnCredentialUIPrompt)
        Me.Controls.Add(Me.btnCommand)
        Me.Controls.Add(Me.tbCommand)
        Me.Controls.Add(Me.tbPassword)
        Me.Controls.Add(Me.tbDomain)
        Me.Controls.Add(Me.lbPassword)
        Me.Controls.Add(Me.lbDomain)
        Me.Controls.Add(Me.lbUserName)
        Me.Controls.Add(Me.btnRunCommand)
        Me.Name = "RunProcessAsUser"
        Me.Text = "RunProcessAsUser"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private WithEvents btnCommand As Button
    Private tbCommand As TextBox
    Private tbPassword As TextBox
    Private tbDomain As TextBox
    Private lbPassword As Label
    Private lbDomain As Label
    Private lbUserName As Label
    Private WithEvents btnRunCommand As Button
    Private WithEvents btnCredentialUIPrompt As Button
    Private tbUserName As TextBox
End Class


