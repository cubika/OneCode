/*************************************** 模块头*****************************\
* 模块名:  IOleDropTarget.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 此接口允许对象和其容器派遣彼此的命令.例如，一个对象的工具栏可能包含如打印、
* 打印预览、 保存、 新建和缩放命令的按钮.
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
    [Guid("00000122-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleDropTarget
    {
        [PreserveSig]
        int OleDragEnter(
            [In, MarshalAs(UnmanagedType.Interface)] object pDataObj, 
            [In, MarshalAs(UnmanagedType.U4)] int grfKeyState, 
            [In, MarshalAs(UnmanagedType.U8)] long pt, 
            [In, Out] ref int pdwEffect);

        [PreserveSig]
        int OleDragOver(
            [In, MarshalAs(UnmanagedType.U4)] int grfKeyState, 
            [In, MarshalAs(UnmanagedType.U8)] long pt, 
            [In, Out] ref int pdwEffect);

        [PreserveSig]
        int OleDragLeave();
        [PreserveSig]
        int OleDrop(
            [In, MarshalAs(UnmanagedType.Interface)] object pDataObj, 
            [In, MarshalAs(UnmanagedType.U4)] int grfKeyState, 
            [In, MarshalAs(UnmanagedType.U8)] long pt,
            [In, Out] ref int pdwEffect);
    }

 

 

}
