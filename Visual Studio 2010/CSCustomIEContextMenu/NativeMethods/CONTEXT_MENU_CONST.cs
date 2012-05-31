/*************************************** 模块头*****************************\
* 模块名:  CONTEXT_MENU_CONST.cs
* 项目名:  CSCustomIEContextMenu
* 版权 (c) Microsoft Corporation.
* 
* 类 CONTEXT_MENU_CONST 定义常数，用以指定要显示的快捷菜单中的标识符
* 这些值在 Mshtmhst.h 中定义．
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

namespace CSCustomIEContextMenu.NativeMethods
{
    public static class CONTEXT_MENU_CONST
    {
        // Web 页的默认快捷菜单.
        public const int CONTEXT_MENU_DEFAULT = 0;

        // 图像的快捷菜单. 
        public const int CONTEXT_MENU_IMAGE = 1;

        // 滚动条和选择元素的快捷菜单. 
        public const int CONTEXT_MENU_CONTROL = 2;

        // 不使用. 
        public const int CONTEXT_MENU_TABLE = 3;

        // 所选文本的的快捷菜单． 
        public const int CONTEXT_MENU_TEXTSELECT = 4;

        // 超链接的快捷菜单. 
        public const int CONTEXT_MENU_ANCHOR = 5;

        // 不使用.
        public const int CONTEXT_MENU_UNKNOWN = 6;

        // 内部使用．
        public const int CONTEXT_MENU_IMGDYNSRC = 7;
        public const int CONTEXT_MENU_IMGART = 8;
        public const int CONTEXT_MENU_DEBUG = 9;

        // 垂直滚动条的快捷菜单. 
        public const int CONTEXT_MENU_VSCROLL = 10;

        // 水平滚动条的快捷菜单. 
        public const int CONTEXT_MENU_HSCROLL = 11;
    }
}
