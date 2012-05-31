/****************************** 模块头 ******************************\
* 模块名称:  HttpDownloadCompletedEventArgs.cs
* 项目名称:	    CSMultiThreadedWebDownloader
* 版权 (c) Microsoft Corporation.
* 
* MultiThreadedWebDownloaderCompletedEventArgs 定义了MultiThreadedWebDownloader
* 的 DownloadCompleted事件所要用的参数。
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

namespace CSMultiThreadedWebDownloader
{
    public class MultiThreadedWebDownloaderCompletedEventArgs:EventArgs
    {
        public long DownloadedSize { get; private set; }
        public long TotalSize { get; private set; }
        public TimeSpan TotalTime { get; private set; }

        public MultiThreadedWebDownloaderCompletedEventArgs(long downloadedSize, 
            long totalSize,TimeSpan totalTime)
        {
            this.DownloadedSize = downloadedSize;
            this.TotalSize = totalSize;
            this.TotalTime = totalTime;
        }
    }
}
