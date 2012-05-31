/*************************************** 模块头*****************************\
* 模块名:  IObjectWithSite.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 提供有轻量级定位机制的简单对象(比 IOleObject 轻便). 一个 BHO 必须实现此接口.
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

namespace CSCustomIEContextMenu.NativeMethods
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")]
    public interface IObjectWithSite
    {
        void SetSite([In, MarshalAs(UnmanagedType.IUnknown)] Object pUnkSite);

        void GetSite(
            ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out Object ppvSite);
    }
}
