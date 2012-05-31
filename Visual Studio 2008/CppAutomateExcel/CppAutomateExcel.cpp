/****************************** 模块头 *************************************\
* 模块名:    CppAutomateExcel.cpp
* 项目名:    CppAutomateExcel
* 版权(c)    Microsoft Corporation.
* 
* CppAutomateExcel案例阐述了通过VC++代码生成Microsoft Excel实例、 填充数据到指定
* 区域、创建，保存工作簿以及关闭Excel应用程序并释放非托管COM资源的相关过程。
* 
* 可以通过下面三种基本方式构建VC++自动化代码：
* 
* 1. 通过使用#import指令和智能指针自动化Excel (Solution1.h/cpp)
* 2. 通过使用C++和COM API 自动化Excel (Solution2.h/cpp)
* 3. 利用MFC方式自动化Excel(该案例不涉及利用MFC 自动化Excel)
* 
* 该来源受微软授予的公共许可证约束。
* 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* 保留其它所有权。
* 
* 该代码和资料不提供任何形式，明示或暗示的担保，包括但不限于适销特定用途的暗示性和
* 适用性的保证。
\***************************************************************************/


#pragma region Includes
#include <stdio.h>
#include <windows.h>

#include "Solution1.h"		// 使用#import指令和智能指针自动化Excel示例
#include "Solution2.h"		// 使用原始的COM API方式自动化Excel示例 
#pragma endregion


int wmain(int argc, wchar_t* argv[])
{
	HANDLE hThread;

	// 阐述了在独立线程中使用#import指令和智能指针自动化Excel
	hThread = CreateThread(NULL, 0, AutomateExcelByImport, NULL, 0, NULL);
	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	_putws(L"");

	// 阐述了在独立线程中使用C++和COM API自动化Excel
	hThread = CreateThread(NULL, 0, AutomateExcelByCOMAPI, NULL, 0, NULL);
	WaitForSingleObject(hThread, INFINITE);
	CloseHandle(hThread);

	return 0;
}


//
// 函数: GetModuleDirectory(LPWSTR, DWORD);
//
// 作用: 
// 这是该示例中的辅助功能函数。它为目录获取包含当前进程可执行文件的
// 完全限定路径。 例如, "D:\Samples\".
//
// 参数:
// * pszDir - 指向缓冲的指针，它为目录获取包含当前进程可执行文件的
// 完全限定路径。 若该路径长度小于参数nSize指定的大小，函数调用成功，
// 路径作为一个空终止字符串被返回。
// * nSize - 以字符形式计算时lpFilename 的缓冲大小。
//
// 返回值: 
// 若函数调用成功，返回值是以字符形式复制到缓冲区的字符串长度，
// 不包括空终止字符。若缓冲区太小而无法保存目录名，函数返回0并设置
// 错误信息到ERROR_INSUFFICIENT_BUFFER。若函数调用失败, 返回值为0.
// 若要获取扩展的错误信息，则调用GetLastError。
//
DWORD GetModuleDirectory(LPWSTR pszDir, DWORD nSize)
{
	// 获取当前进程可执行文件的路径
	nSize = GetModuleFileName(NULL, pszDir, nSize);
	if (!nSize || GetLastError() == ERROR_INSUFFICIENT_BUFFER)
	{
		*pszDir = L'\0'; //确保空终止
		return 0;
	}

	// 运行并搜寻文件路径中的最后一个("/"或"\\)"
    // 找到则将NULL赋给它，从而清空后面跟着的文件名部分

    for (int i = nSize - 1; i >= 0; i--)
	{
        if (pszDir[i] == L'\\' || pszDir[i] == L'/')
		{
			pszDir[i + 1] = L'\0';
			nSize = i + 1;
            break;
		}
    }
	return nSize;
}


//
// 函数: SafeArrayPutName(SAFEARRAY*, long, PCWSTR, PCWSTR);
//
// 作用: 
// 这是该示例中的辅助功能函数。它将用户名(姓,名)
// 赋值给两维的安全数组。数组形式如下：
//
//      John   Smith
//      Tom    Brown
//      Sue    Thomas
// 
// 第1列的值由pszFirstName确定. 第2列的值则由pszLastName确定。
// SafeArrayPutName用于以行的方式向数组添加项(pszFirstName pszLastName),
// 这里数组中的行通过索引参数指定。
//
// 参数:
// * psa - 指向由SafeArrayCreate生成的数组的指针。
// * index - 数组中的一行，即姓名 (姓，名)的下标。
// 例如：两维数组中的第一维
// * pszFirstName - 姓.
// * pszLastName - 名.
//
// 返回值: HRESULT的值用于指示函数是否调用成功。
//
HRESULT SafeArrayPutName(SAFEARRAY* psa, long index, PCWSTR pszFirstName, 
						 PCWSTR pszLastName)
{
	HRESULT hr;

	// 设置姓的值
	long indices1[] = { index, 1 };
	VARIANT vtFirstName;
	vtFirstName.vt = VT_BSTR;
	vtFirstName.bstrVal = SysAllocString(pszFirstName);
	// 将VARIANT复制到数组
	hr = SafeArrayPutElement(psa, indices1, (void*)&vtFirstName);
	VariantClear(&vtFirstName);

	if (SUCCEEDED(hr))
	{
		// 然后设置名的值
		long indices2[] = { index, 2 };
		VARIANT vtLastName;
		vtLastName.vt = VT_BSTR;
		vtLastName.bstrVal = SysAllocString(pszLastName);
		hr = SafeArrayPutElement(psa, indices2, (void*)&vtLastName);
		VariantClear(&vtLastName);
	}

	return hr;
}