/******************************模块头 ******************************\
模块名称:  Reg.h
项目名称:      CppShellExtInfotipHandler
版权 (c) Microsoft Corporation.

该模块声明了以下几个可重用使用的辅助函数：
RegisterInprocServer - 创建注册表信息.
UnregisterInprocServer - 删除注册表信息.
RegisterShellExtInfotipHandler - 创建信息提示处理功能.
UnregisterShellExtInfotipHandler -  删除信息提示功能.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include <windows.h>


//
//   函数名称: RegisterInprocServer
//
//   作用:该组件在项目中创建注册表信息.
//
//   参数:
//   * pszModule - 包含该组件模块的路径
//   * clsid - 该组件的类ID
//   * pszFriendlyName - 友元名称
//   * pszThreadModel - 线程模型
//
//   注意: 该方法在注册表中创建了HKCR\CLSID\{<CLSID>}注册项.
// 
//   HKCR
//   {
//      NoRemove CLSID
//      {
//          ForceRemove {<CLSID>} = s '<Friendly Name>'
//          {
//              InprocServer32 = s '%MODULE%'
//              {
//                  val ThreadingModel = s '<Thread Model>'
//              }
//          }
//      }
//   }
//
HRESULT RegisterInprocServer(PCWSTR pszModule, const CLSID& clsid, 
    PCWSTR pszFriendlyName, PCWSTR pszThreadModel);


//
//   函数名称: UnregisterInprocServer
//
//   作用: 删除该组件在项目中创建的注册表信息.
//
//   参数:
//   * clsid - 该组件的类ID
//
//   注意: 该函数在注册表中删除了 HKCR\CLSID\{<CLSID>} 信息.
//
HRESULT UnregisterInprocServer(const CLSID& clsid);


//
//   函数名称: RegisterShellExtInfotipHandler
//
//   作用: 注册信息提示处理功能.
//
//   参数:
//   * pszFileType - 要处理文件的类型，如“*”代表所有的文件类型， 
//                   “.txt”代表.txt文件类型。参数不能为空
//   * clsid - 该组件的类ID
//
//   注意:该方法在注册表中创建了以下注册信息。
//
//   HKCR
//   {
//      NoRemove <File Type>
//      {
//          NoRemove shellex
//          {
//              {00021500-0000-0000-C000-000000000046} = s '{<CLSID>}'
//          }
//      }
//   }
//
HRESULT RegisterShellExtInfotipHandler(PCWSTR pszFileType, const CLSID& clsid);


//
//   函数名称: UnregisterShellExtInfotipHandler
//
//   作用: 删除信息提示处理功能.
//
//   参数:
//   * pszFileType - T要处理文件的类型，如“*”代表所有的文件类型， 
//                   “.txt”代表.txt文件类型。参数不能为空
//
//   注意: 该方法在注册表中删除了以下注册表信息
//   HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}.
//
HRESULT UnregisterShellExtInfotipHandler(PCWSTR pszFileType);