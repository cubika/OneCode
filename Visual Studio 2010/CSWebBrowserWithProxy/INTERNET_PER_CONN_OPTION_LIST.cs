/****************************** Module Header ******************************\
* 模块名称:  INTERNET_PER_CONN_OPTION_LIST.cs
* 项目名称:	    CSWebBrowserWithProxy
* Copyright (c) Microsoft Corporation.
*
* INTERNET_PER_CONN_OPTION包含internet连接的一个选项列表。
* 在 http://msdn.microsoft.com/en-us/library/aa385146(VS.85).aspx 中你能获得更多的 
* 详情。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Runtime.InteropServices;

namespace CSWebBrowserWithProxy
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct INTERNET_PER_CONN_OPTION_LIST
    {
        public int Size;

        // 设定连接，如果是NULL就意味着使用的是局域网。
        public System.IntPtr Connection;
        
        public int OptionCount;
        public int OptionError;

        // INTERNET_PER_CONN_OPTION的集合。
        public System.IntPtr pOptions;
    }
}
