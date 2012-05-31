/****************************** 模块头 ******************************\
* 模块名称:  IDockingWindow.cs
* 项目:	    CSIEExplorerBar
* 版权： (c) Microsoft Corporation.
* 
* 公开的方法，通知对接窗口对象的变化，包括显示、隐藏和即将到来的移动。
* 此接口由可以与一个Windows资源管理器的边界空间对接的窗口对象实现。
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
    [Guid("012dd920-7b26-11d0-8ca9-00a0c92dbfe8")]
    public interface IDockingWindow
    {
        void GetWindow(out IntPtr phwnd);

        void ContextSensitiveHelp([In] bool fEnterMode);

        void ShowDW([In] bool fShow);

        void CloseDW([In] uint dwReserved);

        void ResizeBorderDW(
            IntPtr prcBorder, 
            [In, MarshalAs(UnmanagedType.IUnknown)] object punkToolbarSite,
            bool fReserved);


    }



}
