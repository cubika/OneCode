'********************************** 模块头 **************************************'
' 模块名:  KeyModifiers.vb
' 项目名:  VBRegisterHotkey
' 版权(c)  Microsoft Corporation.
' 
' 这个枚举定义了产生WM_HOTKEY消息的修饰符  
' See http://msdn.microsoft.com/en-us/library/ms646309(VS.85).aspx.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'


<Flags()>
Public Enum KeyModifiers
    None = 0
    Alt = 1
    Control = 2
    Shift = 4

    '任何一个Windows键被按下，这些按键是贴有Windows的徽标。
    '包括WINDOWS按键的键盘快捷键是保留给操作系统的。
    Windows = 8

End Enum
