/************************************* 模块头 ****************************\
 模块:      MainWindow.xaml.cs
 项目:      CSWPFAnimatedImage
 版权 (c) Microsoft Corporation.
 
 这个示例演示了如何展示一系列的照片，就像数字相框的图像滑入效果。
 
 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
 All other rights reserved.

 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CSWPFAnimatedImage
{
    /// <summary>
    /// Window1.xaml的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        int nextImageIndex;
        List<BitmapImage> images = new List<BitmapImage>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 初始化图片集合
            images.Add(new BitmapImage(new Uri("Images/image1.jpg", UriKind.Relative)));
            images.Add(new BitmapImage(new Uri("Images/image2.jpg", UriKind.Relative)));
            images.Add(new BitmapImage(new Uri("Images/image3.jpg", UriKind.Relative)));
            images.Add(new BitmapImage(new Uri("Images/image4.jpg", UriKind.Relative)));

            nextImageIndex = 2;
        }
               
        private void VisbleToInvisible_Completed(object sender, EventArgs e)
        {
            // 把myImage1的Source属性改成下一张要显示的图片，nextImageIndex加1
            this.myImage1.Source = images[nextImageIndex++];

            // 如果nextImageIndex值超过集合中图片最大索引值，将其置0使其下一次
            // 展示第一张图片
            if (nextImageIndex == images.Count)
            {
                nextImageIndex = 0;
            }

            // 获取InvisibleToVisible故事板并开始动画
            Storyboard sb = this.FindResource("InvisibleToVisible") as Storyboard;
            sb.Begin(this);

        }

        private void InvisibleToVisible_Completed(object sender, EventArgs e)
        {
            // 把myImage2的Source属性改成下一张要显示的图片，nextImageIndex加1
            this.myImage2.Source = images[nextImageIndex++];

            // 如果nextImageIndex值超过集合中图片最大索引值，将其置0使其下一次
            // 展示第一张图片
            if (nextImageIndex == images.Count)
            {
                nextImageIndex = 0;
            }

            // 获取VisibleToInvisible故事板并开始动画
            Storyboard sb = this.FindResource("VisibleToInvisible") as Storyboard;
            sb.Begin(this);
        }   

    }
}
