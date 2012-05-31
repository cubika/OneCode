/****************************** 模块头 ******************************\
* 模块名:                 MainPage.xaml.cs
* 项目名:                     CSSL3SocketClient
* 版权 (c) Microsoft Corporation.
* 
* Silverlight套接字客户端后台代码文件。
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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CSSL3SocketClient;

namespace CSSL3SocketClient
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        ~MainPage()
        {
            if (_client != null)
                _client.Close();
        }

        SocketClient _client;

        // 处理“连接”按钮点击事件。
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            btnConnect.IsEnabled = false;
            OpenSocketClientAsync(tboxServerAddress.Text);
        }

        // 处理“发送”按钮点击事件
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_client != null)
                {
                    _client.SendAsync(tboxMessage.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("当发送消息时发生异常:" + ex.Message);
                CloseSocketClient();
                return;
            }
        }

        // 关闭套接字客户端
        void CloseSocketClient()
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            // 更新UI
            btnConnect.IsEnabled = true;
            btnSend.IsEnabled = false;
        }

        // 创建套接字客户端和连接到服务器
        //
        // 为了方便，我们使用127.0.0.1地址在本地连接套接字服务器。
        // 为了能让不同机器上的客户端通过网络访问套接字服务器，
        // 可在文本框中输入服务器ip地址，再点击“连接”。
        // 为了套接字服务器可通过网络访问，请看CCSL3SocketServer的评论。
        bool OpenSocketClientAsync(string ip)
        {
            try
            {
                var endpoint = new IPEndPoint(
                    IPAddress.Parse(ip),
                    4502);

                _client = new SocketClient(
                    endpoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                // 注册事件
                _client.ClientConnected += new EventHandler<SocketMessageEventArgs>(_client_ClientConnected);
                _client.MessageReceived += new EventHandler<SocketMessageEventArgs>(_client_MessageReceived);
                _client.MessageSended += new EventHandler<SocketMessageEventArgs>(_client_MessageSended);

                _client.ConnectAsync(endpoint);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接套接字时发生异常: " + ex.Message);
                CloseSocketClient();
                return false;
            }
        }

        void _client_ClientConnected(object sender, SocketMessageEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate
            {
                // 如果连接成功，则开始接收消息
                if (e.Error == null)
                {
                    try
                    {
                        _client.StartReceiving();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("当创建套接字客户端时发生异常:" + ex.Message);
                        CloseSocketClient();
                        return;
                    }
                    // 更新UI
                    btnConnect.IsEnabled = false;
                    btnSend.IsEnabled = true;
                    tbSocketStatus.Text = "已连接";
                }
                else
                {
                    _client.Close();
                    btnConnect.IsEnabled = true;
                    tbSocketStatus.Text = "连接失败: " + e.Error.Message;
                }
            });
        }

        // 处理消息接收事件
        void _client_MessageSended(object sender, SocketMessageEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate
            {
                if (e.Error == null)
                    tbSocketStatus.Text = "已发送";
                else
                {
                    tbSocketStatus.Text = "发送失败: " + e.Error.Message;
                    CloseSocketClient();
                }
            });
        }

        // 处理消息发送事件
        void _client_MessageReceived(object sender, SocketMessageEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate
            {
                if (e.Error == null)
                {
                    tbSocketStatus.Text = "已接收";
                    lb1.Items.Insert(0, e.Data);
                }
                else
                {
                    tbSocketStatus.Text = "接收失败: " + e.Error.Message;
                    CloseSocketClient();
                }
            });
        }
    }
}
