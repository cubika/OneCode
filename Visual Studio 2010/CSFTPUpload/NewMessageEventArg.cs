/****************************** 模块头 ******************************\
* 模块名:  NewMessageEventArg.cs
* 项目:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
* NewMessageEventArg类定义参数被FTPClient NewMessageArrived事件使用
 * 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE
\***************************************************************************/

using System;

namespace CSFTPUpload
{
    public class NewMessageEventArg:EventArgs
    {
        public string NewMessage { get; set; }
    }
}
