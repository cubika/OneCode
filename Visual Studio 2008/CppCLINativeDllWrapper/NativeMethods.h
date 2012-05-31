/****************************** 模块头 ***********************************\
模块名:  NativeMethods.h
项目:    CppCLINativeDllWrapper
版权	 (c) Microsoft Corporation.

这个C++/CLI代码示例展示了利用C++/CLI封装类，使得.NET代码可以调用本地C++ DLL模块中
导出的类和方法。

  CSCallNativeDllWrapper/VBCallNativeDllWrapper (任意.NET 应用)
          -->
      CppCLINativeDllWrapper (C++/CLI 封装)
              -->
          CppDynamicLinkLibrary (本地C++ DLL 模块)
在此次代码示例中，CSimpleObjectWrapper类封装了本地C++类CSimpleObject，NativeMethods类
封装了被CppDynamicLinkLibrary.dll导出的全局函数。

当涉及到与本地模块相互操作时，这些被Visual C++/CLI支持的能共同使用的特性提供了一个比其他
.NET语言更大的优势。除了传统的明确的P/Invoke，C++/CLI允许隐式P/Invoke，这也被称为C++ Interop，
或者几乎隐式的混合了托管代码和本地代码的It Just Work（IJW）。这些特性提供了更好的安全性，更
方便地代码，更强劲的编程以及更能减少对本地API的修改。如果本地代码是可用的并且允许任何.NET应用
去通过封装访问本地C++的类和方法，你可以使用技术去编译封装本地C++类和方法。

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

// 引入本地C++模块头文件
#include "CppDynamicLinkLibrary.h"

using namespace System;
using namespace System::Runtime::InteropServices;


namespace CppCLINativeDllWrapper
{
    /// <summary>
    /// 回调函数'PFN_COMPARE'  
    /// </summary>
    /// <remarks>
    /// 委托类型有 UnmanagedFunctionPointerAttribute. 使用这个特性，你可以定义 
    /// 调用本地函数指针类型的转换。在本地API的头文件中，回调函数 PFN_COMPARE 
    /// 被定义为__stdcall (CALLBACK)。所以这里我们可以使用
    /// CallingConvention::StdCall.
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention::StdCall)]
    public delegate int CompareCallback(int a, int b);


    /// <summary>
    /// C++/CLI类封装了本地C++模块CppDynamicLinkLibrary.dll所导出的函数。
    /// </summary>
    public ref class NativeMethods
    {
    public:
        static int GetStringLength1(String ^ str);
        static int GetStringLength2(String ^ str);
        static int Max(int a, int b, CompareCallback ^ cmpFunc);
    };
}