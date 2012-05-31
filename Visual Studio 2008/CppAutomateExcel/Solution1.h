/****************************** 模块头**************************************\
* 模块名:     Solution1.h
* 项目名:     CppAutomateExcel
* 版权(c)     Microsoft Corporation.
* 
* Solution1.h及Solution1.cpp文件中的代码阐述了如何使用#import指令自动化Excel的方
* 法。#import(http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx)  作为伴随
* VC++5.0出现的一种新的指令，能够通过指定类型库生成VC++“智能指针”。 虽然这种方式功
* 能强大，但并不推荐使用，因为将其用于Microsoft Office应用程序时，往往将导致引用计
* 数问题。与Solution2.h及Solution2.cpp中的直接API方法不同，智能指针使我们能够受益
* 于类型信息，从而对对象进行早期/后期绑定。#import 指令添加无序的Guid到项目，而COM
* API则被封装到由#import 指令生成的自定义类中。
* 
* 该来源受微软授予的公共许可证约束。
* 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* 保留其它所有权。
* 
* 该代码和资料不提供任何形式，明示或暗示的担保，包括但不限于适销特定用途的暗示性和
* 适用性的保证。
\***************************************************************************/


#pragma once


//
// 函数：AutomateExcelByImport(LPVOID)

// 作用：通过#import指令和智能指针自动化Microsoft Excel 

// 参数：
// * lpParam - 创建线程时通过 lpParameter参数将线程数据传递给函数
// (http://msdn.microsoft.com/en-us/library/ms686736.aspx)

// 返回值：返回值用于标示函数执行成功或失败

//
DWORD WINAPI AutomateExcelByImport(LPVOID lpParam);