/****************************** 模块头 ******************************\
模块名:  CppDynamicLinkLibrary.h
项目:    CppDynamicLinkLibrary
版权	 (c) Microsoft Corporation.

一个动态链接库（DLL）是一个包含函数和可以被其他模块（应用程序或DLL）使用的数据
的模块。这个Win32 DLL代码示例展示了导出数据，函数和类。

DLL代码示例导出数据，函数和类

    // 全局数据
    int g_nVal1
    int g_nVal2

    // 普通函数
    int __cdecl GetStringLength1(PCWSTR pszString);
    int __stdcall GetStringLength2(PCWSTR pszString);

    // 回调函数
    int __stdcall Max(int a, int b, PFN_COMPARE cmpFunc)

    // 类
    class CSimpleObject
    {
    public:
        CSimpleObject(void);  // 构造函数
        virtual ~CSimpleObject(void);  // 析构函数
          
        // 属性
        float get_FloatProperty(void);
        void set_FloatProperty(float newVal);

        // 方法
        HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);

        // 静态方法
        static int GetStringLength(PCWSTR pszString);

    private:
        float m_fField;
    };
两种方法可以从示例DLL中到处标记

1. 用.DEF 文件导出标记

一个模块定义(.DEF)文件是一个包含了一个或者多个模块描述DLL各种数据的文本文件。
当编译DLL时，可以创建和使用一个.DEF文件。使用这种方法，我们可以从DLL按序号而
不是按名称导出函数。
 

2. 用 __declspec(dllexport)标记导出

__declspec(dllexport)添加了导出指令添加到对象文件，这样我们就可以不需要.DEF文件。
当试图导出带修饰的C++函数名称时，这种是最便利是。


This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>

// 以下的ifdef块是创建宏定义的标准方法，它可以使从DLL导出更容易。所有在DLL中的
//文件都需要用定义在命令行中的CPPDYNAMICLINKLIBRARY_EXPORTS标记来编译（详见 项目
//属性中的C/C++ /预处理页面）。这个标记不应该被定义在其他需要使用这个DLL的项目中。
//这样那些其他项目的源文件包含这个文件可以看见SYMBOL_DECLSPEC函数被从一个DLL中引
//入，尽管这个DLL看见标记被定义在这个宏定义中。

#ifdef CPPDYNAMICLINKLIBRARY_EXPORTS
#define SYMBOL_DECLSPEC __declspec(dllexport)
#define SYMBOL_DEF
#else
#define SYMBOL_DECLSPEC __declspec(dllimport)
#define SYMBOL_DEF      __declspec(dllimport)
#endif


#pragma region Global Data

// 用DEF文件导入或导出的一个全局数据
// 标记: g_nVal1
// 详见: CppDynamicLinkLibrary.def
//      CppDynamicLinkLibrary.cpp
// 参考: http://support.microsoft.com/kb/90530


// 用__declspec(dllexport/dllimport)导入或导出的一个全局数据
// 标记: g_nVal2
// 详见: CppDynamicLinkLibrary.cpp
// 参考: http://support.microsoft.com/kb/90530
EXTERN_C SYMBOL_DECLSPEC int g_nVal2;

#pragma endregion


#pragma region Ordinary Functions

// 用DEF文件导入或导出的一个cdecl(默认)方法
// 这个默认调用导出的转换函数是cdecl
// 标记: GetStringLength1
// 详见: Project Properties / C/C++ / Advanced / Calling Convention
//      CppDynamicLinkLibrary.def
//      CppDynamicLinkLibrary.cpp
// 参考: http://msdn.microsoft.com/en-us/library/d91k01sh.aspx
SYMBOL_DEF int /*__cdecl*/ GetStringLength1(PCWSTR pszString);

// 用__declspec(dllexport/dllimport)导入或导出的一个stdcall方法
// 标记: _GetStringLength2@4
// 详见: CppDynamicLinkLibrary.cpp
// 参考: http://msdn.microsoft.com/en-us/library/a90k134d.aspx
EXTERN_C SYMBOL_DECLSPEC int __stdcall GetStringLength2(PCWSTR pszString);

#pragma endregion


#pragma region Callback Function

// 定义类型：'PFN_COMPARE' 现在可以被当做一个类型使用
typedef int (CALLBACK *PFN_COMPARE)(int, int);

// 用DEF文件导入或导出一个stdcall方法
// 它需要一个回调函数作为参数之一
// 标记: Max
// 详见: CppDynamicLinkLibrary.cpp
SYMBOL_DEF int __stdcall Max(int a, int b, PFN_COMPARE cmpFunc);

#pragma endregion


#pragma region Class

// 用__declspec(dllexport/dllimport)导入或导出的一个类
// 它导入或导出所有的公共类的成员
// 详见: CppDynamicLinkLibrary.cpp
// 参考: http://msdn.microsoft.com/en-us/library/a90k134d.aspx
class SYMBOL_DECLSPEC CSimpleObject
{
public:

    CSimpleObject(void);  // 构造函数
    virtual ~CSimpleObject(void);  // 析构函数

    // 属性
    float get_FloatProperty(void);
    void set_FloatProperty(float newVal);

    // 方法
    HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);

    // 静态方法
    static int GetStringLength(PCWSTR pszString);

private:
    float m_fField;
};

#pragma endregion