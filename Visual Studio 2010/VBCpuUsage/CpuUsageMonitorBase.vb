'************************** 模块头*************************************'
' 模块名:  CpuUsageMonitorBase.vb
' 项目名:  VBCpuUsage
' Copyright (c) Microsoft Corporation.
' 
' 它是ProcessCpuUsageMonitor和TotalCpuUsageMonitor的基类. 它提供了监视器的基本字 
' 段、事件、功能和功能，例如Timer, PerformanceCounter和IDisposable接口.
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

Imports System.Threading

Public MustInherit Class CpuUsageMonitorBase
    Implements IDisposable
    Private _locker As New Object()

    ' 指示实例是否被释放.
    Private _disposed As Boolean = False

    ' timer用来得到每隔几秒钟performance counter的值.
    Private _timer As Timer

    ' CPU usage performance counter 将在ProcessCpuUsageMonitor 和 
    ' TotalCpuUsageMonitor中初始化.
    Protected _cpuPerformanceCounter As PerformanceCounter = Nothing

    ' CpuUsageValueArray的最大长度.
    Private _valueArrayCapacity As Integer

    ' 该列用来存储值.
    Private _cpuUsageValueArray As List(Of Double)

    ''' <summary>
    ''' 有新增数值时发生.
    ''' </summary>
    Public Event CpuUsageValueArrayChanged As EventHandler(Of CpuUsageValueArrayChangedEventArg)

    Public Event ErrorOccurred As EventHandler(Of ErrorEventArgs)

    ''' <summary>
    ''' 初始化timer和performance counter.
    ''' </summary>
    ''' <param name="timerPeriod">
    ''' 如果该数值大于0，则timer将不会启用.
    ''' </param>
    ''' <param name="valueArrayCapacity">
    ''' CpuUsageValueArray的最大长度.
    ''' </param>
    ''' <param name="categoryName">
    ''' 与此performance counter相关的performance counter category（性能对象）的名字.
    ''' </param>
    ''' <param name="counterName">
    ''' performance counter的名字. 
    ''' </param>
    ''' <param name="instanceName">
    ''' performance counter category实例的名字;或者，当某类别含有单个实例时，为空字符串("").
    ''' </param>
    Public Sub New(ByVal timerPeriod As Integer, ByVal valueArrayCapacity As Integer,
                   ByVal categoryName As String, ByVal counterName As String,
                   ByVal instanceName As String)

        ' 初始化PerformanceCounter.
        Me._cpuPerformanceCounter =
            New PerformanceCounter(categoryName, counterName, instanceName)

        Me._valueArrayCapacity = valueArrayCapacity
        _cpuUsageValueArray = New List(Of Double)()

        If timerPeriod > 0 Then

            ' 延迟timer以激发回调..
            Me._timer = New Timer(
                New TimerCallback(AddressOf Timer_Callback), Nothing, timerPeriod, timerPeriod)

        End If

    End Sub

    ''' <summary>
    ''' 这个方法将在timer的回调函数中被执行.
    ''' </summary>
    Private Sub Timer_Callback(ByVal obj As Object)
        SyncLock _locker

            ' 清理列表.
            If Me._cpuUsageValueArray.Count > Me._valueArrayCapacity Then
                Me._cpuUsageValueArray.Clear()
            End If

            Try
                Dim value As Double = GetCpuUsage()

                If value < 0 Then
                    value = 0
                End If
                If value > 100 Then
                    value = 100
                End If

                Me._cpuUsageValueArray.Add(value)

                Dim values(_cpuUsageValueArray.Count - 1) As Double
                _cpuUsageValueArray.CopyTo(values, 0)

                ' 引发事件.
                Me.OnCpuUsageValueArrayChanged(
                    New CpuUsageValueArrayChangedEventArg() With {.Values = values})
            Catch ex As Exception
                Me.OnErrorOccurred(New ErrorEventArgs With {.Error = ex})
            End Try
        End SyncLock
    End Sub

    ''' <summary>
    ''' 得到当前的CPU使用率.
    ''' </summary>
    Protected Overridable Function GetCpuUsage() As Double
        If Not IsInstanceExist() Then
            Throw New ApplicationException(
                String.Format("The instance {0} is not available. ",
                              Me._cpuPerformanceCounter.InstanceName))
        End If


        Dim value As Double = _cpuPerformanceCounter.NextValue()
        Return value
    End Function

    ''' <summary>
    ''' 子类可能会覆盖该方法来决定实例是否存在.
    ''' </summary>
    Protected Overridable Function IsInstanceExist() As Boolean
        Return True
    End Function

    ''' <summary>
    ''' 引发CpuUsageValueArrayChanged事件.
    ''' </summary>
    Protected Sub OnCpuUsageValueArrayChanged(ByVal e As CpuUsageValueArrayChangedEventArg)
        RaiseEvent CpuUsageValueArrayChanged(Me, e)
    End Sub

    ''' <summary>
    ''' 引发ErrorOccurred事件.
    ''' </summary>
    Protected Overridable Sub OnErrorOccurred(ByVal e As ErrorEventArgs)
        RaiseEvent ErrorOccurred(Me, e)
    End Sub

    '  释放资源.
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        ' 保护，以免被多次调用.
        If _disposed Then
            Return
        End If

        If disposing Then
            If _timer IsNot Nothing Then
                _timer.Dispose()
            End If

            If _cpuPerformanceCounter IsNot Nothing Then
                _cpuPerformanceCounter.Dispose()
            End If
            _disposed = True
        End If
    End Sub
End Class
