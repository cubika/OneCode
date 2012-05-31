/****************************** 模块头 ******************************\
* 模块名:              SocketListener.cs
* 项目名:                  CSSL3SocketServer
* 版权 (c) Microsoft Corporation.
* 
* 实现套接字监听器类SocketListener，它封装了套接字，提供一个监听和返回连接套接字的简单方法。
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
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace CSSL3SocketServer
{
    public delegate void GetSocketCallBack(Socket sock);
    public class SocketListener
    {
        public void ListenAsync(int port, GetSocketCallBack callback)
        {
            // 在另一个线程中运行
            new Thread(
                new ThreadStart(delegate
                {
                    Listen(port, callback);
                })).Start();
        }
        public void Listen(int port, GetSocketCallBack callback)
        {
            // 为了方便，我们使用了127.0.0.1作为服务器套接字
            // 地址。这个地址只能在本地访问。
            // 要让服务器可以通过网络访问，就尝试使用机器的网络地址。

            // 127.0.0.1 地址
            IPEndPoint localEP = new IPEndPoint(0x0100007f, port);

            // 网络ip地址
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], port);

            Socket listener = new Socket(localEP.Address.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                listener.Bind(localEP);
                Console.WriteLine("套接字监听器打开： "+localEP);
                while (true)
                {
                    listener.Listen(10);
                    Socket socket = listener.Accept();

                    // 通过回调函数返回连接套接字
                    if (callback != null)
                        callback(socket);
                    else
                    {
                        socket.Close();
                        socket = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("发生异常:" + ex.Message);
            }
            Console.WriteLine("监听器关闭： "+localEP);
            listener.Close();
        }
    }
}
