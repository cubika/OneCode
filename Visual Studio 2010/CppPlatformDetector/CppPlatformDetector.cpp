/****************************** 模块头******************************\
模块名:    CppPlatformDetector.cpp
项目名:        CppPlatformDetector
版权 (c) Microsoft Corporation.

CppPlatformDetector  演示以下几个与平台检测相关的任务：

1. 检测当前操作系统的名字（例如："Microsoft Windows 7 企业版"）
2. 检测当前操作系统的版本（例如："Microsoft Windows NT 6.1.7600.0")
3. 确定当前的操作系统是否是64位的系统。
4. 确定当前的进程是否是64位的进程。
5. 确定任意一个进程是否运行在64位系统。
 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include <stdio.h>
#include <Windows.h>
#include <locale>
#include "PlatformDetector.h"


int wmain(int argc, wchar_t *argv[])
{
    //
    // 打印当前操作系统的名字.
    //

    wchar_t szName[512];
    if (GetOSName(szName, ARRAYSIZE(szName)))
    {
		setlocale(LC_CTYPE, ""); 
        wprintf(L"当前的操作系统: %s\n", szName);
    }
    else
    {

        wprintf(L"无法获得操作系统名字\n");
    }

    //
    // 打印当前操作系统的版本字符串.
    //

    wchar_t szVersionString[512];
    if (GetOSVersionString(szVersionString, ARRAYSIZE(szVersionString)))
    {
        wprintf(L"版本: %s\n", szVersionString);
    }
    else
    {
        wprintf(L"无法获得操作系统版本\n");
    }

    //
    // 确定当前操作系统是否是64位.
    //

    BOOL f64bitOS = Is64BitOS();
    wprintf(L"当前的操作系统是 %s64位\n", f64bitOS ? L"" : L"不是");

    //
    // 确定当前的进程是否是64位的. 
    //

    BOOL f64bitProc = Is64BitProcess();
    wprintf(L"当前的进程是 %s64位\n", f64bitProc ? L"" : L"不是 ");

    //
    // 确定运行在系统中的任意一个进程是否是64位进程.
    //

    if (argc > 1)
    {
        // 如果一个进程ID在命令行中被指定，则获取进程ID，并打开进程句柄.
        DWORD dwProcessId = _wtoi(argv[1]);
        if (dwProcessId != 0 /*conversion succeeds*/)
        {
            HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION, FALSE,
                dwProcessId);
            if (hProcess != NULL)
            {
                //  检测指定进程是否是64位.
                BOOL f64bitProc = Is64BitProcess(hProcess);
                wprintf(L"进程 %d 是 %s64-bit\n", dwProcessId, 
                    f64bitProc ? L"" : L"不是 ");

                CloseHandle(hProcess);
            }
            else
            {
                wprintf(L"打开进程(%d) 失败 w/err 0x%08lx\n", 
                    dwProcessId, GetLastError());
            }
        }
        else
        {
            wprintf(L"无效的进程 ID: %s\n", argv[1]);
        }
    }

    return 0;
}