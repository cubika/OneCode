/****************************** 模块头 ******************************\
模块名:    PlatformDetector.cpp
项目名:        CppPlatformDetector
版权 (c) Microsoft Corporation.

执行帮助函数来检测平台. 

GetOSName - 获取当前操作系统的名字（例如："Microsoft Windows 7 企业版").

GetOSVersionString -  获得当前安装在操作系统中的平台标识符，版本和服务包的连接字符串.

Is64BitOS - 确定当前的操作系统是否是64位的系统.

Is64BitProcess -确定当前运行的进程或任意一个运行的进程是否是64位进程.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "PlatformDetector.h"
#include <strsafe.h>

#include <wbemidl.h>
#pragma comment(lib, "wbemuuid.lib")
#include <comdef.h>
#include <wincred.h>


//
//   函数: GetOSName(PWSTR, DWORD)
//
//   目的: 获取当前操作系统的名字（例如："Microsoft Windows 7 企业版").
//
//   参数:
//   * pszName - 存放操作系统名的缓冲区. 
//   * cch - 由pszName所指向的缓冲区大小，以字符形式.
//
//   返回值: 如果函数成功返回TRUE.
//
BOOL GetOSName(PWSTR pszName, DWORD cch)
{
    HRESULT hr = S_OK;
    IWbemLocator *pLoc = NULL;
    IWbemServices *pSvc = NULL;
    IEnumWbemClassObject *pEnumerator = NULL;
    PCWSTR pszMachineName = L".";

    // 通过调用 CoInitializeEx初始化COM参数.
    hr = CoInitializeEx(0, COINIT_MULTITHREADED);
    if (FAILED(hr))
    {
        return FALSE;
    }

    // 通过调用CoInitializeSecurity初始化COM进程安全. 
    hr = CoInitializeSecurity(
        NULL, 
        -1,                             // COM 认证
        NULL,                           // 认证服务
        NULL,                           // 保留
        RPC_C_AUTHN_LEVEL_DEFAULT,      // 默认认证
        RPC_C_IMP_LEVEL_IDENTIFY,       // 默认身份 
        NULL,                           // 认证信息
        EOAC_NONE,                      // 额外的能力
        NULL                            // 保留
        );
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // 通过调用CoCreateInstance获得WMI的初始定位器. 
    hr = CoCreateInstance(CLSID_WbemLocator, 0, CLSCTX_INPROC_SERVER, 
        IID_IWbemLocator, reinterpret_cast<LPVOID *>(&pLoc));
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // 通过IWbemLocator::ConnectServer连接到WMI.
    wchar_t szPath[200];
    hr = StringCchPrintf(szPath, ARRAYSIZE(szPath), L"\\\\%s\\root\\cimv2", 
        pszMachineName);
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    hr = pLoc->ConnectServer(
        bstr_t(szPath),                 // WMI名空间的路径
        NULL,                           // 用户名
        NULL,                           // 用户密码
        NULL,                           // 本地
        NULL,                           // 安全标志
        NULL,                           // 权限
        NULL,                           // 上下文对象
        &pSvc                           // IWbemServices 代理
        );
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // 在WMI连接中设置安全级别.
    hr = CoSetProxyBlanket(
        pSvc,                           // 指明代理来设置
        RPC_C_AUTHN_DEFAULT,            // RPC_C_AUTHN_xxx
        RPC_C_AUTHZ_DEFAULT,            // RPC_C_AUTHZ_xxx
        COLE_DEFAULT_PRINCIPAL,         // 主要服务器名字
        RPC_C_AUTHN_LEVEL_PKT_PRIVACY,  // RPC_C_AUTHN_LEVEL_xxx 
        RPC_C_IMP_LEVEL_IMPERSONATE,    // RPC_C_IMP_LEVEL_xxx
        NULL,                           // 客户身份
        EOAC_NONE                       // 代理功能 
        );
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // 使用IWbemServices指针提出WMI请求. 
    // 查询指明当前操作系统名的Win32_OperatingSystem.Caption.
    hr = pSvc->ExecQuery(bstr_t(L"WQL"), 
        bstr_t(L"SELECT Caption FROM Win32_OperatingSystem"),
        WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY, 
        NULL, &pEnumerator);
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // 确保枚举代理.
    hr = CoSetProxyBlanket(
        pEnumerator,                    // 指明代理来设置
        RPC_C_AUTHN_DEFAULT,            // RPC_C_AUTHN_xxx
        RPC_C_AUTHZ_DEFAULT,            // RPC_C_AUTHZ_xxx
        COLE_DEFAULT_PRINCIPAL,         // 主要服务器名字
        RPC_C_AUTHN_LEVEL_PKT_PRIVACY,  // RPC_C_AUTHN_LEVEL_xxx 
        RPC_C_IMP_LEVEL_IMPERSONATE,    // RPC_C_IMP_LEVEL_xxx
        NULL,                           // 客户身份
        EOAC_NONE                       // 代理功能 
        );
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // 通过以上的查询来获取数据.
    IWbemClassObject *pclsObj = NULL;
    ULONG uReturn = 0;

    while (pEnumerator)
    {
        // 获得一个对象.
        HRESULT hrTmp = pEnumerator->Next(WBEM_INFINITE, 1, &pclsObj, &uReturn);

        if (0 == uReturn)
        {
            break;
        }

        VARIANT vtProp;

        // 获取Win32_OperatingSystem.Caption属性的值.
        hrTmp = pclsObj->Get(L"Caption", 0, &vtProp, 0, 0);
        if (SUCCEEDED(hrTmp))
        {
            hr = StringCchCopy(pszName, cch, vtProp.bstrVal);

            VariantClear(&vtProp);
        }

        pclsObj->Release();
        pclsObj = NULL;
    }

Cleanup:
    // 集中清理所以的已分配资源.
    if (pLoc)
    {
        pLoc->Release();
        pLoc = NULL;
    }
    if (pSvc)
    {
        pSvc->Release();
        pSvc = NULL;
    }
    if (pEnumerator)
    {
        pEnumerator->Release();
        pEnumerator = NULL;
    }

    CoUninitialize();

    return SUCCEEDED(hr);
}


//
//   函数: GetOSVersionString(PWSTR, DWORD)
//
//   目的: 获得当前安装在操作系统中的平台标识符，版本和服务包的连接字符串.例如, 
//   "Microsoft Windows NT 6.1.7600.0 Workstation"
//
//   参数:
//   * pszVersionString - 用来存储操作系统版本字符串的缓冲区. 
//   * cch - pszVersionString所指向的缓冲区大小，以字符形式.
//
//   返回值: 函数成功返回TRUE.
//
BOOL GetOSVersionString(PWSTR pszVersionString, DWORD cch)
{
    // 调用GetVersionEx来检查当前操作系统的版本. 如果兼容模式有效,
    // GetVersionEx函数报告操作系统鉴别自己本身,这可能不是已安装的
    // 的操作系统.例如,如果兼容模式有效, GetVersionEx函数报告操作 
    // 系统被选定用于应用程序兼容.

    OSVERSIONINFOEX osvi = { sizeof(osvi) };
    if (!GetVersionEx(reinterpret_cast<OSVERSIONINFO *>(&osvi)))
    {
        return FALSE;
    }

    PCWSTR pszPlatform = NULL;
    PCWSTR pszProductType = L"";

    switch (osvi.dwPlatformId)
    {
    case VER_PLATFORM_WIN32s:
        // Microsoft Win32S
        pszPlatform = L"Microsoft Win32S";
        break;

    case VER_PLATFORM_WIN32_WINDOWS:
        if (osvi.dwMajorVersion > 4 || 
            (osvi.dwMajorVersion == 4 && osvi.dwMinorVersion > 0))
        {
            // Microsoft Windows 98
            pszPlatform = L"Microsoft Windows 98";
        }
        else
        {
            // Microsoft Windows 95
            pszPlatform = L"Microsoft Windows 95";
        }
        break;

    case VER_PLATFORM_WIN32_NT:
        // Microsoft Windows NT
        pszPlatform = L"Microsoft Windows NT";

        switch (osvi.wProductType)
        {
        case VER_NT_WORKSTATION:
            pszProductType = L"Workstation";
            break;
        case VER_NT_DOMAIN_CONTROLLER:
            pszProductType = L"DC";
            break;
        case VER_NT_SERVER:
            pszProductType = L"Server";
            break;
        }
        break;

    default:
        // 未知
        pszPlatform = L"<unknown>";
    }

    HRESULT hr = StringCchPrintf(pszVersionString, cch, 
        L"%s %d.%d.%d.%d %s", 
        pszPlatform, 
        osvi.dwMajorVersion, 
        osvi.dwMinorVersion, 
        osvi.dwBuildNumber, 
        osvi.wServicePackMajor << 0x10 | osvi.wServicePackMinor, 
        pszProductType);

    return (SUCCEEDED(hr));
}


typedef BOOL (WINAPI *LPFN_ISWOW64PROCESS) (HANDLE, PBOOL);

LPFN_ISWOW64PROCESS fnIsWow64Process = NULL;

//
//   函数: SafeIsWow64Process(HANDLE, PBOOL)
//
//   目的: 它是IsWow64Process API的包装.它用来确定特定的进程是否是运行在
//   Wow64下.在带SP2的Windows XP系统和带SP1的Window Server 2003之前并不存在
//   IsWow64Process.为了和不支持IsWow64Process的操作系统兼容，我们可以调用
//   GetProcAddress来检测 IsWow64Process是否在Kernel32.dll中执行.如果
//   GetProcAddress成功,那么可以成功地动态调用sWow64Process.否则,WOW64不出现.
//
//   参数:
//   * hProcess - 进程的句柄. 
//   * Wow64Process - 这是一个指向一个值的指针,如果进程运行在WOW64下它被设为TRUE. 
//     如果这个进程运行在32位窗口，它的值被设为FALSE.如果一个进程是一个运行在64位
//     窗口的应用程序，它的值也被设为FALSE.
//
//   返回值: 如果这个函数成功，返回值是TRUE.如果IsWow64Process不存在kernel32.dll中,
//   或者函数失败，返回FALSE.
//
BOOL WINAPI SafeIsWow64Process(HANDLE hProcess, PBOOL Wow64Process)
{
    if (fnIsWow64Process == NULL)
    {
        // 在windows支持的所有版本中IsWow64Process不一定有效. 
        // 使用GetModuleHandle获得包含这个函数的DLL的句柄,
        // 如果可用则用GetProcAddress来获取指向此函数的指针. 

        HMODULE hModule = GetModuleHandle(L"kernel32.dll");
        if (hModule == NULL)
        {
            return FALSE;
        }
        
        fnIsWow64Process = reinterpret_cast<LPFN_ISWOW64PROCESS>(
            GetProcAddress(hModule, "IsWow64Process"));
        if (fnIsWow64Process == NULL)
        {
            return FALSE;
        }
    }
    return fnIsWow64Process(hProcess, Wow64Process);
}


//
//   函数: Is64BitOS()
//
//   目的: 确定当前的操作系统是否是64位的系统.
//
//   返回值: 如果操作系统是64位的返回TRUE;否则，返回FALSE.
//
BOOL Is64BitOS()
{
#if defined(_WIN64)
    return TRUE;   // 64位的程序只运行在Win64
#elif defined(_WIN32)
    // 32位的程序可以运行在32位或64位窗口
    BOOL f64bitOS = FALSE;
    return (SafeIsWow64Process(GetCurrentProcess(), &f64bitOS) && f64bitOS);
#else
    return FALSE;  // 64位Windows不支持Win16
#endif
}


//
//   函数: Is64BitProcess(void)
//   
//   目的: 确定当前运行的进程是否是64位进程.
//
//   返回值: 如果当前运行的进程是64位返回TRUE;否则，返回FALSE.
//
BOOL Is64BitProcess(void)
{
#if defined(_WIN64)
    return TRUE;   // 64位程序
#else
    return FALSE;
#endif
}


//
//   函数: Is64BitProcess(HANDLE)
//   
//   目的: 确定任意一个运行的进程是否是64位进程.
//
//   参数:
//   * hProcess - 进程句柄.
//
//   返回值: 如果给定的进程是64位返回TRUE;
//   否则,返回FALSE.
//
BOOL Is64BitProcess(HANDLE hProcess)
{
    BOOL f64bitProc = FALSE;

    if (Is64BitOS())
    {
        //  在一个64位的操作系统中，如果一个进程不是运行在Wow64模式下，
        //  这个进程就一定是一个64位进程.
        f64bitProc = !(SafeIsWow64Process(hProcess, &f64bitProc) && f64bitProc);
    }

    return f64bitProc;
}