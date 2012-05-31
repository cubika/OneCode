/****************************** 模块头 ******************************\
* 模块名称:  MultiThreadedWebDownloaderProgressChangedEventArgs.cs
* 项目名称:	    CSMultiThreadedWebDownloader
* 版权 (c) Microsoft Corporation.
* 
*  MultiThreadedWebDownloaderProgressChangedEventArgs类定义了MultiThreadedWebDownloader
* 的DownloadProgressChanged 事件所要用的参数 
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
    public class MultiThreadedWebDownloaderProgressChangedEventArgs:EventArgs
    {
        public long ReceivedSize { get; private set; }
        public long TotalSize { get; private set; }
        public int DownloadSpeed { get; private set; }

        public MultiThreadedWebDownloaderProgressChangedEventArgs(long receivedSize,
            long totalSize, int downloadSpeed)
        {
            this.ReceivedSize = receivedSize;
            this.TotalSize = totalSize;
            this.DownloadSpeed = downloadSpeed;
        }
    }
}
