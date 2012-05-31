/****************************** 模块头 ******************************\
* 模块名:  Solution2.h
* 项目名:  CppAutomatePowerPoint
* 版权 (c) Microsoft Corporation.
* 
* Solution2.h/cpp中的代码演示了使用C/C++和COM API自动化操作PowerPoint。原始的自动化
* 非常难操作，但是有时又需要避免使用MFC带来的开销和#import的问题。基本上，你在工作中
* 使用的是一些这样的API，如CoCreateInstance()和COM接口（如IDispatch和IUnknown）。
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
//   函数: AutomatePowerPointByCOMAPI(LPVOID)
//
//   功能: 使用C++和COM API的自动化PowerPoint
//
//   参数:
//      * lpParam - 当创建一个线程的时候使用lpParameter参数将线程数据传递给函数 
//      (http://msdn.microsoft.com/en-us/library/ms686736.aspx)
//
//   返回值: 返回值标识了函数的执行成功与否 
//
DWORD WINAPI AutomatePowerPointByCOMAPI(LPVOID lpParam);