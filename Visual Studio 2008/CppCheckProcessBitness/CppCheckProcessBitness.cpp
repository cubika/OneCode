/****************************** 模块头 ******************************\
* 模块名:  CppCheckProcessBitness.cpp
* 项目名:  CppCheckProcessBitness
* 版权 (c) Microsoft Corporation.
* 
*这个实例代码演示了如何确定一个给定的进程是64位的还是32位的  
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include <stdio.h>
#include <windows.h>
#pragma endregion

//
//   FUNCTION: DoesWin32MethodExist(PCWSTR, PCSTR)
//
//   PURPOSE: 该函数确定是否方法存在于一个特定的模块导出表. .
//
//   PARAMETERS:
//   * pszModuleName - 模块的名称.
//   * pszMethodName - 方法的名称.
//
//   RETURN VALUE: 如果由methodName指定的的方法存在于模块名指定的模块导出表，该函数返回true.
//
BOOL DoesWin32MethodExist(PCWSTR pszModuleName, PCSTR pszMethodName)
{
    HMODULE hModule = GetModuleHandle(pszModuleName);
    if (hModule == NULL)
    {
        return FALSE;
    }
    return (GetProcAddress(hModule, pszMethodName) != NULL);
}


//
//   FUNCTION: Is64BitOperatingSystem()
//
//   PURPOSE: 该函数判断当前操作系统是否是64位操作系统.
//
//   RETURN VALUE: 如果操作系统是64位，该函数返回true，否则返回false.
//
BOOL Is64BitOperatingSystem()
{
#if defined(_WIN64)
    return TRUE;   // 64位程序只能运行在windows x64里
#elif defined(_WIN32)
    // 32位程序运行在32位和64位windows下.
    BOOL fIs64bitOS = FALSE;
    return (DoesWin32MethodExist(L"kernel32.dll", "IsWow64Process") && 
        (IsWow64Process(GetCurrentProcess(), &fIs64bitOS) && fIs64bitOS));
#else
    return FALSE;  // 64位Windows不支持win16.
#endif
}


//
//   FUNCTION: Is64bitProcess(HANDLE hProcess)
//
//   PARAMETERS:
//   * hProcess - 该进程的句柄.
//   
//   PURPOSE: 该函数判断指定的进程是否是64位的.
//
//   RETURN VALUE: 如果指定的进程是64位的，函数返回TRUE；
        ///  否则返回FALSE.
//
BOOL Is64BitProcess(HANDLE hProcess)
{
	BOOL fIs64bitProcess = FALSE;

    if (Is64BitOperatingSystem())
	{
		// 在64位操作系统，如果一个进程是没有在WOW64模式下运行，这个进程一定是一个64位的.
		fIs64bitProcess = !(IsWow64Process(hProcess, &fIs64bitProcess) && 
			fIs64bitProcess);
	}

	return fIs64bitProcess;
}


int wmain(int argc, wchar_t* argv[])
{
	if (argc > 1)
	{
		// 如果指定一个进程的ID作为参数.
		DWORD dwProcessId = _wtoi(argv[1]); 
		HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION, FALSE, 
			dwProcessId);

		if(hProcess != NULL)
		{
			if(Is64BitProcess(hProcess))
			{
				printf("该进程是一个64位进程.\n");
			}
			else
			{
				printf("该进程是一个32位进程.\n");
			}

			CloseHandle(hProcess);
		}
		else
		{
			DWORD errorCode = GetLastError();
			printf("错误发生在得到进程 %d, 错误代码是:0x%08lx\n", 
				dwProcessId, errorCode);
		}
	}
	else
	{
		// 如果没有指定进程ID，则使用当前进程的ID.
		HANDLE hProcess = GetCurrentProcess();
		if(Is64BitProcess(hProcess))
		{
			printf("当前进程是64位的.\n");
		}
		else
		{
			printf("当前进程是32位的.\n");
		}
	}

	return 0;
}

