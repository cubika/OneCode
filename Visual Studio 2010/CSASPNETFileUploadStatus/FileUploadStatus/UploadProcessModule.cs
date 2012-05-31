/****************************** 模块头 *********************************\
* 模块名:    UploadProcessModule.cs
* 项目名:    CSASPNETFileUploadStatus
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
* 像ActiveX 控件,Flash 或者Silverlight.
* 
* 在这个例子中我们可以看到如下的特点：
* 1. 如何用HttpWorkerRequest获取用户请求实体的正文.
* 2. 如何控制服务器端读取请求数据.
* 3. 如何检索并存储上传状态.
* 
* 在这个模块的基础上,我们可以扩展它来实现如下列出的特点:
* 1. 控制多个文件上传的状态.
* 2. 控制大的文件上传到服务器并且不 
*    用服务器的缓存来存储这些文件.
* (注意: 在这个例子中我没有实现上述的特点.
*        我将会在不久的将来把它们添加进去.)
* 
* 在默认情况下IIS限制了请求内容的长度（大约28MB）,
* 如果我们想使这个模块对于大的文件也能工作,
* 请参考根目录下的readme文件.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Web;
using System.Text;
using System.Web.Caching;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace CSASPNETFileUploadStatus
{
    public class UploadProcessModule : IHttpModule
    {
        /// <summary>
        /// 这是一个建立在.Net Framework 4.0基础上 
        /// 的ASP.NET Http模块显示了无组件的文件  
        /// 上传的状态 .
        /// 详细信息，请参考根目录下的Readme文件.
        /// </summary>
        private string _cacheContainer = "fuFile";
        private string _uploadedFilesFolder = "UploadedFiles";
        private string _folderPath = "";

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);

            // 为上传文件核对文件夹.
            _folderPath = HttpContext.Current.Server.MapPath(_uploadedFilesFolder);
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {

            HttpApplication app = sender as HttpApplication;
            HttpContext context = app.Context;


            // 我们需要HttpWorkerRequest的当前内容来处理请求数据.
            // 要想知道HttpWorkerRequest更多更详细的内容,
            // 请参考根目录下的Readme文件.
            IServiceProvider provider = (IServiceProvider)context;
            System.Web.HttpWorkerRequest request =
                (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));

            // 获取当前请求的内容类型.
            string contentType =
                request.GetKnownRequestHeader(
                System.Web.HttpWorkerRequest.HeaderContentType);
            // 如果我们不能获取内容类型，跳过这个模块.
            if (contentType == null)
            {
                return;
            }
            //   如果内容类型不是multipart/form-data,
            //   意味着没有上传请求，
            //   就可以跳过这个模块.
            if (contentType.IndexOf("multipart/form-data") == -1)
            {
                return;
            }
            string boundary = contentType.Substring(contentType.IndexOf("boundary=") + 9);
            // 获取当前请求的内容长度.
            long contentLength = Convert.ToInt64(
                request.GetKnownRequestHeader(
                HttpWorkerRequest.HeaderContentLength));

            // 获取HTTP请求主体的那些
            // 当前已经被读取的数据.
            // 这是我们存储上传文件的第一步.
            byte[] data = request.GetPreloadedEntityBody();

            // 创建一个管理类的实例可以
            // 帮助过滤请求数据.
            FileUploadDataManager storeManager =
                new FileUploadDataManager(boundary);
            // 添加预装载的数据.
            storeManager.AppendData(data);

            UploadStatus status = null;
            if (context.Cache[_cacheContainer] == null)
            {
                //初始化UploadStatus， 
                //它被用来存储客户状态.
                status = new UploadStatus(
                    context,         //  把当前内容发送到status被事件使用 
                                     // 
                    contentLength    // 初始化文件长度.
                    );
                // 当更新状态时绑定事件.
                status.OnDataChanged +=
                    new UploadStatusEventHandler(status_OnDataChanged);

            }
            else
            {
                status = context.Cache[_cacheContainer] as UploadStatus;
                if (status.IsFinished)
                {
                    return;
                }
            }

            // 把首先读到的数据长度设置到status class.
            if (data != null)
            {
                status.UpdateLoadedLength(data.Length);
            }

            // 获取留下的请求数据的长度. 
            long leftdata = status.ContentLength - status.LoadedLength;

            // 定义一个自定义的缓存区的长度
            int customBufferLength = Convert.ToInt32(Math.Ceiling((double)contentLength / 16));
            if (customBufferLength < 1024)
            {
                customBufferLength = 1024;
            }
            while (!request.IsEntireEntityBodyIsPreloaded() && leftdata > 0)
            {
                // 检查用户如果终止了上传，关闭连接.
                if (status.Aborted)
                {
                    // 删除缓存文件.
                    foreach (UploadFile file in storeManager.FilterResult)
                    {
                        file.ClearCache();
                    }
                    request.CloseConnection();
                    return;
                }

                // 如果剩下的请求数据小于缓
                // 冲区的长度，把缓冲区的
                // 长度设置成剩余数据的长度.
                if (leftdata < customBufferLength)
                {
                    customBufferLength = (int)leftdata;
                }

                // 读取自定义缓冲区的长度的请求数据
                data = new byte[customBufferLength];
                int redlen = request.ReadEntityBody(data, customBufferLength);
                if (customBufferLength > redlen)
                {
                    data = BinaryHelper.SubData(data, 0, redlen);
                }
                // 添加剩余数据.
                storeManager.AppendData(data);

                // 把缓冲区的长度添加到status来更新上传status.
                status.UpdateLoadedLength(redlen);

                leftdata -= redlen;
            }

            // 当所有的数据都被读取之后,
            // 保存上传文件.
            foreach (UploadFile file in storeManager.FilterResult)
            {
                file.Save(null);
            }
        }

        private void status_OnDataChanged(object sender, UploadStatusEventArgs e)
        {
            // 保存status class到缓存的当前内容.
            UploadStatus status = sender as UploadStatus;
            if (e.context.Cache[_cacheContainer] == null)
            {
                e.context.Cache.Add(_cacheContainer,
                    status,
                    null,
                    DateTime.Now.AddDays(1),
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.High,
                    null);
            }
            else
            {
                e.context.Cache[_cacheContainer] = status;
            }

        }



    }
}
