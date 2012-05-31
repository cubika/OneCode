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

using System;
using System.Windows;

namespace SilverlightClient
{
    public partial class App : Application
    {
        internal static bool IsAuthenticated = false;
        internal static string WelcomeMessage = null;

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IsAuthenticated = bool.Parse(e.InitParams["IsAuthenticated"]);
            if (IsAuthenticated)
            {
                WelcomeMessage = e.InitParams["WelcomeMessage"];
            }
            this.RootVisual = new MainPage();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // 如果程序运行在调试器外部异常报告将使用浏览器的异常处理机制. 
            // 在IE的场合将会在状态栏显示一个黄色的警告图标
            // Firefox将显示一个脚本错误.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: 这会使应用程序在异常已被抛出但未被捕捉时继续运行. 
                //  产品级应用程序的场合这个错误处理必须被替换为报告错误到网站并中止应用程序.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"未捕获的Silverlight应用程序错误 " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
