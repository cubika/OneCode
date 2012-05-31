Partial Public Class Setting
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
        Me.lbTimerInternal = New System.Windows.Forms.Label()
        Me.dtpInternal = New System.Windows.Forms.NumericUpDown()
        Me.lbMilliseconds = New System.Windows.Forms.Label()
        Me.btnConfirm = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        CType(Me.dtpInternal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lbTimerInternal
        '
        Me.lbTimerInternal.AutoSize = True
        Me.lbTimerInternal.Location = New System.Drawing.Point(29, 22)
        Me.lbTimerInternal.Name = "lbTimerInternal"
        Me.lbTimerInternal.Size = New System.Drawing.Size(61, 13)
        Me.lbTimerInternal.TabIndex = 0
        Me.lbTimerInternal.Text = "时间间隔："
        '
        'dtpInternal
        '
        Me.dtpInternal.Location = New System.Drawing.Point(112, 20)
        Me.dtpInternal.Maximum = New Decimal(New Integer() {10000000, 0, 0, 0})
        Me.dtpInternal.Name = "dtpInternal"
        Me.dtpInternal.Size = New System.Drawing.Size(120, 20)
        Me.dtpInternal.TabIndex = 1
        '
        'lbMilliseconds
        '
        Me.lbMilliseconds.AutoSize = True
        Me.lbMilliseconds.Location = New System.Drawing.Point(248, 22)
        Me.lbMilliseconds.Name = "lbMilliseconds"
        Me.lbMilliseconds.Size = New System.Drawing.Size(31, 13)
        Me.lbMilliseconds.TabIndex = 2
        Me.lbMilliseconds.Text = "毫秒"
        '
        'btnConfirm
        '
        Me.btnConfirm.Location = New System.Drawing.Point(75, 54)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(75, 23)
        Me.btnConfirm.TabIndex = 3
        Me.btnConfirm.Text = "确认"
        Me.btnConfirm.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(177, 54)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "取消"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'Setting
        '
        Me.ClientSize = New System.Drawing.Size(343, 89)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.lbMilliseconds)
        Me.Controls.Add(Me.dtpInternal)
        Me.Controls.Add(Me.lbTimerInternal)
        Me.Name = "Setting"
        Me.Text = "设置"
        CType(Me.dtpInternal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private lbTimerInternal As Label
    Private dtpInternal As NumericUpDown
    Private lbMilliseconds As Label
    Private WithEvents btnConfirm As Button
    Private WithEvents btnCancel As Button
End Class
