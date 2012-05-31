'************************** 模块头*************************************'
' 模块名:   TotalCpuUsageMonitor.vb
' 项目名:   VBCpuUsage
' Copyright (c) Microsoft Corporation.
' 
' 该类继承了CpuUsageMonitorBase,被用来得到总的CPU使用率.
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

Public Class TotalCpuUsageMonitor
    Inherits CpuUsageMonitorBase

    Private Const _categoryName As String = "Processor"
    Private Const _counterName As String = "% Processor Time"
    Private Const _instanceName As String = "_Total"

    Public Sub New(ByVal timerPeriod As Integer, ByVal valueArrayCapacity As Integer)
        MyBase.New(timerPeriod, valueArrayCapacity, _categoryName, _counterName, _instanceName)
    End Sub
End Class