'********************************** 模块头 ******************************'
' 模块名:  CONTEXT_MENU_CONST.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 类 CONTEXT_MENU_CONST 定义常数，用以指定要显示的快捷菜单中的标识符
' 这些值在 Mshtmhst.h 中定义 
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Namespace NativeMethods
    Public NotInheritable Class CONTEXT_MENU_CONST
        ' Web 页的默认快捷菜单.
        Public Const CONTEXT_MENU_DEFAULT As Integer = 0

        ' 图像的快捷菜单.
        Public Const CONTEXT_MENU_IMAGE As Integer = 1

        ' 滚动条和选择元素的快捷菜单.  
        Public Const CONTEXT_MENU_CONTROL As Integer = 2

        ' 不使用.  
        Public Const CONTEXT_MENU_TABLE As Integer = 3

        ' 所选文本的的快捷菜单．  
        Public Const CONTEXT_MENU_TEXTSELECT As Integer = 4

        ' 超链接的快捷菜单. 
        Public Const CONTEXT_MENU_ANCHOR As Integer = 5

        ' 不使用. 
        Public Const CONTEXT_MENU_UNKNOWN As Integer = 6

        ' 内部使用.
        Public Const CONTEXT_MENU_IMGDYNSRC As Integer = 7
        Public Const CONTEXT_MENU_IMGART As Integer = 8
        Public Const CONTEXT_MENU_DEBUG As Integer = 9

        ' 垂直滚动条的快捷菜单. 
        Public Const CONTEXT_MENU_VSCROLL As Integer = 10

        ' 水平滚动条的快捷菜单. 
        Public Const CONTEXT_MENU_HSCROLL As Integer = 11
    End Class
End Namespace

