/********************************* 模块头 *********************************\
* 模块名:  KeyModifiers.cs
* 项目名:  CSRegisterHotkey
* 版权(c)  Microsoft Corporation.
* 
* 这个枚举定义了产生WM_HOTKEY消息的修饰符 
* See http://msdn.microsoft.com/en-us/library/ms646309(VS.85).aspx.
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

namespace CSRegisterHotkey
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,

        // 任何一个Windows键都能被按下，这些按键是贴有Windows的徽标。
        // 包括WINDOWS按键的键盘快捷键是保留给操作系统的。
        Windows = 8
    }
}
