/****************************** 模块头 ******************************\
* 模块名:                MainPage.xaml.cs
* 项目名:                CSSL4WCFNetTcp
* 版权 (c) Microsoft Corporation.
* 
* MainPage 后置代码。
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
using CSSL4WCFNetTcp.ServiceReference1;

namespace CSSL4WCFNetTcp
{
    public partial class MainPage : UserControl,IWeatherServiceCallback
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        bool _subscribed;
        WeatherServiceClient _client;
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _client = new WeatherServiceClient(
                new System.ServiceModel.InstanceContext(this));
            _client.SubscribeCompleted += _client_SubscribeCompleted;
            _client.UnSubscribeCompleted += _client_UnSubscribeCompleted;
        }

        void _client_UnSubscribeCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                _subscribed = false;
                btnSubscribe.Content = "订阅天气报告";
                tbInfo.Text = "";
            }else
                tbInfo.Text = "无法连接到服务器。";
            btnSubscribe.IsEnabled = true;
        }

        void _client_SubscribeCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                _subscribed = true;
                btnSubscribe.Content = "取消订阅天气报告";
                tbInfo.Text = "";
            }
            else
                tbInfo.Text="无法连接到服务器。";
            btnSubscribe.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_subscribed)
            {
                _client.SubscribeAsync();
            }
            else
            {
                _client.UnSubscribeAsync();
            }
            btnSubscribe.IsEnabled = false;
        }

        // 当回调时显示报告。
        public void WeatherReport(string report)
        {
            lbWeather.Items.Insert(
                0,
                new ListBoxItem
                {
                    Content = string.Format("{0} {1}",DateTime.Now, report)
                });
        }
    }
}
