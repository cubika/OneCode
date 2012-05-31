/*************************************** 模块头*****************************\
* 模块名:  IOleInPlaceActiveObject.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 这个接口被对象应用程序实现，以便在他们活动时,为他们的对象提供支持.
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
using System.Security;

namespace CSCustomIEContextMenu.NativeMethods
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressUnmanagedCodeSecurity]
    [Guid("00000117-0000-0000-C000-000000000046")]
    public interface IOleInPlaceActiveObject
    {
        [PreserveSig]
        int GetWindow(out IntPtr hwnd);
        void ContextSensitiveHelp(int fEnterMode);

        [PreserveSig]
        int TranslateAccelerator([In] ref NativeMethods.MSG lpmsg);

        void OnFrameWindowActivate(bool fActivate);

        void OnDocWindowActivate(int fActivate);

        void ResizeBorder(
            [In] NativeMethods.COMRECT prcBorder,
            [In] NativeMethods.IOleInPlaceUIWindow pUIWindow,
            bool fFrameWindow);

        void EnableModeless(int fEnable);
    }
}
