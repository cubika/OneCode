/********************************* 模块头 *********************************\
* 模块名: ChooseMediaPage.xaml.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 这个页面允许用户选择图片.
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using VideoStoryCreator.Models;
using VideoStoryCreator.Transitions;
using VideoStoryCreator.ViewModels;

namespace VideoStoryCreator
{
    public partial class ChooseMediaPage : PhoneApplicationPage
    {
        private ObservableCollection<ChoosePhotoViewModel> _photoDataSource;
        private List<ChoosePhotoViewModel> _selectedPhotos;
        private int _currentPage = 0;

        // TODO: 设计考虑：目前硬编码20。
        // 如果我们将所有的硬编码值到一个地方，
        // 所以在未来，它将会更容易地支持用户配置？
        private int _pageSize = 20;

        public ChooseMediaPage()
        {
            InitializeComponent();

            this._photoDataSource = new ObservableCollection<ChoosePhotoViewModel>();
            this._selectedPhotos = new List<ChoosePhotoViewModel>();
            this.GetPicturesForCurrentPage();
            this.MediaListBox.ItemsSource = _photoDataSource;
        }

        private void GetPicturesForCurrentPage()
        {
            // Page值不能小于0.
            if (this._currentPage < 0)
            {
                this._currentPage = 0;
                return;
            }

            MediaLibrary mediaLibrary = new MediaLibrary();

            // 已经是最后一个page.
            int pageCount = mediaLibrary.Pictures.Count / this._pageSize;
            if (this._currentPage > pageCount)
            {
                this._currentPage = pageCount;
                return;
            }

            // 存储选择.
            this.StoreSelection();

            // 数据源清空
            this._photoDataSource.Clear();

            var picturesOnCurrentPage = mediaLibrary.Pictures.Skip(this._currentPage * this._pageSize).Take(this._pageSize);
            foreach (var picture in picturesOnCurrentPage)
            {
                Stream pictureStream = picture.GetThumbnail();
                ChoosePhotoViewModel viewModel = new ChoosePhotoViewModel()
                {
                    Name = picture.Name,
                    MediaStream = pictureStream
                };

                // 在PhotoViewModel, 我们重写photo名称
                // 如果图片重名，将会返回true
                if (this._selectedPhotos.Contains(viewModel))
                {
                    viewModel.IsSelected = true;
                }

                _photoDataSource.Add(viewModel);
            }
        }

        /// <summary>
        /// 当前页面上选中的照片添加到选定的照片列表，
        /// 并关闭未选中的照片。
        /// </summary>
        private void StoreSelection()
        {
            foreach (ChoosePhotoViewModel photo in this._photoDataSource)
            {
                if (photo.IsSelected && !this._selectedPhotos.Contains(photo))
                {
                    this._selectedPhotos.Add(photo);
                }
                // 如果图片没有关闭，关闭Stream流
                else
                {
                    photo.MediaStream.Close();
                }
            }
        }

        private void OKButton_Click(object sender, System.EventArgs e)
        {
            this.StoreSelection();

            // 添加所有选中的图片到App.MediaCollection.
            foreach (ChoosePhotoViewModel photo in this._selectedPhotos)
            {
                Photo photoModel = new Photo()
                {
                    Name = photo.Name,
                    ThumbnailStream = photo.MediaStream,
                    PhotoDuration = TimeSpan.FromSeconds(5d),
                    Transition = TransitionFactory.CreateDefaultTransition()
                };
                if (!App.MediaCollection.Contains(photoModel))
                {
                    App.MediaCollection.Add(photoModel);
                }
            }

            // 回到调用页面
            this.NavigationService.GoBack();
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            // 关掉thumbnail streams.
            foreach (ChoosePhotoViewModel photo in this._photoDataSource)
            {
                photo.MediaStream.Close();
            }

            // 回到调用页面
            this.NavigationService.GoBack();
        }

        private void PreviousPageButton_Click(object sender, System.EventArgs e)
        {
            this._currentPage--;
            this.GetPicturesForCurrentPage();
        }

        private void NextPageButton_Click(object sender, System.EventArgs e)
        {
            this._currentPage++;
            this.GetPicturesForCurrentPage();
        }
    }
}