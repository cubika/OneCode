/********************************** 模块头 *********************************\
* 模块名:      ATLExeCOMServer.cpp
* 项目名:      ATLExeCOMServer
* 版权 (c) Microsoft Corporation.
* 
* ATL是旨在开发高效的，灵活的，轻量级的COM组件以及简化组件开发过程。
* ATLExeCOMServer提供了一个实现于可执行文件（EXE）中的进程外服务器对象。并且它
* 在一个单独的进程空间中运行。
* 
* ATLExeCOMServer.cpp实现了WinMain和定义了服务器模块。
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
#include "stdafx.h"
#include "resource.h"
#include "ATLExeCOMServer_i.h"
#include "dlldatax.h"
#pragma endregion


class CATLExeCOMServerModule : public CAtlExeModuleT< CATLExeCOMServerModule >
{
public :
    DECLARE_LIBID(LIBID_ATLExeCOMServerLib)
    DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ATLEXECOMSERVER, "{B711EE75-FDA3-4B0E-BFAA-67CB305D62AE}")
};

CATLExeCOMServerModule _AtlModule;



//
extern "C" int WINAPI _tWinMain(HINSTANCE /*hInstance*/, HINSTANCE /*hPrevInstance*/, 
                                LPTSTR /*lpCmdLine*/, int nShowCmd)
{
    return _AtlModule.WinMain(nShowCmd);
}

