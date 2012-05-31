/********************************* 模块头 **********************************\
* 模块名:  MainPage.xaml.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* MainPage的后台代码.
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
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl;
using SilverlightClient.GeocodeServiceReference;
using SilverlightClient.TravelDataServiceReference;

namespace SilverlightClient
{
    public partial class MainPage : UserControl
    {
        // 你的Bing Maps身份证明.
        private string _mapCredential = "[your credential]";
        private GeocodeServiceClient _geocodeClient = new GeocodeServiceClient("CustomBinding_IGeocodeService");
        // 因为这个Silverlight客户端和Web Role服务在同一主机上,
        // 我们使用相对地址.
        private TravelDataServiceContext _dataServiceContext =
            new TravelDataServiceContext(new Uri("../DataService/TravelDataService.svc", UriKind.Relative));
        private ObservableCollection<Travel> _travelItems = new ObservableCollection<Travel>();

        public MainPage()
        {
            InitializeComponent();
            // 根据身份验证显示登入连接或欢迎信息.
            if (App.IsAuthenticated)
            {
                this.LoginLink.Visibility = System.Windows.Visibility.Collapsed;
                this.WelcomeTextBlock.Visibility = System.Windows.Visibility.Visible;
                this.WelcomeTextBlock.Text = App.WelcomeMessage;
            }
            else
            {
                this.LoginLink.Visibility = System.Windows.Visibility.Visible;
                this.WelcomeTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            }
            this.map.CredentialsProvider = new ApplicationIdCredentialsProvider(this._mapCredential);
            this._geocodeClient.ReverseGeocodeCompleted +=
                new EventHandler<ReverseGeocodeCompletedEventArgs>(GeocodeClient_ReverseGeocodeCompleted);
            this.LoginLink.NavigateUri =
                new Uri(Application.Current.Host.Source, "../LoginPage.aspx?returnpage=SilverlightClient.aspx");
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            // 查询数据.
            this._dataServiceContext.Travels.BeginExecute(result =>
            {
                this._travelItems = new ObservableCollection<Travel>
                    (this._dataServiceContext.Travels.EndExecute(result).ToList().OrderBy(t => t.Time));
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.mapItems.ItemsSource = this._travelItems;
                    this.placeList.ItemsSource = this._travelItems;
                }));
            }, null);
        }

        private void map_MouseClick(object sender, Microsoft.Maps.MapControl.MapMouseEventArgs e)
        {
            // 调用Bing Maps Geocode服务获得最近的位置.
            ReverseGeocodeRequest request = new ReverseGeocodeRequest()
            {
                Location = map.ViewportPointToLocation(e.ViewportPoint)
            };
            request.Credentials = new Credentials() { Token = this._mapCredential };
            _geocodeClient.ReverseGeocodeAsync(request);
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
                    // 因此可能不是正巧为所点击的地址
                    Latitude = result.Locations[0].Latitude,
                    Longitude = result.Locations[0].Longitude
                };
                // 添加到ObservableCollection.
                this._travelItems.Add(travel);
                this._dataServiceContext.AddObject("Travels", travel);
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            Travel travel = datePicker.DataContext as Travel;
            if (travel != null && travel.Time != datePicker.SelectedDate.Value)
            {
                travel.Time = datePicker.SelectedDate.Value;
                this._dataServiceContext.UpdateObject(travel);
            }
        }

        /// <summary>
        /// 保存变更.
        /// </summary>
        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // 我们的数据服务提供器实现并不支持MERGE, 所以就执行一次完全更新(PUT).
            this._dataServiceContext.BeginSaveChanges
                (SaveChangesOptions.ReplaceOnUpdate, new AsyncCallback((result) =>
            {
                var response = this._dataServiceContext.EndSaveChanges(result);
            }), null);
        }

        /// <summary>
        /// 从数据源删除项目.
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton button = (HyperlinkButton)sender;
            Travel travel = button.DataContext as Travel;
            if (travel != null)
            {
                this._travelItems.Remove(travel);
                this._dataServiceContext.DeleteObject(travel);
            }
        }
    }
}
