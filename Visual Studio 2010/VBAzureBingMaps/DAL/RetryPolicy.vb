'/********************************* 模块头 **********************************\
'* 模块名:  RetryPolicy.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* A SQL Azure retry policy.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Data.SqlClient
Imports System.Threading

''' <summary>
''' SQL Azure重试策略.
''' </summary>
Public Class RetryPolicy
    Public Property RetryNumber As Integer
    Public Property WaitTime As TimeSpan
    Public Property Task As Action

    Public Sub New()
        Me.RetryNumber = 3
    End Sub

    Public Sub New(ByVal retryNumber As Integer)
        Me.RetryNumber = retryNumber
        Me.WaitTime = TimeSpan.FromSeconds(5.0)
    End Sub

    ''' <summary>
    ''' 如果运行失败,一定时间以后重试,
    ''' 直到到达重试策略最大值.
    ''' </summary>
    Public Sub Execute()
        For i As Integer = 0 To Me.RetryNumber - 1
            Try
                Me.Task.Invoke()
                Exit Try
            Catch ex As SqlException
                If i = Me.RetryNumber - 1 Then
                    Throw New SqlExceptionWithRetry("到达最大重试次数. 仍然无法处理请求. 详细信息请参看内部异常.", ex)
                End If
                Thread.Sleep(Me.WaitTime)
            End Try
        Next
    End Sub
End Class

Public Class SqlExceptionWithRetry
    Inherits Exception
    Public Sub New(ByVal message As String, ByVal innerException As SqlException)
        MyBase.New(message, innerException)
    End Sub
End Class