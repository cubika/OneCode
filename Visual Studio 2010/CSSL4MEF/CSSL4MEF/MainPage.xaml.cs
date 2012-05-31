/********************************* 模块头 **********************************\
* 模块名:                MainPage.xaml.cs
* 项目:                    CSSL4MEF
* Copyright (c) Microsoft Corporation.
* 
* MainPage 隐藏代码文件.
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
using System.Windows.Media;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Windows.Data;

namespace CSSL4MEF
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            // 初始化并绑定文本的 datamodel
            var config = new TextConfig
            {
                Text = "测试",
                FontColor = Colors.Blue,
                FontSize = 16
            };
            this.DataContext = config;

            // 关联当前实例的导入部分.
            CompositionInitializer.SatisfyImports(this);
        }

        // 导入 CatalogService, 使用它动态加载xap.
        [Import]
        public IDeploymentCatalogService CatalogService { set; get; }

        // 当点击按钮后导入 ColorConfigControl.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CatalogService.AddXap("ConfigControl.Extension.xap");
        }
    }
}
