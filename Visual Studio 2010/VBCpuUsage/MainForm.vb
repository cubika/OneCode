'****************************** 模块头 *******************************'
' 模块名:  MainForm.vb
' 项目名:  VBCpuUsage
' Copyright (c) Microsoft Corporation.
' 
' 它是该应用程序的主要窗体，用于处理用户界面的事件和显示CPU使用率的图表. 
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************'


Partial Public Class MainForm
    Inherits Form

    ' 总CPU使用率的监视器.
    Private _totalCpuUsageMonitor As TotalCpuUsageMonitor

    ' 某一进程的CPU使用率监视器.
    Private _processCpuUsageMonitor As ProcessCpuUsageMonitor

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    '''  增加可用进程到复合框.
    ''' </summary>
    Private Sub cmbProcess_DropDown(ByVal sender As Object, ByVal e As EventArgs) _
        Handles cmbProcess.DropDown

        cmbProcess.DataSource = ProcessCpuUsageMonitor.GetAvailableProcesses()
        cmbProcess.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' 处理btnStart的Click事件.
    ''' </summary>
    Private Sub btnStart_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnStart.Click

        If (Not chkTotalUsage.Checked) AndAlso (Not chkProcessCpuUsage.Checked) Then
            Return
        End If

        Try
            StartMonitor()
        Catch ex As Exception
            StopMonitor()
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' 处理btnStop的Click事件.
    ''' 点击该按钮，会销毁totalCpuUsageMonitor 和 processCpuUsageMonitor.
    ''' </summary>
    Private Sub btnStop_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnStop.Click

        StopMonitor()
    End Sub

    Private Sub StartMonitor()
        '  初始化 totalCpuUsageMonitor，并登记CpuUsageValueArrayChanged事件. 
        If chkTotalUsage.Checked Then
            _totalCpuUsageMonitor = New TotalCpuUsageMonitor(1000, 100)
            AddHandler _totalCpuUsageMonitor.CpuUsageValueArrayChanged,
                AddressOf totalCpuUsageMonitor_CpuUsageValueArrayChanged
            AddHandler _totalCpuUsageMonitor.ErrorOccurred,
                AddressOf totalCpuUsageMonitor_ErrorOccurred

        End If

        ' 初始化processCpuUsageMonitor，并登记 CpuUsageValueArrayChanged事件.
        If chkProcessCpuUsage.Checked AndAlso _
            (Not String.IsNullOrEmpty(TryCast(cmbProcess.SelectedItem, String))) Then

            _processCpuUsageMonitor =
                New ProcessCpuUsageMonitor(TryCast(cmbProcess.SelectedItem, String), 1000, 100)
            AddHandler _processCpuUsageMonitor.CpuUsageValueArrayChanged,
                AddressOf processCpuUsageMonitor_CpuUsageValueArrayChanged

            AddHandler _processCpuUsageMonitor.ErrorOccurred,
                AddressOf processCpuUsageMonitor_ErrorOccurred
        End If

        ' 更新用户界面.
        btnStart.Enabled = False
        btnStop.Enabled = True
    End Sub

    Private Sub StopMonitor()
        If _totalCpuUsageMonitor IsNot Nothing Then
            _totalCpuUsageMonitor.Dispose()
            _totalCpuUsageMonitor = Nothing
        End If

        If _processCpuUsageMonitor IsNot Nothing Then
            _processCpuUsageMonitor.Dispose()
            _processCpuUsageMonitor = Nothing
        End If

        ' 更新用户界面.
        btnStart.Enabled = True
        btnStop.Enabled = False
    End Sub

    ''' <summary>
    ''' 激发processCpuUsageMonitor_CpuUsageValueArrayChangedHandler来处理
    ''' processCpuUsageMonitor的CpuUsageValueArrayChanged事件.
    ''' </summary>
    Private Sub processCpuUsageMonitor_CpuUsageValueArrayChanged(ByVal sender As Object,
                                                                 ByVal e As CpuUsageValueArrayChangedEventArg)
        Me.Invoke(New EventHandler(Of CpuUsageValueArrayChangedEventArg)(
                  AddressOf processCpuUsageMonitor_CpuUsageValueArrayChangedHandler), sender, e)
    End Sub

    Private Sub processCpuUsageMonitor_CpuUsageValueArrayChangedHandler(ByVal sender As Object,
                                                                        ByVal e As CpuUsageValueArrayChangedEventArg)
        Dim processCpuUsageSeries = chartProcessCupUsage.Series("ProcessCpuUsageSeries")
        Dim values = e.Values

        ' 在图表中显示进程的CPU使用率.
        processCpuUsageSeries.Points.DataBindY(e.Values)

    End Sub

    ''' <summary>
    ''' 激发processCpuUsageMonitor_ErrorOccurredHandler来处理
    ''' processCpuUsageMonitor的ErrorOccurred事件.
    ''' </summary>
    Private Sub processCpuUsageMonitor_ErrorOccurred(ByVal sender As Object,
                                                     ByVal e As ErrorEventArgs)
        Me.Invoke(New EventHandler(Of ErrorEventArgs)(
                  AddressOf processCpuUsageMonitor_ErrorOccurredHandler), sender, e)
    End Sub

    Private Sub processCpuUsageMonitor_ErrorOccurredHandler(ByVal sender As Object,
                                                            ByVal e As ErrorEventArgs)
        If _processCpuUsageMonitor IsNot Nothing Then
            _processCpuUsageMonitor.Dispose()
            _processCpuUsageMonitor = Nothing

            Dim processCpuUsageSeries = chartProcessCupUsage.Series("ProcessCpuUsageSeries")
            processCpuUsageSeries.Points.Clear()
        End If
        MessageBox.Show(e.Error.Message)
    End Sub

    ''' <summary>
    ''' 激发totalCpuUsageMonitor_CpuUsageValueArrayChangedHandler来处理
    ''' processCpuUsageMonitor的CpuUsageValueArrayChanged事件.
    ''' </summary>
    Private Sub totalCpuUsageMonitor_CpuUsageValueArrayChanged(ByVal sender As Object,
                                                               ByVal e As CpuUsageValueArrayChangedEventArg)
        Me.Invoke(New EventHandler(Of CpuUsageValueArrayChangedEventArg)(
                  AddressOf totalCpuUsageMonitor_CpuUsageValueArrayChangedHandler), sender, e)
    End Sub
    Private Sub totalCpuUsageMonitor_CpuUsageValueArrayChangedHandler(ByVal sender As Object,
                                                                      ByVal e As CpuUsageValueArrayChangedEventArg)
        Dim totalCpuUsageSeries = chartTotalCpuUsage.Series("TotalCpuUsageSeries")
        Dim values = e.Values

        ' 在图表中显示总CPU使用率.
        totalCpuUsageSeries.Points.DataBindY(e.Values)

    End Sub

    ''' <summary>
    ''' 激发totalCpuUsageMonitor_ErrorOccurredHandler来处理
    ''' totalCpuUsageMonitor的ErrorOccurred事件.
    ''' </summary>
    Private Sub totalCpuUsageMonitor_ErrorOccurred(ByVal sender As Object,
                                                   ByVal e As ErrorEventArgs)
        Me.Invoke(New EventHandler(Of ErrorEventArgs)(
                  AddressOf totalCpuUsageMonitor_ErrorOccurredHandler), sender, e)
    End Sub

    Private Sub totalCpuUsageMonitor_ErrorOccurredHandler(ByVal sender As Object,
                                                          ByVal e As ErrorEventArgs)
        If _totalCpuUsageMonitor IsNot Nothing Then
            _totalCpuUsageMonitor.Dispose()
            _totalCpuUsageMonitor = Nothing

            Dim totalCpuUsageSeries = chartTotalCpuUsage.Series("TotalCpuUsageSeries")
            totalCpuUsageSeries.Points.Clear()
        End If
        MessageBox.Show(e.Error.Message)
    End Sub
End Class
