'****************************** 模块头 *******************************'
' 模块名:   CpuUsageValueArrayChangedEventArg.vb
' 项目名:   VBCpuUsage
' Copyright (c)  Microsoft Corporation.
' 
' 该事件参数用于CpuUsageMonitorBase类的CpuUsageValueArrayChanged事件中.
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

Public Class CpuUsageValueArrayChangedEventArg
    Inherits EventArgs
    Public Property Values() As Double()
End Class
