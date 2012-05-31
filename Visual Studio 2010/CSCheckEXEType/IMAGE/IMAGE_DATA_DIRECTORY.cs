/****************************** 模块头 ******************************\
* 模块名:  IMAGE_DATA_DIRECTORY.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 表示数据目录. 
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

using System;

namespace CSCheckEXEType.IMAGE
{
    public struct IMAGE_DATA_DIRECTORY
    {

        // 数据虚地址
        public UInt32 VirtualAddress;

        // 数据大小
        public UInt32 Size;

    }
}
