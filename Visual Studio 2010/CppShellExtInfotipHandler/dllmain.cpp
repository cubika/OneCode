/******************************模块头******************************\
模块名称:  dllmain.cpp
项目名称:      CppShellExtContextMenuHandler
版权 (c) Microsoft Corporation.

该文件执行COM DLL需要的几个函数： DllMain,  DllGetClassObject, DllCanUnloadNow, 
DllRegisterServer, DllUnregisterServer　。

DllGetClassObject 通过反射调用在ClassFactory.h/cpp定义的类和查询指定的接口 。

DllCanUnloadNow 判断我们是否能从内存中加载组件.

DllRegisterServer 注册COM服务和信息提示处理方法。信息提示处理方法是包含所有的.cpp文件 


DllUnregisterServer 停止COM服务和信息提示处理方法. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include <windows.h>
#include <Guiddef.h>
#include "ClassFactory.h"           // 引入ClassFactory.h头文件
#include "Reg.h"


// {A67511FE-371A-498D-9372-A27FDA58BE60}
const CLSID CLSID_FileInfotipExt = 
{ 0xA67511FE, 0x371A, 0x498D, { 0x93, 0x72, 0xA2, 0x7F, 0xDA, 0x58, 0xBE, 0x60 } };


HINSTANCE   g_hInst     = NULL;
long        g_cDllRef   = 0;


BOOL APIENTRY DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
        //在这个DLL模块中使用这个接口，是用来得到注册组件DLL的路径.
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
//   函数名称: DllGetClassObject
//
//   作用:创建一个类并查询指定接口.
//
//   参数:
//   * rclsid -　联合了修改过的数据和代码的ＣＬＳＩＤ .
//   * riid -　接口的一个参考标识符，调用者用来和类对象建立关系。 
//    　
//   * ppv -　指针变量的地址，指向请求ｒｉｉｄ接口的指针 .
//           如果成功返回，*ppv包含请求的接口指针，如果出现错误，则接口指针为空 
//    
//
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, void **ppv)
{
    HRESULT hr = CLASS_E_CLASSNOTAVAILABLE;

    if (IsEqualCLSID(CLSID_FileInfotipExt, rclsid))
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
//   函数名称: DllCanUnloadNow
//
//   作用: 判断我们是否能从内存中加载组件.
//
//   注意: 当参考计数为0时，我们不能从内存中加载组件

STDAPI DllCanUnloadNow(void)
{
    return g_cDllRef > 0 ? S_FALSE : S_OK;
}


//
//   函数名称: DllRegisterServer
//
//   作用:注册COM服务和上下文菜单处理程序.
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

    //注册组件.
    hr = RegisterInprocServer(szModule, CLSID_FileInfotipExt, 
        L"CppShellExtInfotipHandler.FileInfotipExt Class", 
        L"Apartment");
    if (SUCCEEDED(hr))
    {
        //注册信息提示处理方法.信息提示处理程序包含整个.cpp文件
        hr = RegisterShellExtInfotipHandler(L".cpp", CLSID_FileInfotipExt);
    }

    return hr;
}


//
//   函数名称: DllUnregisterServer
//
//   作用: 停止COM服务和上下文菜单处理程序.
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

    // 停止使用组件.
    hr = UnregisterInprocServer(CLSID_FileInfotipExt);
    if (SUCCEEDED(hr))
    {
        //停止使用上下文菜单处理程序.
        hr = UnregisterShellExtInfotipHandler(L".cpp");
    }

    return hr;
}