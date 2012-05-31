/****************************** 模块头 ******************************\
模块名:    Reg.h
项目:      CppShellExtContextMenuHandler
版权 (c) Microsoft Corporation.


该文件定义了在COM对象中重复使用的注册和反注册的方法

RegisterInprocServer -   注册COM组件到注册表中.
UnregisterInprocServer - 从注册表中反注册COM组件.
RegisterShellExtContextMenuHandler - 注册上下文菜单处理程序.
UnregisterShellExtContextMenuHandler - 反注册上下文菜单处理程序.

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
//   函数: RegisterInprocServer
//
//   目的: 注册COM组件到注册表中.
//
//   参数:
//   * pszModule - 包含组件的模块的路径
//   * clsid - 组件对象的Class ID 
//   * pszFriendlyName - 别名
//   * pszThreadModel - 线程模型
//
//   注意: 该函数将在注册表HKCR\CLSID\{<CLSID>} 下创建一个键
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
//   函数: UnregisterInprocServer
//
//   目的: 从注册表中反注册COM组件.
//
//   参数:
//   * clsid - 组件对象的Class ID
//
//   注意: 该函数将在注册表HKCR\CLSID\{<CLSID>} 下删除键
//
HRESULT UnregisterInprocServer(const CLSID& clsid);


//
//   函数: RegisterShellExtContextMenuHandler
//
//   目的: 注册上下文菜单处理程序.
//
//   参数:
//   * pszFileType - 上下文菜单处理程序关联的文件类型
//	   如, '*' 表示所有文件; '.txt' 所有文本文件. 该参数不能为空   
//   * clsid - 组件对象的Class ID
//   * pszFriendlyName - 别名
//
//   注意: 该函数将在注册表中增加下面的键值
//
//   HKCR
//   {
//      NoRemove <File Type>
//      {
//          NoRemove shellex
//          {
//              NoRemove ContextMenuHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//   }
//
HRESULT RegisterShellExtContextMenuHandler(
    PCWSTR pszFileType, const CLSID& clsid, PCWSTR pszFriendlyName);


//
//   函数: UnregisterShellExtContextMenuHandler
//
//   目的: 反注册上下文菜单处理程序.
//
//   参数:
//   * pszFileType - 上下文菜单处理程序关联的文件类型
//	   如, '*' 表示所有文件; '.txt' 所有文本文件. 该参数不能为空   
//   * clsid - 组件对象的Class ID
//
//   注意: 该函数将删除注册表中HKCR\<File Type>\shellex\ContextMenuHandlers下的 {<CLSID>}键
//
HRESULT UnregisterShellExtContextMenuHandler(
    PCWSTR pszFileType, const CLSID& clsid);