/****************************** 模块头 ******************************\
* 模块名:              SocketClient.cs
* 项目名:              CSSL3SocketServer
* 版权 (c) Microsoft Corporation.
* 
* 实现套接字客户端类，它封装了套接字，并提供一组接收/发送字符串类型消息的方法。
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

namespace CSSL3SocketClient
{
    public class SocketMessageEventArgs : EventArgs
    {
        public Exception Error { set; get; }
        public string Data { set; get; }
    }

    public class SocketClient
    {
        // 为异步操作定义了2个事件：
        // 打开，接收和发送
        public event EventHandler<SocketMessageEventArgs> MessageReceived;
        public event EventHandler<SocketMessageEventArgs> MessageSended;
        public event EventHandler<SocketMessageEventArgs> ClientConnected;

        // 设置接收缓冲器的大小
        static readonly int BUFFER_SIZE = 65536;

        // 定义消息结尾字符，用于分隔
        // 字节数组组成字符串消息
        static readonly char EOM_MARKER = (char)0x7F;

        // 封装的套接字
        public Socket InnerSocket { private set; get; }

        
        public SocketClient(Socket socket)
        {
            if (socket==null)
                throw new Exception("套接字不能为空");
            InnerSocket = socket;

            // 初始化字符串编码/解码器
            encoding = encoding = new UTF8Encoding(false, true);

        }
        public SocketClient(AddressFamily addfamily, SocketType socktype,ProtocolType protype)
        {
            InnerSocket = new Socket(addfamily, socktype, protype);
            encoding = encoding = new UTF8Encoding(false, true);
        }

        #region 套接字异步连接

        // 取得套接字连接状态
        public bool Connected
        {
            get
            {
                return InnerSocket.Connected;
            }
        }

        // 关闭套接字
        public void Close()
        {
            InnerSocket.Close();
        }

        /// <summary>
        /// 异步连接套接字到终端。
        /// 可能的异常：
        ///  ArgumentException
        ///  ArgumentNullException
        ///  InvalidOperationException
        ///  SocketException
        ///  NotSupportedException
        ///  ObjectDisposedException
        ///  SecurityException
        ///  详情: http://msdn.microsoft.com/en-us/library/bb538102.aspx
        /// </summary>
        /// <param name="ep">远程终端</param>
        public void ConnectAsync(EndPoint ep)
        {
            if (InnerSocket.Connected)
                return;

            // 初始化socketAsyncEventArgs对象
            // 设置远程连接终端
            var connectEventArgs = new SocketAsyncEventArgs();
            connectEventArgs.RemoteEndPoint = ep;
            connectEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(connectEventArgs_Completed);

            // 调用ConnectAsync方法, 如果该方法返回false
            // 它就意味着返回的结果是异步的
            if (!InnerSocket.ConnectAsync(connectEventArgs))
            // 调用方法来处理连接结果
                ProcessConnect(connectEventArgs);
        }

        // 当connectAsync方法完成时，调用方法处理连接结果
        void connectEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessConnect(e);
        }

        // 调用连接事件ClientConnected来返回结果 
        void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
                OnClientConnected(null);
            else
                OnClientConnected(new SocketException((int)e.SocketError));
        }

        void OnClientConnected(Exception error)
        {
            if(ClientConnected!=null)
            {
                ClientConnected(this, new SocketMessageEventArgs
                {
                    Error = error
                });
            }
        }
        #endregion

        #region 套接字异步发送

        /// <summary>
        /// 使用套接字发送字符串消息
        /// 可能异常:
        ///  FormatException
        ///  ArgumentException
        ///  InvalidOperationException
        ///  NotSupportedException
        ///  ObjectDisposedException
        ///  SocketException
        /// </summary>
        /// <param name="data">要发送的消息</param>
        public void SendAsync(string data)
        {
            // 如果消息包含分隔符EOM_MARKER，
            // 抛出异常
            if (data.Contains(EOM_MARKER))
                throw new Exception("消息中有不允许的字符");

            // 在消息后加结尾分隔符。
            data += EOM_MARKER;

            // 用UTF8编码成字节数组
            var bytesdata = encoding.GetBytes(data);

            // 初始化发送事件变量SendEventArgs
            var sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.SetBuffer(bytesdata, 0, bytesdata.Length);
            sendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(sendEventArgs_Completed);

            // 调用异步发送方法SendAsync，如果方法返回false
            // 就意味着结果是异步的
            if (!InnerSocket.SendAsync(sendEventArgs))
                ProcessSend(sendEventArgs);
        }

        // 当异步发送方法完成时，调用方法处理发送的结果
        void sendEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessSend(e);
        }

        // 调用消息发送事件MessageSended返回结果
        void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
                OnMessageSended(null);
            else
                OnMessageSended(
                    new SocketException((int)e.SocketError));
        }

        void OnMessageSended(Exception error)
        {
            if (MessageSended != null)
                MessageSended(this, new SocketMessageEventArgs
                {
                    Error = error
                });
        }
        #endregion

        #region 套接字异步接收

        // 定义标记来指示接收状态
        bool _isReceiving;

        /// <summary>
        /// 当接收每一条消息时，开始接收套接字的字节和调用
        /// 消息接收事件MessageReceived。
        /// 可能异常：
        ///  ArgumentException
        ///  InvalidOperationException
        ///  NotSupportedException
        ///  ObjectDisposedException
        ///  SocketException
        ///  详情： http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.receiveasync.aspx
        ///  </summary>
        public void StartReceiving()
        {
            
            // 检查套接字是否已经开始接收消息
            if (!_isReceiving)
                _isReceiving = true;
            else
                return;

            try
            {
                // 初始化接收缓冲器
                var buffer = new byte[BUFFER_SIZE];

                // 初始化接收事件变量
                var receiveEventArgs = new SocketAsyncEventArgs();
                receiveEventArgs.SetBuffer(new byte[BUFFER_SIZE], 0, BUFFER_SIZE);
                receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(receiveEventArgs_Completed);

                // 调用异步接收方法ReceiveAsync，如果返回false
                // 就意味着结果是异步返回的
                if (!InnerSocket.ReceiveAsync(receiveEventArgs))
                    ProcessReceive(receiveEventArgs);
            }
            catch (Exception ex)
            {
                StopReceiving();
                throw ex;
            }
        }

        // 停止接收套接字的字节
        public void StopReceiving()
        {
            _isReceiving = false;
        }

        void receiveEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }

        // 处理异步接收完成事件
        string receivemessage = "";
        Encoding encoding;
        int taillength;
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // 当出错时，调用消息接收事件
            // 来传递错误信息给用户
            if (e.SocketError != SocketError.Success)
            {
                StopReceiving();
                OnMessageReceived(null,
                    new SocketException((int)e.SocketError));
                return;
            }

            try
            {
                #region String Decoding
                // 解码字节成字符串
                // 注意UTF-8编码是可变长编码的，我们需要检查字节
                // 数组尾部，以防把一个字母分成两个。
                string receivestr = "";
                // 尝试解码字符串
                try
                { 
                    receivestr = encoding.GetString(e.Buffer, 0, taillength + e.BytesTransferred);
                    // 如果解码成功，重设尾部长度
                    taillength=0;
                }
                // 如果取得解码异常，删除数组尾部，并重新解码
                catch (DecoderFallbackException ex)
                {
                    try{
                        receivestr = encoding.GetString(e.Buffer,0,taillength+e.BytesTransferred-ex.BytesUnknown.Length);
                        // 重设尾部长度
                        taillength=ex.BytesUnknown.Length;
                        ex.BytesUnknown.CopyTo(e.Buffer,0);
                    }
                    // 如果还出现解码异常，就停止接收
                    catch(DecoderFallbackException ex2)
                    {
                        throw new Exception("Message decode failed.",ex2);
                    }

                #endregion
                }
                // 检查消息是否结束
                int eompos = receivestr.IndexOf(EOM_MARKER);
                while (eompos != -1)
                {
                    // 组合成一条完整的消息
                    receivemessage += receivestr.Substring(0, eompos);

                    // 激活接收的消息
                    OnMessageReceived(receivemessage, null);

                    // 取得剩下的字符串
                    receivemessage = "";
                    receivestr = receivestr.Substring(eompos + 1, receivestr.Length - eompos - 1);

                    // 检查字符串是否还有分隔符
                    eompos = receivestr.IndexOf(EOM_MARKER);
                }
                receivemessage += receivestr;

                // 停止接收
                if (!_isReceiving)
                    return;

                // 重设缓冲器开始地址
                e.SetBuffer(taillength, BUFFER_SIZE-taillength);

                // 继续接收
                if (!InnerSocket.ReceiveAsync(e))
                    ProcessReceive(e);
            }
            catch (Exception ex)
            {
                // 通过消息接收事件返回错误
                OnMessageReceived(null, ex);
                StopReceiving();
            }
        }

        void OnMessageReceived(string data,Exception error)
        {
            if (MessageReceived != null)
                MessageReceived(this, new SocketMessageEventArgs
                {
                    Data = data,
                    Error = error
                });
        }
        #endregion
    }
}
