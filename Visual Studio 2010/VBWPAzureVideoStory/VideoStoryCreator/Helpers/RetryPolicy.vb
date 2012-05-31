'********************************* 模块头 *********************************\
' 模块名: RetryPolicy.vb
' 项目名: StoryCreatorWebRole
' 版权(c) Microsoft Corporation.
' 
' 简单的获取策略用来获得service.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Threading

Public Class RetryPolicy
    Public Property Request() As HttpWebRequest
        Get
            Return m_Request
        End Get
        Private Set(value As HttpWebRequest)
            m_Request = value
        End Set
    End Property
    Private m_Request As HttpWebRequest
    Public Property Initialize() As Action
        Get
            Return m_Initialize
        End Get
        Set(value As Action)
            m_Initialize = value
        End Set
    End Property
    Private m_Initialize As Action
    Public RequestCallback As AsyncCallback
    Public ResponseCallback As AsyncCallback
    Public Property RequestAddress() As String
        Get
            Return m_RequestAddress
        End Get
        Set(value As String)
            m_RequestAddress = value
        End Set
    End Property
    Private m_RequestAddress As String
    Public Property WaitTime() As TimeSpan
        Get
            Return m_WaitTime
        End Get
        Set(value As TimeSpan)
            m_WaitTime = value
        End Set
    End Property
    Private m_WaitTime As TimeSpan
    Public Property RetryNumber() As Integer
        Get
            Return m_RetryNumber
        End Get
        Set(value As Integer)
            m_RetryNumber = value
        End Set
    End Property
    Private m_RetryNumber As Integer

    Private retriedTimes As Integer = 0

    Public Sub New(requestAddress As String)
        Me.Request = DirectCast(HttpWebRequest.Create(requestAddress), HttpWebRequest)
        Me.WaitTime = TimeSpan.FromSeconds(10.0)
        Me.RetryNumber = 3
    End Sub

    ''' <summary>
    ''' request开始.
    ''' </summary>
    Public Sub MakeRequest()
        Me.Request = DirectCast(HttpWebRequest.Create(Me.RequestAddress), HttpWebRequest)
        For i As Integer = 0 To Me.RetryNumber - 1
            Try
                Me.m_Initialize()
                Me.Request.BeginGetRequestStream(AddressOf Me.GetRequestStreamCallback, Nothing)
                Exit Try
            Catch
                Thread.Sleep(Me.WaitTime)
            End Try
        Next
    End Sub

    ''' <summary>
    ''' 调用stream回调事件.
    ''' </summary>
    Public Sub GetRequestStreamCallback(result As IAsyncResult)
        Me.RequestCallback(result)
        Me.Request.BeginGetResponse(AddressOf Me.GetResponseCallback, Nothing)
    End Sub

    ''' <summary>
    ''' 调用response回调事件.
    ''' </summary>
    Public Sub GetResponseCallback(result As IAsyncResult)
        Try
            Me.retriedTimes += 1
            Me.ResponseCallback(result)
        Catch
            If Me.retriedTimes < Me.RetryNumber Then
                ' 如果需要错误, 重新获取.
                Thread.Sleep(Me.WaitTime)
                Me.MakeRequest()
            Else
                Throw
            End If
        End Try
    End Sub
End Class
