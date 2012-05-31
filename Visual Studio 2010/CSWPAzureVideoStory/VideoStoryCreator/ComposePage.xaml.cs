/********************************* 模块头 *********************************\
* 模块名: ComposePage.xaml.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 生成短影的页面.
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
using System.Windows;
using Microsoft.Phone.Controls;
using VideoStoryCreator.Models;
using VideoStoryCreator.ServiceLocators;
using VideoStoryCreator.Transitions;
using VideoStoryCreator.ViewModels;

namespace VideoStoryCreator
{
    public partial class ComposePage : PhoneApplicationPage
    {
        private ObservableCollection<ComposePhotoViewModel> _photoDataSource;
        private ComposePhotoViewModel _viewModelBackup;

        public ComposePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // 准备数据源.
            this._photoDataSource = new ObservableCollection<ComposePhotoViewModel>();
            this.nameTextBox.Text = App.CurrentStoryName;
            foreach (Photo photo in App.MediaCollection)
            {
                _photoDataSource.Add(ComposePhotoViewModel.CreateFromModel(photo));
            }
            this.photoListBox.ItemsSource = this._photoDataSource;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.UpdateModels();
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 绑定到试图模型的控件.
        /// 因此我们需要显式更新所需基本模型 .
        /// </summary>
        private void UpdateModels()
        {
            foreach (ComposePhotoViewModel viewModel in this._photoDataSource)
            {
                viewModel.UpdateModel();
            }
        }

        private void PreviewButton_Click(object sender, System.EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/PreviewPage.xaml", UriKind.Relative));
        }

        private void EditPhotoButton_Click(object sender, System.EventArgs e)
        {
            if (this.photoListBox.SelectedItem != null && this.photoListBox.SelectedItem is ComposePhotoViewModel)
            {
                this.photoListBox.IsEnabled = false;

                // 备份视图模型, 因此我们可以撤销更新操作.
                this._viewModelBackup = ((ComposePhotoViewModel)this.photoListBox.SelectedItem).CopyTo();
                this.ShowEditPanelStoryboard.Begin();
            }
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.photoListBox.IsEnabled = true;
            this.CloseEditPanelStoryboard.Begin();
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // 恢复视图模型备份.
            if (this._viewModelBackup != null)
            {
                ((ComposePhotoViewModel)this.photoListBox.SelectedItem).CopyFrom(this._viewModelBackup);
            }
            this.photoListBox.IsEnabled = true;
            this.CloseEditPanelStoryboard.Begin();
        }

        private void AddPhotoButton_Click(object sender, System.EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ChooseMediaPage.xaml", UriKind.Relative));
        }

        private void RemovePhotoButton_Click(object sender, System.EventArgs e)
        {
            // 移除选定项目, 关闭对应流.
            if (this.photoListBox.SelectedItem != null && this.photoListBox.SelectedItem is ComposePhotoViewModel)
            {
                ComposePhotoViewModel photo = (ComposePhotoViewModel)this.photoListBox.SelectedItem;
                photo.MediaStream.Close();
                this._photoDataSource.Remove(photo);
                photo.RemoveModel();
            }
        }

        private void nameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            // 更新短影名.
            if (App.CurrentStoryName != this.nameTextBox.Text && this.nameTextBox.Text != null)
            {
                if (IsolatedStorageHelper.FileExists(this.nameTextBox.Text + ".xml"))
                {
                    if (MessageBox.Show(
                        "同样名称的短影已经存在. 是否覆盖?",
                        "确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        this.RenameStory();
                    }
                    // 回滚旧名.
                    else
                    {
                        this.nameTextBox.Text = App.CurrentStoryName;
                    }
                }
                else
                {
                    this.RenameStory();
                }
            }
        }

        /// <summary>
        /// 重命名短影.
        /// </summary>
        private void RenameStory()
        {
            IsolatedStorageHelper.RenameFile(App.CurrentStoryName + ".xml", this.nameTextBox.Text + ".xml");
            App.CurrentStoryName = this.nameTextBox.Text;
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            // 确保我们正在使用最新的数据.
            this.UpdateModels();
            StoryServiceLocator locator = new StoryServiceLocator();
            locator.StoryUploaded += new EventHandler(locator_StoryUploaded);
            locator.UploadStory();
        }

        void locator_StoryUploaded(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show("短影已成功上传至云.");
            });
        }

        private void TransitionList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.photoListBox.SelectedItem != null && this.photoListBox.SelectedItem is ComposePhotoViewModel)
            {
                // 更新设计器以显示选定特效所需的额外的控件.
                // UI是一个内容绑定到视图模型的TransitionDesigner属性的ContentControl.
                ComposePhotoViewModel photo = (ComposePhotoViewModel)this.photoListBox.SelectedItem;
                photo.TransitionDesigner = TransitionFactory.CreateTransitionDesigner(this.transitionList.SelectedItem.ToString());
            }
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}