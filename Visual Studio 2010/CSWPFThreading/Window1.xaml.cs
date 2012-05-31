/****************************** Module Header ******************************\
* 模块:      Window1.xaml.cs
* 项目:      CSWPFThreading
* 版权 (c) Microsoft Corporation.
* 
* 此示例演示了两种WPF线程模型。第一种是先将一个长时间运行的线程划分成许多小
* 片段，然后由WPF dispatcher按优先级逐个执行队列中的片段。后台执行的工作项不
* 会影响到前台界面处理，所以看起来像是后台运行的工作项是在另一个线程中执行的。
* 但实际上，所有的工作都是在同一个线程中完成的。如果为单线程图形用户界面应用
* 程序，并希望在UI线程中进行复杂处理的时候同事能保持用户界面的响应，这个技巧
* 非常实用。
* 
* 第二种模型类似于WinForm中的多线程模型。后台工作在另一个线程中执行，并调用
* Dispatcher.BeginInvoke 方法来更新用户界面。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace CSWPFThreading
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }


        #region Long-Running Calculation in UI Thread

        public delegate void NextPrimeDelegate();
        private long num = 3;
        private bool continueCalculating = false;
        private bool fNotAPrime = false;

        private void btnPrimeNumber_Click(object sender, RoutedEventArgs e)
        {
            if (continueCalculating)
            {
                continueCalculating = false;
                btnPrimeNumber.Content = "重新开始";
            }
            else
            {
                continueCalculating = true;
                btnPrimeNumber.Content = "停止";
                btnPrimeNumber.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new NextPrimeDelegate(CheckNextNumber));
            }
        }

        public void CheckNextNumber()
        {
            // 重置标记
            fNotAPrime = false;

            for (long i = 3; i <= Math.Sqrt(num); i++)
            {
                if (num % i == 0)
                {
                    // 设置“不是素数”标记为真
                    fNotAPrime = true;
                    break;
                }
            }

            // 如果是素数
            if (!fNotAPrime)
            {
                tbPrime.Text = num.ToString();
            }

            num += 2;
            if (continueCalculating)
            {
                btnPrimeNumber.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new NextPrimeDelegate(this.CheckNextNumber));
            }
        }

        #endregion


        #region Blocking Operation in Worker Thread

        private delegate void NoArgDelegate();
        private delegate void OneArgDelegate(Int32[] arg);

        private void btnRetrieveData_Click(object sender, RoutedEventArgs e)
        {
            this.btnRetrieveData.IsEnabled = false;
            this.btnRetrieveData.Content = "连接服务器";

            NoArgDelegate fetcher = new NoArgDelegate(
                this.RetrieveDataFromServer);
            fetcher.BeginInvoke(null, null);
        }

        /// <summary>
        /// 在后台线程中获取数据
        /// </summary>
        private void RetrieveDataFromServer()
        {
            // 为网络连接延迟5秒
            Thread.Sleep(5000);

            // 生成要展示的随机数据
            Random rand = new Random();
            Int32[] data = {
                               rand.Next(1000), rand.Next(1000), 
                               rand.Next(1000), rand.Next(1000) 
                           };

            // 在UI线程中安排更新操作
            this.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new OneArgDelegate(UpdateUserInterface),
                data);
        }

        /// <summary>
        /// 将新数据更新到用户界面，此方法在UI线程中执行。
        /// </summary>
        /// <param name="data"></param>
        private void UpdateUserInterface(Int32[] data)
        {
            this.btnRetrieveData.IsEnabled = true;
            this.btnRetrieveData.Content = "从服务器获取数据";
            this.tbData1.Text = data[0].ToString();
            this.tbData2.Text = data[1].ToString();
            this.tbData3.Text = data[2].ToString();
            this.tbData4.Text = data[3].ToString();
        }

        #endregion

    }
}
