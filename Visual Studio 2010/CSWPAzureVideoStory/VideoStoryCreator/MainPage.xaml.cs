/********************************* 模块头 *********************************\
* 模块名: MainPage.xaml.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 主页. 包含链接到其他页面的按钮.
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
using Microsoft.Phone.Controls;

namespace VideoStoryCreator
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造器
        public MainPage()
        {
            InitializeComponent();
        }

        private void NewStoryButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "如果您开始一个新的短影, 当前短影中未保存的信息将会丢失. 是否想要继续?"
                , "确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.newStoryStoryboard.Begin();
            }
        }

        private void LastStoryButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(App.CurrentStoryName))
            {
                MessageBox.Show("未找到当前短影.");
                return;
            }
            this.NavigationService.Navigate(new Uri("/ComposePage.xaml", UriKind.Relative));
        }

        private void PreviousStoryButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ChooseStoryPage.xaml", UriKind.Relative));
        }

        private void newStoryOKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.newStoryNameTextBox.Text))
            {
                // 清除内存数据.
                foreach (var photo in App.MediaCollection)
                {
                    photo.ThumbnailStream.Close();
                }
                App.MediaCollection.Clear();
                App.CurrentStoryName = this.newStoryNameTextBox.Text;
                this.closeNewStoryStoryboard.Begin();
                this.NavigationService.Navigate(new Uri("/ComposePage.xaml", UriKind.Relative));
            }
        }

        private void newStoryCancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.closeNewStoryStoryboard.Begin();
        }
    }
}