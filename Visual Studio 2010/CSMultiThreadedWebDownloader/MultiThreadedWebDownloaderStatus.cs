/****************************** 模块头 ******************************\
* 模块名称:  MultiThreadedWebDownloaderStatus.cs
* 项目名称:	    CSMultiThreadedWebDownloader
* 版权 (c) Microsoft Corporation.
* 
* MultiThreadedWebDownloaderStatus 枚举包含MultiThreadedWebDownloader 的所有状态。
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

namespace CSMultiThreadedWebDownloader
{
    public enum MultiThreadedWebDownloaderStatus
    {
        Idle = 0,
        Checked = 1,
        Downloading = 2,
        Pausing = 3,
        Paused = 4,
        Canceling = 5,
        Canceled = 6,
        Completed = 7
    }
}
