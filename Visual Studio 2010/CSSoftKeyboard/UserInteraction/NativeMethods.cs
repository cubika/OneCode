/****************************** Module Header ******************************\
 * Module Name:  NativeMethods.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * 这个类包括了SendInput方法中使用到的结构体。
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

namespace CSSoftKeyboard.UserInteraction
{
    internal static class NativeMethods
    {

        // INPUT结构体中使用到的常量。
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;

        // KEYBDINPUT结构体中使用到的常量。
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;

        /// <summary>
        /// 被SendInput使用，以保存用于合成输入事件的信息，例如击键次数、鼠标移动、鼠标单击。
        /// http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            /// <summary>
            /// INPUT_MOUSE    0
            /// INPUT_KEYBOARD 1
            /// INPUT_HARDWARE 2
            /// </summary>
            public int type;
            public NativeMethods.INPUTUNION inputUnion;
        }

        /// <summary>
        /// 一个INPUTUNION结构体只包括一个域。
        /// http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {         
            [FieldOffset(0)]
            public NativeMethods.HARDWAREINPUT hi;
            [FieldOffset(0)]
            public NativeMethods.KEYBDINPUT ki;
            [FieldOffset(0)]
            public NativeMethods.MOUSEINPUT mi;
        }

        /// <summary>
        /// 有关模拟硬件事件的信息。
        /// http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        /// <summary>
        /// 有关模拟键盘事件的信息。
        /// http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;

            // KEYEVENTF_EXTENDEDKEY 0x0001
            // KEYEVENTF_KEYUP 0x0002
            // KEYEVENTF_SCANCODE 0x0008
            // KEYEVENTF_UNICODE 0x0004
            public int dwFlags;

            public int time;
            public IntPtr dwExtraInfo;
        }

        /// <summary>
        /// 有关模拟鼠标事件的信息。
        /// http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
    }


}
