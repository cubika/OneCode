/****************************** 模块头 ******************************\
模块名:    PlatformDetector.h
项目名:        CppPlatformDetector
版权 (c) Microsoft Corporation.

执行帮助函数来检测平台. 

GetOSName - 获取当前操作系统的名字（例如："Microsoft Windows 7 企业版").

GetOSVersionString -  获得当前安装在操作系统中的平台标识符，版本和服务包的连接字符串.

Is64BitOS - 确定当前的操作系统是否是64位的系统.

Is64BitProcess -确定当前运行的进程或任意一个运行的进程是否是64位进程.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <Windows.h>

//
//   函数: GetOSName(PWSTR, DWORD)
//
//   目的: 获取当前操作系统的名字（例如："Microsoft Windows 7 企业版").
//
//   参数:
//   * pszName - 存放操作系统名的缓冲区. 
//   * cch - 由pszName所指向的缓冲区大小，以字符形式.
//
//   返回值: 如果函数成功返回TRUE.
//

BOOL GetOSName(PWSTR pszName, DWORD cch);


//
//   函数: GetOSVersionString(PWSTR, DWORD)
//
//   目的: 获得当前安装在操作系统中的平台标识符，版本和服务包的连接字符串.例如, 
//   "Microsoft Windows NT 6.1.7600.0 Workstation"
//
//   参数:
//   * pszVersionString - 用来存储操作系统版本字符串的缓冲区. 
//   * cch - pszVersionString所指向的缓冲区大小，以字符形式.
//
//   返回值: 函数成功返回TRUE.
//
BOOL GetOSVersionString(PWSTR pszVersionString, DWORD cch);

//
//   函数: Is64BitOS()
//
//   目的: 确定当前的操作系统是否是64位的系统.
//
//   返回值: 如果操作系统是64位的返回TRUE;否则，返回FALSE.
//
//

BOOL Is64BitOS();


//
//   函数: Is64BitProcess(void)
//   
//   目的: 确定当前运行的进程是否是64位进程.
//
//   返回值: 如果当前运行的进程是64位返回TRUE;否则，返回FALSE.
//

BOOL Is64BitProcess(void);


//
//   函数: Is64BitProcess(HANDLE)
//   
//   目的: 确定任意一个运行的进程是否是64位进程.
//
//   参数:
//   * hProcess - 进程句柄.
//
//   返回值: 如果给定的进程是64位返回TRUE;
//   否则,返回FALSE.
//
BOOL Is64BitProcess(HANDLE hProcess);