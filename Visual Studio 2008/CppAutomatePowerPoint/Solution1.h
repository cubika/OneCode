/****************************** 模块头 ******************************\
* 模块名:  Solution1.h
* 项目名:  CppAutomatePowerPoint
* 版权 (c) Microsoft Corporation.
* 
* Solution1.h/cpp中的代码演示了#import在自动化的PowerPoint中的使用。#import
* (http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx),从Visual C++ 5.0开始使用
* 的一个新指令，用来从一个规定的类型库创建VC++“智能指针”。它很强大，但由于与
* Microsoft Office应用程序协同操作时一般都有引用计数的问题，所以在此时经常不推荐使
* 用此功能。与Solution2.h/cpp中使用的直接调用API不同，智能指针使我们受益于早期后期
* 绑定对象的类型信息。#import将复杂的guid加入项目，并且COM API都被封装在#import指令
* 生成的自定义类中。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once


//
//   函数: AutomatePowerPointByImport(LPVOID)
//
//   功能: 使用#import指令智能指针的自动化PowerPoint
//
//   参数:
//      * lpParam - 当创建一个线程的时候使用lpParameter参数将线程数据传递给函数 
//      (http://msdn.microsoft.com/en-us/library/ms686736.aspx)
//
//   返回值: 返回值标识了函数的执行成功与否 
//
DWORD WINAPI AutomatePowerPointByImport(LPVOID lpParam);