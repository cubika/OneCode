/*************************************** 模块头*****************************\
* 模块名:  IOleInPlaceUIWindow.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 当一个对象被激活时,或如果对象的大小发生了变化,重新协商边框区域时,这个接口被
* 对象的应用程序用来协商在文档或框架窗口的边框区域.
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
    [Guid("00000115-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleInPlaceUIWindow
    {
        IntPtr GetWindow();

        [PreserveSig]
        int ContextSensitiveHelp(int fEnterMode);

        [PreserveSig]
        int GetBorder([Out] NativeMethods.COMRECT lprectBorder);

        [PreserveSig]
        int RequestBorderSpace([In] NativeMethods.COMRECT pborderwidths);

        [PreserveSig]
        int SetBorderSpace([In] NativeMethods.COMRECT pborderwidths);

        void SetActiveObject(
            [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceActiveObject pActiveObject,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszObjName);
    }

 

 

}
