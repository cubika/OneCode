/****************************** 模块头 ******************************\
* 模块名:  IClrMetaHost.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 
* 提供如下方法：根据公共语言运行时（CLR）的版本数字返回一个具体的版本
* 列出所有已安装的CLR.
* 列出指定进程所加载的所有运行时.
* 发现曾用于编译程序集的CLR版本.
* 用一个干净的运行时shutdown退出一个进程, 并查询传统的API binding.
*
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace CSCheckEXEType.Hosting
{
    [ComImport]
    [SecurityCritical]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("D332DB9E-B9B3-4125-8207-A14884F53216")]
    public interface IClrMetaHost
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        object GetRuntime(
            [In, MarshalAs(UnmanagedType.LPWStr)] string version, 
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetVersionFromFile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string filePath, 
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer, 
            [In, Out, MarshalAs(UnmanagedType.U4)] ref uint bufferLength);

        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IEnumUnknown EnumerateInstalledRuntimes();

        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IEnumUnknown EnumerateLoadedRuntimes([In] IntPtr processHandle);

        [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int Reserved01([In] IntPtr reserved1);
    }
}
