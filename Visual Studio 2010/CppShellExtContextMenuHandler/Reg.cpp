/****************************** 模块头 ******************************\
模块名:  Reg.cpp
项目:    CppShellExtContextMenuHandler
版权 (c) Microsoft Corporation.

该文件定义了在COM对象中重复使用的注册和反注册的方法

RegisterInprocServer -   注册COM组件到注册表中.
UnregisterInprocServer - 从注册表中反注册COM组件.
RegisterShellExtContextMenuHandler - 注册上下文菜单处理程序.
UnregisterShellExtContextMenuHandler - 反注册上下文菜单处理程序.

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
//   函数名: SetHKCRRegistryKeyAndValue
//
//   目的: 该函数创建 HKCR 注册表项并设置指定的注册表值
//
//   参数:
//   * pszSubKey - HKCR下的子项，如果该项不存在将被创建
//   * pszValueName - 指定要设置的注册表值. 如果为空则设置默认
//   * pszData - 指定注册表值的字符串的数据.
//
//   返回值:
//	 如果函数执行成功将返回S_OK,如果失败将返回HRESULT类型的值

// 
HRESULT SetHKCRRegistryKeyAndValue(PCWSTR pszSubKey, PCWSTR pszValueName, 
    PCWSTR pszData)
{
    HRESULT hr;
    HKEY hKey = NULL;

    //创建指定的注册表的键. 如果该键已经存在则打开它
    hr = HRESULT_FROM_WIN32(RegCreateKeyEx(HKEY_CLASSES_ROOT, pszSubKey, 0, 
        NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hKey, NULL));

    if (SUCCEEDED(hr))
    {
        if (pszData != NULL)
        {
            // 为指定的键设置值.
            DWORD cbData = lstrlen(pszData) * sizeof(*pszData);
            hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, pszValueName, 0, 
                REG_SZ, reinterpret_cast<const BYTE *>(pszData), cbData));
        }

        RegCloseKey(hKey);
    }

    return hr;
}


//
//   函数名: GetHKCRRegistryKeyAndValue
//
//   目的: 该函数打开 HKCR 注册表项和数据获取指定的注册表值的名称.
//
//   参数:
//   * pszSubKey -  注册表HKCR下的注册表键. 如果该键不存在则返回错误.
//   * pszValueName - 取得该键对应的值. 如果为空则设置默认值.
//   * pszData - 指向接收值的字符串数据的缓冲区的指.
//   * cbData - 指定缓冲区的大小 （以字节为单位）.
//
//   返回值:
//     如果函数成功，则返回 S_OK。否则，它将返回一个HRESULT错误代码。
//     如果不存在指定的注册表项的示例为存在或未设置为指定的值名数据，
//     该函数返回 COR_E_FILENOTFOUND (0x80070002)。
// 
HRESULT GetHKCRRegistryKeyAndValue(PCWSTR pszSubKey, PCWSTR pszValueName, 
    PWSTR pszData, DWORD cbData)
{
    HRESULT hr;
    HKEY hKey = NULL;

    // Try to open the specified registry key. 
    hr = HRESULT_FROM_WIN32(RegOpenKeyEx(HKEY_CLASSES_ROOT, pszSubKey, 0, 
        KEY_READ, &hKey));

    if (SUCCEEDED(hr))
    {
        // Get the data for the specified value name.
        hr = HRESULT_FROM_WIN32(RegQueryValueEx(hKey, pszValueName, NULL, 
            NULL, reinterpret_cast<LPBYTE>(pszData), &cbData));

        RegCloseKey(hKey);
    }

    return hr;
}

#pragma endregion


//
//   函数名: RegisterInprocServer
//
//   目的: 注册COM组件到注册表中.
//
//   参数:
//   * pszModule - 包含组件的模块的路径
//   * clsid - 组件对象的Class ID 
//   * pszFriendlyName - 别名
//   * pszThreadModel - 线程模型
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

    // 创建 HKCR\CLSID\{<CLSID>} 键 .
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"CLSID\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszFriendlyName);

        // 创建  the HKCR\CLSID\{<CLSID>}\InprocServer32 键.
        if (SUCCEEDED(hr))
        {
            hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
                L"CLSID\\%s\\InprocServer32", szCLSID);
            if (SUCCEEDED(hr))
            {
                // 给 InprocServer32键设置模块的路径为其默认值.
                hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszModule);
                if (SUCCEEDED(hr))
                {
                    // 设置线程模型.
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
//   目的: 从注册表中反注册COM组件.
//
//   参数:
//   * clsid - 组件对象的Class ID
//
//   注意: 该函数将在注册表HKCR\CLSID\{<CLSID>} 下删除键
//
HRESULT UnregisterInprocServer(const CLSID& clsid)
{
    HRESULT hr = S_OK;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    // 删除 HKCR\CLSID\{<CLSID>} 键.
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"CLSID\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
    }

    return hr;
}



//
//   函数: RegisterShellExtContextMenuHandler
//
//   目的: 注册上下文菜单处理程序.
//
//   参数:
//   * pszFileType - 上下文菜单处理程序关联的文件类型
//	   如, '*' 表示所有文件; '.txt' 所有文本文件. 该参数不能为空   
//   * clsid - 组件对象的Class ID
//   * pszFriendlyName - 别名
//
//   注意: 该函数将在注册表中增加下面的键值
//   NOTE: The function creates the following key in the registry.
//
//   HKCR
//   {
//      NoRemove <File Type>
//      {
//          NoRemove shellex
//          {
//              NoRemove ContextMenuHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//   }
//

HRESULT RegisterShellExtContextMenuHandler(
    PCWSTR pszFileType, const CLSID& clsid, PCWSTR pszFriendlyName)
{
    if (pszFileType == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    //如果文件的类型是以'.'开头的，然后在注册表的HKCR\<File Type>下获取该文件类型的对应的Program ID来关联该文件
    if (*pszFileType == L'.')
    {
        wchar_t szDefaultVal[260];
        hr = GetHKCRRegistryKeyAndValue(pszFileType, NULL, szDefaultVal, 
            sizeof(szDefaultVal));

        // 如果该键存在且不为空, 使用ProgID 为该文件类型
        if (SUCCEEDED(hr) && szDefaultVal[0] != L'\0')
        {
            pszFileType = szDefaultVal;
        }
    }

    // 创建键  HKCR\<File Type>\shellex\ContextMenuHandlers\{<CLSID>}
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
        L"%s\\shellex\\ContextMenuHandlers\\%s", pszFileType, szCLSID);
    if (SUCCEEDED(hr))
    {
        // 为该键设置默认值.
        hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszFriendlyName);
    }

    return hr;
}


//
//   函数: UnregisterShellExtContextMenuHandler
//
//   目的: 反注册上下文菜单处理程序.
//
//   参数:
//   * pszFileType - 上下文菜单处理程序关联的文件类型
//	   如, '*' 表示所有文件; '.txt' 所有文本文件. 该参数不能为空   
//   * clsid - 组件对象的Class ID
//
//   注意: 该函数将删除注册表中HKCR\<File Type>\shellex\ContextMenuHandlers下的 {<CLSID>}键

//
HRESULT UnregisterShellExtContextMenuHandler(
    PCWSTR pszFileType, const CLSID& clsid)
{
    if (pszFileType == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    //如果文件的类型是以'.'开头的，然后在注册表的HKCR\<File Type>下获取该文件类型的对应的Program ID来关联该文件
    if (*pszFileType == L'.')
    {
        wchar_t szDefaultVal[260];
        hr = GetHKCRRegistryKeyAndValue(pszFileType, NULL, szDefaultVal, 
            sizeof(szDefaultVal));

         // 如果该键存在且不为空, 使用ProgID 为该文件类型
        if (SUCCEEDED(hr) && szDefaultVal[0] != L'\0')
        {
            pszFileType = szDefaultVal;
        }
    }

    // 删除HKCR\<File Type>\shellex\ContextMenuHandlers\{<CLSID>}键.
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
        L"%s\\shellex\\ContextMenuHandlers\\%s", pszFileType, szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
    }

    return hr;
}