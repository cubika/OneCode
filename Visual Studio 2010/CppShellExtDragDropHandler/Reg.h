/****************************** 模块头 ******************************\
模块名:  Reg.h
项目:      CppShellExtPropSheetHandler
版权(c) Microsoft Corporation.

这个文件实现了在进程内COM组件和Shell鼠标拖放处理程序的注册和注销的重复使用的函数。

RegisterInprocServer - 在注册表中注册进程内的组件.
UnregisterInprocServer - 在注册表中注销进程内的组件.
RegisterShellExtDragDropHandler - 注册鼠标拖放处理程序.
UnregisterShellExtDragDropHandler - 注销鼠标拖放处理程序.

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
//   目标: 在注册表中注册进程内的组件.
//
//   参数:
//   * pszModule - 路径的模块包含的组件

//   * clsid -组件的类ID
//   * pszFriendlyName -友好的名称
//   * pszThreadModel - 线程块
//
//   注意: 在注册表中创建HKCR\CLSID\{<CLSID>}键值。
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
//   目录: 在注册表中注销这个进程组件.
//
//   参数:
//   * clsid - 组件的类ID。
//
//   注意: 在注册表中删除 HKCR\CLSID\{<CLSID>}键值。
//
HRESULT UnregisterInprocServer(const CLSID& clsid);


//
//   函数: RegisterShellExtDragDropHandler
//
// 目标: 注册鼠标拖放处理程序.
//
//   参数:
//   * clsid -组建的类ID。
//   * pszFriendlyName - 友好名称
//
//   注意: 在注册表中创建下列键值。.
//
//   HKCR
//   {
//      NoRemove Directory
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//      NoRemove Folder
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//      NoRemove Drive
//      {
//          NoRemove shellex
//          {
//              NoRemove DragDropHandlers
//              {
//                  {<CLSID>} = s '<Friendly Name>'
//              }
//          }
//      }
//   }
//
HRESULT RegisterShellExtDragDropHandler(const CLSID& clsid, PCWSTR pszFriendlyName);


//
//  函数: UnregisterShellExtDragDropHandler
//
//   目标: 注销鼠标处理程序.
//
//   参数:
//   * clsid -组件的类ID。
//
//   注意: 这个函数移除了在注册表中的HKCR\Directory\shellex\DragDropHandlers, 
//   HKCR\Folder\shellex\DragDropHandlers 
//   HKCR\Drive\shellex\DragDropHandlers 下的 {<CLSID>}键值。
//
HRESULT UnregisterShellExtDragDropHandler(const CLSID& clsid);