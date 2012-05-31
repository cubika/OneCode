/********************************* 模块头 *********************************\
* 模块名: PreviewPage.xaml.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 此页面允许用户预览使用DispatcherTimer和Storyboard的story.
* 实际上它并不编码视频的story
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
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using VideoStoryCreator.Models;

namespace VideoStoryCreator
{
    public partial class PreviewPage : PhoneApplicationPage
    {
        private DispatcherTimer _dispatcherTimer;
        private int _currentImageIndex;
        private PictureCollection _pictureCollection;

        public PreviewPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            int mediaCount = App.MediaCollection.Count;
            if (mediaCount < 2)
            {
                throw new InvalidOperationException("您必须至少选择两个媒体文件.");
            }

            MediaLibrary mediaLibrary = new MediaLibrary();
            this._pictureCollection = mediaLibrary.Pictures;

            // 设置前后台的图片.
            if (App.MediaCollection[0].ResizedImage != null)
            {
                this.foregroundImage.Source = App.MediaCollection[0].ResizedImage;
            }
            else
            {
                WriteableBitmap bmp = this.GetResizedImage(App.MediaCollection[0]);
                this.foregroundImage.Source = bmp;
            }
            if (App.MediaCollection[1].ResizedImage != null)
            {
                this.backgroundImage.Source = App.MediaCollection[1].ResizedImage;
            }
            else
            {
                WriteableBitmap bmp = this.GetResizedImage(App.MediaCollection[1]);
                this.backgroundImage.Source = bmp;
            }

            // 设置_currentImageIndex为2, 下次我们可以从App.MediaCollection[2]开始.
            this._currentImageIndex = 2;

            this._dispatcherTimer = new DispatcherTimer();
            this._dispatcherTimer.Interval = App.MediaCollection[0].PhotoDuration;
            this._dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            this._dispatcherTimer.Start();
        }

        void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.backgroundImage.Opacity = 1;
            VideoStoryCreator.Transitions.ITransition transition = App.MediaCollection[this._currentImageIndex - 1].Transition;

            // 显示transition若它不为空.
            // 这个task被委托到transition类.
            if (transition != null)
            {
                transition.ForegroundElement = this.foregroundImage;
                transition.BackgroundElement = this.backgroundImage;
                transition.TransitionCompleted += new EventHandler(TransitionCompleted);
                transition.Begin();
                this._dispatcherTimer.Stop();
            }
            // 没有transition，从下张图片开始.
            else
            {
                this._dispatcherTimer.Stop();
                this.backgroundImage.SetValue(Canvas.ZIndexProperty, 2);
                this.foregroundImage.SetValue(Canvas.ZIndexProperty, 0);
                this.GoToNext();
            }
        }

        void TransitionCompleted(object sender, EventArgs e)
        {
            VideoStoryCreator.Transitions.ITransition transition = (VideoStoryCreator.Transitions.ITransition)sender;

            // 如果没有被transition修改，则修改z-index.
            if (!transition.ImageZIndexModified)
            {
                this.backgroundImage.SetValue(Canvas.ZIndexProperty, 2);
                this.foregroundImage.SetValue(Canvas.ZIndexProperty, 0);
            }
            transition.Stop();

            this.GoToNext();
        }

        /// <summary>
        /// 显示下一张图片.
        /// </summary>
        private void GoToNext()
        {
            // 这样的逻辑是清楚的参考切换。
            // 当传递给过渡类，前景图像，当前图像，
            // 而背景图像显示的新形象，
            Image tempImg = this.foregroundImage;
            this.foregroundImage = this.backgroundImage;
            this.backgroundImage = tempImg;

            // 更多图片可用, 动画继续.
            if (_currentImageIndex < App.MediaCollection.Count)
            {
                if (App.MediaCollection[this._currentImageIndex].ResizedImage != null)
                {
                    this.backgroundImage.Source = App.MediaCollection[this._currentImageIndex].ResizedImage;
                }
                else
                {
                    WriteableBitmap bmp = this.GetResizedImage(App.MediaCollection[this._currentImageIndex]);
                    this.backgroundImage.Source = bmp;
                }

                this._dispatcherTimer.Interval = App.MediaCollection[_currentImageIndex].PhotoDuration;
                this._currentImageIndex++;
                this._dispatcherTimer.Start();
            }
        }

        /// <summary>
        /// 从XNA媒体库中获取的原始图像。
        /// 并调整其大小的目标分辨率。
        /// 调用BitmapHelper.GetResizedImage内部。
        /// 不同的是此方法返回WriteableBitmap的，
        /// 而BitmapHelper.GetResizedImage返回流。
        /// 此方法还设置Photo.ResizedImage和Photo.ResizedImageStream。
        /// </summary>
        /// <param name="photo">图片需要被重设.</param>
        /// <returns>一个WriteableBitmap图片需要被重新设置.</returns>
        private WriteableBitmap GetResizedImage(Photo photo)
        {
            Stream resizedImageStream = BitmapHelper.GetResizedImage(photo.Name);
            WriteableBitmap resizedImage = new WriteableBitmap(BitmapHelper.ResizedImageWidth, BitmapHelper.ResizedImageHeight);
            resizedImage.SetSource(resizedImageStream);
            photo.ResizedImageStream = resizedImageStream;
            photo.ResizedImage = resizedImage;
            return resizedImage;
        }
    }
}