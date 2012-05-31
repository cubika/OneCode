/****************************** 模块头 **************************************\
* 模块名:  RegistryKeyChangeEventArgs.cs
* 项目名:  CSMonitorRegistryChange
* 版权(c)  Microsoft Corporation.
* 
* 这个类派生自EventArgs。用于将ManagementBaseObject包装为EventArrivedEventArgs。
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
using System.Management;

namespace CSMonitorRegistryChange
{
    class RegistryKeyChangeEventArgs : EventArgs
    {
        public string Hive { get; set; }
        public string KeyPath { get; set; }
        public uint[] SECURITY_DESCRIPTOR { get; set; }
        public DateTime TIME_CREATED { get; set; }

        public RegistryKeyChangeEventArgs(ManagementBaseObject arrivedEvent)
        {
            // 类RegistryKeyChangeEvent有以下属性：Hive，KeyPath，SECURITY_DESCRIPTOR
            // 以及TIME_CREATED。这些属性可以从arrivedEvent.Properties中得到。
            this.Hive = arrivedEvent.Properties["Hive"].Value as string;
            this.KeyPath = arrivedEvent.Properties["KeyPath"].Value as string;

            // TIME_CREATED属性是一个代表了事件被创建时间的独特的值。
            // 它表示了从1601年1月1日之后到现在的，以100纳秒为间隔的一个64位FILETIME值。
            // 这个值使用了协调世界时间(UTC)格式。
            this.TIME_CREATED = new DateTime(
                (long)(ulong)arrivedEvent.Properties["TIME_CREATED"].Value,
                DateTimeKind.Utc).AddYears(1600);
        }
    }
}
