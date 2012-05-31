/****************************** 模块头 ******************************\
* 模块名:  FTPDownloadClient.cs
* 项目名:	    CSFTPDownload
* 版权(c)  Microsoft Corporation.
* 
* 这个类被经常用来从FTP下载文件。当下载启动时，他将下载的文件加入到后台的线程
* 中，下载的数据储存在第一个MemoryStream中，然后再写入本地文件。

 
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
using System.Net;
using System.Threading;

namespace CSFTPDownload
{
    public partial class FTPClientManager
    {

        public class FTPDownloadClient
        {
            // 2M bytes.
            public const int MaxCacheSize = 2097152;

            // 2K bytes.
            public const int BufferSize = 2048;

            FTPClientManager manager;

            public event EventHandler<FileDownloadCompletedEventArgs> 
                FileDownloadCompleted;

            public event EventHandler AllFilesDownloadCompleted;

            public FTPDownloadClient(FTPClientManager manager)
            {
                if (manager == null)
                {
                    throw new ArgumentNullException("FTPClientManager cannot be null.");
                }

                this.manager = manager;
            }

            /// <summary>
            ///下载文件，目录和子目录.
            /// </summary>
            public void DownloadDirectoriesAndFiles(IEnumerable<FTPFileSystem> files,
                string localPath)
            {
                if (files == null)
                {
                    throw new ArgumentNullException(
                        "The files to download cannot be null.");
                }

                // 创建一个下载数据的线程.
                ParameterizedThreadStart threadStart =
                    new ParameterizedThreadStart(StartDownloadDirectoriesAndFiles);
                Thread downloadThread = new Thread(threadStart);
                downloadThread.IsBackground = true;
                downloadThread.Start(new object[] { files, localPath });
            }

            /// <summary>
            /// 下载文件，目录和它的子目录.
            /// </summary>
            void StartDownloadDirectoriesAndFiles(object state)
            {
                var paras = state as object[];

                IEnumerable<FTPFileSystem> files = paras[0] as IEnumerable<FTPFileSystem>;
                string localPath = paras[1] as string;

                foreach (var file in files)
                {
                    DownloadDirectoryOrFile(file, localPath);
                }

                this.OnAllFilesDownloadCompleted(EventArgs.Empty);
            }

            /// <summary>
            /// 下载一个单一的文件和目录.
            /// </summary>
            void DownloadDirectoryOrFile(FTPFileSystem fileSystem, string localPath)
            {

                // 下载文件目录.
                if (!fileSystem.IsDirectory)
                {
                    DownloadFile(fileSystem, localPath);
                }

                // 下载一个目录.
                else
                {

                    //目录路径的结构.
                    string directoryPath = localPath + "\\" + fileSystem.Name;

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    //获得子目录和文件.
                    var subDirectoriesAndFiles =
                        this.manager.GetSubDirectoriesAndFiles(fileSystem.Url);

                    // 下载文件夹的文件和子目录.
                    foreach (var subFile in subDirectoriesAndFiles)
                    {
                        DownloadDirectoryOrFile(subFile, directoryPath);
                    }
                }
            }

            /// <summary>
            /// 下载一个单一的文件目录.
            /// </summary>
            void DownloadFile(FTPFileSystem file, string localPath)
            {
                if (file.IsDirectory)
                {
                    throw new ArgumentException(
                        "The FTPFileSystem to download is a directory in fact");
                }

                string destPath = localPath + "\\" + file.Name;

                //创建一个要下载的文件的请求.
                FtpWebRequest request = WebRequest.Create(file.Url) as FtpWebRequest;

                request.Credentials = this.manager.Credentials;

                // 下载文件.
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse response = null;
                Stream responseStream = null;
                MemoryStream downloadCache = null;


                try
                {

                    //检索来自服务器的响应，并得到响应流.
                    response = request.GetResponse() as FtpWebResponse;

                    this.manager.OnNewMessageArrived(new NewMessageEventArg
                    {
                        NewMessage = response.StatusDescription
                    });

                    responseStream = response.GetResponseStream();

                    //内存中的缓存数据.
                    downloadCache = new MemoryStream(FTPDownloadClient.MaxCacheSize);
                    byte[] downloadBuffer = new byte[FTPDownloadClient.BufferSize];

                    int bytesSize = 0;
                    int cachedSize = 0;

                    // 下载这个文件直到下载完成.
                    while (true)
                    {

                        //从流中读取数据的缓冲区。 
                        bytesSize = responseStream.Read(downloadBuffer, 0, 
                            downloadBuffer.Length);

                        //如果这个缓存区是空的，或者下载完成，把在缓存中的数据写到本地文件中。
                       if (bytesSize == 0
                            || MaxCacheSize < cachedSize + bytesSize)
                        {
                            try
                            {
                                // 把在缓存中的数据写到本地文件中.
                                WriteCacheToFile(downloadCache, destPath, cachedSize);

                                //如果被暂停下载将停止正在下载的文件，取消或者完成. 
                                if (bytesSize == 0)
                                {
                                    break;
                                }

                                // 重置缓存.
                                downloadCache.Seek(0, SeekOrigin.Begin);
                                cachedSize = 0;
                            }
                            catch (Exception ex)
                            {
                                string msg = string.Format(
                                    "There is an error while downloading {0}. "
                                    + " See InnerException for detailed error. ", 
                                    file.Url);
                                ApplicationException errorException 
                                    = new ApplicationException(msg, ex);

                                //这个错误给了DownloadCompleted事件
                                ErrorEventArgs e = new ErrorEventArgs
                                {
                                    ErrorException = errorException
                                };

                                this.manager.OnErrorOccurred(e);

                                return;
                            }

                        }

                        //把缓冲区的数据写到内存中的缓存。 
                        
                        downloadCache.Write(downloadBuffer, 0, bytesSize);
                        cachedSize += bytesSize;
                    }

                    var fileDownloadCompletedEventArgs = new FileDownloadCompletedEventArgs
                    {

                        LocalFile = new FileInfo(destPath),
                        ServerPath = file.Url
                    };

                    this.OnFileDownloadCompleted(fileDownloadCompletedEventArgs);
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }

                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }

                    if (downloadCache != null)
                    {
                        downloadCache.Close();
                    }
                }
            }

            /// <summary>
            ///写缓存中的数据到本地文件.
            /// </summary>
            void WriteCacheToFile(MemoryStream downloadCache, string downloadPath,
                int cachedSize)
            {
                using (FileStream fileStream = new FileStream(downloadPath, 
                    FileMode.Append))
                {
                    byte[] cacheContent = new byte[cachedSize];
                    downloadCache.Seek(0, SeekOrigin.Begin);
                    downloadCache.Read(cacheContent, 0, cachedSize);
                    fileStream.Write(cacheContent, 0, cachedSize);
                }
            }

            protected virtual void OnFileDownloadCompleted(FileDownloadCompletedEventArgs e)
            {

                if (FileDownloadCompleted != null)
                {
                    FileDownloadCompleted(this, e);
                }
            }

            protected virtual void OnAllFilesDownloadCompleted(EventArgs e)
            {
                if (AllFilesDownloadCompleted != null)
                {
                    AllFilesDownloadCompleted(this, e);
                }
            }
        }
    }
}
