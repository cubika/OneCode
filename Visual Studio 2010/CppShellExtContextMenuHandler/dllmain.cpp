/****************************** 模块头 ******************************\
*模块名:  dllmain.cpp
*项目名:  CppShellExtContextMenuHandler
*版权(c)  Microsoft Corporation.


该文件实现了DllMain,DllGetClassObject,DllCanUnloadNow,DllRegisterServer,DllUnregisterServer这些COM DLL的必备函数。

DllGetClassObject 调用 ClassFactory.h/cpp 和查询，以特定的接口中定义的类工厂。

DllCanUnloadNow 检查是否可以从内存中卸载该组件

DllRegisterServer 注册 COM 服务器和上下文菜单处理程序在注册表中实施通过调用 Reg.h/cpp 中定义的 helper 函数。上下文菜单处理程序是与.cpp 文件类关联
DllUnregisterServer 注销 COM 服务器和上下文菜单处理程序。

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include <windows.h>
#include <Guiddef.h>
#include "ClassFactory.h"           // For the class factory
#include "Reg.h"


// {BFD98515-CD74-48A4-98E2-13D209E3EE4F}
const CLSID CLSID_FileContextMenuExt = 
{ 0xBFD98515, 0xCD74, 0x48A4, { 0x98, 0xE2, 0x13, 0xD2, 0x09, 0xE3, 0xEE, 0x4F } };


HINSTANCE   g_hInst     = NULL;
long        g_cDllRef   = 0;


BOOL APIENTRY DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
        // 保存此 DLL 模块的实例, 我们将使用它来获取注册组件 DLL 的路径
        g_hInst = hModule;
        DisableThreadLibraryCalls(hModule);
        break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}


//
//   函数: DllGetClassObject
//
//   目的: 创建类工厂查询特定接口.
//
//   参数:
//   * rclsid - 将代码与正确的数据相关联的 CLSID.
//   * riid - 对调用方是与类对象进行通信所使用的接口的标识符的引用.
//   * ppv - 收到请求的接口指针的指针变量的地址，在成功返回时 *ppv包含请求的接口指针，发生错误的接口指针为空
// 
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, void **ppv)
{
    HRESULT hr = CLASS_E_CLASSNOTAVAILABLE;

    if (IsEqualCLSID(CLSID_FileContextMenuExt, rclsid))
    {
        hr = E_OUTOFMEMORY;

        ClassFactory *pClassFactory = new ClassFactory();
        if (pClassFactory)
        {
            hr = pClassFactory->QueryInterface(riid, ppv);
            pClassFactory->Release();
        }
    }

    return hr;
}


//
//   函数: DllCanUnloadNow
//
//   目的: 检查是否可以从内存中删除组件对象.
//
//   注意: 组件在引用计算为0的情况下可以从内存中卸载（例如 没有人在使用该组件） 

// 
STDAPI DllCanUnloadNow(void)
{
    return g_cDllRef > 0 ? S_FALSE : S_OK;
}


//
//   函数: DllRegisterServer
//
//   目的: 注册 COM 服务器和上下文菜单处理程序.
// 
STDAPI DllRegisterServer(void)
{
    HRESULT hr;

    wchar_t szModule[MAX_PATH];
    if (GetModuleFileName(g_hInst, szModule, ARRAYSIZE(szModule)) == 0)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        return hr;
    }

    // 注册组件.
    hr = RegisterInprocServer(szModule, CLSID_FileContextMenuExt, 
        L"CppShellExtContextMenuHandler.FileContextMenuExt Class", 
        L"Apartment");
    if (SUCCEEDED(hr))
    {
        // 注册上下文菜单处理程序，将该程序关联.cpp类型文件
        hr = RegisterShellExtContextMenuHandler(L".cpp", 
            CLSID_FileContextMenuExt, 
            L"CppShellExtContextMenuHandler.FileContextMenuExt");
    }

    return hr;
}


//
//   函数: DllUnregisterServer
//
//   目的: 注销 COM 服务器和上下文菜单处理程序.
// 
STDAPI DllUnregisterServer(void)
{
    HRESULT hr = S_OK;

    wchar_t szModule[MAX_PATH];
    if (GetModuleFileName(g_hInst, szModule, ARRAYSIZE(szModule)) == 0)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        return hr;
    }

    // 注销组件.
    hr = UnregisterInprocServer(CLSID_FileContextMenuExt);
    if (SUCCEEDED(hr))
    {
        // 注销上下文菜单处理程序.
        hr = UnregisterShellExtContextMenuHandler(L".cpp", 
            CLSID_FileContextMenuExt);
    }

    return hr;
}