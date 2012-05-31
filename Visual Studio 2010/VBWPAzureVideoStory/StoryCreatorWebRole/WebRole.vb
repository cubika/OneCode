Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.Diagnostics
Imports Microsoft.WindowsAzure.ServiceRuntime

Public Class WebRole
    Inherits RoleEntryPoint

    Public Overrides Function OnStart() As Boolean

        ' 启动AzureLocalStorageTraceListner, 在web.config取消相关节点  
        Dim diagnosticConfig As DiagnosticMonitorConfiguration = DiagnosticMonitor.GetDefaultInitialConfiguration()
        diagnosticConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1)
        diagnosticConfig.Directories.DataSources.Add(AzureLocalStorageTraceListener.GetLogDirectory())
 
        Return MyBase.OnStart()

    End Function

End Class

