'*************************** 模块头 ******************************'
' 模块名:  FileUploadCompletedEventArgs.vb
' 项目:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
' FileUploadCompletedEventArgs 类定义FTPClient FileUploadCompleted事件使用的参数。
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.IO

Public Class FileUploadCompletedEventArgs
    Inherits EventArgs
    Public Property ServerPath() As Uri
    Public Property LocalFile() As FileInfo
End Class
