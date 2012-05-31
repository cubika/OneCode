/****************************** 模块头 ******************************\
* 模块名称:  HttpDownloadClient.cs
* 项目名称:	    CSMultiThreadedWebDownloader
* 版权 (c) Microsoft Corporation.
* 
* 这个类是用来通过网络下载文件的。 它提供了一些公有的下载方法：Start, Pause, Resume and Cancel
*  一个客户端将使用一个线程来下载整个文件的一部分。
* 
* 下载的数据最先是存储为 MemoryStream,然后在转变成本地文件。
* 
* 当开始下载指定大小的数据文件时，触发DownloadProgressChanged 事件，
* 当完成或者取消下载的文件时，触发DownloadCompleted事件。

* 
* DownloadedSize 属性存储下载数据的大小（继续下载的时候使用）。
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
using System.Net;
using System.Threading;

namespace CSMultiThreadedWebDownloader
{
    public class HttpDownloadClient
    {
        //当创建或者写文件的时候使用。
        static object locker = new object();

        // 要下载文件的Url。
        public Uri Url { get; private set; }

        //存储文件的路径。
        // 如果没有相同名字的文件，将创建一个新的文件。
        public string DownloadPath { get; private set; }

        //  StartPoint and EndPoint 属性用在多线程现在的情况下，每一个线程开始下载一个特定的部分。 
        public long StartPoint { get; private set; }

        public long EndPoint { get; private set; }

        public WebProxy Proxy { get; set; }

        // 在响应流中当读取数据时，设置 BufferSize 的值。
        public int BufferSize { get; private set; }

        // 内存中缓冲区的大小。
        public int MaxCacheSize { get; private set; }



        // 向服务器请求要下载文件的大小并存储。
        public long TotalSize { get; set; }


        //下载数据变成本地文件时的大小。
        public long DownloadedSize { get; private set; }

        HttpDownloadClientStatus status;

        //如果状态改变，触发StatusChanged事件。
        public HttpDownloadClientStatus Status
        {
            get
            { return status; }

            private set
            {
                if (status != value)
                {
                    status = value;
                    this.OnStatusChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler<HttpDownloadClientProgressChangedEventArgs>
            DownloadProgressChanged;

        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        public event EventHandler StatusChanged;

        /// <summary>
        /// 下载整个文件。
        /// </summary>
        public HttpDownloadClient(Uri url, string downloadPath)
            : this(url, downloadPath, 0)
        {
        }

        /// <summary>
        /// 从一个指定开始点开始下载，到结束点。
        /// </summary>
        public HttpDownloadClient(Uri url, string downloadPath,
           long startPoint)
            : this(url, downloadPath, startPoint, long.MaxValue)
        {
        }

        /// <summary>
        /// 下载文件的某一块.默认的缓冲区大小是1KB，内存缓存是1MB，每个通知缓冲区计数为64。
        /// </summary>
        public HttpDownloadClient(Uri url, string downloadPath,
          long startPoint, long endPoint)
            : this(url, downloadPath, startPoint, endPoint, 1024, 1048576)
        {
        }

        public HttpDownloadClient(Uri url, string downloadPath, long startPoint,
            long endPoint, int bufferSize, int cacheSize)
        {
            if (startPoint < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "StartPoint不能小于0. ");
            }

            if (endPoint < startPoint)
            {
                throw new ArgumentOutOfRangeException(
                    "EndPoint 不能小于StartPoint ");
            }

            if (bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferSize 不能小于0. ");
            }

            if (cacheSize < bufferSize)
            {
                throw new ArgumentOutOfRangeException(
                    "MaxCacheSize 不能小于BufferSize ");
            }


            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.BufferSize = bufferSize;
            this.MaxCacheSize = cacheSize;

            this.Url = url;
            this.DownloadPath = downloadPath;

            // 设置闲置状态.
            this.status = HttpDownloadClientStatus.Idle;

        }

        /// <summary>
        /// 开始下载.
        /// </summary>
        public void Start()
        {

            //只有闲置的情况下才能开始。
            if (this.Status != HttpDownloadClientStatus.Idle)
            {
                throw new ApplicationException("只有闲置的客户端才能开始");
            }

            // 在主线程下开始下载。
            BeginDownload();
        }

        /// <summary>
        /// 暂停下载.
        /// </summary>
        public void Pause()
        {
            // 只有正在下载的情况下才能暂停。
            if (this.Status != HttpDownloadClientStatus.Downloading)
            {
                throw new ApplicationException("只有正在下载的客户端才能暂停。");
            }

            // 主线程会检查状态。如果暂停了，下载将会被暂停，并且状态会变成暂停状态。
            this.Status = HttpDownloadClientStatus.Pausing;
        }

        /// <summary>
        /// 继续下载。
        /// </summary>
        public void Resume()
        {
            // 只有暂停的情况下才能继续下载。
            if (this.Status != HttpDownloadClientStatus.Paused)
            {
                throw new ApplicationException("只有暂停的客户端菜能继续下载。");
            }

            //在主线程下开始下载。
            BeginDownload();
        }

        /// <summary>
        /// 取消下载。
        /// </summary>
        public void Cancel()
        {
            // 只有正在下载或者暂停的情况下才能被取消。
            if (this.Status != HttpDownloadClientStatus.Paused
                && this.Status != HttpDownloadClientStatus.Downloading)
            {
                throw new ApplicationException("只有正在下载或者暂停的情况下才能被取消");
            }

            // 主线程会检查状态，如果是取消，下载将会取消，状态同时改变成取消状态。
            this.Status = HttpDownloadClientStatus.Canceling;
        }

        /// <summary>
        ///创建新的下载线程。
        /// </summary>
        void BeginDownload()
        {
            ThreadStart threadStart = new ThreadStart(Download);
            Thread downloadThread = new Thread(threadStart);
            downloadThread.IsBackground = true;
            downloadThread.Start();
        }

        /// <summary>
        /// 用 HttpWebRequest下载数据. 它从响应流中读取缓冲区的数据。
        /// 并且先把数据存储在内存缓存中。
        /// 如果缓存满了，或者下载被暂停，取消或者完成，就将缓存中的数据写成本地文件。
        /// </summary>
        void Download()
        {
            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            Stream responseStream = null;
            MemoryStream downloadCache = null;

            // 设置状态。
            this.Status = HttpDownloadClientStatus.Downloading;

            try
            {

                // 创建一个下载的请求。
                webRequest = (HttpWebRequest)WebRequest.Create(Url);
                webRequest.Method = "GET";
                webRequest.Credentials = CredentialCache.DefaultCredentials;


                // 指定下载的模块。
                if (EndPoint != long.MaxValue)
                {
                    webRequest.AddRange(StartPoint + DownloadedSize, EndPoint);
                }
                else
                {
                    webRequest.AddRange(StartPoint + DownloadedSize);
                }

                // 设置 proxy.
                if (Proxy != null)
                {
                    webRequest.Proxy = Proxy;
                }

                // 得到从服务器中的响应和响应流。
                webResponse = (HttpWebResponse)webRequest.GetResponse();

                responseStream = webResponse.GetResponseStream();


                // 在内存中缓存数据。
                downloadCache = new MemoryStream(this.MaxCacheSize);

                byte[] downloadBuffer = new byte[this.BufferSize];

                int bytesSize = 0;
                int cachedSize = 0;

                // 下载文件，知道被取消，暂停或者完成。
                while (true)
                {

                    // 从响应流中读取缓冲数据。
                    bytesSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length);

                    // 如果缓存满了，或者下载被暂停，取消，或者完成了，将缓存数据写成本地文件。
                    if (this.Status != HttpDownloadClientStatus.Downloading
                        || bytesSize == 0
                        || this.MaxCacheSize < cachedSize + bytesSize)
                    {
                        try
                        {
                            // 将缓存数据写成本地文件。
                            WriteCacheToFile(downloadCache, cachedSize);

                            this.DownloadedSize += cachedSize;

                            // 停止下载文件，如果下载被暂停，取消，或者完成。 
                            if (this.Status != HttpDownloadClientStatus.Downloading
                                || bytesSize == 0)
                            {
                                break;
                            }

                            // 重新设置缓存。
                            downloadCache.Seek(0, SeekOrigin.Begin);
                            cachedSize = 0;
                        }
                        catch (Exception ex)
                        {
                            //触发 DownloadCompleted事件。
                            this.OnError(new ErrorEventArgs { Error = ex });
                            return;
                        }
                    }

                    // 从缓冲区写入数据到内存中的缓存。

                    downloadCache.Write(downloadBuffer, 0, bytesSize);

                    cachedSize += bytesSize;

                    // 触发DownloadProgressChanged事件。
                    OnDownloadProgressChanged(
                        new HttpDownloadClientProgressChangedEventArgs
                        {
                            Size = bytesSize
                        });
                }

                // 更新状态。当状态是暂停，取消，完成时，上面的循环将会停止。
                if (this.Status == HttpDownloadClientStatus.Pausing)
                {
                    this.Status = HttpDownloadClientStatus.Paused;
                }
                else if (this.Status == HttpDownloadClientStatus.Canceling)
                {
                    this.Status = HttpDownloadClientStatus.Canceled;

                    Exception ex = new Exception("下载被用户取消 ");

                    this.OnError(new ErrorEventArgs { Error = ex });
                }
                else
                {
                    this.Status = HttpDownloadClientStatus.Completed;
                    return;
                }

            }
            finally
            {
                // 当以上代码执行结束时，关闭流。
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (webResponse != null)
                {
                    webResponse.Close();
                }
                if (downloadCache != null)
                {
                    downloadCache.Close();
                }
            }
        }

        /// <summary>
        /// 将缓存数据写到本地文件
        /// </summary>
        void WriteCacheToFile(MemoryStream downloadCache, int cachedSize)
        {
            // 锁定其他线程或者进程，去阻止向本地文件写数据。
            lock (locker)
            {
                using (FileStream fileStream = new FileStream(DownloadPath, FileMode.Open))
                {
                    byte[] cacheContent = new byte[cachedSize];
                    downloadCache.Seek(0, SeekOrigin.Begin);
                    downloadCache.Read(cacheContent, 0, cachedSize);
                    fileStream.Seek(DownloadedSize + StartPoint, SeekOrigin.Begin);
                    fileStream.Write(cacheContent, 0, cachedSize);
                }
            }
        }

        /// <summary>
        /// 引发ErrorOccurred 事件。
        /// </summary>
        protected virtual void OnError(ErrorEventArgs e)
        {
            if (ErrorOccurred != null)
            {
                ErrorOccurred(this, e);
            }
        }

        /// <summary>
        /// 引发 DownloadProgressChanged 事件。
        /// </summary>
        protected virtual void OnDownloadProgressChanged(HttpDownloadClientProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(this, e);
            }
        }

        /// <summary>
        /// 引发 StatusChanged事件。
        /// </summary>
        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, e);
            }
        }
    }
}
