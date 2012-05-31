'*************************** Module Header ******************************.
' 模块名:  FileDownloadCompletedEventArgs.vb
' 项目名:	    VBFTPDownload
' 版权(c)  Microsoft Corporation.
' 
'这个类FileDownloadCompletedEventArgs通过使用FTPClient的FileDownloadCompleted事件定义一些变量 

' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************.

Imports System.IO

Public Class FileDownloadCompletedEventArgs
    Inherits EventArgs
    Public Property ServerPath() As Uri
    Public Property LocalFile() As FileInfo
End Class
