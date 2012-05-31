/******************************** 模块头 **************************************\
* 模块名:	HttpDownloadClient.cs
* 项目名:   CSWebDownloadProgress
* 版权(c)   Microsoft Corporation.
* 
* 这个类被用来从网上下载文件. 它提供的公有方法来实现开始、暂停、重新开始和取消一次下载。
* 
* 下载开始时，会检查目标文件是否存在，如果不存在，就创建一个和将要下载的文件同样大小的本地文件，
* 然后开始在后台线程下载。 
* 
* 已下载的数据先储存在一个MemoryStream中，然后写入本地文件中。 
* 
* 当读指定数目的数据时，就会引发DownloadProgressChanged事件。
* 当下载任务完成或被取消时，也会引发DownloadCompleted事件。
* 
* DownloadedSize属性存储着已下载的数据的大小，这里的数据用来重新开始下载。
* 
* StartPoint属性可用于多线程并发的情况，并且每个线程分别下载整个文件的一个特定的数据块。
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

namespace CSWebDownloadProgress
{
    public class HttpDownloadClient
    {
        //在创建或写一个文件的时候用到
        static object locker = new object();

        // 存储下载数据已用的时间. 
        // 不包括暂停的时间并且只在下载任务被暂停、取消或完成的时候被更新.
        private TimeSpan usedTime = new TimeSpan();

        private DateTime lastStartTime;

        /// <summary>
        /// 如果状态时正在下载，那么总时间就是下载用的时间. 否则的话总时间应该
        /// 包含在当前下载线程所用的时间中.
        /// </summary>
        public TimeSpan TotalUsedTime
        {
            get
            {
                if (this.Status != HttpDownloadClientStatus.Downloading)
                {
                    return usedTime;
                }
                else
                {
                    return usedTime.Add(DateTime.Now - lastStartTime);
                }
            }
        }

        // 最后的DownloadProgressChanged事件中的时间和大小.
        // 这两个字段是用来计算下载速度的.
        private DateTime lastNotificationTime;
        private Int64 lastNotificationDownloadedSize;

        // 当读指定数目的数据时，就会引发DownloadProgressChanged事件
        public int BufferCountPerNotification { get; private set; }

        // 下载文件的地址
        public Uri Url { get; private set; }

        // 存储文件的本地路径.
        // 如果没有相同名字的文件，就创建一个新的.
        public string DownloadPath { get; private set; }

        // StartPoint属性和EndPoint属性可用于多线程并发的情况，
        // 并且每个线程分别下载整个文件的一个特定的数据块。
        public int StartPoint { get; private set; }

        public int EndPoint { get; private set; }

        // 当读取响应流的数据时设定缓冲区的大小
        public int BufferSize { get; private set; }

        // 内存中缓存的大小
        public int MaxCacheSize { get; private set; }


        Int64 totalSize = 0;

        // 向服务器请求文件的大小并保存它.
        public Int64 TotalSize
        {
            get
            {
                if (totalSize == 0)
                {
                    var webRequest = (HttpWebRequest)WebRequest.Create(Url);
                    if (EndPoint != int.MaxValue)
                    {
                        webRequest.AddRange(StartPoint, EndPoint);
                    }
                    else
                    {
                        webRequest.AddRange(StartPoint);
                    }

                    // 处理Web响应
                    using (var response = webRequest.GetResponse())
                    {
                        totalSize = response.ContentLength;
                    }

                }
                return totalSize;
            }
        }

        // 已下载的数据的大小已写入本地文件.
        public Int64 DownloadedSize { get; private set; }

        HttpDownloadClientStatus status;

        // 如果状态改变，触发StatusChanged事件
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

        public event EventHandler<HttpDownloadProgressChangedEventArgs> DownloadProgressChanged;

        public event EventHandler<HttpDownloadCompletedEventArgs> DownloadCompleted;

        public event EventHandler StatusChanged;

        /// <summary>
        /// 下载整个文件
        /// </summary>
        public HttpDownloadClient(string url, string downloadPath)
            : this(url, downloadPath, 0)
        {
        }

        /// <summary>
        /// 从一个开始点下载文件到最后.
        /// </summary>
        public HttpDownloadClient(string url, string downloadPath,
           int startPoint)
            : this(url, downloadPath, startPoint, int.MaxValue)
        {
        }

        /// <summary>
        /// 下载文件的一个数据块. 默认的缓冲区大小是1KB，内存缓存是1MB，
        /// BufferCountPerNotification是64个
        /// </summary>
        public HttpDownloadClient(string url, string downloadPath,
          int startPoint, int endPoint)
            : this(url, downloadPath, startPoint, endPoint, 1024, 1048576, 64)
        {
        }

        public HttpDownloadClient(string url, string downloadPath, int startPoint,
            int endPoint, int bufferSize, int cacheSize, int bufferCountPerNotification)
        {
            if (startPoint < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "StartPoint不能小于0.");
            }

            if (endPoint < startPoint)
            {
                throw new ArgumentOutOfRangeException(
                    "EndPoint不能小于StartPoint. ");
            }

            if (bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferSize不能小于0.");
            }

            if (cacheSize < bufferSize)
            {
                throw new ArgumentOutOfRangeException(
                    "MaxCacheSize不能小于BufferSize.");
            }

            if (bufferCountPerNotification <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferCountPerNotification不能小于0. ");
            }

            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
            this.BufferSize = bufferSize;
            this.MaxCacheSize = cacheSize;
            this.BufferCountPerNotification = bufferCountPerNotification;

            this.Url = new Uri(url, UriKind.Absolute);
            this.DownloadPath = downloadPath;

            // 设定空闲的状态
            this.status = HttpDownloadClientStatus.Idle;

        }

        /// <summary>
        /// 开始下载. 
        /// </summary>
        public void Start()
        {
            // 检查目标文件是否存在.
            CheckFileOrCreateFile();

            // 只有空闲的下载客户端才能开始
            if (this.Status != HttpDownloadClientStatus.Idle)
            {
                throw new ApplicationException("只有空闲的下载客户端才能开始.");
            }

            // 开始在后台线程下载
            BeginDownload();
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void Pause()
        {
            // 只有正在下载的客户端才能暂停.
            if (this.Status != HttpDownloadClientStatus.Downloading)
            {
                throw new ApplicationException("只有正在下载的客户端才能暂停.");
            }

            // 后台线程会查看状态，如果状态时暂停的，
            // 下载将会被暂停并且状态将随之改为暂停.
            this.Status = HttpDownloadClientStatus.Pausing;
        }

        /// <summary>
        /// 重新开始下载.
        /// </summary>
        public void Resume()
        {
            // 只有暂停的客户端才能重新下载.
            if (this.Status != HttpDownloadClientStatus.Paused)
            {
                throw new ApplicationException("只有暂停的客户端才能重新下载.");
            }

            // 开始在后台线程进行下载.
            BeginDownload();
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        public void Cancel()
        {
            // 只有正在下载的或者是暂停的客户端才能被取消.
            if (this.Status != HttpDownloadClientStatus.Paused
                && this.Status != HttpDownloadClientStatus.Downloading)
            {
                throw new ApplicationException("只有正在下载的或者是暂停的客户端"
                    + "才能被取消.");
            }

            // 后台线程将查看状态.如果是正在取消，
            // 那么下载将被取消并且状态将改成已取消.
            this.Status = HttpDownloadClientStatus.Canceling;
        }

        /// <summary>
        /// 创建一个线程下载数据.
        /// </summary>
        void BeginDownload()
        {
            ThreadStart threadStart = new ThreadStart(Download);
            Thread downloadThread = new Thread(threadStart);
            downloadThread.IsBackground = true;
            downloadThread.Start();
        }

        /// <summary>
        /// 通过HttpWebRequest线程来下载数据.它会从响应流中读取一个字节的缓冲区，
        /// 并把它先储存在一个MemoryStream的缓存中。
        /// 如果缓存没有空间或者下载暂停、取消或完成了，就将缓存中的数据写入本地文件中。
        /// </summary>
        void Download()
        {
            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            Stream responseStream = null;
            MemoryStream downloadCache = null;
            lastStartTime = DateTime.Now;

            // 设定状态.
            this.Status = HttpDownloadClientStatus.Downloading;

            try
            {

                // 为需要下载的文件创建一个请求.
                webRequest = (HttpWebRequest)WebRequest.Create(Url);
                webRequest.Method = "GET";
                webRequest.Credentials = CredentialCache.DefaultCredentials;


                // 指定块下载
                if (EndPoint != int.MaxValue)
                {
                    webRequest.AddRange(StartPoint + DownloadedSize, EndPoint);
                }
                else
                {
                    webRequest.AddRange(StartPoint + DownloadedSize);
                }

                // 接受服务器端的响应并得到响应流.
                webResponse = (HttpWebResponse)webRequest.GetResponse();

                responseStream = webResponse.GetResponseStream();


                // 内存中的缓存数据.
                downloadCache = new MemoryStream(this.MaxCacheSize);

                byte[] downloadBuffer = new byte[this.BufferSize];

                int bytesSize = 0;
                int cachedSize = 0;
                int receivedBufferCount = 0;

                // 下载文件直到下载被暂停、取消或完成.
                while (true)
                {

                    // 从流中读取缓冲区的数据.
                    bytesSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length);

                    // 如果缓存是满的，或者下载被暂停、取消或完成，
                    // 就把缓存中的数据写入本地文件.
                    if (this.Status != HttpDownloadClientStatus.Downloading
                        || bytesSize == 0
                        || this.MaxCacheSize < cachedSize + bytesSize)
                    {

                        try
                        {
                            // 把缓存中的数据写入本地文件.
                            WriteCacheToFile(downloadCache, cachedSize);

                            this.DownloadedSize += cachedSize;

                            // 如果下载被暂停、取消或完成了，
                            // 就停止下载文件.
                            if (this.Status != HttpDownloadClientStatus.Downloading
                                || bytesSize == 0)
                            {
                                break;
                            }

                            // 读取缓存. 
                            downloadCache.Seek(0, SeekOrigin.Begin);
                            cachedSize = 0;
                        }
                        catch (Exception ex)
                        {
                            // 如果错误，触发DownloadCompleted事件.
                            this.OnDownloadCompleted(new HttpDownloadCompletedEventArgs(
                                               this.DownloadedSize, this.TotalSize,
                                               this.TotalUsedTime, ex));
                            return;
                        }

                    }


                    // 将数据从缓冲区写入内存的缓存中.
                    downloadCache.Write(downloadBuffer, 0, bytesSize);

                    cachedSize += bytesSize;

                    receivedBufferCount++;

                    // 触发DownloadProgressChanged事件.
                    if (receivedBufferCount == this.BufferCountPerNotification)
                    {
                        InternalDownloadProgressChanged(cachedSize);
                        receivedBufferCount = 0;
                    }
                }


                // 如果当前下载被停止了，更新所用的时间.
                usedTime = usedTime.Add(DateTime.Now - lastStartTime);

                // 更新客户端的状态. 在客户端的状态为暂停、取消或完成时，
                // 以上的循环将被终止.
                if (this.Status == HttpDownloadClientStatus.Pausing)
                {
                    this.Status = HttpDownloadClientStatus.Paused;
                }
                else if (this.Status == HttpDownloadClientStatus.Canceling)
                {
                    this.Status = HttpDownloadClientStatus.Canceled;

                    Exception ex = new Exception("由于用户的需求下载已被取消. ");

                    this.OnDownloadCompleted(new HttpDownloadCompletedEventArgs(
                                   this.DownloadedSize, this.TotalSize, this.TotalUsedTime, ex));
                }
                else
                {
                    this.Status = HttpDownloadClientStatus.Completed;
                    this.OnDownloadCompleted(new HttpDownloadCompletedEventArgs(
                                this.DownloadedSize, this.TotalSize, this.TotalUsedTime, null));
                    return;
                }

            }
            finally
            {
                // 当以上的代码执行完毕，关闭流.
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
        /// 检查目标文件是否存在，如果不存在，
        /// 创建一个与将要下载的文件一样大小的文件.
        /// </summary>
        void CheckFileOrCreateFile()
        {
            // 锁定其他的线程或者进程来避免创建文件.
            lock (locker)
            {
                FileInfo fileToDownload = new FileInfo(DownloadPath);
                if (fileToDownload.Exists)
                {

                    // 目标文件应该与将要被下载的文件大小一样.
                    if (fileToDownload.Length != this.TotalSize)
                    {
                        throw new ApplicationException(
                            "下载路径已经存在一个文件不匹配"
                            + "要下载的文件.");
                    }
                }

                // 创建一个文件.
                else
                {
                    if (TotalSize == 0)
                    {
                        throw new ApplicationException("要下载的文件不存在！");
                    }

                    using (FileStream fileStream = File.Create(this.DownloadPath))
                    {
                        long createdSize = 0;
                        byte[] buffer = new byte[4096];
                        while (createdSize < TotalSize)
                        {
                            int bufferSize = (TotalSize - createdSize) < 4096 ? (int)(TotalSize - createdSize) : 4096;
                            fileStream.Write(buffer, 0, bufferSize);
                            createdSize += bufferSize;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将缓存中的数据写入本地文件.
        /// </summary>
        void WriteCacheToFile(MemoryStream downloadCache, int cachedSize)
        {
            // 锁定其他线程或进程避免数据写入文件.
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


        protected virtual void OnDownloadCompleted(HttpDownloadCompletedEventArgs e)
        {
            if (DownloadCompleted != null)
            {
                DownloadCompleted(this, e);
            }
        }

        /// <summary>
        /// 计算下载速度并触发DownloadProgressChanged事件.
        /// </summary>
        /// <param name="cachedSize"></param>
        private void InternalDownloadProgressChanged(int cachedSize)
        {
            int speed = 0;
            DateTime current = DateTime.Now;
            TimeSpan interval = current - lastNotificationTime;

            if (interval.TotalSeconds < 60)
            {
                speed = (int)Math.Floor((this.DownloadedSize + cachedSize - this.lastNotificationDownloadedSize) / interval.TotalSeconds);
            }
            lastNotificationTime = current;
            lastNotificationDownloadedSize = this.DownloadedSize + cachedSize;

            this.OnDownloadProgressChanged(new HttpDownloadProgressChangedEventArgs
                           (this.DownloadedSize + cachedSize, this.TotalSize, speed));


        }

        protected virtual void OnDownloadProgressChanged(HttpDownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(this, e);
            }
        }

        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, e);
            }
        }
    }
}
