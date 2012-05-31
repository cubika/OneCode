/******************************** 模块头 ********************************\
模块名:  FileDragDropExt.cpp
项目:    CppShellExtDragDropHandler
版权 (c) Microsoft Corporation.

这个示例代码演示了用C++创建鼠标拖放处理程序Shell.

当用户右击Shell对象来拖动一个对象，且用户尝试放下这个对象时，一个上下文菜单将显示出来。
鼠标拖放处理程序是一个能添加项到上下文菜单的上下文菜单。

这个鼠标拖放处理程序,将添加菜单项“在这创建硬连接”到上下文菜单中。当右击一个文件并且把它拖到
一个目录、硬盘或桌面上，上下文菜单中将显示这个“在这创建硬连接”菜单项。通过点击这个菜单项，
处理程序将在放置目录中，创建一个拖动文件的硬连接。这个链接的名字是“对于<原文件名>的硬连接”。


This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

#include "FileDragDropExt.h"
#include <strsafe.h>
#include <Shlwapi.h>
#pragma comment(lib, "shlwapi.lib")


#define IDM_RUNFROMHERE            0  // 命令的销毁标识符。

FileDragDropExt::FileDragDropExt() : m_cRef(1), 
    m_pszMenuText(L"在这里创建硬链接")
{
}


FileDragDropExt::~FileDragDropExt()
{
}


void FileDragDropExt::OnCreateHardLink(HWND hWnd)
{      
    // 仅仅得到拖动文件移动的部分路径，例如： "D:\test\file.txt" is reduced to "file.txt".
    wchar_t szExistingFileName[MAX_PATH];
    StringCchCopy(szExistingFileName, ARRAYSIZE(szExistingFileName), m_szSrcFile);
    PathStripPath(szExistingFileName);

    // 创建新的路径名。
    wchar_t szNewLinkName[MAX_PATH];
    HRESULT hr = StringCchPrintf(szNewLinkName, ARRAYSIZE(szNewLinkName), 
        L"%s\\硬链接到 %s", m_szTargetDir, szExistingFileName);
    if (FAILED(hr))
    {
        MessageBox(hWnd, L"新链接名称过长。操作失败.", 
            L"CppShellExtDragDropHandler", MB_ICONERROR);
        return;
    }

    // 检查这个文件名是否存在。
    if (PathFileExists(szNewLinkName))
    {
        MessageBox(hWnd, L"在此位置,已存在具有相同名称的文件.", 
            L"CppShellExtDragDropHandler", MB_ICONERROR);
        return;
    }

    // 在存在的文件和新的文件之间建立硬连接。
    if (!CreateHardLink(szNewLinkName, m_szSrcFile, NULL))
    {
        wchar_t szMessage[260];
        DWORD dwError = GetLastError();
        
        if (dwError == ERROR_NOT_SAME_DEVICE)
        {
            StringCchCopy(szMessage, ARRAYSIZE(szMessage), 
                L"无法建立该硬链接，因为所有硬链接到的文件," \
                L"必须是在同一卷上.");
        }
        else
        {
            StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
                L"无法建立该硬链接 w/err 0x%08lx", dwError);
        }
        
        MessageBox(hWnd, szMessage, L"CppShellExtDragDropHandler", MB_ICONERROR);
    }
}


#pragma region IUnknown

// 查询该组件支持的借口。
IFACEMETHODIMP FileDragDropExt::QueryInterface(REFIID riid, void **ppv)
{
    HRESULT hr = S_OK;

    if (IsEqualIID(IID_IUnknown, riid) || 
        IsEqualIID(IID_IContextMenu, riid))
    {
        *ppv = static_cast<IContextMenu *>(this);
    }
    else if (IsEqualIID(IID_IShellExtInit, riid))
    {
        *ppv = static_cast<IShellExtInit *>(this);
    }
    else
    {
        hr = E_NOINTERFACE;
        *ppv = NULL;
    }

    if (*ppv)
    {
        AddRef();
    }

    return hr;
}

// 增加对一个对象接口的引用计数。
IFACEMETHODIMP_(ULONG) FileDragDropExt::AddRef()
{
    return InterlockedIncrement(&m_cRef);
}

//  减少对一个对象接口的引用计数。
IFACEMETHODIMP_(ULONG) FileDragDropExt::Release()
{
    ULONG cRef = InterlockedDecrement(&m_cRef);
    if (0 == cRef)
    {
        delete this;
    }

    return cRef;
}

#pragma endregion


#pragma region IShellExtInit

// 初始化鼠标拖放处理程序。
IFACEMETHODIMP FileDragDropExt::Initialize(
    LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hKeyProgID)
{
    // 得到文件放置的路径。
    if (!SHGetPathFromIDList(pidlFolder, this->m_szTargetDir))
    {
		return E_FAIL;
    }

    // 获取正在被移动的文件。
    if (NULL == pDataObj)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = E_FAIL;

    FORMATETC fe = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
    STGMEDIUM stm;

    // pDataObj指针包含的对象正在运行。在这个例子中，我们得到一个对于移动文件或文件夹的枚举的HDROP处理。
    if (SUCCEEDED(pDataObj->GetData(&fe, &stm)))
    {
        //得到一个HDROP处理。
        HDROP hDrop = static_cast<HDROP>(GlobalLock(stm.hGlobal));
        if (hDrop != NULL)
        {
            // 确定多少个文件在这个操作中被调用。这个示例代码显示菜单项当仅仅一个文件（不是目录）被移动时。
            UINT nFiles = DragQueryFile(hDrop, 0xFFFFFFFF, NULL, 0);
            if (nFiles == 1)
            {
                // 获取文件的路径。
                if (0 != DragQueryFile(hDrop, 0, m_szSrcFile, ARRAYSIZE(m_szSrcFile)))
                {
                    // 这个路径必须不是目录，因为硬连接仅仅对文件而不是目录。
                    if (!PathIsDirectory(m_szSrcFile))
                    {
                        hr = S_OK;
                    }
                }
            }

            // [-or-]

            // 正准备移动文件和文件夹的枚举。
            //if (nFiles > 0)
            //{
            //    std::list<std::wstring> draggedFiles;
            //    wchar_t szFileName[MAX_PATH];
            //    for (UINT i = 0; i < nFiles; i++)
            //    {
            //        // 获取下一个文件的名称.
            //        if (0 != DragQueryFile(hDrop, i, szFileName, ARRAYSIZE(szFileName)))
            //        {
            //            // 路径不能为目录,因为硬链接只是用于文件而非目录.
            //            if (!PathIsDirectory(szFileName))
            //            {
            //                // 添加文件名称到列表中.
            //                draggedFiles.push_back(szFileName);
            //            }
            //        }
            //    }
            //          
            //    // 如果发现任何能工作的文件，返回S_OK。
            //    if (draggedFiles.size() > 0)
            //    {
            //        hr = S_OK;
            //    }
            //}

            GlobalUnlock(stm.hGlobal);
        }

        ReleaseStgMedium(&stm);
    }

    // 如果出了S_OK的任何值从这个方法中返回，这个菜党项不显示。
    return hr;
}

#pragma endregion


#pragma region IContextMenu

//
//   方法: FileDragDropExt::QueryContextMenu
//          
//   目的: 这个Shell调用IContextMenu::QueryContextMenu来允许上下文菜单处理程序添加菜单项到菜单中。
//	    它传递在hmenu参数的HMENU处理。这个indexMenu参数是来设置第一个添加的菜单项的索引。
//
IFACEMETHODIMP FileDragDropExt::QueryContextMenu(
    HMENU hMenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags)
{
    // 如果uFlags包含CMF_DEFAULTONLY则我们不做任何事情。
    if (CMF_DEFAULTONLY & uFlags)
    {
        return MAKE_HRESULT(SEVERITY_SUCCESS, 0, USHORT(0));
    }

    // 用InsertMenu或者InsertMenuItem来添加菜单项。

    MENUITEMINFO mii = { sizeof(mii) };
    mii.fMask = MIIM_ID | MIIM_TYPE | MIIM_STATE;
    mii.wID = idCmdFirst + IDM_RUNFROMHERE;
    mii.fType = MFT_STRING;
    mii.dwTypeData = m_pszMenuText;
    mii.fState = MFS_ENABLED;
    if (!InsertMenuItem(hMenu, indexMenu, TRUE, &mii))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }

    // 根据严重性返回一个HRESULT值来设置SEVERITY_SUCCESS。设置被分配代码抵消的最大命令标识符，加一（1）。
    return MAKE_HRESULT(SEVERITY_SUCCESS, 0, USHORT(IDM_RUNFROMHERE + 1));
}


//
//   方法: FileDragDropExt::InvokeCommand
//        
//   目的:  当一个用户点击菜单项来告诉处理程序来运行联系命令是这个方法被调用。lpcmi参数指向一个包含需要信息的结构体。
//
IFACEMETHODIMP FileDragDropExt::InvokeCommand(LPCMINVOKECOMMANDINFO pici)
{ 
    // pici->lpVerb的high-word必须是NULL，因为我们对于命令没有实现IContextMenu::GetCommandString 来指定任何变量。
    if (NULL != HIWORD(pici->lpVerb))
	{
		return E_INVALIDARG;
	}

    // 然而，lpcmi->lpVerb得low-word应该包含命令的抵消标识符。
    if (LOWORD(pici->lpVerb) == IDM_RUNFROMHERE)
	{
		OnCreateHardLink(pici->hwnd);
	}
	else
	{
		// 如果这个变量不能被鼠标拖放处理程序识别 ，它必须返回E_FAIL来允许它传到另外的鼠标拖放处理程序，这也许能是想这个变量。
		return E_FAIL;
	}

	return S_OK;
}


//
//  方法: FileDragDropExt::GetCommandString
//  如果一个用户的突出项目之一是添加上下文菜单处理程序，这个处理程序的IContextMenu::GetCommandString
//  方法被调用来请求一个将显示在Windows Explorer状态栏的Help文本字符串。这个方法也被调用来请求被分配到一个命令的变量字符串。
//  ANSI或者Unicode都能被请求。这个例子不需要为命令指定任何变量，所以这个方法直接返回E_NOTIMPL。
//
IFACEMETHODIMP FileDragDropExt::GetCommandString(UINT_PTR idCommand, 
    UINT uFlags, UINT *pwReserved, LPSTR pszName, UINT cchMax)
{
    return E_NOTIMPL;
}

#pragma endregion
