=========================================================================================
				Windows 应用程序: VBCpuUsage 概述                      
========================================================================================= 

//////////////////////////////////////////////////////////////////////////////////////////
摘要：

这个实例演示了如何使用PerfermanceCounter来得到具有以下特征的CPU使用率（CPU Usage)。
	
1.总的处理器时间。
2.一项特定进程的处理器时间。
3.像任务管理器一般绘制CPU使用率历史。

///////////////////////////////////////////////////////////////////////////////////////////
演示：

步骤1. 在Visual Studio 2010中打开这个项目。

步骤2. 生成项目，并运行 VBCpuUsage.exe.

步骤3. 检查“显示总CPU使用率”和“显示某一进程的CPU使用率”。单击复合框，选中一个进程（例如:
       taskmgr,如果任务管理器正在运行)给监视器。

	   你将看到在此应用程序上会有2张图表来显示CPU使用率历史。


//////////////////////////////////////////////////////////////////////////////////
代码逻辑：

A. 设计 CpuUsageMonitorBase 类，用来提供监视器的基本字段、事件、功能和接口。例如，
   Timer、PerformanceCounter 和 IDisposable 接口。

   CpuUsageMonitorBase 是一个抽象类，所以不能被实例化。当它的子类被实例化时，它们会传递
   categoryName, counterName 和 instanceName 给类 CpuUsageMonitorBase 的构造器来初始
   化performance counter。
   
         Me._cpuPerformanceCounter =
            New PerformanceCounter(categoryName, counterName, instanceName)
    
   Timer被用来得到每隔几秒钟performance counter的值，并引发CpuUsageValueArrayChanged事
   件。如果在读取performance counter数值时有任何异常发生，ErrorOccurred事件将会被激发。 
     

B. 设计TotalCpuUsageMonitor 类以用于监视总的CPU使用率。它继承了CpuUsageMonitorBase类，
   并且定义了3个常量：

        Private Const _categoryName As String = "Processor"
        Private Const _counterName As String = "% Processor Time"
        Private Const _instanceName As String = "_Total"

   为了得到总的CPU使用率，可以用上述常量来初始化performance counter.
    
        Public Sub New(ByVal timerPeriod As Integer, ByVal valueArrayCapacity As Integer)
            MyBase.New(timerPeriod, valueArrayCapacity, _categoryName,
                       _counterName, _instanceName)
        End Sub

C. 设计 ProcessCpuUsageMonitor类来监视某一特定进程的CPU使用率。它也继承了CpuUsageMonitor-
   -Base 类，并定义了2个常量：
        
        Private Const _categoryName As String = "Process"
        Private Const _counterName As String = "% Processor Time"
   
   为了初始化一个performance counter，仍然需要实例名（进程名）。因此，该类也提供了一个方法来
   得到可用的进程。

        Private Shared _category As PerformanceCounterCategory
        Public Shared ReadOnly Property Category() As PerformanceCounterCategory
            Get
                If _category Is Nothing Then
                    _category = New PerformanceCounterCategory(_categoryName)
                End If
                Return _category
            End Get
        End Property
               
        Public Shared Function GetAvailableProcesses() As String()
            Return Category.GetInstanceNames().OrderBy(Function(name) name).ToArray()
        End Function


D. 设计MainForm，用于初始化totalCpuUsageMonitor 和 processCpuUsageMonitor，登记
   CpuUsageValueArrayChanged，并将CPU使用率历史显示在图表中。

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
        ''' 激发totalCpuUsageMonitor_CpuUsageValueArrayChangedHandler以处理
        ''' totalCpuUsageMonitor的CpuUsageValueArrayChanged事件.
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
        
            ' 在图表中显示总的CPU使用率.
            totalCpuUsageSeries.Points.DataBindY(e.Values)
        
        End Sub
        
      
  如果在读取performance counter的值时有任何异常发生，用户界面也将处理这一事件。

        ''' <summary>
        ''' 激发processCpuUsageMonitor_ErrorOccurredHandler来处理processCpuUsageMonitor
        ''' 的ErrorOccurred事件.
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
        ''' 激发totalCpuUsageMonitor_ErrorOccurredHandler来处理totalCpuUsageMonitor
        ''' 的ErrorOccurred 事件 .
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

///////////////////////////////////////////////////////////////////////////////////////////
参考：

PerformanceCounter Class
http://msdn.microsoft.com/en-us/library/system.diagnostics.performancecounter.aspx

Chart Class
http://msdn.microsoft.com/en-us/library/system.windows.forms.datavisualization.charting.chart.aspx

///////////////////////////////////////////////////////////////////////////////////////////