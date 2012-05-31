'****************************** 模块头 ******************************\
' 模块名:	NativeMethods.vb
' 项目名: VideoEncodingWorkerRole
' 版权 (c) Microsoft Corporation.
' 
' 定义所有非托管代码的静态类.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Runtime.InteropServices

Public Class NativeMethods
    <DllImport("NativeVideoEncoder.dll", CharSet:=CharSet.Unicode)> _
    Friend Shared Function EncoderVideo(inputFile As String, outputFile As String, logFile As String) As Integer
    End Function
End Class
