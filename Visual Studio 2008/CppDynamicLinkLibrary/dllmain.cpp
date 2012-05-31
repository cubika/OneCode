/****************************** 模块头 *************************************\
模块名:  dllmain.cpp
项目:    CppDynamicLinkLibrary
版权	 (c) Microsoft Corporation.

这个文件定义了作为DLL可选入口的回调函数DLLMain。当系统启动或结束一个进程或线程
时，它会调用入口函数。当它通过使用LoadLibrary 和FreeLibrary，加载或卸载DLL，系
统同样会调用为DLL调用入口函数。

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include <windows.h>


BOOL APIENTRY DllMain(HMODULE hModule,
                      DWORD  ul_reason_for_call,
                      LPVOID lpReserved
                      )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

