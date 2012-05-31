/********************************* 模块头 **********************************\
* 模块名:  MainPage.xaml.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* MainPage后台代码.
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
using System.Windows;
using System.Windows.Input;
using AzureBingMaps.DAL;
using GeocodeServiceReference;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace WindowsPhoneClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 你的Bing Maps身份证明.
        private string _mapCredential = "[your credential]";

        private GeocodeServiceClient _geocodeClient = new GeocodeServiceClient();
        private Point clickedPoint;

        // 构造器
        public MainPage()
        {
            InitializeComponent();

            // 设定listbox控件数据内容为示例数据
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            this.map.CredentialsProvider = new ApplicationIdCredentialsProvider(this._mapCredential);
            this._geocodeClient.ReverseGeocodeCompleted += new EventHandler<ReverseGeocodeCompletedEventArgs>(GeocodeClient_ReverseGeocodeCompleted);
        }

        // ViewModel项目载入数据
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            this.mapItems.ItemsSource = App.DataSource.TravelItems;
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            App.DataSource.DataLoaded += new EventHandler(DataSource_DataLoaded);
        }

        void DataSource_DataLoaded(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.mapItems.ItemsSource = App.DataSource.TravelItems;
            }));
        }

        /// <summary>
        /// Bing Maps Geocode服务的回调方法.
        /// </summary>
        private void GeocodeClient_ReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Result.Results.Count > 0)
            {
                // 我们只关心第一个结果.
                var result = e.Result.Results[0];
                Travel travel = new Travel()
                {
                    // PartitionKey代表用户.
                    // 然而, 客户端可以如下演示般发送一个假身份.
                    // 为了防止客户端使用假身份,
                    // 我们的服务一直在服务器端查询真实的身份.
                    PartitionKey = "fake@live.com",
                    RowKey = Guid.NewGuid(),
                    Place = result.DisplayName,
                    Time = DateTime.Now,
                    // Latitude/Longitude通过服务获得地址,
                    // 因此可能不是正巧为所点击的地址.
                    Latitude = result.Locations[0].Latitude,
                    Longitude = result.Locations[0].Longitude
                };
                // 添加到ObservableCollection.
                App.DataSource.AddToTravel(travel);
            }
        }

        /// <summary>
        /// Windows Phone map控件并不支持Click,
        /// 因此我们必须捕获MouseLeftButtonDown/Up.
        /// 这些事件将在用户触摸屏幕时触发.
        /// </summary>
        private void map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.clickedPoint = e.GetPosition(this.map);
        }

        private void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 检查手指是否移动.
            Point clickedPoint = e.GetPosition(this.map);
            if (Math.Abs(clickedPoint.X - this.clickedPoint.X) < 5 && Math.Abs(clickedPoint.Y - this.clickedPoint.Y) < 5)
            {
                // 调用Bing Maps Geocode服务获得最近的位置.
                ReverseGeocodeRequest request = new ReverseGeocodeRequest() { Location = map.ViewportPointToLocation(e.GetPosition(this.map)) };
                request.Credentials = new Credentials() { ApplicationId = this._mapCredential };
                _geocodeClient.ReverseGeocodeAsync(request);
            }
        }

        /// <summary>
        /// 应用程序bar事件句柄: 导航到ListPage.
        /// </summary>
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ListPage.xaml", UriKind.Relative));
        }
    }
}