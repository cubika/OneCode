'****************************** 模块头 ******************************'
' 模块名称:  MultiThreadedWebDownloaderProgressChangedEventArgs.vb
' 项目名称:	    VBMultiThreadedWebDownloader
' 版权 (c) Microsoft Corporation.
' 
' MultiThreadedWebDownloaderProgressChangedEventArgs类定义了MultiThreadedWebDownloader
' 的DownloadProgressChanged 事件所要用的参数 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Public Class MultiThreadedWebDownloaderProgressChangedEventArgs
    Inherits EventArgs

    Private privateReceivedSize As Long
    Public Property ReceivedSize() As Long
        Get
            Return privateReceivedSize
        End Get
        Private Set(ByVal value As Long)
            privateReceivedSize = value
        End Set
    End Property

    Private privateTotalSize As Long
    Public Property TotalSize() As Long
        Get
            Return privateTotalSize
        End Get
        Private Set(ByVal value As Long)
            privateTotalSize = value
        End Set
    End Property

    Private privateDownloadSpeed As Integer
    Public Property DownloadSpeed() As Integer
        Get
            Return privateDownloadSpeed
        End Get
        Private Set(ByVal value As Integer)
            privateDownloadSpeed = value
        End Set
    End Property

    Public Sub New(ByVal receivedSize As Long, ByVal totalSize As Long,
                   ByVal downloadSpeed As Integer)
        Me.ReceivedSize = receivedSize
        Me.TotalSize = totalSize
        Me.DownloadSpeed = downloadSpeed
    End Sub
End Class

