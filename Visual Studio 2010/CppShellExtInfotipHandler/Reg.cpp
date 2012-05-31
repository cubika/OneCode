/****************************** 模块头 ******************************\
模块名称:  Reg.cpp
项目名称:      CppShellExtInfotipHandler
版权 (c) Microsoft Corporation.

该模块声明了以下几个可重用使用的辅助函数：
RegisterInprocServer - 创建注册表信息.
UnregisterInprocServer - 删除注册表信息.
RegisterShellExtInfotipHandler - 创建信息提示处理功能.
UnregisterShellExtInfotipHandler -  删除信息提示功能.

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
//   函数名称: SetHKCRRegistryKeyAndValue
//
//   作用: 该函数创建了一个 HKCR 注册表并且设置了注册码 。
//   
//
//   参数:
//   * pszSubKey - 指定注册码.如果注册码不存在，该函数就会创建指定的注册码 
//   
//   * pszValueName - 指定注册表的值，如果pszValueName 为空，该函数就付给它默认值.

//   * pszData - 指定一个字符串作为注册表的值.
//
//   返回值: 
//   如果函数成功执行，则返回S_OK。其他，则返回 HRESULT 错误代码.
//   
// 
HRESULT SetHKCRRegistryKeyAndValue(PCWSTR pszSubKey, PCWSTR pszValueName, 
    PCWSTR pszData)
{
    HRESULT hr;
    HKEY hKey = NULL;

    //创建指定的注册表的注册码.如果注册码存在，函数将打开这个注册表. 
    hr = HRESULT_FROM_WIN32(RegCreateKeyEx(HKEY_CLASSES_ROOT, pszSubKey, 0, 
        NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hKey, NULL));

    if (SUCCEEDED(hr))
    {
        if (pszData != NULL)
        {
            // 设置指定的注册码.
            DWORD cbData = lstrlen(pszData) * sizeof(*pszData);
            hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, pszValueName, 0, 
                REG_SZ, reinterpret_cast<const BYTE *>(pszData), cbData));
        }

        RegCloseKey(hKey);
    }

    return hr;
}


//
//   函数名称: GetHKCRRegistryKeyAndValue
//
//   作用:该函数打开一个HKCR注册表项和获取指定的注册表值名称的数据。
//
//   参数:
//   * pszSubKey -指定HKCR下的注册表项. 如果注册表项不存在，则返回一个错误.
//   * pszValueName - 指定要检索的注册表值。如果pszValueName是空的，则赋给它默认值.
//   * pszData - 是一个指针，指向接收注册表值的字符串数据的缓冲区.
//   * cbData -指定缓冲区的大小.
//
//   返回值:
//   如果函数成功执行，返回S_OK。 其他，则返回一个HRSULT错误。 
//   例如,如果指定注册表项不存在或者指定项值名字没设置，函数将返回 COR_E_FILENOTFOUND (0x80070002). 
// 
HRESULT GetHKCRRegistryKeyAndValue(PCWSTR pszSubKey, PCWSTR pszValueName, 
    PWSTR pszData, DWORD cbData)
{
    HRESULT hr;
    HKEY hKey = NULL;

    //打开指定的注册表项 . 
    hr = HRESULT_FROM_WIN32(RegOpenKeyEx(HKEY_CLASSES_ROOT, pszSubKey, 0, 
        KEY_READ, &hKey));

    if (SUCCEEDED(hr))
    {
        // 获取指定的注册表值名称的数据.
        hr = HRESULT_FROM_WIN32(RegQueryValueEx(hKey, pszValueName, NULL, 
            NULL, reinterpret_cast<LPBYTE>(pszData), &cbData));

        RegCloseKey(hKey);
    }

    return hr;
}

#pragma endregion


//
//   函数名称: RegisterInprocServer
//
//   作用: 在注册表中注册进程内组件
//
//   参数:
//   * pszModule -包含该组件模块的路径
//   * clsid -组件的类ID
//   * pszFriendlyName -友元名称
//   * pszThreadModel -线程模型
//
//  
//  注意: 该方法在注册表中创建了HKCR\CLSID\{<CLSID>}注册项.
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

    // 创建 HKCR\CLSID\{<CLSID>}注册项.
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"CLSID\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszFriendlyName);

        //创建 HKCR\CLSID\{<CLSID>}\InprocServer32注册项.
        if (SUCCEEDED(hr))
        {
            hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
                L"CLSID\\%s\\InprocServer32", szCLSID);
            if (SUCCEEDED(hr))
            {
                //以InprocServer32注册项的值作为为COM模块的路径的默认值  
                
                hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, pszModule);
                if (SUCCEEDED(hr))
                {
                    //为组件设置线程模型.
                    hr = SetHKCRRegistryKeyAndValue(szSubkey, 
                        L"ThreadingModel", pszThreadModel);
                }
            }
        }
    }

    return hr;
}


//
//   函数名称: UnregisterInprocServer
//
//   作用:  在注册表中删除进程内组件。
//
//   参数:
//   * clsid - 组件的类ＩＤ
//
//   注意:该函数在注册表中删除了 HKCR\CLSID\{<CLSID>}注册项.
//
HRESULT UnregisterInprocServer(const CLSID& clsid)
{
    HRESULT hr = S_OK;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    //删除 HKCR\CLSID\{<CLSID>}注册项.
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), L"CLSID\\%s", szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
    }

    return hr;
}


//
//   函数名称: RegisterShellExtInfotipHandler
//
//   作用: 注册ｓｈｅｌｌ　信息提示处理方法.
//
//   参数:
//   * pszFileType - 要处理文件的类型，如“*”代表所有的文件类型， 
//                   “.txt”代表.txt文件类型。参数不能为空
//   * clsid -　组件的类ID。
//
//   注意: 该方法在注册表中创建了以下注册信息。
//
//   HKCR
//   {
//      NoRemove <File Type>
//      {
//          NoRemove shellex
//          {
//              {00021500-0000-0000-C000-000000000046} = s '{<CLSID>}'
//          }
//      }
//   }
//
HRESULT RegisterShellExtInfotipHandler(PCWSTR pszFileType, const CLSID& clsid)
{
    if (pszFileType == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr;

    wchar_t szCLSID[MAX_PATH];
    StringFromGUID2(clsid, szCLSID, ARRAYSIZE(szCLSID));

    wchar_t szSubkey[MAX_PATH];

    // 如果 pszFileType以‘.’开始,就试着去找包含ProgID的HKCR\<FIle Type>的默认值
    if (*pszFileType == L'.')
    {
        wchar_t szDefaultVal[260];
        hr = GetHKCRRegistryKeyAndValue(pszFileType, NULL, szDefaultVal, 
            sizeof(szDefaultVal));

        // 如果key存在并且默认值不空，就用ProgID作为文件的类型
        if (SUCCEEDED(hr) && szDefaultVal[0] != L'\0')
        {
            pszFileType = szDefaultVal;
        }
    }

    //创建注册项 
    // HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
        L"%s\\shellex\\{00021500-0000-0000-C000-000000000046}", pszFileType);
    if (SUCCEEDED(hr))
    {
        //设置注册项的默认值.
        hr = SetHKCRRegistryKeyAndValue(szSubkey, NULL, szCLSID);
    }

    return hr;
}


//
//   函数名称: UnregisterShellExtInfotipHandler
//
//   作用:删除ｓｈｅｌｌ　信息提示处理方法。
//
//   参数:
//   * pszFileType - 要处理文件的类型，如“*”代表所有的文件类型， 
//                   “.txt”代表.txt文件类型。参数不能为空。
//   
//
//   注意:该函数删除了注册项：
//   HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}.
//
HRESULT UnregisterShellExtInfotipHandler(PCWSTR pszFileType)
{
    if (pszFileType == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr;

    wchar_t szSubkey[MAX_PATH];

    // 如果 pszFileType以‘.’开始,就试着去找包含ProgID的HKCR\<FIle Type>的默认值
    if (*pszFileType == L'.')
    {
        wchar_t szDefaultVal[260];
        hr = GetHKCRRegistryKeyAndValue(pszFileType, NULL, szDefaultVal, 
            sizeof(szDefaultVal));

        // 如果key存在并且默认值不空，就用ProgID作为文件的类型
        if (SUCCEEDED(hr) && szDefaultVal[0] != L'\0')
        {
            pszFileType = szDefaultVal;
        }
    }

    // 删除注册项：
    // HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}
    hr = StringCchPrintf(szSubkey, ARRAYSIZE(szSubkey), 
        L"%s\\shellex\\{00021500-0000-0000-C000-000000000046}", pszFileType);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(HKEY_CLASSES_ROOT, szSubkey));
    }

    return hr;
}