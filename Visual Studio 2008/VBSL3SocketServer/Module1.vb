'****************************** 模块头 ******************************'
' 模块名:              Module1.vb
' 项目名:              VBSL3SocketServer
' 版权 (c) Microsoft Corporation.
' 
' 套接字服务器应用程序代码文件，能够服务Silverlight和标准的套接字客户端。
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
Imports System.Threading
Imports System.IO

Namespace VBSL3SocketServer
    Class Program
        Shared policybytes As Byte()
        Public Shared Sub Main(ByVal args As String())
            ' 读入policy.xml文件，并存入字节数组中。
            Dim filestream = New FileStream("policy.xml", FileMode.Open, FileAccess.Read)
            policybytes = New Byte(filestream.Length - 1) {}
            filestream.Read(policybytes, 0, CInt(filestream.Length))
            filestream.Close()

            ' 初始化策略套接字监听器
            Dim socketp = New SocketListener()
            socketp.ListenAsync(943, AddressOf socketp_SocketConnected)

            ' 初始化套接字监听器
            Dim socketp2 = New SocketListener()
            socketp2.ListenAsync(4502, AddressOf socketp2_SocketConnected)

            Console.Read()
        End Sub

        ' 连接的客户端
        Private Shared Sub socketp2_SocketConnected(ByVal sock As Socket)
            ' 创建新的线程来处理客户会话
            ' 初始化套接字客户端
            ' 准备接收

            Dim thread As New Thread(AddressOf CreateSocketClient)
            thread.Start(sock)

        End Sub

        Private Shared Sub CreateSocketClient(ByVal param As Socket)
            Dim client = New SocketClient(param)
            Try
                AddHandler client.MessageReceived, AddressOf client_MessageReceived
                AddHandler client.MessageSended, AddressOf client_MessageSended
                client.StartReceiving()
                Console.WriteLine("客户端已连接.")
            Catch ex As Exception
                Console.WriteLine("当开始接收消息时发生异常:" & vbLf & ex.Message)
                client.Close()
            End Try
        End Sub

        ' 处理消息发送事件
        Private Shared Sub client_MessageSended(ByVal sender As Object, ByVal e As SocketMessageEventArgs)
            If e.[Error] IsNot Nothing Then
                Console.WriteLine("消息发送失败: " & e.[Error].Message)
                DirectCast(sender, SocketClient).Close()
                Console.WriteLine("客户端断开连接")
            Else
                Console.WriteLine("消息发送成功！")
            End If
        End Sub

        ' 处理消息接收事件
        Private Shared Sub client_MessageReceived(ByVal sender As Object, ByVal e As SocketMessageEventArgs)
            If e.[Error] IsNot Nothing Then
                Console.WriteLine("消息接收失败: " & e.[Error].Message)
                DirectCast(sender, SocketClient).Close()
                Console.WriteLine("客户端断开连接。")
            Else
                ' 等待1秒后，回发消息
                Console.WriteLine("收到消息: " & e.Data)
                Thread.Sleep(1000)
                SendMessage(TryCast(sender, SocketClient), "处理:" & e.Data)
            End If
        End Sub

        ' 使用套接字客户端发送消息
        Private Shared Sub SendMessage(ByVal client As SocketClient, ByVal data As String)
            Try
                client.SendAsync(data)
            Catch ex As Exception
                Console.WriteLine("当发送消息时，发生异常:" & vbLf & ex.Message)
                client.Close()
                Console.WriteLine("客户端断开连接。")
            End Try
        End Sub

        ' 在Silverlight套接字客户端连接套接字服务器时，
        ' 它将会连接到服务器943端口来请求访问策略。
        Shared ReadOnly POLICY_REQUEST As String = "<policy-file-request/>"
        Private Shared Sub socketp_SocketConnected(ByVal sock As Socket)
            ' 在另一线程中运行

            ' 检查是否客户端请求服务器策略
            ' 发送策略
            Dim thread As New Thread(AddressOf CreatePolicySocket)
            thread.Start(sock)
        End Sub
        Private Shared Sub CreatePolicySocket(ByVal sock As Socket)
            Try
                Console.WriteLine("策略客户端连接。")
                Dim receivebuffer As Byte() = New Byte(999) {}
                Dim receivedcount = sock.Receive(receivebuffer)
                Dim requeststr As String = Encoding.UTF8.GetString(receivebuffer, 0, receivedcount)
                If requeststr = POLICY_REQUEST Then
                    sock.Send(policybytes, 0, policybytes.Length, SocketFlags.None)
                End If
            Catch
                Console.WriteLine("策略套接字客户端取得一个错误。")
            Finally
                sock.Close()
                Console.WriteLine("策略客户端断开连接。")
            End Try
        End Sub
    End Class
End Namespace