/****************************** 模块头 ***********************************\
模块名:  CppDynamicLinkLibrary.cpp
项目:    CppDynamicLinkLibrary
版权	 (c) Microsoft Corporation.

定义了从DLL应用中导出的数据和方法

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "CppDynamicLinkLibrary.h"
#include <strsafe.h>


#pragma region Global Data

// 用DEF文件导入或导出一个全局数据
// 初始化为1
int g_nVal1 = 1;


// 用__declspec(dllexport/dllimport)导入或导出一个全局数据
// 初始化为2
SYMBOL_DECLSPEC int g_nVal2 = 2;

#pragma endregion


#pragma region Ordinary Functions

// 用DEF文件导入或导出一个cdecl(默认)方法
int /*__cdecl*/ GetStringLength1(PCWSTR pszString)
{
    return static_cast<int>(wcslen(pszString));
}


//用__declspec(dllexport/dllimport)导入或导出一个stdcall方法
SYMBOL_DECLSPEC int __stdcall GetStringLength2(PCWSTR pszString)
{
    return static_cast<int>(wcslen(pszString));
}

#pragma endregion


#pragma region Callback Function

// 用DEF文件导入或导出一个stdcall方法
// 它需要一个回调函数作为参数之一
int __stdcall Max(int a, int b, PFN_COMPARE cmpFunc)
{
	// 用回调比较函数

	// 如果a大于b，则返回a
    // 如果b大于a，则返回b
    return ((*cmpFunc)(a, b) > 0) ? a : b;
}

#pragma endregion


#pragma region Class

// C++类的构造函数
CSimpleObject::CSimpleObject(void) : m_fField(0.0f)
{
}


// C++类实例的析构函数
CSimpleObject::~CSimpleObject(void)
{
}


float CSimpleObject::get_FloatProperty(void)
{
	return this->m_fField;
}


void CSimpleObject::set_FloatProperty(float newVal)
{
	this->m_fField = newVal;
}


HRESULT CSimpleObject::ToString(PWSTR pszBuffer, DWORD dwSize)
{
    return StringCchPrintf(pszBuffer, dwSize, L"%.2f", this->m_fField);
}


int CSimpleObject::GetStringLength(PCWSTR pszString)
{
    return static_cast<int>(wcslen(pszString));
}

#pragma endregion