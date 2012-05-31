/****************************** 模块头 ************************************\
 * 模块名:  MainForm.cs
 * 项目名:  CSCpuUsage
 * Copyright (c) Microsoft Corporation.
 * 
 * 它是该应用程序的主要窗体，用于处理用户界面的事件和显示CPU使用率的图表.
 * 
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
using System.Windows.Forms;

namespace CSCpuUsage
{
    public partial class MainForm : Form
    {

        int[] xValues = new int[100];

        // 总CPU使用率的监视器.
        TotalCpuUsageMonitor totalCpuUsageMonitor;

        // 某一进程的CPU使用率监视器.
        ProcessCpuUsageMonitor processCpuUsageMonitor;

        public MainForm()
        {
            InitializeComponent();
            for (int i = 0; i < 100; i++)
            {
                xValues[i] = i;
            }
        }

        /// <summary>
        /// 增加可用进程到复合框.
        /// </summary>
        private void cmbProcess_DropDown(object sender, EventArgs e)
        {
            cmbProcess.DataSource = ProcessCpuUsageMonitor.GetAvailableProcesses();
            cmbProcess.SelectedIndex = 0;
        }

        /// <summary>
        /// 处理btnStart的Click事件.
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!chkTotalUsage.Checked && !chkProcessCpuUsage.Checked)
            {
                return;
            }

            try
            {
                StartMonitor();
            }
            catch (Exception ex)
            {
                StopMonitor();
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 处理btnStop的Click事件.
        /// 点击该按钮，会销毁totalCpuUsageMonitor 和 processCpuUsageMonitor.
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMonitor();
        }

        void StartMonitor()
        {
            // 初始化 totalCpuUsageMonitor，并登记CpuUsageValueArrayChanged事件.           
            if (chkTotalUsage.Checked)
            {
                totalCpuUsageMonitor = new TotalCpuUsageMonitor(1000, 100);
                totalCpuUsageMonitor.CpuUsageValueArrayChanged +=
                    new EventHandler<CpuUsageValueArrayChangedEventArg>(
                        totalCpuUsageMonitor_CpuUsageValueArrayChanged);
                totalCpuUsageMonitor.ErrorOccurred += new EventHandler<ErrorEventArgs>(
                    totalCpuUsageMonitor_ErrorOccurred);

            }

            // 初始化processCpuUsageMonitor，并登记 CpuUsageValueArrayChanged事件. 
            if (chkProcessCpuUsage.Checked && !string.IsNullOrEmpty(cmbProcess.SelectedItem as string))
            {
                processCpuUsageMonitor =
                    new ProcessCpuUsageMonitor(cmbProcess.SelectedItem as string, 1000, 100);
                processCpuUsageMonitor.CpuUsageValueArrayChanged +=
                    new EventHandler<CpuUsageValueArrayChangedEventArg>(
                        processCpuUsageMonitor_CpuUsageValueArrayChanged);
                processCpuUsageMonitor.ErrorOccurred += new EventHandler<ErrorEventArgs>(
                    processCpuUsageMonitor_ErrorOccurred);
            }

            // 更新用户界面.
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }



        void StopMonitor()
        {
            if (totalCpuUsageMonitor != null)
            {
                totalCpuUsageMonitor.Dispose();
                totalCpuUsageMonitor = null;
            }

            if (processCpuUsageMonitor != null)
            {
                processCpuUsageMonitor.Dispose();
                processCpuUsageMonitor = null;
            }

            // 更新用户界面.
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        /// <summary>
        /// 激发processCpuUsageMonitor_CpuUsageValueArrayChangedHandler来处理
        /// processCpuUsageMonitor的CpuUsageValueArrayChanged事件.
        /// </summary>
        void processCpuUsageMonitor_CpuUsageValueArrayChanged(object sender,
            CpuUsageValueArrayChangedEventArg e)
        {
            this.Invoke(new EventHandler<CpuUsageValueArrayChangedEventArg>(
                processCpuUsageMonitor_CpuUsageValueArrayChangedHandler), sender, e);
        }

        void processCpuUsageMonitor_CpuUsageValueArrayChangedHandler(object sender,
            CpuUsageValueArrayChangedEventArg e)
        {
            var processCpuUsageSeries = chartProcessCupUsage.Series["ProcessCpuUsageSeries"];
            var values = e.Values;

            // 在图表中显示进程的CPU使用率.
            processCpuUsageSeries.Points.DataBindY(e.Values);

        }

        /// <summary>
        /// 激发processCpuUsageMonitor_ErrorOccurredHandler来处理processCpuUsage-
        /// -Monitor的ErrorOccurred事件.
        /// </summary>
        void processCpuUsageMonitor_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            this.Invoke(new EventHandler<ErrorEventArgs>(
                processCpuUsageMonitor_ErrorOccurredHandler), sender, e);
        }

        void processCpuUsageMonitor_ErrorOccurredHandler(object sender, ErrorEventArgs e)
        {
            if (processCpuUsageMonitor != null)
            {
                processCpuUsageMonitor.Dispose();
                processCpuUsageMonitor = null;

                var processCpuUsageSeries = chartProcessCupUsage.Series["ProcessCpuUsageSeries"];
                processCpuUsageSeries.Points.Clear();
            }
            MessageBox.Show(e.Error.Message);
        }



        /// <summary>
        /// 激发totalCpuUsageMonitor_CpuUsageValueArrayChangedHandler以处理totalCpuUsageMonitor
        /// 的CpuUsageValueArrayChanged事件.
        /// </summary>
        void totalCpuUsageMonitor_CpuUsageValueArrayChanged(object sender,
            CpuUsageValueArrayChangedEventArg e)
        {
            this.Invoke(new EventHandler<CpuUsageValueArrayChangedEventArg>(
                totalCpuUsageMonitor_CpuUsageValueArrayChangedHandler), sender, e);
        }
        void totalCpuUsageMonitor_CpuUsageValueArrayChangedHandler(object sender,
            CpuUsageValueArrayChangedEventArg e)
        {
            var totalCpuUsageSeries = chartTotalCpuUsage.Series["TotalCpuUsageSeries"];
            var values = e.Values;

            //在图表中显示总CPU使用率.
            totalCpuUsageSeries.Points.DataBindY(e.Values);

        }

        /// <summary>
        /// 激发 totalCpuUsageMonitor_ErrorOccurredHandler以处理totalCpuUsageMonitor的
        /// ErrorOccurred事件.
        /// </summary>
        void totalCpuUsageMonitor_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            this.Invoke(new EventHandler<ErrorEventArgs>(
                totalCpuUsageMonitor_ErrorOccurredHandler), sender, e);
        }

        void totalCpuUsageMonitor_ErrorOccurredHandler(object sender, ErrorEventArgs e)
        {
            if (totalCpuUsageMonitor != null)
            {
                totalCpuUsageMonitor.Dispose();
                totalCpuUsageMonitor = null;

                var totalCpuUsageSeries = chartTotalCpuUsage.Series["TotalCpuUsageSeries"];
                totalCpuUsageSeries.Points.Clear();
            }
            MessageBox.Show(e.Error.Message);
        }
    }
}
