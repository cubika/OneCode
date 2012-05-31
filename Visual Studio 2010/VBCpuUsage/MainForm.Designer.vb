
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

        If disposing AndAlso (_processCpuUsageMonitor IsNot Nothing) Then
            _processCpuUsageMonitor.Dispose()
        End If

        If disposing AndAlso (_totalCpuUsageMonitor IsNot Nothing) Then
            _totalCpuUsageMonitor.Dispose()
        End If


        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim ChartArea2 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Title2 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Me.pnlCounter = New System.Windows.Forms.Panel()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.lbErrorMessage = New System.Windows.Forms.Label()
        Me.cmbProcess = New System.Windows.Forms.ComboBox()
        Me.chkProcessCpuUsage = New System.Windows.Forms.CheckBox()
        Me.chkTotalUsage = New System.Windows.Forms.CheckBox()
        Me.pnlChart = New System.Windows.Forms.Panel()
        Me.chartProcessCupUsage = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.chartTotalCpuUsage = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.pnlCounter.SuspendLayout()
        Me.pnlChart.SuspendLayout()
        CType(Me.chartProcessCupUsage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.chartTotalCpuUsage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlCounter
        '
        Me.pnlCounter.Controls.Add(Me.btnStop)
        Me.pnlCounter.Controls.Add(Me.btnStart)
        Me.pnlCounter.Controls.Add(Me.lbErrorMessage)
        Me.pnlCounter.Controls.Add(Me.cmbProcess)
        Me.pnlCounter.Controls.Add(Me.chkProcessCpuUsage)
        Me.pnlCounter.Controls.Add(Me.chkTotalUsage)
        Me.pnlCounter.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlCounter.Location = New System.Drawing.Point(0, 0)
        Me.pnlCounter.Name = "pnlCounter"
        Me.pnlCounter.Size = New System.Drawing.Size(834, 57)
        Me.pnlCounter.TabIndex = 0
        '
        'btnStop
        '
        Me.btnStop.Enabled = False
        Me.btnStop.Location = New System.Drawing.Point(108, 27)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(75, 23)
        Me.btnStop.TabIndex = 3
        Me.btnStop.Text = "停止"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(12, 27)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(75, 23)
        Me.btnStart.TabIndex = 3
        Me.btnStart.Text = "开始"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'lbErrorMessage
        '
        Me.lbErrorMessage.AutoSize = True
        Me.lbErrorMessage.Location = New System.Drawing.Point(4, 76)
        Me.lbErrorMessage.Name = "lbErrorMessage"
        Me.lbErrorMessage.Size = New System.Drawing.Size(0, 12)
        Me.lbErrorMessage.TabIndex = 2
        '
        'cmbProcess
        '
        Me.cmbProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProcess.FormattingEnabled = True
        Me.cmbProcess.Location = New System.Drawing.Point(379, 3)
        Me.cmbProcess.Name = "cmbProcess"
        Me.cmbProcess.Size = New System.Drawing.Size(201, 20)
        Me.cmbProcess.TabIndex = 1
        '
        'chkProcessCpuUsage
        '
        Me.chkProcessCpuUsage.AutoSize = True
        Me.chkProcessCpuUsage.Location = New System.Drawing.Point(175, 4)
        Me.chkProcessCpuUsage.Name = "chkProcessCpuUsage"
        Me.chkProcessCpuUsage.Size = New System.Drawing.Size(162, 16)
        Me.chkProcessCpuUsage.TabIndex = 0
        Me.chkProcessCpuUsage.Text = "显示某一进程的CPU使用率"
        Me.chkProcessCpuUsage.UseVisualStyleBackColor = True
        '
        'chkTotalUsage
        '
        Me.chkTotalUsage.AutoSize = True
        Me.chkTotalUsage.Checked = True
        Me.chkTotalUsage.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkTotalUsage.Location = New System.Drawing.Point(4, 4)
        Me.chkTotalUsage.Name = "chkTotalUsage"
        Me.chkTotalUsage.Size = New System.Drawing.Size(114, 16)
        Me.chkTotalUsage.TabIndex = 0
        Me.chkTotalUsage.Text = "显示总CPU使用率"
        Me.chkTotalUsage.UseVisualStyleBackColor = True
        '
        'pnlChart
        '
        Me.pnlChart.Controls.Add(Me.chartProcessCupUsage)
        Me.pnlChart.Controls.Add(Me.chartTotalCpuUsage)
        Me.pnlChart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlChart.Location = New System.Drawing.Point(0, 57)
        Me.pnlChart.Name = "pnlChart"
        Me.pnlChart.Size = New System.Drawing.Size(834, 533)
        Me.pnlChart.TabIndex = 1
        '
        'chartProcessCupUsage
        '
        ChartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number
        ChartArea1.AxisX.LabelStyle.Enabled = False
        ChartArea1.AxisX.MajorGrid.Enabled = False
        ChartArea1.AxisX.MajorTickMark.Enabled = False
        ChartArea1.AxisX.Maximum = 100.0R
        ChartArea1.AxisX.Minimum = 0.0R
        ChartArea1.AxisY.IsMarginVisible = False
        ChartArea1.Name = "ChartAreaProcessCpuUsage"
        Me.chartProcessCupUsage.ChartAreas.Add(ChartArea1)
        Me.chartProcessCupUsage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chartProcessCupUsage.Location = New System.Drawing.Point(0, 279)
        Me.chartProcessCupUsage.Name = "chartProcessCupUsage"
        Series1.ChartArea = "ChartAreaProcessCpuUsage"
        Series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series1.Name = "ProcessCpuUsageSeries"
        Series1.ToolTip = "TotalCpuUsageSeries"
        Series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32
        Series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.[Double]
        Me.chartProcessCupUsage.Series.Add(Series1)
        Me.chartProcessCupUsage.Size = New System.Drawing.Size(834, 254)
        Me.chartProcessCupUsage.SuppressExceptions = True
        Me.chartProcessCupUsage.TabIndex = 1
        Me.chartProcessCupUsage.Text = "某一进程的CPU使用率"
        Title1.Name = "TitleProcess"
        Title1.Text = "某一进程CPU使用率"
        Me.chartProcessCupUsage.Titles.Add(Title1)
        '
        'chartTotalCpuUsage
        '
        ChartArea2.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number
        ChartArea2.AxisX.LabelStyle.Enabled = False
        ChartArea2.AxisX.MajorGrid.Enabled = False
        ChartArea2.AxisX.MajorTickMark.Enabled = False
        ChartArea2.AxisX.Maximum = 100.0R
        ChartArea2.AxisX.Minimum = 0.0R
        ChartArea2.AxisY.IsMarginVisible = False
        ChartArea2.Name = "ChartAreaTotalCpuUsage"
        Me.chartTotalCpuUsage.ChartAreas.Add(ChartArea2)
        Me.chartTotalCpuUsage.Dock = System.Windows.Forms.DockStyle.Top
        Me.chartTotalCpuUsage.Location = New System.Drawing.Point(0, 0)
        Me.chartTotalCpuUsage.Name = "chartTotalCpuUsage"
        Series2.ChartArea = "ChartAreaTotalCpuUsage"
        Series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series2.Name = "TotalCpuUsageSeries"
        Series2.ToolTip = "TotalCpuUsageSeries"
        Series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32
        Series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.[Double]
        Me.chartTotalCpuUsage.Series.Add(Series2)
        Me.chartTotalCpuUsage.Size = New System.Drawing.Size(834, 279)
        Me.chartTotalCpuUsage.SuppressExceptions = True
        Me.chartTotalCpuUsage.TabIndex = 0
        Me.chartTotalCpuUsage.Text = "总CPU使用率"
        Title2.Name = "TitleTotal"
        Title2.Text = "总CPU使用率"
        Me.chartTotalCpuUsage.Titles.Add(Title2)
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(834, 590)
        Me.Controls.Add(Me.pnlChart)
        Me.Controls.Add(Me.pnlCounter)
        Me.Name = "MainForm"
        Me.Text = "VBCpu使用率监视器"
        Me.pnlCounter.ResumeLayout(False)
        Me.pnlCounter.PerformLayout()
        Me.pnlChart.ResumeLayout(False)
        CType(Me.chartProcessCupUsage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.chartTotalCpuUsage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private pnlCounter As Panel
    Private lbErrorMessage As Label
    Private WithEvents cmbProcess As ComboBox
    Private chkProcessCpuUsage As CheckBox
    Private chkTotalUsage As CheckBox
    Private pnlChart As Panel
    Private chartTotalCpuUsage As DataVisualization.Charting.Chart
    Private WithEvents btnStart As Button
    Private chartProcessCupUsage As DataVisualization.Charting.Chart
    Private WithEvents btnStop As Button
End Class

