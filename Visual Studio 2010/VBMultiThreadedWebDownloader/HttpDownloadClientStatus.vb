'****************************** 模块头******************************'
' 模块名称:  HttpDownloadClientStatus.vb
' 项目名称:	    VBMultiThreadedWebDownloader
' 版权 (c) Microsoft Corporation.
' 
' HttpDownloadClientStatus 枚举包含HttpDownloadClient 的所有状态。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Public Enum HttpDownloadClientStatus
    Idle = 0
    Downloading = 2
    Pausing = 3
    Paused = 4
    Canceling = 5
    Canceled = 6
    Completed = 7
End Enum

