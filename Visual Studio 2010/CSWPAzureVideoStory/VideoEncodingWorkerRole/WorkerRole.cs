/*********************************** 模块头 ***********************************\
* 模块名:  WorkerRole.cs
* 项目名:  VideoEncodingWorkerRole
* 版权 (c) Microsoft Corporation.
* 
* Worker Role对视频进行编码.
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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using StoryDataModel;

namespace VideoEncodingWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobContainer _container;
        private CloudQueue _queue;

        public override void Run()
        {
            // 这是worker实现示例.请替换为您的逻辑.
            Trace.WriteLine("VideoEncodingWorkerRole入口点调用", "Information");

            // 获取本地存储的路径 .
            string localStorageRoot = RoleEnvironment.GetLocalResource("PhotoStore").RootPath;
            string diagnosticsRoot = RoleEnvironment.GetLocalResource("DiagnosticStore").RootPath;
            while (true)
            {
                try
                {
                    // 从队列中得到一条消息.
                    CloudQueueMessage message = this._queue.GetMessage(TimeSpan.FromMinutes(20d));
                    if (message != null)
                    {
                        string storyID = message.AsString;

                        // 短影无法编码...
                        if (message.DequeueCount > 3)
                        {
                            Trace.Write("短影" + storyID + "已尝试多次均未能被处理.", "Error");
                            this._queue.DeleteMessage(message);
                        }
                        else
                        {
                            Trace.Write("开始处理短影" + storyID + ".", "Information");

                            string storyFolderPath = Path.Combine(localStorageRoot, storyID);
                            Directory.CreateDirectory(storyFolderPath);

                            // 下载Xml 配置文件.
                            CloudBlob blob = this._container.GetBlobReference(storyID + ".xml");
                            string config = blob.DownloadText();
                            XDocument xdoc = XDocument.Parse(config);
                            string storyName = xdoc.Root.Attribute("Name").Value;
                            foreach (var photo in xdoc.Root.Elements("Photo"))
                            {
                                // 下载照片，并将其保存在本地存储.
                                string photoBlobUri = photo.Attribute("Uri").Value;
                                CloudBlob photoBlob = this._container.GetBlobReference(photoBlobUri);
                                string localPhotoFilePath = Path.Combine(localStorageRoot, storyID, photo.Attribute("Name").Value);
                                photoBlob.DownloadToFile(localPhotoFilePath);

                                // 修改图片的名称，包括完整路径 .
                                photo.Attribute("Name").Value = localPhotoFilePath;
                            }

                            // 保存配置文件到本地存储.
                            string configFilePath = Path.Combine(localStorageRoot, storyID + ".xml");
                            xdoc.Save(configFilePath, SaveOptions.None);

                            // 编码视频.
                            string outputFilePath = Path.Combine(localStorageRoot, storyID + ".mp4");
                            string logFilePath = Path.Combine(diagnosticsRoot, storyID + ".log");
                            int hr = NativeMethods.EncoderVideo(configFilePath, outputFilePath, logFilePath);
                            if (hr != 0)
                            {
                                Trace.Write("编码短影" + storyID + "时出错. 非托管代码返回的HRESULT: " + hr + ".", "Error");
                            }
                            else
                            {
                                // 上传结果视频至blob.
                                string blobName = storyID + "/";
                                blobName += string.IsNullOrEmpty(storyName) ? storyID : storyName;
                                blobName += ".mp4";
                                CloudBlob outputBlob = this._container.GetBlobReference(blobName);
                                outputBlob.UploadFile(outputFilePath);
                                outputBlob.Properties.ContentType = "video/mp4";
                                outputBlob.SetProperties();

                                StoryDataContext storyDataContext = new StoryDataContext(
                                    this._storageAccount.TableEndpoint.AbsoluteUri,
                                    this._storageAccount.Credentials);
                                storyDataContext.AddObject("Stories", new Story(storyID, storyName, outputBlob.Uri.AbsoluteUri));
                                storyDataContext.SaveChanges();

                                // 删除本地文件.
                                File.Delete(configFilePath);
                                File.Delete(outputFilePath);
                                foreach (string fileName in Directory.GetFiles(storyFolderPath))
                                {
                                    File.Delete(fileName);
                                }
                                Directory.Delete(storyFolderPath);

                                this._queue.DeleteMessage(message);

                                // TODO: 我们应该从 blob删除短影的配置文件和照片?

                                Trace.Write("短影 " + storyID + " 成功编码.", "Information");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write("处理短影错误: " + ex.Message);
                }

                Thread.Sleep(10000);
            }
        }

        public override bool OnStart()
        {
            // 设置的最大并发连接数
            ServicePointManager.DefaultConnectionLimit = 12;

            // 有关处理配置更改信息
            // 请参阅 MSDN 主题在  http://go.microsoft.com/fwlink/?LinkId=166357.

            // 如果没有被初始化，初始化存储.
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
            });
            this._storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            CloudBlobClient blobClient = new CloudBlobClient(this._storageAccount.BlobEndpoint, this._storageAccount.Credentials);
            this._container = blobClient.GetContainerReference("videostories");
            this._container.CreateIfNotExist();
            this._container.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            CloudQueueClient queueClient = new CloudQueueClient(this._storageAccount.QueueEndpoint, this._storageAccount.Credentials);
            this._queue = queueClient.GetQueueReference("videostories");
            this._queue.CreateIfNotExist();
            CloudTableClient tableClient = new CloudTableClient(this._storageAccount.TableEndpoint.AbsoluteUri, this._storageAccount.Credentials);
            tableClient.CreateTableIfNotExist("Stories");

            // 配置诊断程序.
            var config = DiagnosticMonitor.GetDefaultInitialConfiguration();
            config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(10d);
            config.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(10d);
            config.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(10d);
            config.Directories.DataSources.Add(new DirectoryConfiguration()
            {
                Path = RoleEnvironment.GetLocalResource("DiagnosticStore").RootPath,
                Container = "videostorydiagnosticstore",
                DirectoryQuotaInMB = 200
            });
            config.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(10d);
            Microsoft.WindowsAzure.Diagnostics.CrashDumps.EnableCollection(true);
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);
            RoleEnvironment.Changing += new EventHandler<RoleEnvironmentChangingEventArgs>(RoleEnvironment_Changing);

            return base.OnStart();
        }

        private void RoleEnvironment_Changing(object sender, RoleEnvironmentChangingEventArgs e)
        {
            if (e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Count() > 0)
            {
                e.Cancel = true;
            }
        }
    }
}
