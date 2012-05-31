/****************************** Module Header ******************************\
 * Module Name:  UnsafeNativeMethods.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 *
 * 这些方法用来获得或者设置前台窗口。
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

namespace CSSoftKeyboard.NoActivate
{
    internal static class UnsafeNativeMethods
    {
        /// <summary>
        /// 获取前台窗口的句柄。
        /// http://msdn.microsoft.com/en-us/library/ms633505(VS.85).aspx
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 把创建了指定窗口的线程设置为前台线程，并且激活窗口。
        /// http://msdn.microsoft.com/en-us/library/ms633539(VS.85).aspx
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 判断指定的窗口句柄是否能确定一个已经存在的窗口。
        /// http://msdn.microsoft.com/en-us/library/ms633528(VS.85).aspx
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool IsWindow(IntPtr hWnd);
    }
}
