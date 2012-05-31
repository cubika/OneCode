/****************************** 模块头 ******************************\
模块名称:  FileInfotipExt.h
项目名称:      CppShellExtInfotipHandler
版权 (c) Microsoft Corporation.

该代码块演示了如何用C++创建一个Shell 信息提示处理程序。信息提示处理程序是一
个Shell扩展处理程序，当鼠标悬停在文件上时，会弹出文本提示信息。这是一个非常灵
活的信息定制提示处理程序。另外的一种方法是指定一个固定的字符串或者文件的
属性列表显示提示信息（详细信息请点击以下链接：
http://msdn.microsoft.com/en-us/library/cc144105.aspx)

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

#pragma once

#include <windows.h>
#include <shlobj.h>      

class FileInfotipExt : IPersistFile, IQueryInfo
{
public:
    // IUnknown
    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
    IFACEMETHODIMP_(ULONG) AddRef();
    IFACEMETHODIMP_(ULONG) Release();

    // IPersistFile
    IFACEMETHODIMP GetClassID(CLSID *pClassID);
    IFACEMETHODIMP IsDirty(void);
    IFACEMETHODIMP Load(LPCOLESTR pszFileName, DWORD dwMode);
    IFACEMETHODIMP Save(LPCOLESTR pszFileName, BOOL fRemember);
    IFACEMETHODIMP SaveCompleted(LPCOLESTR pszFileName);
	IFACEMETHODIMP GetCurFile(LPOLESTR *ppszFileName);

    // IQueryInfo
    IFACEMETHODIMP GetInfoTip(DWORD dwFlags, LPWSTR *ppwszTip);
    IFACEMETHODIMP GetInfoFlags(DWORD *pdwFlags);
	
    FileInfotipExt();

protected:
    ~FileInfotipExt();

private:
    // 参考计数.
    long m_cRef;

    //被选择文件的名字.
    wchar_t m_szSelectedFile[MAX_PATH];
};