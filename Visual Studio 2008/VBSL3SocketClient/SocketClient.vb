'****************************** 模块头 ******************************'
' 模块名:              SocketClient.vb
' 项目名:              VBSL3SocketServer
' 版权 (c) Microsoft Corporation.
' 
' 套接字服务器应用程序代码文件，能服务Silverlight和标准的套接字客户端。
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
Imports System.Text
Imports System.Net.Sockets
Imports System.Net

Namespace VBSL3SocketClient
    Public Class SocketMessageEventArgs
        Inherits EventArgs
        Private _Error As Exception
        Public Property [Error]() As Exception
            Get
                Return _Error
            End Get
            Set(ByVal value As Exception)
                _Error = value
            End Set
        End Property
        Private _Data As String
        Public Property Data() As String
            Get
                Return _Data
            End Get
            Set(ByVal value As String)
                _Data = value
            End Set
        End Property
    End Class

    Public Class SocketClient
        ' 为异步操作定义了3个事件：
        ' 打开，接收和发送
        Public Event MessageReceived As EventHandler(Of SocketMessageEventArgs)
        Public Event MessageSended As EventHandler(Of SocketMessageEventArgs)
        Public Event ClientConnected As EventHandler(Of SocketMessageEventArgs)

        ' 设置接收缓冲器的大小
        Shared ReadOnly BUFFER_SIZE As Integer = 65536

        ' 定义消息结尾字符，用于分隔
        ' 字节数组组成字符串消息
        Shared ReadOnly EOM_MARKER As Char = ChrW(127)

        ' 封装的套接字
        Private _InnerSocket As Socket
        Public Property InnerSocket() As Socket
            Get
                Return _InnerSocket
            End Get
            Private Set(ByVal value As Socket)
                _InnerSocket = value
            End Set
        End Property


        Public Sub New(ByVal socket As Socket)
            If socket Is Nothing Then
                Throw New Exception("套接字不能为空")
            End If
            InnerSocket = socket

            ' 初始化字符串编码/解码器
            encoding = New UTF8Encoding(False, True)
        End Sub
        Public Sub New(ByVal addfamily As AddressFamily, ByVal socktype As SocketType, ByVal protype As ProtocolType)
            InnerSocket = New Socket(addfamily, socktype, protype)
            encoding = New UTF8Encoding(False, True)
        End Sub

#Region "套接字异步连接"

        ' 取得套接字连接状态
        Public ReadOnly Property Connected() As Boolean
            Get
                Return InnerSocket.Connected
            End Get
        End Property

        ' 关闭套接字
        Public Sub Close()
            InnerSocket.Close()
        End Sub

        ''' <summary>
        ''' 异步连接套接字到终端。
        ''' 可能的异常：
        ''' ArgumentException
        ''' ArgumentNullException
        ''' InvalidOperationException
        ''' SocketException
        ''' NotSupportedException
        ''' ObjectDisposedException
        ''' SecurityException
        ''' Details at: http://msdn.microsoft.com/en-us/library/bb538102.aspx
        ''' </summary>
        ''' <param name="ep">远程终端</param>
        Public Sub ConnectAsync(ByVal ep As EndPoint)
            If InnerSocket.Connected Then
                Exit Sub
            End If

            ' 初始化socketAsyncEventArgs对象 
            ' 设置远程连接终端
            Dim connectEventArgs = New SocketAsyncEventArgs()
            connectEventArgs.RemoteEndPoint = ep
            AddHandler connectEventArgs.Completed, AddressOf connectEventArgs_Completed

            ' 调用ConnectAsync方法, 如果该方法返回false
            ' 它就意味着返回的结果是异步的
            If Not InnerSocket.ConnectAsync(connectEventArgs) Then
                ' 调用方法来处理连接结果
                ProcessConnect(connectEventArgs)
            End If
        End Sub

        ' 当connectAsync方法完成时，调用方法处理连接结果
        Private Sub connectEventArgs_Completed(ByVal sender As Object, ByVal e As SocketAsyncEventArgs)
            ProcessConnect(e)
        End Sub

        ' 调用连接事件ClientConnected来返回结果 
        Private Sub ProcessConnect(ByVal e As SocketAsyncEventArgs)
            If e.SocketError = SocketError.Success Then
                OnClientConnected(Nothing)
            Else
                OnClientConnected(New SocketException(CInt(e.SocketError)))
            End If
        End Sub

        Private Sub OnClientConnected(ByVal [error] As Exception)
            Dim sockargs As New SocketMessageEventArgs()
            sockargs.Error = [error]
            RaiseEvent ClientConnected(Me, sockargs)
        End Sub
#End Region

#Region "套接字异步发送"

        ''' <summary>
        ''' 使用套接字发送字符串消息
        ''' 可能异常:
        ''' FormatException
        ''' ArgumentException
        ''' InvalidOperationException
        ''' NotSupportedException
        ''' ObjectDisposedException
        ''' SocketException
        ''' </summary>
        ''' <param name="data">要发送的消息</param>
        Public Sub SendAsync(ByVal data As String)

            ' 如果消息包含分隔符EOM_MARKER，
            ' 就抛出异常
            If data.Contains(EOM_MARKER) Then
                Throw New Exception("Unallowed chars existed in message")
            End If

            ' 在消息后加结尾分隔符。
            data += EOM_MARKER

            ' 用UTF8编码成字节数组
            Dim bytesdata = encoding.GetBytes(data)

            ' 初始化发送事件变量SendEventArgs
            Dim sendEventArgs = New SocketAsyncEventArgs()
            sendEventArgs.SetBuffer(bytesdata, 0, bytesdata.Length)
            AddHandler sendEventArgs.Completed, AddressOf sendEventArgs_Completed

            ' 调用异步发送方法SendAsync，如果方法返回false
            ' 就意味着结果是异步的
            If Not InnerSocket.SendAsync(sendEventArgs) Then
                ProcessSend(sendEventArgs)
            End If
        End Sub

        ' 当异步发送方法完成时，调用方法处理发送的结果
        Private Sub sendEventArgs_Completed(ByVal sender As Object, ByVal e As SocketAsyncEventArgs)
            ProcessSend(e)
        End Sub

        ' 调用消息发送事件MessageSended返回结果
        Private Sub ProcessSend(ByVal e As SocketAsyncEventArgs)
            If e.SocketError = SocketError.Success Then
                OnMessageSended(Nothing)
            Else
                OnMessageSended(New SocketException(CInt(e.SocketError)))
            End If
        End Sub

        Private Sub OnMessageSended(ByVal [error] As Exception)
            Dim sockargs As New SocketMessageEventArgs()
            sockargs.Error = [error]
            RaiseEvent MessageSended(Me, sockargs)
        End Sub
#End Region

#Region "套接字异步接收"

        ' 定义标记来指示接收状态
        Private _isReceiving As Boolean

        ''' <summary>
        ''' 当接收每一条消息时，开始接收套接字的字节和调用
        ''' 消息接收事件MessageReceived。
        ''' 可能异常：
        ''' ArgumentException
        ''' InvalidOperationException
        ''' NotSupportedException
        ''' ObjectDisposedException
        ''' SocketException
        ''' Details at http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.receiveasync.aspx
        ''' </summary>
        Public Sub StartReceiving()

            ' 检查套接字是否已经开始接收消息
            If Not _isReceiving Then
                _isReceiving = True
            Else
                Exit Sub
            End If

            Try
                ' 初始化接收缓冲器
                Dim buffer = New Byte(BUFFER_SIZE) {}

                ' 初始化接收事件变量
                Dim receiveEventArgs = New SocketAsyncEventArgs()
                receiveEventArgs.SetBuffer(buffer, 0, BUFFER_SIZE)
                AddHandler receiveEventArgs.Completed, AddressOf receiveEventArgs_Completed

                ' 调用异步接收方法ReceiveAsync，如果返回false
                ' 就意味着结果是异步返回的
                If Not InnerSocket.ReceiveAsync(receiveEventArgs) Then
                    ProcessReceive(receiveEventArgs)
                End If
            Catch ex As Exception
                StopReceiving()
                Throw ex
            End Try
        End Sub

        ' 停止接收套接字的字节
        Public Sub StopReceiving()
            _isReceiving = False
        End Sub

        Private Sub receiveEventArgs_Completed(ByVal sender As Object, ByVal e As SocketAsyncEventArgs)
            ProcessReceive(e)
        End Sub

        ' 处理异步接收完成事件
        Private receivemessage As String = ""
        Private encoding As Encoding
        Private taillength As Integer
        Private Sub ProcessReceive(ByVal e As SocketAsyncEventArgs)
            ' 当出错时，调用消息接收事件
            ' 来传递错误信息给用户
            If e.SocketError <> SocketError.Success Then
                StopReceiving()
                OnMessageReceived(Nothing, New SocketException(CInt(e.SocketError)))
                Exit Sub
            End If

            Try
                '#Region "String Decoding"
                ' 解码字节成字符串
                ' 注意UTF-8编码是可变长编码的，我们需要检查字节
                ' 数组尾部，以防把一个字母分成两个。
                Dim receivestr As String = ""
                ' 尝试解码字符串
                Try
                    receivestr = encoding.GetString(e.Buffer, 0, taillength + e.BytesTransferred)
                    ' 如果解码成功，重设尾部长度
                    taillength = 0
                Catch ex As DecoderFallbackException
                    ' 如果取得解码异常，删除数组尾部，并重新解码
                    Try
                        receivestr = encoding.GetString(e.Buffer, 0, taillength + e.BytesTransferred - ex.BytesUnknown.Length)
                        ' 重设尾部长度
                        taillength = ex.BytesUnknown.Length
                        ex.BytesUnknown.CopyTo(e.Buffer, 0)
                    Catch ex2 As DecoderFallbackException
                        ' 如果还出现解码异常，就停止接收
                        Throw New Exception("Message decode failed.", ex2)

                        '#End Region
                    End Try
                End Try
                ' 检查消息是否结束
                Dim eompos As Integer = receivestr.IndexOf(EOM_MARKER)
                While eompos <> -1
                    ' 组合成一条完整的消息
                    receivemessage += receivestr.Substring(0, eompos)

                    ' 激活接收的消息
                    OnMessageReceived(receivemessage, Nothing)

                    ' 取得剩下的字符串
                    receivemessage = ""
                    receivestr = receivestr.Substring(eompos + 1, receivestr.Length - eompos - 1)

                    ' 检查字符串是否还有分隔符
                    eompos = receivestr.IndexOf(EOM_MARKER)
                End While
                receivemessage += receivestr

                ' 停止接收
                If Not _isReceiving Then
                    Exit Sub
                End If

                ' 重设缓冲器开始地址
                e.SetBuffer(taillength, BUFFER_SIZE - taillength)

                ' 继续接收
                If Not InnerSocket.ReceiveAsync(e) Then
                    ProcessReceive(e)
                End If
            Catch ex As Exception
                ' 通过消息接收事件返回错误
                OnMessageReceived(Nothing, ex)
                StopReceiving()
            End Try
        End Sub

        Private Sub OnMessageReceived(ByVal data As String, ByVal [error] As Exception)
            Dim sockargs As New SocketMessageEventArgs()
            sockargs.Data = data
            sockargs.Error = [error]
            RaiseEvent MessageReceived(Me, sockargs)
        End Sub
#End Region
    End Class
End Namespace