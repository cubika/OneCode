/****************************** 模块头 *************************************\
* 模块名:    Solution1.cpp
* 项目名:    CppAutomateExcel
* 版权(c)    Microsoft Corporation.
* 
* Solution1.h及Solution1.cpp文件中的代码阐述了如何使用#import指令自动化Excel的方
* 法。#import(http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx)作为伴随
* VC++5.0出现的一种新的指令，能够通过指定类型库生成VC++“智能指针”。 虽然这种方式功
* 能强大，但并不推荐使用，因为将其用于Microsoft Office应用程序时，往往将导致引用计
* 数问题。与Solution2.h及Solution2.cpp中的直接API方法不同，智能指针使我们能够受益
* 于类型信息，从而对对象进行早期/后期绑定。#import指令添加无序的Guid到项目，而COM 
* API则被封装到由#import 指令生成的自定义类中。
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
#include "Solution1.h"
#pragma endregion


#pragma region Import the type libraries

#import "libid:2DF8D04C-5BFA-101B-BDE5-00AA0044DE52" \
	rename("RGB", "MSORGB") \
	rename("DocumentProperties", "MSODocumentProperties")
// [-或者-]
// #import "C:\\Program Files\\Common Files\\Microsoft Shared\\OFFICE12\\MSO.DLL" \
// rename("RGB", "MSORGB") \
// rename("DocumentProperties", "MSODocumentProperties")

using namespace Office;

#import "libid:0002E157-0000-0000-C000-000000000046"
// [-或者-]
// #import "C:\\Program Files\\Common Files\\Microsoft Shared\\VBA\\VBA6\\VBE6EXT.OLB"

using namespace VBIDE;

#import "libid:00020813-0000-0000-C000-000000000046" \
	rename("DialogBox", "ExcelDialogBox") \
	rename("RGB", "ExcelRGB") \
	rename("CopyFile", "ExcelCopyFile") \
	rename("ReplaceText", "ExcelReplaceText") \
	no_auto_exclude
// [-或者-]
// #import "C:\\Program Files\\Microsoft Office\\Office12\\EXCEL.EXE" \
// rename("DialogBox", "ExcelDialogBox") \
// rename("RGB", "ExcelRGB") \
// rename("CopyFile", "ExcelCopyFile") \
// rename("ReplaceText", "ExcelReplaceText") \
// no_auto_exclude

#pragma endregion


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

//
DWORD GetModuleDirectory(LPWSTR pszDir, DWORD nSize);


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
						 PCWSTR pszLastName);


//
// 函数: AutomateExcelByImport(LPVOID)
//
// 作用: 使用#import指令和智能指针自动化Microsot Excel。

// 
DWORD WINAPI AutomateExcelByImport(LPVOID lpParam)
{
	// 在当前线程上初始化COM类库并通过调用CoInitializeEx或CoInitialize方法确认
    // 并发模型为单线程单元(STA)。
	// [-或者-] CoInitialize(NULL);
	// [-或者-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	try
	{

		/////////////////////////////////////////////////////////////////////
	    // 使用#import指令和智能指针创建Excel.Application COM对象

		// 选择1) 使用智能指针构造函数创建对象。
		// _ApplicationPtr 为原接口名称, _Application加上"Ptr"后缀。

		// Excel::_ApplicationPtr spXlApp(
		//	__uuidof(Excel::Application)	// 组件的CLSID 
		//	);

		// [-或者-]

		// 选择2) 使用智能指针函数CreateInstance创建对象
		Excel::_ApplicationPtr spXlApp;
		HRESULT hr = spXlApp.CreateInstance(__uuidof(Excel::Application));
		if (FAILED(hr))
		{
			wprintf(L"CreateInstance failed w/err 0x%08lx\n", hr);
			return 1;
		}

		_putws(L"Excel.Application is started");


		/////////////////////////////////////////////////////////////////////
		// 令Excel不可见(如： Application.Visible = 0)

		spXlApp->Visible[0] = VARIANT_FALSE;


		/////////////////////////////////////////////////////////////////////
		// 创建一个新的工作簿 (如： Application.Workbooks.Add)

		Excel::WorkbooksPtr spXlBooks = spXlApp->Workbooks;
		Excel::_WorkbookPtr spXlBook = spXlBooks->Add();

		_putws(L"A new workbook is created");


		/////////////////////////////////////////////////////////////////////
		// 获取处于活动状态的工作表并为其设置名称

		Excel::_WorksheetPtr spXlSheet = spXlBook->ActiveSheet;
		spXlSheet->Name = _bstr_t(L"Report");
		_putws(L"The active worksheet is renamed as Report");


		/////////////////////////////////////////////////////////////////////
		// 填充数据到工作表单元

		_putws(L"Filling data into the worksheet ...");

		// 为用户姓名构建一个5*2的安全数组
		VARIANT saNames;
		saNames.vt = VT_ARRAY | VT_VARIANT;
		{
			SAFEARRAYBOUND sab[2];
			sab[0].lLbound = 1; sab[0].cElements = 5;
			sab[1].lLbound = 1; sab[1].cElements = 2;
			saNames.parray = SafeArrayCreate(VT_VARIANT, 2, sab);

			SafeArrayPutName(saNames.parray, 1, L"John", L"Smith");
			SafeArrayPutName(saNames.parray, 2, L"Tom", L"Brown");
			SafeArrayPutName(saNames.parray, 3, L"Sue", L"Thomas");
			SafeArrayPutName(saNames.parray, 4, L"Jane", L"Jones");
			SafeArrayPutName(saNames.parray, 5, L"Adam", L"Johnson");
		}

		 // 将数组值填充到A2:B6区域(姓与名)
		// 获取区域A2:B6的Range对象

		param.vt = VT_BSTR;
		param.bstrVal = SysAllocString(L"A2:B6");
		Excel::RangePtr spXlRange = spXlSheet->Range[param];

		spXlRange->Value2 = saNames;

		// 清除数组
		VariantClear(&saNames);


		/////////////////////////////////////////////////////////////////////
		// 将工作簿保存为xlsx文件并关闭
		

		_putws(L"Save and close the workbook");

		// 确定文件名

		// 获取当前exe的目录
		wchar_t szFileName[MAX_PATH];
		if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
		{
			_putws(L"GetModuleDirectory failed");
			return 1;
		}

		// 将"Sample1.xlsx" 合并到目录
		wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample1.xlsx", 12);

		// 将空终止字符串转换为BSTR
		variant_t vtFileName(szFileName);

		spXlBook->SaveAs(vtFileName, Excel::xlOpenXMLWorkbook, vtMissing, 
			vtMissing, vtMissing, vtMissing, Excel::xlNoChange);

		spXlBook->Close();


		/////////////////////////////////////////////////////////////////////
		// 退出Excel应用程序(如：Application.Quit)
		 

		_putws(L"Quit the Excel application");
		spXlApp->Quit();


		/////////////////////////////////////////////////////////////////////
		// 释放COM对象
		// 

		// 为智能指针释放不必要的引用
		// ...
		// spXlApp.Release();
		// ...

	}
	catch (_com_error &err)
	{
		wprintf(L"Excel throws the error: %s\n", err.ErrorMessage());
		wprintf(L"Description: %s\n", (LPCWSTR) err.Description());
	}

	// 为该线程撤销COM
	CoUninitialize();

	return 0;
}