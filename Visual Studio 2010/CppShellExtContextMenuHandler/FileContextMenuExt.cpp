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

#include "FileContextMenuExt.h"
#include <strsafe.h>
#include <Shlwapi.h>
#pragma comment(lib, "shlwapi.lib")


#define IDM_DISPLAY             0  // The command's identifier offset

FileContextMenuExt::FileContextMenuExt() : m_cRef(1), 
    pszMenuText(L"&Display File Name (C++)"),
    pszVerb("cppdisplay"),
    pwszVerb(L"cppdisplay"),
    pszVerbCanonicalName("CppDisplayFileName"),
    pwszVerbCanonicalName(L"CppDisplayFileName"),
    pszVerbHelpText("Display File Name (C++)"),
    pwszVerbHelpText(L"Display File Name (C++)")
{
}

FileContextMenuExt::~FileContextMenuExt()
{
}


void FileContextMenuExt::OnVerbDisplayFileName(HWND hWnd)
{
    wchar_t szMessage[300];
    if (SUCCEEDED(StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
        L"您选中的文件是:\r\n\r\n%s", this->m_szSelectedFile)))
    {
        MessageBox(hWnd, szMessage, L"CppShellExtContextMenuHandler", MB_OK);
    }
}


#pragma region IUnknown

//实现COM组件的QueryInterface方法.
IFACEMETHODIMP FileContextMenuExt::QueryInterface(REFIID riid, void **ppv)
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

//组件的应用计数加一.
IFACEMETHODIMP_(ULONG) FileContextMenuExt::AddRef()
{
    return InterlockedIncrement(&m_cRef);
}

// 组件的应用计数减一.
IFACEMETHODIMP_(ULONG) FileContextMenuExt::Release()
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

// 初始化上下文菜单程序.
IFACEMETHODIMP FileContextMenuExt::Initialize(
    LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hKeyProgID)
{
    if (NULL == pDataObj)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = E_FAIL;

    FORMATETC fe = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
    STGMEDIUM stm;

    // pDataObj 指针指向对象. 在这示例，我们得到一个HDROP的句柄的枚举在选定的文件和文件夹。
    if (SUCCEEDED(pDataObj->GetData(&fe, &stm)))
    {
        // 得到HDROP句柄
        HDROP hDrop = static_cast<HDROP>(GlobalLock(stm.hGlobal));
        if (hDrop != NULL)
        {
            //确定在此操作中涉及多少个文件.  
            //只有一个文件被选中时，示例代码将显示自定义上下文菜单项中. 
            UINT nFiles = DragQueryFile(hDrop, 0xFFFFFFFF, NULL, 0);
            if (nFiles == 1)
            {
                //获取文件名.
                if (0 != DragQueryFile(hDrop, 0, m_szSelectedFile, 
                    ARRAYSIZE(m_szSelectedFile)))
                {
                    hr = S_OK;
                }
            }

            // [-或者-]

            // 枚举被选中的文件和文件夹.
            //if (nFiles > 0)
            //{
            //    std::list<std::wstring> selectedFiles;
            //    wchar_t szFileName[MAX_PATH];
            //    for (UINT i = 0; i < nFiles; i++)
            //    {
            //        //取得下一个文件的文件名.
            //        if (0 != DragQueryFile(hDrop, i, szFileName, ARRAYSIZE(szFileName)))
            //        {
            //            // Add the file name to the list.
            //            selectedFiles.push_back(szFileName);
            //        }
            //    }

            //    // If we found any files we can work with, return S_OK.
            //    if (selectedFiles.size() > 0) 
            //    {
            //        hr = S_OK;
            //    }
            //}

            GlobalUnlock(stm.hGlobal);
        }

        ReleaseStgMedium(&stm);
    }

        
	//如果返回的不是S_OK ，则上下文菜单将不会显示
    return hr;
}

#pragma endregion


#pragma region IContextMenu

//
//   函数: FileContextMenuExt::QueryContextMenu
//
//   目的:    外壳程序调用IContextMenu::QueryContextMenu添加子菜单项到上下文菜单中. 通过在 hmenu参数中的句柄. indexMenu为第一个菜单项目的索引.
//
IFACEMETHODIMP FileContextMenuExt::QueryContextMenu(
    HMENU hMenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags)
{
    // 如果uFlags参数包含CMF_DEFAULTONLY，我们就可以什么都不要做了.
    if (CMF_DEFAULTONLY & uFlags)
    {
        return MAKE_HRESULT(SEVERITY_SUCCESS, 0, USHORT(0));
    }

    // 使用InsertMenu 或 InsertMenuItem添加菜单项
    // 如何添加子菜单项:
    // http://www.codeproject.com/KB/shell/ctxextsubmenu.aspx

    MENUITEMINFO mii = { sizeof(mii) };
    mii.fMask = MIIM_ID | MIIM_TYPE | MIIM_STATE;
    mii.wID = idCmdFirst + IDM_DISPLAY;
    mii.fType = MFT_STRING;
    mii.dwTypeData = pszMenuText;
    mii.fState = MFS_ENABLED;
    if (!InsertMenuItem(hMenu, indexMenu, TRUE, &mii))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }

    // 增加一个分隔符.
    MENUITEMINFO sep = { sizeof(sep) };
    sep.fMask = MIIM_TYPE;
    sep.fType = MFT_SEPARATOR;
    if (!InsertMenuItem(hMenu, indexMenu + 1, TRUE, &sep))
    {
        return HRESULT_FROM_WIN32(GetLastError());
    }

    // 返回一个HRESULT类型的值.
    // 将代码值设置为最大的命令标识符的偏移量
    // 将被分配, 加一.
    return MAKE_HRESULT(SEVERITY_SUCCESS, 0, USHORT(IDM_DISPLAY + 1));
}


//
//   函数: FileContextMenuExt::InvokeCommand
//
//   目的:	  当用户点击相关联的菜单项后则调用该方法. 参数lpcmi是一个指向相关必须信息的指针.
//
IFACEMETHODIMP FileContextMenuExt::InvokeCommand(LPCMINVOKECOMMANDINFO pici)
{
    BOOL fUnicode = FALSE;

	//检查什么类型的结构体被传递进函数，CMINVOKECOMMANDINFO还是CMINVOKECOMMANDINFOEX取决于
	//lpcmi的成员变量cbSize，虽然lpcmi在 Shlobj.h中指向CMINVOKECOMMANDINFO结构体
	//但通常lpcmi指向的是一个CMINVOKECOMMANDINFOEX结构体，CMINVOKECOMMANDINFOEX是CMINVOKECOMMANDINFO的扩展
	//可以接受Unicode字符串
    if (pici->cbSize == sizeof(CMINVOKECOMMANDINFOEX))
    {
        if (pici->fMask & CMIC_MASK_UNICODE)
        {
            fUnicode = TRUE;
        }
    }

    // 检查命令是动态的还是偏移产生的.
    // 有两种方法来确定:
    // 
    //   1) 命令是动态的 
    //   2) 命令是偏移的
    // 
	//如果在ANSI的情况下lpcmi->lpVerb或Unicode情况下的lpcmi->lpVerbW为非0,且保存了一个动态的字符串则可以判断其为动态的，
	//反之则可以确定为偏移的

    if (!fUnicode && HIWORD(pici->lpVerb))
    {
        //上下文菜单是否支持动态的
        if (StrCmpIA(pici->lpVerb, pszVerb) == 0)
        {
            OnVerbDisplayFileName(pici->hwnd);
        }
        else
        {
            // 如果上下文菜单处理程序不能识别该命令, 必须返回E_FAIL,并传递到其他的能识别该命令的上下文菜单项上.
            return E_FAIL;
        }
    }

    //在Unicode情况下, 不为0, 动态命令保存在 lpcmi->lpVerbW. 
    else if (fUnicode && HIWORD(((CMINVOKECOMMANDINFOEX*)pici)->lpVerbW))
    {
        // 该上下文菜单是否支持动态
        if (StrCmpIW(((CMINVOKECOMMANDINFOEX*)pici)->lpVerbW, pwszVerb) == 0)
        {
            OnVerbDisplayFileName(pici->hwnd);
        }
        else
        {
            // 如果上下文菜单处理程序不能识别该命令, 必须返回E_FAIL,并传递到其他的能识别该命令的上下文菜单项上.
            return E_FAIL;
        }
    }

    // 如果该命令不能通过动词字符串确定,则可以确定为偏移的
    else
    {
        //上下文菜单是否支持偏移的
        if (LOWORD(pici->lpVerb) == IDM_DISPLAY)
        {
            OnVerbDisplayFileName(pici->hwnd);
        }
        else
        {
            //如果上下文菜单处理程序不能识别该命令, 必须返回E_FAIL,并传递到其他的能识别该命令的上下文菜单项上.
            return E_FAIL;
        }
    }

    return S_OK;
}


//
//   函数: CFileContextMenuExt::GetCommandString
//
//   目的: IContextMenu::GetCommandString方法被用来检索并返回子菜单项的文本, 例如，为菜单项显示帮助文本. 
//		   如果用户选中了上下文菜单中添加的子菜单,
//		   应用程序的IContextMenu::GetCommandString 方法将被调用来获取帮助文本并显示在资源管理器的状态栏上
//         ANSI或Unicode的字符集都可以被使用. 示例程序只使用Unicode的uFlags参数, 
//		   因为自Windows 2000以后资源管理器只接受Unicode的字符集.
//
IFACEMETHODIMP FileContextMenuExt::GetCommandString(UINT_PTR idCommand, 
    UINT uFlags, UINT *pwReserved, LPSTR pszName, UINT cchMax)
{
    HRESULT hr = E_INVALIDARG;

    if (idCommand == IDM_DISPLAY)
    {
        switch (uFlags)
        {
        case GCS_HELPTEXTW:
            // 每个Vista都有一个状态栏
            hr = StringCchCopy(reinterpret_cast<PWSTR>(pszName), cchMax, 
                pwszVerbHelpText);
            break;

        case GCS_VERBW:
            // GCS_VERBW 是个可选功能
            hr = StringCchCopy(reinterpret_cast<PWSTR>(pszName), cchMax, 
                pwszVerbCanonicalName);
            break;

        default:
            hr = S_OK;
        }
    }

    // 如果上下文菜单应用程序不支持命令 (idCommand)将返回E_INVALIDARG.

    return hr;
}

#pragma endregion