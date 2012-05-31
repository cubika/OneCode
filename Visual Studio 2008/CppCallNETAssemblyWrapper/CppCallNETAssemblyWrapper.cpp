/********************************* 模块头 **********************************\
模块名:  CppCallNETAssemblyWrapper.cpp
项目:    CppCallNETAssemblyWrapper
版权     (c) Microsoft Corporation.

这个代码示例演示了一个本地C++应用程序通过C++/CLI封装类对托管代码进行封装后的调用

CppCallNETAssemblyWrapper (本地C++应用程序)
-->
CppCLINETAssemblyWrapper (C++/CLI的封装)
-->
CSClassLibrary (.NET程序集)

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include <stdio.h>
#include <windows.h>
#include <locale.h>

#include "CppCLINETAssemblyWrapper.h"
#pragma endregion


//
//   方法: IsModuleLoaded(PCWSTR)
//
//   目的: 检查是否指定模块被当前进程加载.
//
//   参数:
//   * pszModuleName - 模块名
//
//   返回值: 该函数返回TRUE，如果指定的模块是在当前进程中加载​​。如果模块没有被加载，该函数返回FALSE
//
BOOL IsModuleLoaded(PCWSTR pszModuleName) 
{
    // 在进程中，根据模块名获得模块.
    HMODULE hMod = GetModuleHandle(pszModuleName);
    return (hMod != NULL);
}


int wmain(int argc, wchar_t *argv[])
{
    BOOL fLoaded = FALSE;

	// 将 locale 设置为默认， 以支持输出中文.
	setlocale(LC_ALL, "");

    // .NET程序集名 
    PCWSTR pszModuleName = L"CSClassLibrary";

    // 检察模块是否被加载.
    fLoaded = IsModuleLoaded(pszModuleName);
    wprintf(L"模块 \"%s\" %s被加载\n", pszModuleName, fLoaded ? L"" : L"没有");

    //
    // 在.NET程序集中引用公共类。
    // 

	// 通过C++/CLI封装类CSSimpleObjectWrapper，调用.NET类CSSimpleObject
    CSSimpleObjectWrapper obj;

    obj.set_FloatProperty(1.2F);
    float fProp = obj.get_FloatProperty();
    wprintf(L"Class: CSSimpleObject::FloatProperty = %.2f\n", fProp);

    wchar_t szStr[100];
    HRESULT hr = obj.ToString(szStr, ARRAYSIZE(szStr));
    if (SUCCEEDED(hr))
    {
        wprintf(L"Class: CSSimpleObject::ToString => \"%s\"\n", szStr);
    }
    else
    {
        wprintf(L"CSSimpleObject::ToString 失败 w/hr 0x%08lx\n", hr);
    }

    // 你不能卸载.NET程序集 
    // 检查是否加载该模块
    fLoaded = IsModuleLoaded(pszModuleName);
    wprintf(L"模块 \"%s\" %s被加载\n", pszModuleName, fLoaded ? L"" : L"没有");

    return 0;
}