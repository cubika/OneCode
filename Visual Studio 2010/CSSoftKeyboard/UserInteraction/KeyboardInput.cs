/****************************** Module Header ******************************\
 * Module Name:  KeyboardInput.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * 这个类不再使用原来的UnsafeNativeMethods.SendInput 方法来合成按键。
 *   
 * 有三种情形：
 * 1.单个键被按下，例如“A”。
 * 2.带有修饰键的键被按下，例如“Ctrl+A”。
 * 3.可以被锁定的键被按下，例如大小写锁定键、数码锁定键、滚动锁定键。
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CSSoftKeyboard.UserInteraction
{
    public static class KeyboardInput
    {

        /// <summary>
        /// 单个按键被按下。
        /// </summary>
        public static void SendKey(int key)
        {
            SendKey(null, key);
        }

        /// <summary>
        /// 带有修饰键的按键被按下。
        /// </summary>
        public static void SendKey(IEnumerable<int> modifierKeys, int key)
        {
            if (key <= 0)
            {
                return;
            }

            // 只有单个按键被按下。
            if (modifierKeys == null || modifierKeys.Count()==0)
            {
                var inputs = new NativeMethods.INPUT[1];
                inputs[0].type = NativeMethods.INPUT_KEYBOARD;
                inputs[0].inputUnion.ki.wVk = (short)key;
                UnsafeNativeMethods.SendInput(1, inputs, Marshal.SizeOf(inputs[0]));
            }

            // 带有修饰符的按键被按下。
            else
            {
                // 为了模拟这种情形，输入的信息包括被锁定修饰键、按键事件、释放修饰键事件。
                // 例如，为了模拟Ctrl+C，我们必须发送三次输入信息。
                // 1、Ctrl被按下。
                // 2、C被按下。
                // 3、Ctrl被释放。
                var inputs = new NativeMethods.INPUT[modifierKeys.Count()*2 + 1];
                
                int i = 0;

                // 模拟锁定修饰键。
                foreach (var modifierKey in modifierKeys)
                {
                    inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                    inputs[i].inputUnion.ki.wVk = (short)modifierKey;
                    i++;
                }

                // 模拟按键。
                inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                inputs[i].inputUnion.ki.wVk = (short)key;
                i++;

                // 模拟释放修饰键。
                foreach (var modifierKey in modifierKeys)
                {
                    inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                    inputs[i].inputUnion.ki.wVk = (short)modifierKey; 
                    inputs[i].inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;
                    i++;
                }
                
                UnsafeNativeMethods.SendInput((uint)inputs.Length, inputs,
                    Marshal.SizeOf(inputs[0]));
            }
        }

        /// <summary>
        /// 可以被锁定的键被按下，例如大小写锁定键、数码锁定键、滚动锁定键。为了模拟，这样的键应该被按下和释放。
        /// </summary>
        public static void SendToggledKey(int key)
        {
            var inputs = new NativeMethods.INPUT[2];

            // 按下按键。
            inputs[0].type = NativeMethods.INPUT_KEYBOARD;
            inputs[0].inputUnion.ki.wVk = (short)key;

            // 释放按键。
            inputs[1].type = NativeMethods.INPUT_KEYBOARD;
            inputs[1].inputUnion.ki.wVk = (short)key;
            inputs[1].inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;

            UnsafeNativeMethods.SendInput(2, inputs, Marshal.SizeOf(inputs[0]));
        }
    }
}
