/******************************模块头 ******************************\
模块名:  FileContextMenuExt.h
项目:      CppShellExtContextMenuHandler
版权 (c) Microsoft Corporation.

该代码示例演示如何创建一个C++上下文菜单处理程序. 

上下文菜单应用程序是一个在已存在的上下文菜单中添加命令的系统外壳扩展应用程序.
上下文菜单应用程序是用来关联特定的文件类型，并执行一些具体的操作. 
尽管你可以在注册表中注册增加上下文菜单的文件类型，该处理程序可以动态地将菜单项添加到自定义对象的上下文菜单中.
当你在资源管理器中右击一个.cpp类型的文件时候，该程序将添加一个名叫"Display File Name (C++)"的子菜单项到上下文菜单中，
当你点击该子菜单项时候将提示该文件的全路径.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>
#include <shlobj.h>     // 接口IShellExtInit and IContextMenu定义头文件


class FileContextMenuExt : IShellExtInit, IContextMenu
{
public:
    // IUnknown接口
    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
    IFACEMETHODIMP_(ULONG) AddRef();
    IFACEMETHODIMP_(ULONG) Release();

    // IShellExtInit接口
    IFACEMETHODIMP Initialize(LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hKeyProgID);

    // IContextMenu接口
    IFACEMETHODIMP QueryContextMenu(HMENU hMenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags);
    IFACEMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO pici);
    IFACEMETHODIMP GetCommandString(UINT_PTR idCommand, UINT uFlags, UINT *pwReserved, LPSTR pszName, UINT cchMax);
	
    FileContextMenuExt();

protected:
    ~FileContextMenuExt();

private:
    // 组件引用计数 .
    long m_cRef;

    // 被选中文件名.
    wchar_t m_szSelectedFile[MAX_PATH];

    // 显示文件名的实现函数.
    void OnVerbDisplayFileName(HWND hWnd);

    PWSTR pszMenuText;
    PCSTR pszVerb;
    PCWSTR pwszVerb;
    PCSTR pszVerbCanonicalName;
    PCWSTR pwszVerbCanonicalName;
    PCSTR pszVerbHelpText;
    PCWSTR pwszVerbHelpText;
};