/****************************** 模块头 ******************************\
* 模块名:  CppAutomatePowerPoint.cpp
* 项目名:  CppAutomatePowerPoint
* 版权 (c) Microsoft Corporation.
* 
* CppAutomatePowerPoint案例演示了怎样使用VC++代码来创建一个Microsoft 
* PowerPoint实例，创建一个演示文稿，添加一个新的幻灯片，向幻灯片中加入一些文本，
* 保存演示文稿，退出Microsoft PowerPoint并进行非托管的COM资源的清理。
* 
* 你可以使用VC++代码通过这里提供的三种基本方法实现自动化应用:
* 
* 1. 使用#import指令智能指针的自动化PowerPoint (Solution1.h/cpp)
* 2. 使用C++和COM API的自动化PowerPoint (Solution2.h/cpp)
* 3. 使用MFC创建自动化的PowerPoint (This is not covered in this sample)
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

#include "Solution1.h"		// 使用#import指令智能指针的自动化操作PowerPoint的案例 

#include "Solution2.h"		// 使用原始COM API来自动化操作PowerPoint的案例  

#include <locale.h>		// 区域设置，我们需要它使_putws输出支持中文

#pragma endregion


int wmain(int argc, wchar_t* argv[])
{
	// 使_putws输出支持中文
	setlocale( LC_ALL, "chs" );

	HANDLE hThread;

	// 演示了在一个独立的线程中使用#import指令智能指针自动化PowerPoint 
	hThread = CreateThread(NULL, 0, AutomatePowerPointByImport, NULL, 0, NULL);
	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	_putws(L"");

	// 演示了在一个独立的线程中使用C++和COM API自动化PowerPoint
	hThread = CreateThread(NULL, 0, AutomatePowerPointByCOMAPI, NULL, 0, NULL);
	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	return 0;
}


//
//   函数: GetModuleDirectory(LPWSTR, DWORD);
//
//   功能: 在此例中这是一个帮助函数。它可以获取当前进程中的可执行文件的目录的
//         完全限定路径。例如, "D:\Samples\"。
//
//   参数:
//      * pszDir - 一个指向缓冲区（存放当前进程中的可执行文件的目录的完全限定
//         路径）的指针。如果路径长度小于nSize参数指定的长度，则函数成功执行，
//	  路径作为一个非终止的字符串返回。
//      * nSize - 存放在缓冲区的lpFilename字符串长度。
//
//   返回值: 如果函数执行成功，返回值是被复制到缓冲区的字符串的长度，以字符计算，
//	  不包含NULL终止字符。如果缓冲区太小不足以保存目录名，函数返回0并设置最
//	  终错误为ERROR_INSUFFICIENT_BUFFER。如果函数执行失败, 返回值为0.可以调用
//	  GetLastError获得更多的错误信息。
//
DWORD GetModuleDirectory(LPWSTR pszDir, DWORD nSize)
{
	// 获得当前线程中执行文件的路径
	nSize = GetModuleFileName(NULL, pszDir, nSize);
	if (!nSize || GetLastError() == ERROR_INSUFFICIENT_BUFFER)
	{
		*pszDir = L'\0'; // 确认它的NULL终止字符
		return 0;
	}

	// 在运行时在文件路径中查找最后的反斜线。
    // 当我们发现它时, 截去后面的文件名部分并终止字符串。

    for (int i = nSize - 1; i >= 0; i--)
	{
        if (pszDir[i] == L'\\' || pszDir[i] == L'/')
		{
			pszDir[i + 1] = L'\0';
			nSize = i + 1;
            break;
		}
    }
	return nSize;
}