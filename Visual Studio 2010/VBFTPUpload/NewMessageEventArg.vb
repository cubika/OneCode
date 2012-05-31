'*************************** 模块头 ******************************'
' 模块名:  NewMessageEventArg.vb
' 项目:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
' NewMessageEventArg类定义参数被FTPClient NewMessageArrived事件使用
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Public Class NewMessageEventArg
    Inherits EventArgs
    Public Property NewMessage() As String
End Class
