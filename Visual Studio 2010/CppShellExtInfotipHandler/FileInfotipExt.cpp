/******************************模块头 ******************************\
模块名称:  FileInfotipExt.cpp
项目名称:      CppShellExtInfotipHandler
版权 (c) Microsoft Corporation.

该代码块演示了如何用C++创建一个Shell 信息提示处理程序。信息提示处理程序是一
个Shell扩展处理程序，当鼠标悬停在文件上时，会弹出文本提示信息。这是一个非常灵
活的信息定制提示处理程序。另外的一种方法是指定一个固定的字符串或者文件的
属性列表显示提示信息（详细信息请点击以下链接：
http://msdn.microsoft.com/en-us/library/cc144067.aspx)

该例子是显示一个.cpp文件的定制的提示信息。
当你的鼠标悬停在一个.cpp文件上时，你会看到以下文本信息：
    File: <File path, e.g. D:\Test.cpp>
    Lines: <Line number, e.g. 123 or N/A>
    -  通过CppShellExtInfotipHandler显示信息提示信息


This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "FileInfotipExt.h"
#include <fstream>
#include <string>
#include <strsafe.h>
using namespace std;


FileInfotipExt::FileInfotipExt() : m_cRef(1)
{
}

FileInfotipExt::~FileInfotipExt()
{
}


#pragma region IUnknown

// 查询组件支持的接口.
IFACEMETHODIMP FileInfotipExt::QueryInterface(REFIID riid, void **ppv)
{
    HRESULT hr = S_OK;

    if (IsEqualIID(IID_IUnknown, riid) || 
        IsEqualIID(IID_IQueryInfo, riid))
    {
        *ppv = static_cast<IQueryInfo *>(this);
    }
    else if (IsEqualIID(IID_IPersistFile, riid))
    {
        *ppv = static_cast<IPersistFile *>(this);
    }
    else
    {
        hr = E_NOINTERFACE;
        *ppv = NULL;
    }

    if (*ppv)
    {
        AddRef();
    }

    return hr;
}

//增加在项目中使用接口的计数.
IFACEMETHODIMP_(ULONG) FileInfotipExt::AddRef()
{
    return InterlockedIncrement(&m_cRef);
}

//减少在项目中使用接口的计数
IFACEMETHODIMP_(ULONG) FileInfotipExt::Release()
{
    ULONG cRef = InterlockedDecrement(&m_cRef);
    if (0 == cRef)
    {
        delete this;
    }

    return cRef;
}

#pragma endregion


#pragma region IPersistFile

IFACEMETHODIMP FileInfotipExt::GetClassID(CLSID *pClassID)
{
    return E_NOTIMPL;
}

IFACEMETHODIMP FileInfotipExt::IsDirty(void)
{
    return E_NOTIMPL;
}

IFACEMETHODIMP FileInfotipExt::Load(LPCOLESTR pszFileName, DWORD dwMode)
{
    // pszFileName 包含被打开文件的据对路径.
    return StringCchCopy(
        m_szSelectedFile, 
        ARRAYSIZE(m_szSelectedFile), 
        pszFileName);
}

IFACEMETHODIMP FileInfotipExt::Save(LPCOLESTR pszFileName, BOOL fRemember)
{
    return E_NOTIMPL;
}

IFACEMETHODIMP FileInfotipExt::SaveCompleted(LPCOLESTR pszFileName)
{
    return E_NOTIMPL;
}

IFACEMETHODIMP FileInfotipExt::GetCurFile(LPOLESTR *ppszFileName)
{
    return E_NOTIMPL;
}

#pragma endregion


#pragma region IQueryInfo

IFACEMETHODIMP FileInfotipExt::GetInfoTip(DWORD dwFlags, LPWSTR *ppwszTip)
{
    // ppwszTip是一个Unicode字符串的指针，它接收端的字符串指针地址。
	//实施这一方法的扩展，必须通过调用CoTaskMemAlloc为ppwszTip分配内存。
	//当信息提示是不再需要时，shell知道要释放内存。
    const int cch = MAX_PATH + 512;
    *ppwszTip = static_cast<LPWSTR>(CoTaskMemAlloc(cch * sizeof(wchar_t)));
    if (*ppwszTip == NULL)
    {
        return E_OUTOFMEMORY;
    }

    // 先准备信息提示文本.本例子的信息提示文本时由文件路径和代码行数 
    wchar_t szLineNum[50] = L"N/A";
    {
        wifstream infile(m_szSelectedFile);
        if (infile.good())
        {
            __int64 lineNum = 0;
            wstring line;
            while (getline(infile, line))
            {
                lineNum++;
            }
            // 忽略返回值，因为这个调用不会失败
            StringCchPrintf(szLineNum, ARRAYSIZE(szLineNum), L"%I64i", lineNum);
        }
    }

    HRESULT hr = StringCchPrintf(*ppwszTip, cch, 
        L"File: %s\nLines: %s\n- 通过 CppShellExtInfotipHandler 显示提示信息", 
        m_szSelectedFile, szLineNum);
    if (FAILED(hr))
    {
        CoTaskMemFree(*ppwszTip);
    }

    return hr;
}

IFACEMETHODIMP FileInfotipExt::GetInfoFlags(DWORD *pdwFlags)
{
    return E_NOTIMPL;
}

#pragma endregion