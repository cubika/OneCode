/******************************** 模块头 **************************************\
* 模块名:	HttpDownloadCompletedEventArgs.cs
* 项目名:   CSWebDownloadProgress
* 版权(c)   Microsoft Corporation.
* 
* HttpDownloadCompletedEventArgs类定义了HttpDownloadClient的DownloadCompleted事件
* 中使用的参数.
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

namespace CSWebDownloadProgress
{
    public class HttpDownloadCompletedEventArgs:EventArgs
    {
        public Int64 DownloadedSize { get; private set; }
        public Int64 TotalSize { get; private set; }
        public Exception Error { get;  private set; }
        public TimeSpan TotalTime { get; private set; }

        public HttpDownloadCompletedEventArgs(Int64 downloadedSize, 
            Int64 totalSize,TimeSpan totalTime, Exception ex)
        {
            this.DownloadedSize = downloadedSize;
            this.TotalSize = totalSize;
            this.TotalTime = totalTime;
            this.Error = ex;
        }
    }
}
