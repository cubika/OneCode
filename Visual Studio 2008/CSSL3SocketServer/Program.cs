/****************************** 模块头 ******************************\
* 模块名:              Program.cs
* 项目名:              CSSL3SocketServer
* 版权 (c) Microsoft Corporation.
* 
* 套接字服务器应用程序代码文件，能够服务Silverlight和标准的套接字客户端。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using CSSL3SocketServer;

namespace CSSL3SocketServer
{
    class Program
    {
        static byte[] policybytes;
        static void Main(string[] args)
        {
            // 读入crossdomainpolicy.xml文件，并存入字节数组中。
            var filestream = new FileStream("policy.xml", FileMode.Open, FileAccess.Read);
            policybytes = new byte[filestream.Length];
            filestream.Read(policybytes, 0, (int)filestream.Length);
            filestream.Close();

            // 初始化策略套接字监听器
            var socketp = new SocketListener();
            socketp.ListenAsync(943, socketp_SocketConnected);

            // 初始化套接字监听器
            var socketp2 = new SocketListener();
            socketp2.ListenAsync(4502, socketp2_SocketConnected);

            Console.Read();
            
        }

        // 连接的客户端
        static void socketp2_SocketConnected(Socket sock)
        {
            // 创建新的线程来处理客户会话
            new Thread(
                new ThreadStart(delegate
            {
                // 初始化套接字客户端
                var client = new SocketClient(sock);
                try
                {
                    client.MessageReceived += new EventHandler<SocketMessageEventArgs>(client_MessageReceived);
                    client.MessageSended += new EventHandler<SocketMessageEventArgs>(client_MessageSended);
                    // 准备接收
                    client.StartReceiving();
                    Console.WriteLine("客户端已连接!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("当开始接收消息时发生异常:\n" + ex.Message);
                    client.Close();
                }
            })).Start();
        }

        // 处理消息发送事件
        static void client_MessageSended(object sender, CSSL3SocketServer.SocketMessageEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("消息发送失败: " + e.Error.Message);
                ((SocketClient)sender).Close();
                Console.WriteLine("客户端断开连接。");
            }
            else
                Console.WriteLine("消息发送成功！");
        }

        // 处理消息接收事件
        static void client_MessageReceived(object sender, CSSL3SocketServer.SocketMessageEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("消息接收失败: " + e.Error.Message);
                ((SocketClient)sender).Close();
                Console.WriteLine("客户端断开连接。");
            }
            else
            {
                // 等待1秒后，回发消息
                Console.WriteLine("收到消息: " + e.Data);
                Thread.Sleep(1000);
                SendMessage(sender as SocketClient,
                    "处理: " + e.Data);
            }
        }

        // 使用套接字客户端发送消息
        static void SendMessage(SocketClient client, string data)
        {
            try
            {
                client.SendAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("当发送消息时，发生异常:\n" + ex.Message);
                client.Close();
                Console.WriteLine("客户端断开连接。");
            }
        }

        // 在Silverlight套接字客户端连接套接字服务器时，
        // 它将会连接到服务器943端口来请求访问策略。
        static readonly string POLICY_REQUEST = "<policy-file-request/>";
        static void socketp_SocketConnected(Socket sock)
        {
            // 在另一线程中运行
            new Thread(
                new ThreadStart(delegate
                    {
                        try
                        {
                            Console.WriteLine("策略客户端连接。");
                            byte[] receivebuffer = new byte[1000];
                            var receivedcount = sock.Receive(receivebuffer);
                            string requeststr = Encoding.UTF8.GetString(receivebuffer, 0, receivedcount);

                            // 检查是否客户端请求服务器策略
                            if (requeststr == POLICY_REQUEST)
                            {
                                // 发送策略
                                sock.Send(policybytes, 0, policybytes.Length, SocketFlags.None);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("策略套接字客户端取得一个错误。");
                        }
                        finally
                        {
                            sock.Close();
                            Console.WriteLine("策略客户端断开连接。");
                        }
                    })).Start();
        }
    }
}
