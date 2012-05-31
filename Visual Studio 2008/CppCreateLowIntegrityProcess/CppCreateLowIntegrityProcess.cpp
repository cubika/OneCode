/****************************** 模块头 ******************************\
* 模块名:  CppCreateLowIntegrityProcess.cpp
* 项目名:      CppCreateLowIntegrityProcess
* 版权 (c) Microsoft Corporation.
* 
* 这个代码示例演示了如何启动一个低完整性进程。当你点击本程序中“以低完整等级执
* 行本程序”按钮，此应用程序使用低完整性再次启动一个本程序实例。低完整性进程只
* 能在低完整性区域内写入数据，比如%USERPROFILE%\AppData\LocalLow文件夹或者注册
* 表中的HKEY_CURRENT_USER\Software\AppDataLow键值。即使当前用户的SID在自由访问
* 控制列表（discretionary access control list）中拥有写入权限，如果你想要访问一
* 个完整性高的对象，你也将会收到一个无法访问的错误。
*
* 默认情况下，子进程继承其父进程的完整性等级。要启动一个低完整性进程，你必须使用
* CreateProcessAsUser和低完整性访问令牌启动一个新的子进程。详细信息请参考示例
* CreateLowIntegrityProcess中的相关函数。
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
#include <sddl.h>
#include <shlobj.h>
#include "Resource.h"

// Enable Visual Style
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


#pragma region Helper Functions related to Process Integrity Level

// 
//   函数：CreateLowIntegrityProcess(PCWSTR)
//
//   用途：此函数以低完整性级别启动一个应用程序
//
//   参数：
//   * pszCommandLine - 需要被执行的命令行。此字符串最长为32K个字符。此字符串
//     不能是一个指向只读内存的指针（比如常量或者常量字符串）。如果这个参数是
//     一个常量字符串，此函数会导致一个访问冲突（access violation）的错误。
//
//   返回值：如果此函数成功，则返回值是TRUE。如果失败，则为0。调用GetLastError
//   获取额外的错误信息。
//
//   注释：
//   启动一个低完整性进程
//   1) 复制当前进程的句柄，它拥有中完整性级别
//   2）使用SetTokenInformation设置访问进程的完整性级别为低。
//   3）使用CreateProcessAsUser及低完整性级别的访问令牌创建一个新的进程。
//

BOOL CreateLowIntegrityProcess(PWSTR pszCommandLine)
{
    DWORD dwError = ERROR_SUCCESS;
    HANDLE hToken = NULL;
    HANDLE hNewToken = NULL;
    wchar_t szIntegritySid[] = L"S-1-16-4096";  // 低完整性SID字符串
    PSID pIntegritySid = NULL;
    TOKEN_MANDATORY_LABEL tml = { 0 };
    STARTUPINFO si = { sizeof(si) };
    PROCESS_INFORMATION pi = { 0 };

    // 打开进程的主访问令牌
    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_DUPLICATE | TOKEN_QUERY |
        TOKEN_ADJUST_DEFAULT | TOKEN_ASSIGN_PRIMARY, &hToken))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 复制当前进程的主令牌
    if (!DuplicateTokenEx(hToken, 0, NULL, SecurityImpersonation, 
        TokenPrimary, &hNewToken))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 创建低完整性SID
    if (!ConvertStringSidToSid(szIntegritySid, &pIntegritySid))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    tml.Label.Attributes = SE_GROUP_INTEGRITY;
    tml.Label.Sid = pIntegritySid;

    // 设置访问令牌的完整性级别为低
    if (!SetTokenInformation(hNewToken, TokenIntegrityLevel, &tml, 
        (sizeof(tml) + GetLengthSid(pIntegritySid))))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

    // 以低完整性级别创建一个新进程。
    if (!CreateProcessAsUser(hNewToken, NULL, pszCommandLine, NULL, NULL, 
        FALSE, 0, NULL, NULL, &si, &pi))
    {
        dwError = GetLastError();
        goto Cleanup;
    }

Cleanup:
    // 集中清理已分配的资源
    if (hToken)
    {
        CloseHandle(hToken);
        hToken = NULL;
    }
    if (hNewToken)
    {
        CloseHandle(hNewToken);
        hNewToken = NULL;
    }
    if (pIntegritySid)
    {
        LocalFree(pIntegritySid);
        pIntegritySid = NULL;
    }
    if (pi.hProcess)
    {
        CloseHandle(pi.hProcess);
        pi.hProcess = NULL;
    }
    if (pi.hThread)
    {
        CloseHandle(pi.hThread);
        pi.hThread = NULL;
    }

    if (ERROR_SUCCESS != dwError)
    {
        // 失败时确保此能够获取此错误代码
        SetLastError(dwError);
        return FALSE;
    }
    else
    {
        return TRUE;
    }
}


//
//   函数： GetProcessIntegrityLevel(PDWORD)
//
//   用途：此函数获取当前线程的完整性级别。完整性级别只有在Windows Vista及后
//   续版本的Windows中有效。所以在Windows Vista之前的版本中执行GetProcessIntegrityLevel， 
//   它返回FALSE。
//
//   参数：
//   * pdwIntegrityLevel - 输出当前进程的完整性级别。它可能是以下某一个值。
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
//   返回值: 如果函数调用成功，它返回TRUE。如果失败则为FALSE。调用GetLastError
//   获取额外的错误信息。例如如果GetProcessIntegrityLevel在Windows Vista之前系统
//   或则pdwIntegrityLevel为空时，ERROR_INVALID_PARAMETER是最后的错误信息
//
//   调用示例：
//     DWORD dwIntegrityLevel;
//     if (!GetProcessIntegrityLevel(&dwIntegrityLevel))
//     {
//         wprintf(L"GetProcessIntegrityLevel failed w/err %lu\n", 
//             GetLastError());
//     }
//
BOOL GetProcessIntegrityLevel(PDWORD pdwIntegrityLevel)
{
    DWORD dwError = ERROR_SUCCESS;
    HANDLE hToken = NULL;
    DWORD cbTokenIL = 0;
    PTOKEN_MANDATORY_LABEL pTokenIL = NULL;

    if (pdwIntegrityLevel == NULL)
    {
        dwError = ERROR_INVALID_PARAMETER;
        goto Cleanup;
    }

    // 以TOKEN_QUERY开启此线程的主访问令牌。
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
    *pdwIntegrityLevel = *GetSidSubAuthority(pTokenIL->Label.Sid, 0);

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

    if (ERROR_SUCCESS != dwError)
    {
        // 失败时确保此能够获取此错误代码
        SetLastError(dwError);
        return FALSE;
    }
    else
    {
        return TRUE;
    }
}

#pragma endregion


//
//   函数：ReportError(LPWSTR, DWORD)
//
//   用途：为调用失败的函数显示一个错误信息对话框
//
//   参数：
//   * pszFunction - 调用失败的函数的名称
//   * dwError - Win32错误代码。 它的默认值为线程最后的错误代码。
//
//   注意：如果你没有明确指定dwError的值，你必须在函数失败后立刻调用ReportError
//   这能确保正在调用的进程的最后错误代码没有被其他API改写。
//
void ReportError(LPCWSTR pszFunction, DWORD dwError = GetLastError())
{
    wchar_t szMessage[200];
    if (-1 != swprintf_s(szMessage, ARRAYSIZE(szMessage), 
        L"%s 失败 w/err 0x%08lx", pszFunction, dwError))
    {
        MessageBox(NULL, szMessage, L"错误", MB_ICONERROR);
    }
}


// 
//   函数：OnInitDialog(HWND, HWND, LPARAM)
//
//   用途：处理 WM_INITDIALOG消息.
//
BOOL OnInitDialog(HWND hWnd, HWND hwndFocus, LPARAM lParam)
{
    // 获取和显示进程的完整性级别。
    HWND hILLabel = GetDlgItem(hWnd, IDC_IL_STATIC);
    DWORD dwIntegrityLevel;
    if (GetProcessIntegrityLevel(&dwIntegrityLevel))
    {
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
    else
    {
        ReportError(L"GetProcessIntegrityLevel");
        SetWindowText(hILLabel, L"N/A");
    }

    return TRUE;
}


// 
//   函数：CreateTestFileInKnownFolder(REFKNOWNFOLDERID)
//
//   用途：此函数尝试在指定的Windows已知文件夹中创建一个测试文件(testfile.txt)，
//   并且显示在消息对话框中测试结果。
//
//   参数：
//   * rfid - KNOWNFOLDERID结构，用于指定文件夹
//
void CreateTestFileInKnownFolder(REFKNOWNFOLDERID rfid) 
{
    HRESULT hr = S_OK;
    PWSTR pszFolder = NULL;
    wchar_t szPath[MAX_PATH];
    HANDLE hFile = INVALID_HANDLE_VALUE;
    wchar_t szMessage[1024];

    // 获取已知文件夹
    hr = SHGetKnownFolderPath(rfid, 0, NULL, &pszFolder);
    if (FAILED(hr))
    {
        goto Cleanup;
    }

    // 把文件名添加到文件夹路径后，以便获得完整的文件路径。
    wcscpy_s(szPath, ARRAYSIZE(szPath), pszFolder);
    wcscat_s(szPath, ARRAYSIZE(szPath), L"\\testfile.txt");

    // 创建测试文件
    hFile = CreateFile(szPath, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, 
        FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile == INVALID_HANDLE_VALUE)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
    }
    
    // 显示测试结果
    if (SUCCEEDED(hr))
    {
        swprintf_s(szMessage, ARRAYSIZE(szMessage), 
            L"成功写入测试文件: %s", szPath);
        MessageBox(NULL, szMessage, L"测试结果", MB_ICONINFORMATION);
    }
    else if (hr == E_ACCESSDENIED)
    {
        swprintf_s(szMessage, ARRAYSIZE(szMessage), 
            L"拒绝访问 '%s'.", szPath);
        MessageBox(NULL, szMessage, L"测试结果", MB_ICONERROR);
    }
    else
    {
        swprintf_s(szMessage, ARRAYSIZE(szMessage), 
            L"写入测试文件 '%s'失败 w/err 0x%08lx", 
            szPath, hr);
        MessageBox(NULL, szMessage, L"测试结果", MB_ICONERROR);
    }

Cleanup:
    // 集中清理所有已分配的内存资源
    if (pszFolder)
    {
        CoTaskMemFree(pszFolder);
        pszFolder = NULL;
    }
    if (hFile != INVALID_HANDLE_VALUE)
    {
        CloseHandle(hFile);
        hFile = INVALID_HANDLE_VALUE;
    }
}


//
//   函数:OnCommand(HWND, int, HWND, UINT)
//
//   用途：处理WM_COMMAND消息
//
void OnCommand(HWND hWnd, int id, HWND hwndCtl, UINT codeNotify)
{
    switch (id)
    {
    case IDC_CREATELOWPROCESS_BN:
        {
            wchar_t szPath[MAX_PATH];
            if (GetModuleFileName(NULL, szPath, ARRAYSIZE(szPath)))
            {
                // 以低完整性级别启动应用程序
                if (!CreateLowIntegrityProcess(szPath))
                {
                    ReportError(L"CreateLowIntegrityProcess");
                }
            }
        }
        break;

    case IDC_WRITELOCALAPPDATA_BN:
        // 尝试在LocalAppData中创建测试文件
        CreateTestFileInKnownFolder(FOLDERID_LocalAppData);
        break;

    case IDC_WRITELOCALAPPDATALOW_BN:
        // 尝试在LocalAppDataLow中创建测试文件
        CreateTestFileInKnownFolder(FOLDERID_LocalAppDataLow);
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
//   用途:处理WM_CLOSE消息
//
void OnClose(HWND hWnd)
{
    EndDialog(hWnd, 0);
}


//
//  函数：DialogProc(HWND, UINT, WPARAM, LPARAM)
//
//  用途： 处理主对话框的消息循环
//
INT_PTR CALLBACK DialogProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // 在OnInitDialog中处理WM_INITDIALOG消息
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitDialog);

        // 在OnCommand中处理WM_COMMAND消息
        HANDLE_MSG (hWnd, WM_COMMAND, OnCommand);

        // 在OnClose中处理WM_CLOSE消息
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;
    }
    return 0;
}


//
//  函数：wWinMain(HINSTANCE, HINSTANCE, LPWSTR, int)
//
//  用途：应用程序入口点
//
int APIENTRY wWinMain(HINSTANCE hInstance,
                      HINSTANCE hPrevInstance,
                      LPWSTR    lpCmdLine,
                      int       nCmdShow)
{
    return DialogBox(hInstance, MAKEINTRESOURCE(IDD_MAINDIALOG), NULL, DialogProc);
}