/********************************* 模块头 *********************************\
* 模块名:   WorkerRole.cs
* 项目名:   CSWorkerRoleHostingWCF
* 版权 (c) Microsoft Corporation.
* 
* 这个Worker Role托管了一个WCF服务,暴露了两个tcp端点.一个是为元数据,另一个
* 是为MyService.
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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace WorkerRoleHostingWCF
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // 这是一个实现Worker的例子.用你的逻辑替换.
            // Trace.WriteLine("WorkerRoleHostingWCF entry point called", "Information");

            // 主要逻辑

            using (ServiceHost host = new ServiceHost(typeof(MyService)))
            {

                string ip = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["tcpinput"].IPEndpoint.Address.ToString();
                int tcpport = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["tcpinput"].IPEndpoint.Port;
                int mexport = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["mexinput"].IPEndpoint.Port;

                // 为客户端代理服务器添加一个metadatabehavior
                // 元数据通过net.tcp暴露
                ServiceMetadataBehavior metadatabehavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(metadatabehavior);
                Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
                string mexlistenurl = string.Format("net.tcp://{0}:{1}/MyServiceMetaDataEndpoint", ip, mexport);
                string mexendpointurl = string.Format("net.tcp://{0}:{1}/MyServiceMetaDataEndpoint", RoleEnvironment.GetConfigurationSettingValue("Domain"), 8001);
                host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, mexendpointurl, new Uri(mexlistenurl));

                // 为MyService添加端点
                string listenurl = string.Format("net.tcp://{0}:{1}/MyServiceEndpoint", ip, tcpport);
                string endpointurl = string.Format("net.tcp://{0}:{1}/MyServiceEndpoint", RoleEnvironment.GetConfigurationSettingValue("Domain"), 9001);
                host.AddServiceEndpoint(typeof(IMyService), new NetTcpBinding(SecurityMode.None), endpointurl, new Uri(listenurl));
                host.Open();

                while (true)
                {
                    Thread.Sleep(1800000);
                    //Trace.WriteLine("Working", "Information");
                }
            }
        }

        public override bool OnStart()
        {
            // 设置最大的并发连接数
            ServicePointManager.DefaultConnectionLimit = 12;
            // 为了避免潜在的错误关闭诊断程序.
            // DiagnosticMonitor.Start("DiagnosticsConnectionString");

            // 关于处理配置的变化请参考MSDN,网址为 http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // 如果一个配置环境发生变化
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // 将e.Cancel设为true重启这个角色实例
                e.Cancel = true;
            }
        }
    }
}
