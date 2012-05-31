'****************************** 模块头 ******************************'
' 模块名:                 MainPage.xaml.cs
' 项目名:                 CSSL3Socket
' 版权 (c) Microsoft Corporation.
' 
' Silverlight套接字客户端后台代码文件。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports VBSL3SocketClient.VBSL3SocketClient

Partial Public Class MainPage
    Inherits UserControl
    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub Finalize()
        Try
            If _client IsNot Nothing Then
                _client.Close()
            End If
        Finally
            MyBase.Finalize()
        End Try
    End Sub

    Private _client As SocketClient

    ' 处理“连接”按钮点击事件。
    Private Sub btnConnect_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        btnConnect.IsEnabled = False
        OpenSocketClientAsync(tboxServerAddress.Text)
    End Sub

    ' 处理“发送”按钮点击事件
    Private Sub btnSend_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            If _client IsNot Nothing Then
                _client.SendAsync(tboxMessage.Text)
            End If
        Catch ex As Exception
            MessageBox.Show("当发送消息时发生异常:" & ex.Message)
            CloseSocketClient()
            Exit Sub
        End Try
    End Sub

    ' 关闭套接字客户端
    Private Sub CloseSocketClient()
        If _client IsNot Nothing Then
            _client.Close()
            _client = Nothing
        End If

        ' 更新UI
        btnConnect.IsEnabled = True
        btnSend.IsEnabled = False
    End Sub

    ' 创建套接字客户端和连接到服务器
    '
    ' 为了方便，我们使用127.0.0.1地址在本地连接套接字服务器。
    ' 为了能让不同机器上的客户端通过网络访问套接字服务器，
    ' 可在文本框中输入服务器ip地址，再点击“连接”。
    ' 为了套接字服务器可通过网络访问，请看CCSL3SocketServer的评论。
    Private Function OpenSocketClientAsync(ByVal ip As String) As Boolean
        Try
            Dim endpoint = New IPEndPoint(IPAddress.Parse(ip), 4502)

            _client = New SocketClient(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)

            ' 注册事件
            AddHandler _client.ClientConnected, AddressOf _client_ClientConnected
            AddHandler _client.MessageReceived, AddressOf _client_MessageReceived
            AddHandler _client.MessageSended, AddressOf _client_MessageSended

            _client.ConnectAsync(endpoint)
            Return True
        Catch ex As Exception
            MessageBox.Show("连接套接字时发生异常:" & ex.Message)
            CloseSocketClient()
            Return False
        End Try
    End Function

    Delegate Sub HandleSocketEvent(ByVal e As SocketMessageEventArgs)

    Private Sub _client_ClientConnected(ByVal sender As Object, ByVal e As SocketMessageEventArgs)
        Dim clientConnect As New HandleSocketEvent(AddressOf onClientConnect)
        Me.Dispatcher.BeginInvoke(clientConnect, New Object() {e})
    End Sub

    Private Sub onClientConnect(ByVal e As SocketMessageEventArgs)
        ' 如果连接成功，则开始接收消息
        If e.[Error] Is Nothing Then
            Try
                _client.StartReceiving()
            Catch ex As Exception
                MessageBox.Show("当创建套接字客户端时发生异常:" & ex.Message)
                CloseSocketClient()
                Exit Sub
            End Try
            ' 更新UI
            btnConnect.IsEnabled = False
            btnSend.IsEnabled = True
            tbSocketStatus.Text = "已连接"
        Else
            _client.Close()
            btnConnect.IsEnabled = True
            tbSocketStatus.Text = "连接失败: " & e.[Error].Message
        End If
    End Sub

    ' 处理消息接收事件
    Private Sub _client_MessageSended(ByVal sender As Object, ByVal e As SocketMessageEventArgs)
        Dim messageSended As New HandleSocketEvent(AddressOf onMessageSended)
        Me.Dispatcher.BeginInvoke(messageSended, New Object() {e})
    End Sub

    Private Sub onMessageSended(ByVal e As SocketMessageEventArgs)
        If e.[Error] Is Nothing Then
            tbSocketStatus.Text = "已发送"
        Else
            tbSocketStatus.Text = "发送失败: " & e.[Error].Message
            CloseSocketClient()
        End If
    End Sub

    ' 处理消息发送事件
    Private Sub _client_MessageReceived(ByVal sender As Object, ByVal e As SocketMessageEventArgs)
        Dim messageReceived As New HandleSocketEvent(AddressOf onMessageReceived)
        Me.Dispatcher.BeginInvoke(messageReceived, New Object() {e})
    End Sub

    Private Sub onMessageReceived(ByVal e As SocketMessageEventArgs)
        If e.[Error] Is Nothing Then
            tbSocketStatus.Text = "已接收"
            lb1.Items.Insert(0, e.Data)
        Else
            tbSocketStatus.Text = "接收失败: " & e.[Error].Message
            CloseSocketClient()
        End If
    End Sub
End Class