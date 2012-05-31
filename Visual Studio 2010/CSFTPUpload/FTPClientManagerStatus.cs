/****************************** 模块头 ******************************\
*模块名:  FTPClientStatus.cs
* 项目:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.

 * FTPClientStatus枚举包括所有FTPClient状态
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE
\***************************************************************************/

namespace CSFTPUpload
{
    public enum FTPClientManagerStatus
    {
        Idle,
        Uploading
    }
}
