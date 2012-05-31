/****************************** 模块头 ******************************\
* 模块名:  FileUploadCompletedEventArgs.cs
* 项目:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.

 * FileUploadCompletedEventArgs 类定义FTPClient FileUploadCompleted事件使用的参数。
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

namespace CSFTPUpload
{
    public class FileUploadCompletedEventArgs : EventArgs
    {
        public Uri ServerPath { get; set; }
        public FileInfo LocalFile { get; set; }
    }
}
