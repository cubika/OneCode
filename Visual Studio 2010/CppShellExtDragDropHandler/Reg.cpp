/****************************** 模块头 ******************************\
模块名:  Reg.cpp
项目名:  CppShellExtPropSheetHandler
版权(c) Microsoft Corporation.

这个文件实现了在进程内COM组件和Shell鼠标拖放处理程序的注册和注销的重复使用的函数。


RegisterInprocServer - 在注册表中注册进程内的组件.
UnregisterInprocServer - 在注册表中注销进程内的组件.
RegisterShellExtDragDropHandler - 注册鼠标拖放处理程序.
UnregisterShellExtDragDropHandler - 注销鼠标拖放处理程序.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "Reg.h"
#include <strsafe.h>


#pragma region Registry Helper Functions

//
//  函数: SetHKCRRegistryKeyAndValue
//
//  目标:该函数创建一个注册表项HKCR和设置指定注册表值。
//
//
//  参数:
//   * pszSubKey -在HKCR下指定注册表项。如果这个项不存在，这个函数将创建这个注册表项。
//   * pszValueName -指定的注册表值被设置。如果pszValueName
//    为NULL，该函数将设置默认值。

//   * pszData -指定注册表值的字符串数据
//
//   返回值：
//如果函数成功，返回S_OK。否则，它将返回一个
// HRESULT错误代码。

// 
HRESULT SetHKCRRegistryKeyAndValue(PCWSTR pszSubKey, PCWSTR pszValueName, 
    PCWSTR pszData)
{
    HRESULT hr;
    HKEY hKey = NULL;

    //创建一个指定的注册表值. 如果这个值已经存在，这个函数就将打开它。 
    //
    hr = HRESULT_FROM_WIN32(RegCreateKeyEx(HKEY_CLASSES_ROOT, pszSubKey, 0, 
        NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hKey, NULL));

    if (SUCCEEDED(hr))
    {
        if (pszData != NULL)
        {
            //设置指定的键值
            DWORD cbData = lstrlen(pszData) * sizeof(*pszData);
            hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, pszValueName, 0, 
                REG_SZ, reinterpret_cast<const BYTE *>(pszData), cbData));
        }

        RegCloseKey(hKey);
    }

    return hr;
}


//
//   函数: GetHKCRRegistryKeyAndValue
//
//   目标: 该函数打开一个注册表项HKCR并得到了
//         指定的注册表值的名称数据。
//
//   PARAMETERS:
//   * pszSubKey - 在HKCR下指定注册表值，如果这个值不存在，这个函数将返回一个错误。
//   * pszValueName -指定被检索的注册表值.如果
//     pszValueName 是空的, 这个函数将获得默认值.
//   * pszData - 缓存区的指针接收字符串数据的值。
//   * cbData - 指定缓冲区的大小。
//
//   返回值:
//     如果函数成功，返回S_OK。否则，它将返回一个
//     HRESULT错误代码。例如，如果指定的注册表项不
//     存在或指定值的名称的数据还没有设置，函数
//     返回COR_E_FILENOTFOUND（0x80070002）。

// 
HRESULT GetHKCRRegistryKeyAndValue(PCWSTR pszSubKey, PCWSTR pszValueName, 
    PWSTR pszData, DWORD cbData)
{
    HRESULT hr;
    HKEY hKey = NULL;

    // 尝试打开指定的注册表项。
    hr = HRESULT_FROM_WIN32(RegOpenKeyEx(HKEY_CLASSES_ROOT, pszSubKey, 0, 
        KEY_READ, &hKey));

    if (SUCCEEDED(hr))
    {
        // 获取指定的值的名称的数据。

        hr = HRESULT_FROM_WIN32(RegQueryValueEx(hKey, pszValueName, NULL, 
            NULL, reinterpret_cast<LPBYTE>(pszData), &cbData));

        RegCloseKey(hKey);
    }

    return hr;
}

#pragma endregion


//
//   函数: RegisterInprocServer
//
//   目标:在注册表中注册进程内组件.
//
//   参数:
//   * pszModule - 路径的模块包含的组件
//   * clsid -组件的类ID
//   * pszFriendlyName -友好的名称
//   * pszThreadModel -线程块
//
//   NOTE: The function creates the HKCR\CLSID\{<CLSID>} key in the registry.
// 
//   HKCR
//   {
//      NoRemove CLSID
//      {
//          ForceRemove {<CLSID>} = s '<Friendly Name>'
//          {
//              InprocServer32 = s '%MODULE%'
//              {
//                  val ThreadingModel = s '<Thread Model>'
//              }
//          }
//      }
//   }
//
HRESULT RegisterInprocServer(PCWSTR pszModule, const CLSID& clsid, 
    PCWSTR pszFriendlyName, PCWSTR pszThreadModel)
{
    if (pszModule == NULL || pszThreadModel == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    //创建HKCR\CLSID\{<CLSID>} 的键值.
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"CLSID\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszFriendlyName);

        // 创建 HKCR\CLSID\{<CLSID>}\InprocServer32 键值.
        if (SUCCEEDED(hr))
        {
            hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
                L"CLSID\\%s\\InprocServer32", szCLSID);
            if (SUCCEEDED(hr))
            {
                // 设置COM模块的路径的InprocServer32的默认值。
                hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszModule);
                if (SUCCEEDED(hr))
                {
                    // 设置组件的线程块.
                    hr = SetHKCRRegistryKeyAndValue(szSubkey, 
                        L"ThreadingModel", pszThreadModel);
                }
            }
        }
    }

    return hr;
}


//
//   函数: UnregisterInprocServer
//
//   目标:在注册表中注销进程内的组件.
//
//   参数:
//   * clsid - 组件的类ID。
//
//  注意: 这个函数在注册表中删除这个HKCR\CLSID\{<CLSID>}键值。
//
HRESULT UnregisterInprocServer(const CLSID& clsid)
{
    HRESULT hr = S_OK;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    // 删除 HKCR\CLSID\{<CLSID>} 键值.
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"CLSID\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
    }

    return hr;
}


//
//   函数: RegisterShellExtDragDropHandler
//
//   目标: 注册鼠标拖放处理程序.
//
//   参数:
//   * clsid - 组件的类ID
//   * pszFriendlyName - 友好的名称
//
//   注意: 这个函数在注册表中创建了下面的键值.
//
//   HKCR
//   {
//      NoRemove Directory
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//      NoRemove Folder
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//      NoRemove Drive
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//   }
//
HRESULT RegisterShellExtDragDropHandler(const CLSID& clsid, PCWSTR pszFriendlyName)
{
    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    // 鼠标拖放处理程序通常按下列注册 
    //子项: HKCR\Directory\shellex\DragDropHandlers.
    // 创建这个键值 HKCR\Directory\shellex\DragDropHandlers\{<CLSID>}
    HRESULT hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
        L"Directory\\shellex\\DragDropHandlers\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        // 设置默认的键值.
        hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszFriendlyName);
    }

    //在一些情况下，你必须在HKCR\Folder下注册，为了处理桌面，和HKCR\Drive的跟驱动器 
    if (SUCCEEDED(hr))
    {
        hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
            L"Folder\\shellex\\DragDropHandlers\\%s", szCLSID);
        if (SUCCEEDED(hr))
        {
            // 设置默认的键值
            hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszFriendlyName);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
            L"Drive\\shellex\\DragDropHandlers\\%s", szCLSID);
        if (SUCCEEDED(hr))
        {
            // 设置默认的键值.
            hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszFriendlyName);
        }
    }

    return hr;
}


//
//   函数: UnregisterShellExtDragDropHandler
//
//  目标: Unregister the drag-and-drop handler.
//
//  参数:
//   * clsid - 组件的类ID
//
//   注意: 这个函数移除了在注册表中的HKCR\Directory\shellex\DragDropHandlers, 
//   HKCR\Folder\shellex\DragDropHandlers 
//   HKCR\Drive\shellex\DragDropHandlers 下的 {<CLSID>}键值。
//
HRESULT UnregisterShellExtDragDropHandler(const CLSID& clsid)
{
    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    // 移除这个 HKCR\Directory\shellex\DragDropHandlers\{<CLSID>} 键值.
    HRESULT hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
        L"Directory\\shellex\\DragDropHandlers\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
    }

    if (SUCCEEDED(hr))
    {
        // 移除 HKCR\Folder\shellex\DragDropHandlers\{<CLSID>} 键值.
        hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
            L"Folder\\shellex\\DragDropHandlers\\%s", szCLSID);
        if (SUCCEEDED(hr))
        {
            hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
        }
    }

    if (SUCCEEDED(hr))
    {
        //移除 HKCR\Drive\shellex\DragDropHandlers\{<CLSID>} 键值.
        hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
            L"Drive\\shellex\\DragDropHandlers\\%s", szCLSID);
        if (SUCCEEDED(hr))
        {
            hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
        }
    }

    return hr;
}