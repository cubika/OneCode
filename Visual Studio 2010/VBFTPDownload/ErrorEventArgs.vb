'*************************** Module Header ******************************'
' 模块名:  ErrorEventArgs.vb
' 项目名:	    VBFTPDownload
' 版权(c)  Microsoft Corporation.
' 
' 这个类ErrorEventArgs通过使用FTPClient的ErrorOccurred事件定义一些变量。 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Public Class ErrorEventArgs
    Inherits EventArgs
    Public Property ErrorException() As Exception
End Class
