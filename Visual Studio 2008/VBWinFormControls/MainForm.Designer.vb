<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.roundButton1 = New VBWinFormControls.RoundButton
        Me.label6 = New System.Windows.Forms.Label
        Me.label5 = New System.Windows.Forms.Label
        Me.textBox1 = New System.Windows.Forms.TextBox
        Me.label4 = New System.Windows.Forms.Label
        Me.listBox1 = New System.Windows.Forms.ListBox
        Me.label3 = New System.Windows.Forms.Label
        Me.label2 = New System.Windows.Forms.Label
        Me.comboBox1 = New System.Windows.Forms.ComboBox
        Me.label1 = New System.Windows.Forms.Label
        Me.toolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.groupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.roundButton1)
        Me.groupBox1.Controls.Add(Me.label6)
        Me.groupBox1.Controls.Add(Me.label5)
        Me.groupBox1.Controls.Add(Me.textBox1)
        Me.groupBox1.Controls.Add(Me.label4)
        Me.groupBox1.Controls.Add(Me.listBox1)
        Me.groupBox1.Controls.Add(Me.label3)
        Me.groupBox1.Controls.Add(Me.label2)
        Me.groupBox1.Controls.Add(Me.comboBox1)
        Me.groupBox1.Controls.Add(Me.label1)
        Me.groupBox1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.groupBox1.Location = New System.Drawing.Point(22, 25)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(700, 429)
        Me.groupBox1.TabIndex = 10
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "自定义控件"
        '
        'roundButton1
        '
        Me.roundButton1.BackColor = System.Drawing.Color.Green
        Me.roundButton1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.roundButton1.Location = New System.Drawing.Point(384, 283)
        Me.roundButton1.Name = "roundButton1"
        Me.roundButton1.Size = New System.Drawing.Size(142, 68)
        Me.roundButton1.TabIndex = 10
        Me.roundButton1.Text = "我是一个椭圆形按钮，点击我!"
        Me.roundButton1.UseVisualStyleBackColor = False
        '
        'label6
        '
        Me.label6.Location = New System.Drawing.Point(370, 212)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(281, 61)
        Me.label6.TabIndex = 9
        Me.label6.Text = "这个例子展示了一个椭圆形按钮，只有单击在椭圆内才能触发Click事件"
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Location = New System.Drawing.Point(370, 182)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(138, 16)
        Me.label5.TabIndex = 7
        Me.label5.Text = "4. 一个椭圆形按钮"
        '
        'textBox1
        '
        Me.textBox1.Location = New System.Drawing.Point(373, 107)
        Me.textBox1.Name = "textBox1"
        Me.textBox1.Size = New System.Drawing.Size(267, 23)
        Me.textBox1.TabIndex = 6
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(370, 76)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(202, 16)
        Me.label4.TabIndex = 5
        Me.label4.Text = "3. 只允许输入数字的文本框"
        '
        'listBox1
        '
        Me.listBox1.FormattingEnabled = True
        Me.listBox1.ItemHeight = 16
        Me.listBox1.Location = New System.Drawing.Point(25, 214)
        Me.listBox1.Name = "listBox1"
        Me.listBox1.Size = New System.Drawing.Size(235, 164)
        Me.listBox1.TabIndex = 4
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(22, 182)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(266, 16)
        Me.label3.TabIndex = 3
        Me.label3.Text = "2. 每个列表项拥有不同提示的列表框"
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(22, 76)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(122, 16)
        Me.label2.TabIndex = 2
        Me.label2.Text = "1. 多列的组合框"
        '
        'comboBox1
        '
        Me.comboBox1.FormattingEnabled = True
        Me.comboBox1.Location = New System.Drawing.Point(25, 107)
        Me.comboBox1.Name = "comboBox1"
        Me.comboBox1.Size = New System.Drawing.Size(235, 24)
        Me.comboBox1.TabIndex = 1
        '
        'label1
        '
        Me.label1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label1.Location = New System.Drawing.Point(17, 19)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(676, 46)
        Me.label1.TabIndex = 0
        Me.label1.Text = "" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10) & "本示例展示了怎样自定义Windows Forms 控件"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(745, 478)
        Me.Controls.Add(Me.groupBox1)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "VBWinFormControls"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents label6 As System.Windows.Forms.Label
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents textBox1 As System.Windows.Forms.TextBox
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents listBox1 As System.Windows.Forms.ListBox
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents comboBox1 As System.Windows.Forms.ComboBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents toolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents roundButton1 As VBWinFormControls.RoundButton

End Class
