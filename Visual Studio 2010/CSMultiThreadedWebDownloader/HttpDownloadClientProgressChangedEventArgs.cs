/****************************** 模块头 ******************************\
* 模块名称:  HttpDownloadClientProgressChangedEventArgs.cs
* 项目名称:	    CSMultiThreadedWebDownloader
* 版权 (c) Microsoft Corporation.
* 
* HttpDownloadClientProgressChangedEventArgs 类定义了HttpDownloadClientdefines
 * 里的DownloadProgressChanged 事件所要用的参数。
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
    public class HttpDownloadClientProgressChangedEventArgs : EventArgs
    {
        public int Size { get; set; }
    }
}
