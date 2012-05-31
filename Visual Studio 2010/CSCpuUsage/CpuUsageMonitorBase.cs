/****************************** 模块头 **************************************\
* 模块名:  CpuUsageMonitorBase.cs
* 项目名:  CSCpuUsage
* Copyright (c) Microsoft Corporation.
* 
* 它是ProcessCpuUsageMonitor和TotalCpuUsageMonitor的基类. 它提供了监视器的基本字
* 段、事件、功能和功能，例如Timer, PerformanceCounter和IDisposable接口.
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
using System.Diagnostics;
using System.Threading;

namespace CSCpuUsage
{
    public abstract class CpuUsageMonitorBase : IDisposable
    {
        object locker = new object();

        // 指示实例是否被释放.
        bool disposed = false;

        // timer用来得到每隔几秒钟performance counter的值.
        Timer timer;

        // CPU usage performance counter 将在ProcessCpuUsageMonitor 和 
        // TotalCpuUsageMonitor中初始化.
        protected PerformanceCounter cpuPerformanceCounter = null;

        // CpuUsageValueArray的最大长度.
        int valueArrayCapacity;

        // 该列用来存储值.
        List<double> cpuUsageValueArray;

        /// <summary>
        /// 有新增数值时发生.
        /// </summary>
        public event EventHandler<CpuUsageValueArrayChangedEventArg> CpuUsageValueArrayChanged;


        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        /// <summary>
        /// 初始化timer和performance counter.
        /// </summary>
        /// <param name="timerPeriod">
        /// 如果该数值大于0，则timer将不会启用.
        /// </param>
        /// <param name="valueArrayCapacity">
        /// CpuUsageValueArray的最大长度.
        /// </param>
        /// <param name="categoryName">
        /// 与此performance counter相关的performance counter category（性能对象）的名字.
        /// </param>
        /// <param name="counterName">
        /// performance counter的名字. 
        /// </param>
        /// <param name="instanceName">
        /// performance counter category实例的名字，或者，当某类别含有单个实例时，为空字符串.
        /// </param>
        public CpuUsageMonitorBase(int timerPeriod, int valueArrayCapacity,
            string categoryName, string counterName, string instanceName)
        {

            // 初始化 PerformanceCounter.
            this.cpuPerformanceCounter =
                new PerformanceCounter(categoryName, counterName, instanceName);

            this.valueArrayCapacity = valueArrayCapacity;
            cpuUsageValueArray = new List<double>();

            if (timerPeriod > 0)
            {

                // 延迟timer以激发回调.
                this.timer = new Timer(new TimerCallback(Timer_Callback),
                    null, timerPeriod, timerPeriod);
            }
        }

        /// <summary>
        /// 这个方法将在timer的回调函数中被执行.
        /// </summary>
        void Timer_Callback(object obj)
        {
            lock (locker)
            {

                // 清理列表.
                if (this.cpuUsageValueArray.Count > this.valueArrayCapacity)
                {
                    this.cpuUsageValueArray.Clear();
                }

                try
                {
                    double value = GetCpuUsage();

                    if (value < 0)
                    {
                        value = 0;
                    }
                    if (value > 100)
                    {
                        value = 100;
                    }

                    this.cpuUsageValueArray.Add(value);

                    double[] values = new double[cpuUsageValueArray.Count];
                    cpuUsageValueArray.CopyTo(values, 0);

                    // 引发事件.
                    this.OnCpuUsageValueArrayChanged(
                        new CpuUsageValueArrayChangedEventArg() { Values = values });
                }
                catch (Exception ex)
                {
                    this.OnErrorOccurred(new ErrorEventArgs { Error = ex });
                }
            }
        }

        /// <summary>
        /// 得到当前的CPU使用率.
        /// </summary>
        protected virtual double GetCpuUsage()
        {
            if (!IsInstanceExist())
            {
                throw new ApplicationException(
                    string.Format("The instance {0} is not available. ",
                    this.cpuPerformanceCounter.InstanceName));
            }


            double value = cpuPerformanceCounter.NextValue();
            return value;
        }

        /// <summary>
        /// 子类可能会覆盖该方法来决定实例是否存在.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsInstanceExist()
        {
            return true;
        }

        /// <summary>
        /// 引发CpuUsageValueArrayChanged事件.
        /// </summary>
        protected virtual void OnCpuUsageValueArrayChanged(CpuUsageValueArrayChangedEventArg e)
        {
            if (this.CpuUsageValueArrayChanged != null)
            {
                this.CpuUsageValueArrayChanged(this, e);
            }
        }

        /// <summary>
        /// 引发ErrorOccurred事件.
        /// </summary>
        protected virtual void OnErrorOccurred(ErrorEventArgs e)
        {
            if (this.ErrorOccurred != null)
            {
                this.ErrorOccurred(this, e);
            }
        }

        // 释放资源.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // 保护，以免被多次调用.
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (timer != null)
                {
                    timer.Dispose();
                }

                if (cpuPerformanceCounter != null)
                {
                    cpuPerformanceCounter.Dispose();
                }
                disposed = true;
            }
        }
    }    
}
