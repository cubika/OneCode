/*********************************** 模块头 ***********************************\
* 模块名:  Global.cs
* 项目名:  StoryCreatorWebRole
* 版权 (c) Microsoft Corporation.
* 
* Global.asax 文件.用来初始化Azure的存储 .
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
using System.Diagnostics;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace StoryCreatorWebRole
{
    public class Global : System.Web.HttpApplication
    {
        internal static CloudStorageAccount StorageAccount;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapServiceRoute<StoryService>("stories");
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
            InitializeStorage();
        }

        /// <summary>
        /// 初始化Azure的存储.
        /// 如果未运行在模拟计算器或云上,
        /// 我们使用存储模拟器.
        /// 忽略 CA2122因为RoleEnvironment.IsAvailable不会引入任何安全问题.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal static bool InitializeStorage()
        {
            try
            {
                // 仅为测试目的，如果我们不在计算仿真程序中运行该服务，我们始终使用 dev 存储.
                if (RoleEnvironment.IsAvailable)
                {
                    CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                    {
                        configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
                    });
                    StorageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
                }
                else
                {
                    StorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                }

                CloudBlobClient blobClient = new CloudBlobClient(StorageAccount.BlobEndpoint, StorageAccount.Credentials);
                CloudBlobContainer container = blobClient.GetContainerReference("videostories");
                container.CreateIfNotExist();
                CloudQueueClient queueClient = new CloudQueueClient(StorageAccount.QueueEndpoint, StorageAccount.Credentials);
                CloudQueue queue = queueClient.GetQueueReference("videostories");
                queue.CreateIfNotExist();
                CloudTableClient tableClient = new CloudTableClient(StorageAccount.TableEndpoint.AbsoluteUri, StorageAccount.Credentials);
                tableClient.CreateTableIfNotExist("Stories");
                return true;
            }
            catch (Exception ex)
            {
                Trace.Write("错误初始化存储: " + ex.Message, "Error");
                return false;
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}