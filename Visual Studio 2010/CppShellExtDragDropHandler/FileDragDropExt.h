/******************************模块头 ******************************\
模块名:  FileDragDropExt.h
项目:      CppShellExtDragDropHandler
版权(c) Microsoft Corporation.

这个代码示例演示了作为C++创建一个Shell鼠标拖放处理程序. 

当用户右键点击一个Shell对象拖动一个对象时，当用户试图移除这个对象时上下文菜单被显
示.一个鼠标处理程序是一个上下文菜单处理程序，该上下文菜单处理程序对于上下文菜单能添加项

这个鼠标处理程序例子对于上下文菜单添加这个菜单项“在这创建硬链接”。当您右键单击一个文件，
拖动文件到目录或驱动器或桌面时，上下文菜单将显示“这里创建硬连接“菜单项。通过点击菜单项，
处理程序将在文件拖放的位置中创建一个硬链接这个链接的名字是“硬链接<资源文件名>”。


This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>
#include <shlobj.h>     // For IShellExtInit and IContextMenu


class FileDragDropExt : IShellExtInit, IContextMenu
{
public:
    // IUnknown
    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
    IFACEMETHODIMP_(ULONG) AddRef();
    IFACEMETHODIMP_(ULONG) Release();

    // IShellExtInit
    IFACEMETHODIMP Initialize(LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hKeyProgID);

    // IContextMenu
    IFACEMETHODIMP QueryContextMenu(HMENU hMenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags);
    IFACEMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO pici);
    IFACEMETHODIMP GetCommandString(UINT_PTR idCommand, UINT uFlags, UINT *pwReserved, LPSTR pszName, UINT cchMax);
	
    FileDragDropExt();

protected:
    ~FileDragDropExt();

private:
    // 引用组件的数量.
    long m_cRef;

    // 拖动的文件.
    wchar_t m_szSrcFile[MAX_PATH];

    // 这个目录是文件放的地方.
    wchar_t m_szTargetDir[MAX_PATH];

    // 这个方法是处理“这里创建硬链接”命令.
    void OnCreateHardLink(HWND hWnd);

    PWSTR m_pszMenuText;
};