/****************************** 模块头 **************************************\
* 模块名:  RegistryWatcher.cs
* 项目名:  CSMonitorRegistryChange
* 版权(c)  Microsoft Corporation.
* 
* 这个类派生自ManagementEventWatcher。本类被用于
* 1. 提供所支持的节点。
* 2. 从Hive和KeyPath中构造WqlEventQuery。
* 3. 把EventArrivedEventArgs包装为RegistryKeyChangeEventArg。
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
using System.Collections.ObjectModel;
using System.Management;
using Microsoft.Win32;

namespace CSMonitorRegistryChange
{
    class RegistryWatcher : ManagementEventWatcher, IDisposable
    {

        static ReadOnlyCollection<RegistryKey> supportedHives = null;

        /// <summary>
        /// 对于HKEY_CLASSES_ROOT与HKEY_CURRENT_USER节点的更改不被RegistryEvent
        /// 或派生自它的类，例如RegistryKeyChangeEvent所支持。
        /// </summary>
        public static ReadOnlyCollection<RegistryKey> SupportedHives
        {
            get
            {
                if (supportedHives == null)
                {
                    RegistryKey[] hives = new RegistryKey[] 
                    {
                        Registry.LocalMachine,
                        Registry.Users,
                        Registry.CurrentConfig
                    };
                    supportedHives = Array.AsReadOnly<RegistryKey>(hives);
                }
                return supportedHives;
            }
        }

        public RegistryKey Hive { get; private set; }
        public string KeyPath { get; private set; }
        public RegistryKey KeyToMonitor { get; private set; }

        public event EventHandler<RegistryKeyChangeEventArgs> RegistryKeyChangeEvent;

        /// <exception cref="System.Security.SecurityException">
        /// 当前用户没有访问监控器中项的许可时抛出。
        /// </exception> 
        /// <exception cref="System.ArgumentException">
        /// 当监控器中的项不存在时抛出。
        /// </exception> 
        public RegistryWatcher(RegistryKey hive, string keyPath)
        {
            this.Hive = hive;
            this.KeyPath = keyPath;

            // 如果你把这个项目的平台设为x86，则在一个64位的机器上运行此项目时，当你的项路径
            // 是HKEY_LOCAL_MACHINE\SOFTWARE时，你将会在HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node
            // 下找到注册表项。
            this.KeyToMonitor = hive.OpenSubKey(keyPath);

            if (KeyToMonitor != null)
            {
                // 构造查询字符串。
                string queryString = string.Format(@"SELECT * FROM RegistryKeyChangeEvent 
                   WHERE Hive = '{0}' AND KeyPath = '{1}' ", this.Hive.Name, this.KeyPath);

                WqlEventQuery query = new WqlEventQuery();
                query.QueryString = queryString;
                query.EventClassName = "RegistryKeyChangeEvent";
                query.WithinInterval = new TimeSpan(0, 0, 0, 1);
                this.Query = query;

                this.EventArrived += new EventArrivedEventHandler(RegistryWatcher_EventArrived);
            }
            else
            {
                string message = string.Format(
                    @"注册表项 {0}\{1} 不存在。",
                    hive.Name,
                    keyPath);
                throw new ArgumentException(message);
            }
        }

        void RegistryWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (RegistryKeyChangeEvent != null)
            {
                // 从EventArrivedEventArgs.NewEvent.Properties中获取RegistryKeyChangeEventArgs。
                RegistryKeyChangeEventArgs args = new RegistryKeyChangeEventArgs(e.NewEvent);

                // 引发事件处理句柄。
                RegistryKeyChangeEvent(sender, args);
            }
        }

        /// <summary>
        /// 释放RegistryKey。
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            if (this.KeyToMonitor != null)
            {
                this.KeyToMonitor.Dispose();
            }
        }
    }
}
