/****************************** 模块头 ******************************\
* 模块名:  NewMessageEventArg.cs
* 项目名:	    CSFTPDownload
* 版权(c)  Microsoft Corporation.
* 
*  这个类NewMessageEventArg通过使用FTPClient的ewMessageArrived事件定义一些变量.

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

namespace CSFTPDownload
{
    public class NewMessageEventArg:EventArgs
    {
        public string NewMessage { get; set; }
    }
}
