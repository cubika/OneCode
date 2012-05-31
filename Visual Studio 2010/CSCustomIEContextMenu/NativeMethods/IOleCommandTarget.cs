/*************************************** 模块头*****************************\
* 模块名: IOleCommandTarget.cs
* 项目名: CSCustomIEContextMenu
* 版权 (c)  Microsoft Corporation.
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
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
    public interface IOleCommandTarget
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryStatus(
            ref Guid pguidCmdGroup, 
            int cCmds, 
            [In, Out] NativeMethods.OLECMD prgCmds, 
            [In, Out] IntPtr pCmdText);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Exec(
            ref Guid pguidCmdGroup, 
            int nCmdID, 
            int nCmdexecopt, 
            [In, MarshalAs(UnmanagedType.LPArray)] object[] pvaIn, int pvaOut);
    }
}
