/********************************** 模块头 *********************************\
* 模块名:      CppUACSelfElevation.cpp
* 项目名:      CppUACSelfElevation
* 版权 (c) Microsoft Corporation.
* 
* 用户账户控制 （UAC）是Windows Vista及后续操作系统中的一个新安全组件。当UAC被
* 完全开启时，管理员的交互操作通常运行在普通用户权限下。这个示例演示了如何去检
* 测当前进程的权限等级，和如何通过许可验证对话框来确认并自我提升此线程的权限等
* 级。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes and Manifest Dependencies
#include <stdio.h>
#include <windows.h>
#include <windowsx.h>
#include <shlobj.h>
#include "Resource.h"

// 开始视觉风格
#if defined _M_IX86
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_IA64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='ia64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_X64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='amd64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#else
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif
#pragma endregion


#pragma region "Helper Functions for Admin Privileges and Elevation Status"

//
//   函数：IsUserInAdminGroup()
// 
//   用途：即使在还没有为当前用户提升权限的前提下，此函数检测拥有此进程的主访
//   问令牌的用户是否是本地管理员组的成员。
//
//   返回值：如果拥有主访问令牌的用户是管理员组成员则返回TRUE，反之则返回FALSE。
//
//   异常：如果此函数出错，它抛出一个包含Win32相关错误代码的C++ DWORD异常。
//
//
//   调用示例：
//     try 
//     {
//         if (IsUserInAdminGroup())
//             wprintf (L"用户是管理员组成员\n");
//         else
//             wprintf (L"用户不是管理员组成员\n");
//     }
//     catch (DWORD dwError)
//     {
//         wprintf(L"IsUserInAdminGroup 调用失败 w/err %lu\n", dwError);
//     }
//
BOOL IsUserInAdminGroup()
{
    BOOL fInAdminGroup = FALSE;
    DWORD dwError = ERROR_SUCCESS;
    HANDLE hToken = NULL;
    HANDLE hTokenToCheck = NULL;
    DWORD cbSize = 0;
    OSVERSIONINFO osver = { sizeof(osver) };

    // 打开此进程的主访问令牌（使用TOKEN_QUERY | TOKEN_DUPLICATE）
    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY | TOKEN_DUPLICATE, 
        &hToken))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 检测是否此系统是Windows Vista或后续版本（主版本号 >= 6）。这是由于自
    // Windows Vista开始支持链接令牌（LINKED TOKEN），而之前的版本不支持。
    // （主版本 < 6） 
    if (!GetVersionEx(&osver))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    if (osver.dwMajorVersion >= 6)
    {
        // 运行于Windows Vista 或后续版本（主版本 >= 6）.
        // 检测令牌类型：受限（limited）,（已提升）elevated, 
        // 或者默认（default）

        TOKEN_ELEVATION_TYPE elevType;
        if (!GetTokenInformation(hToken, TokenElevationType, &elevType, 
            sizeof(elevType), &cbSize))
        {
            dwError = GetLastError();
            goto Cleanup;
        }

        // 如果为受限的，获取相关联的已提升令牌以便今后使用。
        if (TokenElevationTypeLimited == elevType)
        {
            if (!GetTokenInformation(hToken, TokenLinkedToken, &hTokenToCheck, 
                sizeof(hTokenToCheck), &cbSize))
            {
                dwError = GetLastError();
                goto Cleanup;
            }
        }
    }
    
    // CheckTokenMembership要求一个伪装令牌。如果我们仅得到一个链接令牌，那
    // 它就是一个伪装令牌。如果我们没有得到一个关联式令牌，我们需要复制当前
    // 令牌以便得到一个伪装令牌。
    if (!hTokenToCheck)
    {
        if (!DuplicateToken(hToken, SecurityIdentification, &hTokenToCheck))
        {
            dwError = GetLastError();
            goto Cleanup;
        }
    }

    // 创建管理员组相关的SID
    BYTE adminSID[SECURITY_MAX_SID_SIZE];
    cbSize = sizeof(adminSID);
    if (!CreateWellKnownSid(WinBuiltinAdministratorsSid, NULL, &adminSID,  
        &cbSize))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 检测是否被检测的令牌包含管理员组SID
    // http://msdn.microsoft.com/en-us/library/aa379596(VS.85).aspx:
    // 调用CheckTokenMembership来确定是否一个SID在一个令牌中启用，换而言之，
    // 是否它拥有SE_GROUP_ENABLED属性
    if (!CheckTokenMembership(hTokenToCheck, &adminSID, &fInAdminGroup)) 
    {
        dwError = GetLastError();
        goto Cleanup;
    }

Cleanup:
    // 集中清理所有已分配的内存资源
    if (hToken)
    {
        CloseHandle(hToken);
        hToken = NULL;
    }
    if (hTokenToCheck)
    {
        CloseHandle(hTokenToCheck);
        hTokenToCheck = NULL;
    }

    // 一旦有任何异常发生，抛出错误。
    if (ERROR_SUCCESS != dwError)
    {
        throw dwError;
    }

    return fInAdminGroup;
}


// 
//   函数：IsRunAsAdmin()
//
//   用途：此函数检测当前进程是否以管理员身份运行。换而言之，此进程要求用户是
//   拥有主访问令牌的用户是管理员组成员并且已经执行了权限提升。
//
//   返回值：如果拥有主访问令牌的用户是管理员组成员且已经执行了权限提升则返回
//   TRUE，反之则返回FALSE。
//
//   异常：如果此函数出错，它抛出一个包含Win32相关错误代码的C++ DWORD异常。
//
//   调用示例：
//     try 
//     {
//         if (IsRunAsAdmin())
//             wprintf (L"进程以管理员身份运行\n");
//         else
//             wprintf (L"进程没有以管理员身份运行\n");
//     }
//     catch (DWORD dwError)
//     {
//         wprintf(L"IsRunAsAdmin 失败 w/err %lu\n", dwError);
//     }
//
BOOL IsRunAsAdmin()
{
    BOOL fIsRunAsAdmin = FALSE;
    DWORD dwError = ERROR_SUCCESS;
    PSID pAdministratorsGroup = NULL;

    // 内存分配及初始化一个管理员组的SID
    SID_IDENTIFIER_AUTHORITY NtAuthority = SECURITY_NT_AUTHORITY;
    if (!AllocateAndInitializeSid(
        &NtAuthority, 
        2, 
        SECURITY_BUILTIN_DOMAIN_RID, 
        DOMAIN_ALIAS_RID_ADMINS, 
        0, 0, 0, 0, 0, 0, 
        &pAdministratorsGroup))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 检测是否管理员组的SID在线程的主访问令牌中有效。
    if (!CheckTokenMembership(NULL, pAdministratorsGroup, &fIsRunAsAdmin))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

Cleanup:
     // 集中清理所有已分配的内存资源
    if (pAdministratorsGroup)
    {
        FreeSid(pAdministratorsGroup);
        pAdministratorsGroup = NULL;
    }

    // 一旦有任何异常发生，抛出错误。
    if (ERROR_SUCCESS != dwError)
    {
        throw dwError;
    }

    return fIsRunAsAdmin;
}


//
//   函数：IsProcessElevated
//
//   用途：此函数获取当前进程的权限提升信息。它由此进程是否进行了权限提升所
//   决定。令牌权限提升只有在Windows Vista及后续版本的Windows中有效。所以在
//   Windows Vista之前的版本中执行IsProcessElevated， 它会抛出一个C++异常。
//   此函数并不适用于检测是否此进程以管理员身份运行。
//
//   返回值：如果此进程的权限已被提升，返回TRUE，反之则返回FALSE。
//
//
//   异常：如果此函数出错，它抛出一个包含Win32相关错误代码的C++ DWORD异常。
//   比如在Windows Vista之前的Windows中调用IsProcessElevated，被抛出的错误
//   代码为ERROR_INVALID_PARAMETER。
//
//   注意：TOKEN_INFORMATION_CLASS提供了TokenElevationType以便对当前进程的提升
//   类型（TokenElevationTypeDefault / TokenElevationTypeLimited /
//   TokenElevationTypeFull）进行检测。 它和TokenElevation的不同之处在于：当UAC
//   关闭时，即使当前进程已经被提升(完整性级别 == 高)，权限提升类型总是返回
//   TokenElevationTypeDefault。换而言之，以此来确认当前线程的提升类型是不安全的。
//   相对的，我们应该使用TokenElevation。
//
//   调用示例：
//     try 
//     {
//         if (IsProcessElevated())
//             wprintf (L"进程已经提升\n");
//         else
//             wprintf (L"进程尚未提升\n");
//     }
//     catch (DWORD dwError)
//     {
//         wprintf(L"IsProcessElevated 失败 w/err %lu\n", dwError);
//     }
//
BOOL IsProcessElevated()
{
    BOOL fIsElevated = FALSE;
    DWORD dwError = ERROR_SUCCESS;
    HANDLE hToken = NULL;

    // 使用TOKEN_QUERY打开进程主访问令牌
    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &hToken))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 获取令牌提升信息
    TOKEN_ELEVATION elevation;
    DWORD dwSize;
    if (!GetTokenInformation(hToken, TokenElevation, &elevation, 
        sizeof(elevation), &dwSize))
    {
        // 当进程运行于Windows Vista之前的系统中，GetTokenInformation返回
        // FALSE和错误码ERROR_INVALID_PARAMETER。这是由于这些操作系统不支
        // 持TokenElevation。
        dwError = GetLastError();
        goto Cleanup;
    }

    fIsElevated = elevation.TokenIsElevated;

Cleanup:
    // 集中清理所有已分配的内存资源
    if (hToken)
    {
        CloseHandle(hToken);
        hToken = NULL;
    }

    // 一旦有任何异常发生，抛出错误。
    if (ERROR_SUCCESS != dwError)
    {
        throw dwError;
    }

    return fIsElevated;
}


//
//   函数： GetProcessIntegrityLevel()
//
//   用途：此函数获取当前线程的完整性级别。完整性级别只有在Windows Vista及后
//   续版本的Windows中有效。所以在Windows Vista之前的版本中执行GetProcessIntegrityLevel， 
//   它会抛出一个C++异常。
//
//   返回值：返回当前进程的完整性级别。它可能是以下某一个值。
//
//     SECURITY_MANDATORY_UNTRUSTED_RID (SID: S-1-16-0x0)
//     表示不被信任的级别。它被用于一个匿名组成员起动的进程。这时大多数
//     写入操作被禁止。
//
//     SECURITY_MANDATORY_LOW_RID (SID: S-1-16-0x1000)
//     表示低完整性级别。它被用于保护模式下的IE浏览器。这时大多数系统中对
//     象（包括文件及注册表键值）的写入操作被禁止。
//
//     SECURITY_MANDATORY_MEDIUM_RID (SID: S-1-16-0x2000)
//     表示中完整性级别。它被用于在UAC开启时启动普通应用程序。
//
//
//     SECURITY_MANDATORY_HIGH_RID (SID: S-1-16-0x3000)
//     表示高完整性级别。它被用于用户通过UAC提升权限启动一个管理员应用程序。
//     或则当UAC关闭时，管理员用户启动一个普通程序。
//
//
//     SECURITY_MANDATORY_SYSTEM_RID (SID: S-1-16-0x4000)
//     表示系统完整性级别。它被用于服务或则其他系统级别的应用程序（比如
//     Wininit, Winlogon, Smss，等等）
//
//
//   异常：如果此函数出错，它抛出一个包含Win32相关错误代码的C++ DWORD异常。
//   比如在Windows Vista之前的Windows中调用GetProcessIntegrityLevel，被抛
//   出的错误代码为ERROR_INVALID_PARAMETER。
// 
//   调用示例：
//     try 
//     {
//         DWORD dwIntegrityLevel = GetProcessIntegrityLevel();
//     }
//     catch (DWORD dwError)
//     {
//         wprintf(L"GetProcessIntegrityLevel 失败 w/err %lu\n", dwError);
//     }
//
DWORD GetProcessIntegrityLevel()
{
    DWORD dwIntegrityLevel = 0;
    DWORD dwError = ERROR_SUCCESS;
    HANDLE hToken = NULL;
    DWORD cbTokenIL = 0;
    PTOKEN_MANDATORY_LABEL pTokenIL = NULL;

    // 使用TOKEN_QUERY打开线程的主访问令牌。
    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &hToken))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 查询令牌完整性级别信息的大小。注意：我们预期得到一个FALSE结果及错误
    // ERROR_INSUFFICIENT_BUFFER， 这是由于我们在GetTokenInformation输入一个
    // 空缓冲。同时，在cbTokenIL中我们会得知完整性级别信息的大小。
    if (!GetTokenInformation(hToken, TokenIntegrityLevel, NULL, 0, &cbTokenIL))
    {
        if (ERROR_INSUFFICIENT_BUFFER != GetLastError())
        {
            // 当进程运行于Windows Vista之前的系统中，GetTokenInformation返回
            // FALSE和错误码ERROR_INVALID_PARAMETER。这是由于这些操作系统不支
            // 持TokenElevation。
            dwError = GetLastError();
            goto Cleanup;
        }
    }

    // 现在我们为完整性级别信息分配一个缓存。
    pTokenIL = (TOKEN_MANDATORY_LABEL *)LocalAlloc(LPTR, cbTokenIL);
    if (pTokenIL == NULL)
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 获得令牌完整性级别信息。
    if (!GetTokenInformation(hToken, TokenIntegrityLevel, pTokenIL, 
        cbTokenIL, &cbTokenIL))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 完整性级别SID为S-1-16-0xXXXX形式。（例如：S-1-16-0x1000表示为低完整性
    // 级别的SID）。而且有且仅有一个次级授权信息。
    dwIntegrityLevel = *GetSidSubAuthority(pTokenIL->Label.Sid, 0);

Cleanup:
    // 集中清理所有已分配的内存资源

    if (hToken)
    {
        CloseHandle(hToken);
        hToken = NULL;
    }
    if (pTokenIL)
    {
        LocalFree(pTokenIL);
        pTokenIL = NULL;
        cbTokenIL = 0;
    }

    // 一旦有任何异常发生，抛出错误。
    if (ERROR_SUCCESS != dwError)
    {
        throw dwError;
    }

    return dwIntegrityLevel;
}

#pragma endregion


//
//   函数： ReportError(LPWSTR, DWORD)
//
//   用途：在一个函数出错后用一个对话框显示错误信息
//
//   参数：
//   * pszFunction - 调用失败的函数的名称。
//   * dwError - Win32错误代码。它的默认值为进程的GetLastError的返回值。
//
//   注意：如果没有明确指定dwError的值，我们必须在函数失败后立刻调用ReportError
//   这能确保正在调用的进程的最后错误代码没有被其他API改写。
void ReportError(LPCWSTR pszFunction, DWORD dwError = GetLastError())
{
    wchar_t szMessage[200];
    if (-1 != swprintf_s(szMessage, ARRAYSIZE(szMessage), 
        L"%s failed w/err 0x%08lx", pszFunction, dwError))
    {
        MessageBox(NULL, szMessage, L"Error", MB_ICONERROR);
    }
}


// 
//   函数： OnInitDialog(HWND, HWND, LPARAM) 
//   
//   用途：处理WM_INITDIALOG消息。检测及显示“以管理员账户身份运行”状态，
//   权限提升信息和当前进程的完整性级别。

BOOL OnInitDialog(HWND hWnd, HWND hwndFocus, LPARAM lParam)
{
    // 获取并显示即使在还没有为当前用户提升权限的前提下，拥有此进程的主访
    // 问令牌的用户是否是本地管理员组的成员。(IsUserInAdminGroup)。
    HWND hInAdminGroupLabel = GetDlgItem(hWnd, IDC_INADMINGROUP_STATIC);
    try
    {
        BOOL const fInAdminGroup = IsUserInAdminGroup();
        SetWindowText(hInAdminGroupLabel, fInAdminGroup ? L"是" : L"否");
    }
    catch (DWORD dwError)
    {
        SetWindowText(hInAdminGroupLabel, L"N/A");
        ReportError(L"IsUserInAdminGroup", dwError);
    }

     // 获取并显示是否此进程以管理员身份运行。（IsRunAsAdmin）。
    HWND hIsRunAsAdminLabel = GetDlgItem(hWnd, IDC_ISRUNASADMIN_STATIC);
    try
    {
        BOOL const fIsRunAsAdmin = IsRunAsAdmin();
        SetWindowText(hIsRunAsAdminLabel, fIsRunAsAdmin ? L"是" : L"否");
    }
    catch (DWORD dwError)
    {
        SetWindowText(hIsRunAsAdminLabel, L"N/A");
        ReportError(L"IsRunAsAdmin", dwError);
    }
    

     // 获取并显示进程权限提升信息(IsProcessElevated)和完整性级别（GetProcessIntegrityLevel）
    // 注意：这些信息在Windows Vista之前的Windows中不存在。

    HWND hIsElevatedLabel = GetDlgItem(hWnd, IDC_ISELEVATED_STATIC);
    HWND hILLabel = GetDlgItem(hWnd, IDC_IL_STATIC);

    OSVERSIONINFO osver = { sizeof(osver) };
    if (GetVersionEx(&osver) && osver.dwMajorVersion >= 6)
    {
        // 运行于Windows Vista或后续版本（主版本号 >= 6）。
        try
        {
            // 获取并显示进程权限提升信息
            BOOL const fIsElevated = IsProcessElevated();
            SetWindowText(hIsElevatedLabel, fIsElevated ? L"是" : L"否");

            // 如果进程尚未被提升，更新“自我提升权限”按钮以在UI中显示UAC盾形
            // 图标。宏Button_SetElevationRequiredState（在Commctrl.h中定义）用
            // 于显示或隐藏按钮上的盾形图标。你也可以通过调用SHGetStockIconInfo
            // （参量SIID_SHIELD）来获取此图标。
            HWND hElevateBtn = GetDlgItem(hWnd, IDC_ELEVATE_BN);
            Button_SetElevationRequiredState(hElevateBtn, !fIsElevated);
        }
        catch (DWORD dwError)
        {
            SetWindowText(hIsElevatedLabel, L"N/A");
            ReportError(L"IsProcessElevated", dwError);
        }

        try
        {
            // 获取并显示进程的完整性级别
            DWORD const dwIntegrityLevel = GetProcessIntegrityLevel();
            switch (dwIntegrityLevel)
            {
            case SECURITY_MANDATORY_UNTRUSTED_RID: SetWindowText(hILLabel, L"不信任"); break;
            case SECURITY_MANDATORY_LOW_RID: SetWindowText(hILLabel, L"低"); break;
            case SECURITY_MANDATORY_MEDIUM_RID: SetWindowText(hILLabel, L"中"); break;
            case SECURITY_MANDATORY_HIGH_RID: SetWindowText(hILLabel, L"高"); break;
            case SECURITY_MANDATORY_SYSTEM_RID: SetWindowText(hILLabel, L"系统"); break;
            default: SetWindowText(hILLabel, L"未知"); break;
            }
        }
        catch (DWORD dwError)
        {
            SetWindowText(hILLabel, L"N/A");
            ReportError(L"GetProcessIntegrityLevel", dwError);
        }
    }
    else
    {
        SetWindowText(hIsElevatedLabel, L"N/A");
        SetWindowText(hILLabel, L"N/A");
    }

    return TRUE;
}


//
//   函数：OnCommand(HWND, int, HWND, UINT)
//
//   用途：处理WM_COMMAND消息
//
void OnCommand(HWND hWnd, int id, HWND hwndCtl, UINT codeNotify)
{
    switch (id)
    {
    case IDC_ELEVATE_BN:
        {
            // 检查当前进程是否以管理员权限运行，如果不是则提升。           
            BOOL fIsRunAsAdmin;
            try
            {
                fIsRunAsAdmin = IsRunAsAdmin();
            }
            catch (DWORD dwError)
            {
                ReportError(L"IsRunAsAdmin", dwError);
                break;
            }

             // 如果此进程不是以管理员身份运行，提升权限等级。
            if (!fIsRunAsAdmin)
            {
                wchar_t szPath[MAX_PATH];
                if (GetModuleFileName(NULL, szPath, ARRAYSIZE(szPath)))
                {
                    // 以管理员身份启动本程序。
                    SHELLEXECUTEINFO sei = { sizeof(sei) };
                    sei.lpVerb = L"runas";
                    sei.lpFile = szPath;
                    sei.hwnd = hWnd;
                    sei.nShow = SW_NORMAL;

                    if (!ShellExecuteEx(&sei))
                    {
                        DWORD dwError = GetLastError();
                        if (dwError == ERROR_CANCELLED)
                        {
                                // 用户拒绝提升
                                // 什么都不做...
                        }
                    }
                    else
                    {
                        EndDialog(hWnd, TRUE);  // 退出
                    }
                }
            }
            else
            {
                MessageBox(hWnd, L"此进程已以管理员身份运行", L"UAC", MB_OK);
            }
        }
        break;

    case IDOK:
    case IDCANCEL:
        EndDialog(hWnd, 0);
        break;
    }
}


//
//   函数：OnClose(HWND)
//
//   用途：处理WM_CLOSE消息
//
void OnClose(HWND hWnd)
{
    EndDialog(hWnd, 0);
}


//
//  函数：DialogProc(HWND, UINT, WPARAM, LPARAM)
//
//  用途： 为主对话框处理消息
//
INT_PTR CALLBACK DialogProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // 调用OnInitDialog处理WM_INITDIALOG消息
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitDialog);

        // 调用OnCommand处理WM_COMMAND消息
        HANDLE_MSG (hWnd, WM_COMMAND, OnCommand);

        // 调用OnClose处理WM_CLOSE消息
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;
    }
    return 0;
}


//
//  函数：wWinMain(HINSTANCE, HINSTANCE, LPWSTR, int)
//
//  用途：主入口函数
//
int APIENTRY wWinMain(HINSTANCE hInstance,
                      HINSTANCE hPrevInstance,
                      LPWSTR    lpCmdLine,
                      int       nCmdShow)
{
    return DialogBox(hInstance, MAKEINTRESOURCE(IDD_MAINDIALOG), NULL, DialogProc);
}