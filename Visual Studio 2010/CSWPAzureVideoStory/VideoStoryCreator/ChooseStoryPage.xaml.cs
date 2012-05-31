/********************************* 模块头 *********************************\
* 模块名: ChooseStoryPage.xaml.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 用户打开现有短影的页面.
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
    public partial class ChooseStoryPage : PhoneApplicationPage
    {
        public ChooseStoryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.StoryListBox.ItemsSource = PersistenceHelper.EnumerateStories();
            base.OnNavigatedTo(e);
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.StoryListBox.SelectedItem != null || this.StoryListBox.SelectedItem is string)
            {
                string storyName = (string)this.StoryListBox.SelectedItem;
                try
                {
                    // 清除内存数据.
                    foreach (var photo in App.MediaCollection)
                    {
                        photo.ThumbnailStream.Close();
                    }
                    App.MediaCollection.Clear();
                    App.CurrentStoryName = storyName;
                    PersistenceHelper.ReadStoryFile(storyName);
                    this.NavigationService.Navigate(new Uri("/ComposePage.xaml", UriKind.Relative));
                }
                catch
                {
                    MessageBox.Show("无法载入短影. 文件似乎已经损坏.");
                }
            }
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}