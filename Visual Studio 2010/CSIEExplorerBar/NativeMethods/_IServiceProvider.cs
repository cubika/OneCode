/****************************** 模块头 ******************************\
* 模块名称:  _IServiceProvider.cs
* 项目:	    CSIEExplorerBar
* 版权： (c) Microsoft Corporation.
* 
* 提供了一个查找确定的GUID服务的通用访问机制
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
using System.Runtime.InteropServices;

namespace CSIEExplorerBar.NativeMethods
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    internal interface _IServiceProvider
    {
        /// <summary>
        /// 作为任何通过实现IServiceProvider公开的服务的factory方法。
        /// </summary>
        void QueryService(
            ref Guid guid,
            ref Guid riid, 
            [MarshalAs(UnmanagedType.Interface)] out object Obj);
    }

 

 

}
