'INSTANT C# NOTE: Formerly VB project-level imports:
Imports System.Collections
Imports DynamicCondition

<CompilerServices.DesignerGenerated()>
 Partial Public Class MainForm
    Inherits Form

    'Form overrides dispose to clean up the component list.
    <DebuggerNonUserCode()>
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.dgResult = New System.Windows.Forms.DataGridView()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.lbTip = New System.Windows.Forms.Label()
        Me.ConditionBuilder1 = New DynamicCondition.ConditionBuilder()
        CType(Me.dgResult, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgResult
        '
        Me.dgResult.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgResult.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.dgResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgResult.DefaultCellStyle = DataGridViewCellStyle5
        Me.dgResult.Location = New System.Drawing.Point(0, 152)
        Me.dgResult.Name = "dgResult"
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgResult.RowHeadersDefaultCellStyle = DataGridViewCellStyle6
        Me.dgResult.Size = New System.Drawing.Size(729, 598)
        Me.dgResult.TabIndex = 0
        '
        'btnSearch
        '
        Me.btnSearch.Location = New System.Drawing.Point(426, 123)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(75, 23)
        Me.btnSearch.TabIndex = 2
        Me.btnSearch.Text = "搜索"
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'lbTip
        '
        Me.lbTip.AutoSize = True
        Me.lbTip.Location = New System.Drawing.Point(12, 128)
        Me.lbTip.Name = "lbTip"
        Me.lbTip.Size = New System.Drawing.Size(245, 12)
        Me.lbTip.TabIndex = 3
        Me.lbTip.Text = "点击""搜索""，以便在下面的网格中显示结果"
        '
        'ConditionBuilder1
        '
        Me.ConditionBuilder1.AutoScroll = True
        Me.ConditionBuilder1.Lines = 3
        Me.ConditionBuilder1.Location = New System.Drawing.Point(0, 12)
        Me.ConditionBuilder1.Name = "ConditionBuilder1"
        Me.ConditionBuilder1.OperatorType = DynamicCondition.ConditionBuilder.Compare.[And]
        Me.ConditionBuilder1.Size = New System.Drawing.Size(501, 105)
        Me.ConditionBuilder1.TabIndex = 1
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(729, 750)
        Me.Controls.Add(Me.lbTip)
        Me.Controls.Add(Me.btnSearch)
        Me.Controls.Add(Me.ConditionBuilder1)
        Me.Controls.Add(Me.dgResult)
        Me.Name = "MainForm"
        Me.Text = "VBDynamicallyBuildLambdaExpressionWithField"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.dgResult, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend dgResult As DataGridView
    Friend ConditionBuilder1 As DynamicCondition.ConditionBuilder
    Friend WithEvents btnSearch As Button
    Friend lbTip As Label

End Class

