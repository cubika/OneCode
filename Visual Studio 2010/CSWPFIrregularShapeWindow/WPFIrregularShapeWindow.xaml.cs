/********************************** Module Header **********************************\
 * 模块名:  WPFIrregularShapeWindow.xaml.cs
 * 项目名:      CSWPFIrregularShapeWindow
 * Copyright (c) Microsoft Corporation.
 *
 * WPFIrregularShapeWindow.xaml.cs 文件定义了一个继承自 Window 类 的 MainWindow 子类，其职责是
 * 显示5个不规则窗体.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
 * EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System.Windows;
using System.Windows.Controls;
using System;

namespace CSWPFIrregularShapeWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WPFIrregularShapeWindow : Window
    {
        public int height;
        public int width;
        public WPFIrregularShapeWindow()
        {
            InitializeComponent();
        }


        private void Window_Click(object sender, RoutedEventArgs e)
        {

            // 为子窗体的宽和高生成随机数目的是避免子窗体互相重叠.
            Random ro = new Random();
            height = ro.Next(500);
            width = ro.Next(500);
            Button btn = e.Source as Button;
            if (btn != null)
            {
                switch (btn.Tag as string)
                {
                    case "Ellipse Window":
                        EllipseWindow ellipseWindow = new EllipseWindow();

                        // 显示椭圆子窗体
                        ellipseWindow.Left = this.Left + this.Width + width;
                        ellipseWindow.Top = this.Top + this.Height + height;
                        ellipseWindow.Show();
                        break;
                    case "Rounded Corners Window":
                        RoundedCornersWindow roundedCornersWindow = new RoundedCornersWindow();

                        // 显示圆角子窗体
                        roundedCornersWindow.Left = this.Left + this.Width + width;
                        roundedCornersWindow.Top = this.Top + this.Height + height;

                        roundedCornersWindow.Show();
                        break;
                    case "Triangle Corners Window":
                        TriangleCornersWindow triangleCornersWindow = new TriangleCornersWindow();

                        // 显示三角形子窗体
                        triangleCornersWindow.Left = this.Left + this.Width + width;
                        triangleCornersWindow.Top = this.Top + this.Height + height;

                        triangleCornersWindow.Show();
                        break;
                    case "Popup Corners Window":
                        PopupCornersWindow popupCornersWindow = new PopupCornersWindow();

                        // 显示对话子窗体
                        popupCornersWindow.Left = this.Left + this.Width + width;
                        popupCornersWindow.Top = this.Top + this.Height + height;

                        popupCornersWindow.Show();
                        break;
                    case "Picture Based Windows":
                        PictureBasedWindow picturebasedWindows = new PictureBasedWindow();

                        // 显示图片背景子窗体
                        picturebasedWindows.Left = this.Left + this.Width + width;
                        picturebasedWindows.Top = this.Top + this.Height + height;

                        picturebasedWindows.Show();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
