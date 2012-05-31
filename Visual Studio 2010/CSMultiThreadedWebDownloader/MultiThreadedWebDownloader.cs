/****************************** 模块头 ******************************\
* 模块名称:  MultiThreadedWebDownloader.cs
* 项目名称:	    CSMultiThreadedWebDownloader
* 版权 (c) Microsoft Corporation.
* 
* 这个类是采用多线程通过网络下载文件，它提供了一些公有的方法： 
*  Start, Pause, Resume and Cancel 
* 
* 在开始下载之前，远程服务器会检查它是否支持"Accept-Ranges" .
* 
* 当下载开始，它将检查文件是否存在。如果不存在，则创建一个新的和要下载一样大小的文件。
*  然后创建多个HttpDownloadClients开始下载文件
*
* 当下载一个指定大小的数据时 ，它将触发 DownloadProgressChanged事件。
* 当下载结束或者被取消时，会触发 DownloadCompleted事件。
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
using System.IO;
using System.Linq;
using System.Net;

namespace CSMultiThreadedWebDownloader
{
    public class MultiThreadedWebDownloader
    {
        // 用于计算下载速度。
        static object locker = new object();


        /// <summary>
        /// 文件下载的地址。 
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// 判断远程是否支持"Accept-Ranges"头。
        /// </summary>
        public bool IsRangeSupported { get; private set; }

        /// <summary>
        /// 文件的大小。
        /// </summary>
        public long TotalSize { get; private set; }

        private string downloadPath;

        /// <summary>
        /// 文件的存储路径。
        /// 如果不存在相同名字的文件，则创建一个新的。
        /// </summary>
        public string DownloadPath
        {
            get
            {
                return downloadPath;
            }
            set
            {
                if (this.Status != MultiThreadedWebDownloaderStatus.Checked)
                {
                    throw new ApplicationException(" 当状态是Checked时，才可以设置路径.");
                }

                downloadPath = value;
            }
        }

        /// <summary>
        ///  所有下载端的Proxy情况。
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        /// 下载文件的大小。
        /// </summary>
        public long DownloadedSize { get; private set; }


        // 存储下载时已用多长时间.不包含暂停的时间。 
        // 当下载状态是暂停，取消，或者完成 时，它才会更新。
        private TimeSpan usedTime = new TimeSpan();

        private DateTime lastStartTime;

        /// <summary>
        /// 如果下载状态是Downloading,总共用的时间就是usedTime. 否则，总时间应该包括 
        /// 当前现在线程所用的时间。
        /// </summary>
        public TimeSpan TotalUsedTime
        {
            get
            {
                if (this.Status != MultiThreadedWebDownloaderStatus.Downloading)
                {
                    return usedTime;
                }
                else
                {
                    return usedTime.Add(DateTime.Now - lastStartTime);
                }
            }
        }

        //  DownloadProgressChanged时间中的时间和大小。
        // 下面的2个是用来计算下载速度的。
        private DateTime lastNotificationTime;

        private long lastNotificationDownloadedSize;

        private int bufferCount = 0;

        /// <summary>
        /// 如果得到缓冲数，则触发DownloadProgressChanged事件。
        /// </summary>
        public int BufferCountPerNotification { get; private set; }

        /// <summary>
        /// 当在响应流中读取数据时设置 BufferSize。
        /// </summary>
        public int BufferSize { get; private set; }

        /// <summary>
        /// 缓存大小
        /// </summary>
        public int MaxCacheSize { get; private set; }

        MultiThreadedWebDownloaderStatus status;

        /// <summary>
        /// 如果状态改变，触发StatusChanged事件。
        /// </summary>
        public MultiThreadedWebDownloaderStatus Status
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

        /// <summary>
        /// 最大线程数。线程数的计算=最小值+TotalSize / MaxCacheSize。
        /// </summary>
        public int MaxThreadCount { get; private set; }

        //  HttpDownloadClients 用来下载文件。每一个它的实例使用一个线程，下载文件的一部分。
        List<HttpDownloadClient> downloadClients = null;

        public int DownloadThreadsCount
        {
            get
            {
                if (downloadClients != null)
                {
                    return downloadClients.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public event EventHandler<MultiThreadedWebDownloaderProgressChangedEventArgs>
            DownloadProgressChanged;

        public event EventHandler<MultiThreadedWebDownloaderCompletedEventArgs>
            DownloadCompleted;

        public event EventHandler StatusChanged;

        public event EventHandler<ErrorEventArgs> ErrorOccurred;


        /// <summary>
        /// 下载整个文件。默认的缓冲区大小是1KB，内存缓存是1MB，每个通知缓冲区计数为64。线程个数是逻辑处理器数量的一倍。
        /// </summary>
        public MultiThreadedWebDownloader(string url)
            : this(url, 1024, 1048576, 512, Environment.ProcessorCount * 2)
        {
        }

        public MultiThreadedWebDownloader(string url, int bufferSize, int cacheSize,
            int bufferCountPerNotification, int maxThreadCount)
        {

            if (bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferSize 不能小于 0. ");
            }

            if (cacheSize < bufferSize)
            {
                throw new ArgumentOutOfRangeException(
                    "MaxCacheSize 不能小于 BufferSize ");
            }

            if (bufferCountPerNotification <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "BufferCountPerNotification不能小于 0. ");
            }

            if (maxThreadCount < 1)
            {
                throw new ArgumentOutOfRangeException(
                       "maxThreadCount 不能小于 1. ");
            }

            this.Url = new Uri(url);
            this.BufferSize = bufferSize;
            this.MaxCacheSize = cacheSize;
            this.BufferCountPerNotification = bufferCountPerNotification;
            
            this.MaxThreadCount = maxThreadCount;
            
            //设置最大连接数（被ServicePoint允许的范围下）
            ServicePointManager.DefaultConnectionLimit = maxThreadCount;

            //初始化HttpDownloadClient。
            downloadClients = new List<HttpDownloadClient>();

            // 设置闲置状态。
            this.status = MultiThreadedWebDownloaderStatus.Idle;
        }

        /// <summary>
        /// 检查总共大小和文件的IsRangeSupported。
        /// 如果没有任何异常，文件就可以被下载 。 
        /// </summary>
        public void CheckFile()
        {

            // 文件只会在闲置的状态下检查。
            if (this.status != MultiThreadedWebDownloaderStatus.Idle)
            {
                throw new ApplicationException(
                    "文件只会在闲置的状态下检查。");
            }

            // 在远程服务器上检查文件的信息。
            var webRequest = (HttpWebRequest)WebRequest.Create(Url);

            //设置 proxy.
            if (Proxy != null)
            {
                webRequest.Proxy = Proxy;
            }

            using (var response = webRequest.GetResponse())
            {
                this.IsRangeSupported = response.Headers.AllKeys.Contains("Accept-Ranges");
                this.TotalSize = response.ContentLength;

                if (TotalSize <= 0)
                {
                    throw new ApplicationException(
                        "要下载的文件不存在！");
                }
            }

            // 设置为checked状态。
            this.Status = MultiThreadedWebDownloaderStatus.Checked;
        }


        /// <summary>
        ///检查目标文件是否存在。如果不存在，则创建一个新的和药下载的一样大小的文件。
        /// </summary>
        void CheckFileOrCreateFile()
        {
            // 锁定其他线程或者进程，去阻止创建本地文件。
            lock (locker)
            {
                FileInfo fileToDownload = new FileInfo(DownloadPath);
                if (fileToDownload.Exists)
                {

                    // 目标文件应该和下载的文件一样大小。
                    if (fileToDownload.Length != this.TotalSize)
                    {
                        throw new ApplicationException(
                            "路径下有同名的文件 ");
                    }
                }

                // 创建一个文件。
                else
                {
                    using (FileStream fileStream = File.Create(this.DownloadPath))
                    {
                        long createdSize = 0;
                        byte[] buffer = new byte[4096];
                        while (createdSize < TotalSize)
                        {
                            int bufferSize = (TotalSize - createdSize) < 4096 ?
                                (int)(TotalSize - createdSize) : 4096;
                            fileStream.Write(buffer, 0, bufferSize);
                            createdSize += bufferSize;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 开始下载。
        /// </summary>
        public void Start()
        {
            // 检查文件是否存在。 
            CheckFileOrCreateFile();

            // 只有状态是Checked 时，才会开始下载 。
            if (this.status != MultiThreadedWebDownloaderStatus.Checked)
            {
                throw new ApplicationException(
                    "只有状态是Checked 时，才会开始下载 。");
            }

            // 如果文件不支持"Accept-Ranges",然后创建一个 HttpDownloadClient
            // 使用单线程下载文件。 否则就创建多个HttpDownloadClients下载文件。
             
            if (!IsRangeSupported)
            {
                HttpDownloadClient client = new HttpDownloadClient(
                    this.Url,
                    this.DownloadPath,
                    0,
                    long.MaxValue,
                    this.BufferSize,
                    this.BufferCountPerNotification);
                client.TotalSize = this.TotalSize;
                this.downloadClients.Add(client);
            }
            else
            {
                // 计算每个client要下载的大小。
                int maxSizePerThread =
                    (int)Math.Ceiling((double)this.TotalSize / this.MaxThreadCount);
                if (maxSizePerThread < this.MaxCacheSize)
                {
                    maxSizePerThread = this.MaxCacheSize;
                }

                long leftSizeToDownload = this.TotalSize;

                // 线程的个数为：  MaxThreadCount 的最小值+ TotalSize / MaxCacheSize.
                
                int threadsCount =
                    (int)Math.Ceiling((double)this.TotalSize / maxSizePerThread);

                for (int i = 0; i < threadsCount; i++)
                {
                    long endPoint = maxSizePerThread * (i + 1) - 1;
                    long sizeToDownload = maxSizePerThread;

                    if (endPoint > this.TotalSize)
                    {
                        endPoint = this.TotalSize - 1;
                        sizeToDownload = endPoint - maxSizePerThread * i;
                    }

                    // 下载整个文件的一部分。
                    HttpDownloadClient client = new HttpDownloadClient(
                        this.Url,
                        this.DownloadPath,
                        maxSizePerThread * i,
                        endPoint);

                    client.TotalSize = sizeToDownload;
                    this.downloadClients.Add(client);
                }
            }

            // 设置 lastStartTime ，用于计算用时多少。
            lastStartTime = DateTime.Now;

            // 设置状态为：downloading 
            this.Status = MultiThreadedWebDownloaderStatus.Downloading;

            //开始所有的HttpDownloadClients.
            foreach (var client in this.downloadClients)
            {
                if (this.Proxy != null)
                {
                    client.Proxy = this.Proxy;
                }

                //加载 HttpDownloadClients事件。
                client.DownloadProgressChanged +=
                    new EventHandler<HttpDownloadClientProgressChangedEventArgs>(
                        client_DownloadProgressChanged);

                client.StatusChanged += new EventHandler(client_StatusChanged);

                client.ErrorOccurred += new EventHandler<ErrorEventArgs>(
                    client_ErrorOccurred);
                client.Start();
            }


        }

        /// <summary>
        /// 暂停下载。
        /// </summary>
        public void Pause()
        {
            //只有在 downloading状态下才能暂停。
            if (this.status != MultiThreadedWebDownloaderStatus.Downloading)
            {
                throw new ApplicationException(
                    "只有在 downloading状态下才能暂停。");
            }

            this.Status = MultiThreadedWebDownloaderStatus.Pausing;

            //暂停所有的 HttpDownloadClients. 只有所有client被暂停 ，
            //下载状态才会改成Paused.
            foreach (var client in this.downloadClients)
            {
                if (client.Status != HttpDownloadClientStatus.Completed)
                {
                    client.Pause();
                }
            }
        }

        /// <summary>
        ///继续下载 。
        /// </summary>
        public void Resume()
        {
            //只有paused状态才能暂停  
            if (this.status != MultiThreadedWebDownloaderStatus.Paused)
            {
                throw new ApplicationException(
                    "只有paused状态才能暂停 ");
            }

            // 设置lastStartTime 
            lastStartTime = DateTime.Now;

            // 设置状态为： downloading .
            this.Status = MultiThreadedWebDownloaderStatus.Downloading;

            //继续所有的HttpDownloadClients.
            foreach (var client in this.downloadClients)
            {
                if (client.Status != HttpDownloadClientStatus.Completed)
                {
                    client.Resume();
                }
            }

        }

        /// <summary>
        ///取消下载
        /// </summary>
        public void Cancel()
        {
            // 只有downloading状态才能取消。
            if (this.status != MultiThreadedWebDownloaderStatus.Downloading)
            {
                throw new ApplicationException(
                    "只有downloading状态才能取消。");
            }

            this.Status = MultiThreadedWebDownloaderStatus.Canceling;

            this.OnError(new ErrorEventArgs
                {
                    Error = new Exception("下载被取消")
                });

            // 取消所有的 HttpDownloadClients.
            foreach (var client in this.downloadClients)
            {
                if (client.Status != HttpDownloadClientStatus.Completed)
                {
                    client.Cancel();
                }
            }

        }

        /// <summary>
        /// 处理所有的 HttpDownloadClients的StatusChanged 事件。
        /// </summary>
        void client_StatusChanged(object sender, EventArgs e)
        {

            //如果所有的client都完成了,状态才改成 completed.
            if (this.downloadClients.All(
                client => client.Status == HttpDownloadClientStatus.Completed))
            {
                this.Status = MultiThreadedWebDownloaderStatus.Completed;
            }
            else
            {

                // 已完成的client不用考虑。 
                var nonCompletedClients = this.downloadClients.
                    Where(client => client.Status != HttpDownloadClientStatus.Completed);

                //如果所有的 nonCompletedClients是Paused,状态才能是Paused.
                if (nonCompletedClients.All(
                    client => client.Status == HttpDownloadClientStatus.Paused))
                {
                    this.Status = MultiThreadedWebDownloaderStatus.Paused;
                }

                // 如果所有的nonCompletedClients是Canceled,状态才是Canceled.
                if (nonCompletedClients.All(
                    client => client.Status == HttpDownloadClientStatus.Canceled))
                {
                    this.Status = MultiThreadedWebDownloaderStatus.Canceled;
                }
            }

        }

        /// <summary>
        /// 处理所有 HttpDownloadClients的  DownloadProgressChanged事件，并计算速度
        /// </summary>
        void client_DownloadProgressChanged(object sender, HttpDownloadClientProgressChangedEventArgs e)
        {
            lock (locker)
            {
                DownloadedSize += e.Size;
                bufferCount++;

                if (bufferCount == BufferCountPerNotification)
                {
                    if (DownloadProgressChanged != null)
                    {
                        int speed = 0;
                        DateTime current = DateTime.Now;
                        TimeSpan interval = current - lastNotificationTime;

                        if (interval.TotalSeconds < 60)
                        {
                            speed = (int)Math.Floor((this.DownloadedSize -
                                this.lastNotificationDownloadedSize) / interval.TotalSeconds);
                        }

                        lastNotificationTime = current;
                        lastNotificationDownloadedSize = this.DownloadedSize;

                        var downloadProgressChangedEventArgs =
                            new MultiThreadedWebDownloaderProgressChangedEventArgs(
                                DownloadedSize, TotalSize, speed);
                        this.OnDownloadProgressChanged(downloadProgressChangedEventArgs);

                    }

                    //重新设置bufferCount.
                    bufferCount = 0;
                }

            }
        }

        /// <summary>
        ///处理所有HttpDownloadClients的  ErrorOccurred 事件.
        /// </summary>
        void client_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            if (this.status != MultiThreadedWebDownloaderStatus.Canceling
                && this.status != MultiThreadedWebDownloaderStatus.Canceled)
            {
                this.Cancel();

                // 引发 ErrorOccurred 事件
                this.OnError(e);

            }

        }

        /// <summary>
        /// 引发 DownloadProgressChanged 事件.如果状态是Completed,则引发 DownloadCompleted事件。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDownloadProgressChanged(
            MultiThreadedWebDownloaderProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(this, e);
            }
        }

        /// <summary>
        /// 引发 StatusChanged 事件。
        /// </summary>
        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (StatusChanged != null)
            {

                if (this.status == MultiThreadedWebDownloaderStatus.Paused
                    || this.Status == MultiThreadedWebDownloaderStatus.Canceled
                    || this.Status == MultiThreadedWebDownloaderStatus.Completed)
                {
                    //当暂停时，更新时间 .
                    usedTime = usedTime.Add(DateTime.Now - lastStartTime);
                }


                StatusChanged(this, e);

                if (this.Status == MultiThreadedWebDownloaderStatus.Completed)
                {
                    MultiThreadedWebDownloaderCompletedEventArgs downloadCompletedEventArgs =
                        new MultiThreadedWebDownloaderCompletedEventArgs
                        (
                            this.DownloadedSize,
                            this.TotalSize,
                            this.TotalUsedTime
                        );

                    this.OnDownloadCompleted(downloadCompletedEventArgs);
                }
            }
        }

        /// <summary>
        ///引发   DownloadCompleted事件。
        /// </summary>
        protected virtual void OnDownloadCompleted(
            MultiThreadedWebDownloaderCompletedEventArgs e)
        {
            if (DownloadCompleted != null)
            {
                DownloadCompleted(this, e);
            }
        }

        /// <summary>
        ///引发 ErrorOccurred事件。
        /// </summary>
        protected virtual void OnError(ErrorEventArgs e)
        {
            if (ErrorOccurred != null)
            {
                ErrorOccurred(this, e);
            }
        }
    }
}
