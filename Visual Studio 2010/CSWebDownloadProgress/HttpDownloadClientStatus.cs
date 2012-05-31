/******************************** 模块头 **************************************\
* 模块名:   HttpDownloadClientStatus.cs
* 项目名:   CSWebDownloadProgress
* 版权(c)   Microsoft Corporation.
* 
* HttpDownloadClientStatus枚举包含所有HttpDownloadClient的状态.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

namespace CSWebDownloadProgress
{
    public enum HttpDownloadClientStatus
    {
        Idle,
        Downloading,
        Pausing,
        Paused,
        Canceling,
        Canceled,
        Completed
    }
}
