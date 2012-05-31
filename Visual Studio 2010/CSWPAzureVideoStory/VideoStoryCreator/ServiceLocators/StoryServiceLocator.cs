/********************************* 模块头 *********************************\
* 模块名: StoryServiceLocator.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 这并不真正实现服务定位器模式。
* 但这个类封装了所有的逻辑来访问REST服务，
* UI组件不再直接依赖服务。
* 这是一种依赖注入。
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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using VideoStoryCreator.Models;
using VideoStoryCreator.Helpers;

namespace VideoStoryCreator.ServiceLocators
{
    public class StoryServiceLocator
    {
        // 云REST服务的基础地质.
        // 改变云服务的地质如果你想在云中测试它.
        //private string _baseServiceUri = "http://127.0.0.1:81/stories";
        private string _baseServiceUri = "http://storycreator.cloudapp.net/stories";
        private string _storyID;

        // 下面的字段用来上传图片到Blob中.
        // 我们需要等待所有的图片都被上传了，然后提交.
        private int _allPhotoCount = 0;
        private int _uploadedPhotoCount = 0;
        private bool _uploadFailed = false;
        private object _lockObject = new object();

        public event EventHandler StoryUploaded;

        public void UploadStory()
        {
            // 建立和上传story配置文件.
            XDocument storyConfig = PersistenceHelper.SerializeStory();
            WebClient webClient = new WebClient();
            webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(UploadConfigCompleted);
            webClient.UploadStringAsync(new Uri(this._baseServiceUri), storyConfig.ToString());
        }

        void UploadConfigCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // TODO: 记录错误...
                MessageBox.Show("现在连接不上服务,请稍后再试.");
            }
            else
            {
                try
                {
                    // 返回xml文件，包括Blob的SAS.
                    XDocument resultXDoc = XDocument.Parse(e.Result);
                    this._storyID = resultXDoc.Root.Attribute("ID").Value;
                    var photoElements = resultXDoc.Root.Elements("Photo");

                    lock (this._lockObject)
                    {
                        this._allPhotoCount = photoElements.Count();
                    }

                    // 创建一个背后的线程，等待所有图片被上传.
                    // 然后提交.
                    Thread thread = new Thread(new ThreadStart(this.WaitUntilAllPhotosUploaded));
                    thread.Start();

                    foreach (var photoElement in photoElements)
                    {
                        string name = photoElement.Attribute("Name").Value;
                        string blobUri = photoElement.Attribute("Uri").Value;

                        // 找到当前story的所有图片.
                        Photo photo = App.MediaCollection.Where(p => p.Name == name).FirstOrDefault();
                        if (photo == null)
                        {
                            throw new InvalidOperationException("找不到图片.");
                        }
                        if (photo.ResizedImageStream == null)
                        {
                            photo.ResizedImageStream = BitmapHelper.GetResizedImage(photo.Name);
                        }

                        // 上传图片到blob.
                        photo.ResizedImageStream.Position = 0;

                        RetryPolicy policy = new RetryPolicy(blobUri);
                        policy.RequestAddress = blobUri;
                        policy.Initialize = new Action(() =>
                        {
                            policy.Request.Method = "PUT";
                        });
                        policy.RequestCallback = new AsyncCallback((requestStreamResult) =>
                        {
                            Stream requestStream = policy.Request.EndGetRequestStream(requestStreamResult);
                            byte[] buffer = new byte[photo.ResizedImageStream.Length];
                            photo.ResizedImageStream.Position = 0;
                            photo.ResizedImageStream.Read(buffer, 0, buffer.Length);
                            requestStream.Write(buffer, 0, buffer.Length);
                            requestStream.Close();
                        });

                        policy.ResponseCallback = new AsyncCallback((responseResult) =>
                            {
                                HttpWebResponse response = (HttpWebResponse)policy.Request.EndGetResponse(responseResult);
                                if (response.StatusCode != HttpStatusCode.Created)
                                {
                                    throw new InvalidOperationException("上传失败");
                                }
                                lock (this._lockObject)
                                {
                                    this._uploadedPhotoCount++;
                                }
                            });
                        policy.MakeRequest();
                    }
                }
                catch
                {
                    // TODO: 记录错误...

                    lock (this._lockObject)
                    {
                        this._uploadFailed = true;
                    }
                    MessageBox.Show("现在无法上传，请稍后再试.");
                }
            }
        }

        /// <summary>
        /// 调用后台线程上传
        /// </summary>
        private void WaitUntilAllPhotosUploaded()
        {
            while (true)
            {
                lock (this._lockObject)
                {
                    // 所有图片正在上传或已经失败，请断开.
                    if ((this._allPhotoCount == this._uploadedPhotoCount) || (this._uploadFailed == true))
                    {
                        break;
                    }
                }
                Thread.Sleep(3000);
            }
            if (!this._uploadFailed)
            {
                string requestUri = this._baseServiceUri + "/" + this._storyID + "?commit=true";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.Method = "PUT";
                request.BeginGetRequestStream((requestStreamResult) =>
                {
                    Stream requestStream = request.EndGetRequestStream(requestStreamResult);

                    // 没有request body.
                    requestStream.Close();
                    request.BeginGetResponse((responseResult) =>
                    {
                        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(responseResult);
                        if (response.StatusCode != HttpStatusCode.NoContent)
                        {
                            throw new InvalidOperationException("上传失败.");
                        }
                        else
                        {
                            if (this.StoryUploaded != null)
                            {
                                this.StoryUploaded(this, EventArgs.Empty);
                            }
                        }
                    }, null);
                }, null);
            }
        }
    }
}
