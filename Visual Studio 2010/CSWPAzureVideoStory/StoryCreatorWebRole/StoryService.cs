/*********************************** 模块头 ***********************************\
* 模块名:  StoryService.cs
* 项目名:  StoryCreatorWebRole
* 版权 (c) Microsoft Corporation.
* 
* 使用 Web API 构建 WCF REST服务.
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
using System.Data.Services.Client;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.WindowsAzure.StorageClient;
using StoryDataModel;

namespace StoryCreatorWebRole
{
    [ServiceContract]
    public class StoryService
    {
        /// <summary>
        /// 建立一个新的story.
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            // 保证Azure存储账号已就绪.
            if (Global.StorageAccount == null)
            {
                if (!Global.InitializeStorage())
                {
                    return this.CreateStringResponse(HttpStatusCode.BadRequest,
                        "服务现在不可用. 请稍后再试.");
                }
            }

            try
            {
                // Request 内文.
                XDocument docSource = XDocument.Parse(request.Content.ReadAsString());

                // Response 内文.
                XDocument docResult = new XDocument(new XElement("Story"));
                ;
                var photos = docSource.Root.Elements("Photo");
                int photoElementsCount = photos.Count();
                if (photos.Count() < 2)
                {
                    return this.CreateStringResponse(HttpStatusCode.BadRequest, "短影需要至少2张照片.");
                }

                int photoCount = 0;
                try
                {
                    photoCount = int.Parse(docSource.Root.Attribute("PhotoCount").Value);
                }
                catch
                {
                    return this.CreateStringResponse(HttpStatusCode.BadRequest,
                        "请求正文的格式不正确.短影所需的属性PhotoCount丢失或不正确.");
                }

                if (photoElementsCount != photoCount)
                {
                    return this.CreateStringResponse(HttpStatusCode.BadRequest,
                        "请求正文的格式不正确.PhotoCount不等于照片文件数.");
                }

                CloudBlobClient blobClient = new CloudBlobClient(Global.StorageAccount.BlobEndpoint, Global.StorageAccount.Credentials);
                CloudBlobContainer container = blobClient.GetContainerReference("videostories");

                // 代表短影的唯一ID.
                string id = Guid.NewGuid().ToString();
                CloudBlob configBlob = container.GetBlobReference(id + ".xml");
                docResult.Root.Add(new XAttribute("ID", id));

                foreach (XElement photo in photos)
                {
                    string name = photo.Attribute("Name").Value;

                    // 构造SAS.  开始时间设置为 1 分钟前 ,
                    // 确保客户端能够上传blob.
                    CloudBlob blob = container.GetBlobReference(id + "/" + name);
                    string sas = blob.GetSharedAccessSignature(new SharedAccessPolicy()
                    {
                        Permissions = SharedAccessPermissions.Write,
                        SharedAccessStartTime = DateTime.Now.AddMinutes(-1d),
                        SharedAccessExpiryTime = DateTime.Now.AddHours(0.5)
                    });

                    // 创建空的 blob，因此客户端可以上载至正确的 blob .
                    blob.UploadText("");

                    // 修改原始配置.添加无 SAS 的 URI.
                    photo.Add(new XAttribute("Uri", blob.Uri.AbsoluteUri));

                    // 将图片元素添加到响应，包括 SAS.
                    string fullUri = blob.Uri.AbsoluteUri + sas;
                    docResult.Root.Add(new XElement("Photo", new XAttribute("Name", name), new XAttribute("Uri", fullUri)));
                }

                // 在 blob存储中存储配置.
                configBlob.UploadText(docSource.ToString());

                Trace.Write("短影配置已创建: " + configBlob.Uri, "Information");

                // 返回成功响应.
                return this.CreateXmlResponse(HttpStatusCode.Created, docResult.ToString());
            }
            catch (XmlException)
            {
                return this.CreateStringResponse(HttpStatusCode.BadRequest,
                    " 请求主体不是格式化好的 xml 文档.");
            }
            catch (StorageClientException ex)
            {
                Trace.Write("处理blob错误: " + ex.Message, "Error");
                return this.CreateStringResponse(HttpStatusCode.InternalServerError,
                    "该服务目前不可用.请稍后再试.");
            }
        }

        /// <summary>
        /// 更新短影数据源.
        /// 目前唯一的更新是提交 （表示我们就可以对视频进行编码）短影.
        /// </summary>
        [WebInvoke(Method = "PUT", UriTemplate = "{id}?commit={commit}")]
        public HttpResponseMessage Put(HttpRequestMessage request, string id, bool? commit)
        {
            // 请确保该存储帐户是准备好了.
            if (Global.StorageAccount == null)
            {
                if (!Global.InitializeStorage())
                {
                    return this.CreateStringResponse(HttpStatusCode.BadRequest,
                        "该服务目前不可用.请稍后再试.");
                }
            }
            if (string.IsNullOrEmpty(id))
            {
                return this.CreateStringResponse(HttpStatusCode.BadRequest, "必需的参数标识已丢失.");
            }
            if (commit == null || !commit.Value)
            {
                return this.CreateStringResponse(HttpStatusCode.BadRequest, "当前仅支持 \"commit=true\" 选项.");
            }

            try
            {
                CloudBlobClient blobClient = new CloudBlobClient(Global.StorageAccount.BlobEndpoint, Global.StorageAccount.Credentials);
                CloudBlobContainer container = blobClient.GetContainerReference("videostories");
                CloudBlob configBlob = container.GetBlobReference(id + ".xml");

                // 我们其实不需要这些属性.我们只是检查blob是否存在.
                // 如果blob不存在，将引发 StorageClientException，所以我们跳至 catch 块.
                configBlob.FetchAttributes();

                // 添加到队列的作业.
                CloudQueueClient queueClient = new CloudQueueClient(Global.StorageAccount.QueueEndpoint, Global.StorageAccount.Credentials);
                CloudQueue queue = queueClient.GetQueueReference("videostories");
                queue.AddMessage(new CloudQueueMessage(id));

                // 返回一个空的成功消息.
                return this.CreateStringResponse(HttpStatusCode.NoContent, "");

            }
            catch (StorageClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return this.CreateStringResponse(HttpStatusCode.NotFound, "请求的Story不存在");
                }

                // 一般错误，跟踪并返回泛用消息.
                Trace.Write("处理blob错误: " + ex.Message, "Error");
                return this.CreateStringResponse(HttpStatusCode.InternalServerError,
                    "该服务目前不可用.请稍后再试.");
            }
        }

        [WebGet(UriTemplate = "")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            // 请确保该存储帐户是准备好了.
            if (Global.StorageAccount == null)
            {
                if (!Global.InitializeStorage())
                {
                    return this.CreateStringResponse(HttpStatusCode.BadRequest,
                        "该服务目前不可用.请稍后再试.");
                }
            }

            try
            {
                StoryDataContext storyDataContext = new StoryDataContext(Global.StorageAccount.TableEndpoint.AbsoluteUri, Global.StorageAccount.Credentials);

                //  查询表存储.
                var query = from s in storyDataContext.Stories select s;

                // 将结果转换为一个简化的类，不包含分区/行键.
                List<Story> stories = new List<Story>();
                foreach (StoryDataModel.Story s in query)
                {
                    stories.Add(new Story() { Name = s.Name, VideoUri = s.VideoUri });
                }
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Story>));
                using (MemoryStream stream = new MemoryStream())
                {
                    jsonSerializer.WriteObject(stream, stories);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd();
                        return this.CreateJsonResponse(HttpStatusCode.OK, result);
                    }
                }
            }
            catch (StorageClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return this.CreateStringResponse(HttpStatusCode.NotFound, "请求的Story不存在");
                }

                // 一般错误，跟踪并返回泛用消息.
                Trace.Write("处理表服务错误: " + ex.Message, "Error");
                return this.CreateStringResponse(HttpStatusCode.InternalServerError,
                    "该服务目前不可用.请稍后再试.");
            }
            catch (DataServiceQueryException ex2)
            {
                Trace.Write("处理表服务错误: " + ex2.Message, "Error");
                return this.CreateStringResponse(HttpStatusCode.InternalServerError,
                    "该服务目前不可用.请稍后再试.");
            }
        }

        private HttpResponseMessage CreateStringResponse(HttpStatusCode statusCode, string body)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = statusCode;
            response.Content = new StringContent(body);
            return response;
        }

        private HttpResponseMessage CreateXmlResponse(HttpStatusCode statusCode, string body)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = statusCode;
            response.Content = new StringContent(body, Encoding.UTF8, "text/xml");
            return response;
        }

        private HttpResponseMessage CreateJsonResponse(HttpStatusCode statusCode, string body)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = statusCode;
            response.Content = new StringContent(body, Encoding.UTF8, "application/json");
            return response;
        }
    }
}