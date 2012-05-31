/********************************* 模块头 **********************************\
* 模块名:  ListPage.xaml.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* ListPage后台代码.
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
using System.Windows.Controls;
using AzureBingMaps.DAL;
using Microsoft.Phone.Controls;

namespace WindowsPhoneClient
{
    public partial class ListPage : PhoneApplicationPage
    {
        public ListPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.placeList.ItemsSource = App.DataSource.TravelItems;
        }

        /// <summary>
        /// 当DatePicker的值变更时, 更新数据源.
        /// </summary>
        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            Travel travel = datePicker.DataContext as Travel;
            if (travel != null && travel.Time != datePicker.Value)
            {
                travel.Time = datePicker.Value.Value;
                App.DataSource.UpdateTravel(travel);
            }
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
                App.DataSource.RemoveFromTravel(travel);
            }
        }

        /// <summary>
        /// 保存变更.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            App.DataSource.SaveChanges();
        }

        /// <summary>
        /// 应用程序bar事件句柄: 导航到MainPage.
        /// </summary>
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}