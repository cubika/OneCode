/********************************* 模块头 **********************************\
* 模块名:  App.xaml.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* App后台代码.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WindowsPhoneClient
{
    public partial class App : Application
    {
        private static MainViewModel viewModel = null;
        internal static DataSource DataSource = new DataSource();

        /// <summary>
        /// 用来绑定视图的静态ViewModel.
        /// </summary>
        /// <returns>MainViewModel对象.</returns>
        public static MainViewModel ViewModel
        {
            get
            {
                // 有需要之前延迟视图模型的创建
                if (viewModel == null)
                    viewModel = new MainViewModel();

                return viewModel;
            }
        }

        /// <summary>
        /// 提供Phone应用程序的根帧的快速访问.
        /// </summary>
        /// <returns>Phone应用程序的根帧.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Application对象构造器.
        /// </summary>
        public App()
        {
            // 未捕获异常的全局句柄. 
            UnhandledException += Application_UnhandledException;

            // 调试时显示图形化存档信息.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 显示当前帧率指针.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // 显示应用程序在每一帧中重绘的区域.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // 启用非产品级分析可视化模式, 
                // 用彩色的覆盖显示了一个页面中正在使用GPU加速区域.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // 标准Silverlight初始化
            InitializeComponent();

            // Phone相关初始化
            InitializePhoneApplication();
        }

        // 应用程序启动时执行的代码（例如 开始）
        // 此代码将不会在应用程序重新激活时执行
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            DataSource.LoadDataAsync();
        }

        // 应用程序被激活时执行的代码（转到前台）
        // 此代码将不会在应用程序首次加载时执行
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // 确保应用程序状态已被正确恢复
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        // 应用程序非活动时执行的代码（转到后台）
        // 此代码将不会在应用程序关闭时执行
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // 确保所需应用程序状态被正确保存.
        }

        // 应用程序关闭时执行的代码 (例如用户点击Back)
        // 此代码将不会在应用程序非活动时执行
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // 导航失败时执行的代码 
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 导航失败;中断进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        // 处理未捕获的异常的代码
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 发生未捕获的异常; 中断进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // 避免重复初始化
        private bool phoneApplicationInitialized = false;

        // 请不要向这个方法添加其他代码
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // 建立框架，但暂不设置RootVisual，这使直到应用程序准备渲染前启动画面仍然有效.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // 处理浏览失败
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // 确保我们不再初始化
            phoneApplicationInitialized = true;
        }

        // 请不要向这个方法添加其他代码
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // 设置RootVisual允许应用程序渲染
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // 删除不再被使用的句柄
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}