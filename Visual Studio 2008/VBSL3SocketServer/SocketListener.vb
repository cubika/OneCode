'****************************** 模块头 ******************************'
' 模块名:              SocketListener.vb
' 项目名:              VBSL3SocketServer
' 版权 (c) Microsoft Corporation.
' 
' 实现套接字监听器类SocketListener，它封装了套接字，
' 提供一个监听和返回连接套接字的简单方法。
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
Imports System.Net
Imports System.Threading
Imports System.Net.Sockets

Namespace VBSL3SocketServer
    Public Delegate Sub GetSocketCallBack(ByVal sock As Socket)
    Public Class SocketListener

        Public Sub ListenAsync(ByVal port As Integer, ByVal callback As GetSocketCallBack)
            ' 在另一个线程中运行
            Dim thread As New Thread(AddressOf Listen)
            thread.Start(New Object() {port, callback})
        End Sub

        Public Sub Listen(ByVal param As Object)
            Dim params As Object() = param
            Dim port As Integer = param(0)
            Dim callback As GetSocketCallBack = param(1)
            ' 为了方便，我们使用了127.0.0.1作为服务器套接字
            ' 地址。这个地址只能在本地访问。
            ' 要让服务器可以通过网络访问，就尝试使用机器的网络地址。
            ' address.

            ' 127.0.0.1 地址
            Dim localEP As New IPEndPoint(&H100007F, port)

            ' 网络ip地址
            'IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            'IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], port);

            Dim listener As New Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)

            Try
                listener.Bind(localEP)
                Console.WriteLine("套接字监听器打开: " & localEP.ToString())
                While True
                    listener.Listen(10)
                    Dim socket As Socket = listener.Accept()

                    ' 通过回调函数返回连接套接字
                    If callback IsNot Nothing Then
                        callback(socket)
                    Else
                        socket.Close()
                        socket = Nothing
                    End If
                End While
            Catch ex As Exception
                Console.Write("发生异常:" & ex.Message)
            End Try
            Console.WriteLine("监听器关闭: " & localEP.ToString())
            listener.Close()
        End Sub
    End Class
End Namespace