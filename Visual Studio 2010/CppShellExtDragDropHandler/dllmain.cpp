/****************************** 模块头******************************\
模块名:  dllmain.cpp
项目:      CppShellExtPropSheetHandler
版权 (c) Microsoft Corporation.

这个文件实现了DllMain,和 DllGetClassObject, DllCanUnloadNow, 
DllRegisterServer, DllUnregisterServer函数，这些函数对于COM DLL是必要的。

DllGetClassObject 调用类工厂，这个类工厂定义在ClassFactory.h/cpp 和指定的接口。

DllCanUnloadNow检查，如果我们可以从内存卸载组件。.

COM服务器的DllRegisterServer寄存器和鼠标拖放处理程序通过调用定义Reg.h/cpp的函数来注册的。
改鼠标拖放处理程序与.exe类文件有联系。


DllUnregisterServer没有注册在  COM 服务器和鼠标拖放处理程序. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include <windows.h>
#include <Guiddef.h>
#include "ClassFactory.h"           // 这个类工厂
#include "Reg.h"


// {F574437A-F944-4350-B7E9-95B6C7008029}
const CLSID CLSID_FileDragDropExt = 
{ 0xF574437A, 0xF944, 0x4350, { 0xB7, 0xE9, 0x95, 0xB6, 0xC7, 0x00, 0x80, 0x29 } };


HINSTANCE   g_hInst     = NULL;
long        g_cDllRef   = 0;


BOOL APIENTRY DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
        // 保存此 DLL 模块的实例，我们将用它来获取此DLL的路径以注册组件.
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
//   目的：创建类工厂，查询特定的接口。

//
//   参数:
//   * rclsid - CLSID将联系正确的数据和代码.
//   * riid - 接口的标识的参考，这个接口的调用者是用来与类对象的沟通。
//   * ppv -指针变量的地址，这个变量接收访问riid内的接口指针。成功返回后， *ppv包括这个请求的接口指针。如果出现错误，该接口指针
//   为NULL。

      

STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, void **ppv)
{
    HRESULT hr = CLASS_E_CLASSNOTAVAILABLE;

    if (IsEqualCLSID(CLSID_FileDragDropExt, rclsid))
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
//  函数: DllCanUnloadNow
//
//   目的:检查 如果我们能从内存中卸载组件.
//
//   注意: 当引用的数量是零时这个组件能从内存中卸载（没有任何人一直可以使用这个组件）
//
STDAPI DllCanUnloadNow(void)
{
    return g_cDllRef > 0 ? S_FALSE : S_OK;
}


//
// 函数: DllRegisterServer
//
//  目标: 注册 COM 服务 和鼠标拖放处理程序.
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

    // 注册这个组件.
    hr = RegisterInprocServer(szModule, CLSID_FileDragDropExt, 
        L"CppShellExtDragDropHandler.FileDragDropExt Class", 
        L"Apartment");
    if (SUCCEEDED(hr))
    {
        // 注册这个鼠标处理程序. 
        hr = RegisterShellExtDragDropHandler(CLSID_FileDragDropExt, 
            L"CppShellExtDragDropHandler.FileDragDropExt");
    }

    return hr;
}


//
//   函数: DllUnregisterServer
//
//   目标:没有注册 COM 服务和鼠标拖放处理程序.
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
    hr = UnregisterInprocServer(CLSID_FileDragDropExt);
    if (SUCCEEDED(hr))
    {
        // 注销鼠标处理程序.
        hr = UnregisterShellExtDragDropHandler(CLSID_FileDragDropExt);
    }

    return hr;
}