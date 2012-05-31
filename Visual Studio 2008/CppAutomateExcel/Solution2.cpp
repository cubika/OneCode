/****************************** 模块头 *************************************\
* 模块名:  Solution2.cpp
* 项目名:  CppAutomateExcel
* 版权(c)  Microsoft Corporation.
* 
* Solution2.h 和Solution2.cpp文件中的代码阐述了使用C/C++及COM API自动化Excel的过
* 程。这种原始的自动化方式虽然实现起来更加困难，但有时通过它能够有效避免利用MFC方式
* 或#import指令时所产生的开销及问题。基本上，较为常用的有API中的CoCreateInstance()
* 及COM中的IDispatch和IUnknown。
* 
* 该来源受微软授予的公共许可证约束。
* 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* 保留其它所有权。
* 
* 该代码和资料不提供任何形式，明示或暗示的担保， 包括但不限于适销特定用途的暗示性和
* 适用性的保证。
\***************************************************************************/


#pragma region Includes
#include <stdio.h>
#include <windows.h>
#include "Solution2.h"
#pragma endregion


//
// 函数: AutoWrap(int, VARIANT*, IDispatch*, LPOLESTR, int,...)
//
// 作用: 
// Automation辅助功能函数。AutoWrap()函数简化了直接使用IDispatch接口时
// 所涉及的大部分底层细节问题。用户可根据自己的情况实现相关调用。
// 需要注意的是，在传递多个参数时，必须以相反的顺序传递。
//
// 参数:
// * autoType - 可以取下列值之一: DISPATCH_PROPERTYGET, 
// DISPATCH_PROPERTYPUT, DISPATCH_PROPERTYPUTREF, DISPATCH_METHOD.
// * pvResult - 保存返回值的VARIANT类型
// * pDisp - IDispatch接口
// * ptName - 接口提供的属性/方法名
// * cArgs - 形参数量
//
// 返回值: HRESULT的值指示了函数是否调用成功
//
// 示例: 
// AutoWrap(DISPATCH_METHOD, NULL, pDisp, L"call", 2, parm[1], parm[0]);
//
HRESULT AutoWrap(int autoType, VARIANT *pvResult, IDispatch *pDisp, 
				 LPOLESTR ptName, int cArgs...) 
{
	// 启动变量参数列表
	va_list marker;
	va_start(marker, cArgs);

	if (!pDisp) 
	{
		_putws(L"NULL IDispatch passed to AutoWrap()");
		_exit(0);
		return E_INVALIDARG;
	}

	// 使用的变量
	DISPPARAMS dp = { NULL, NULL, 0, 0 };
	DISPID dispidNamed = DISPID_PROPERTYPUT;
	DISPID dispID;
	HRESULT hr;

	// 为姓名传递获取DISPID 
	hr = pDisp->GetIDsOfNames(IID_NULL, &ptName, 1, LOCALE_USER_DEFAULT, &dispID);
	if (FAILED(hr))
	{
		wprintf(L"IDispatch::GetIDsOfNames(\"%s\") failed w/err 0x%08lx\n", 
			ptName, hr);
		_exit(0);
		return hr;
	}

	// 为参数分配内存
	VARIANT *pArgs = new VARIANT[cArgs + 1];
	// 提取参数
	for(int i=0; i < cArgs; i++) 
	{
		pArgs[i] = va_arg(marker, VARIANT);
	}

	// 构建DISPPARAMS
	dp.cArgs = cArgs;
	dp.rgvarg = pArgs;

	// 处理特殊情况下的属性设定
	if (autoType & DISPATCH_PROPERTYPUT)
	{
		dp.cNamedArgs = 1;
		dp.rgdispidNamedArgs = &dispidNamed;
	}

	// 调用
	hr = pDisp->Invoke(dispID, IID_NULL, LOCALE_SYSTEM_DEFAULT,
		autoType, &dp, pvResult, NULL, NULL);
	if (FAILED(hr)) 
	{
		wprintf(L"IDispatch::Invoke(\"%s\"=%08lx) failed w/err 0x%08lx\n", 
			ptName, dispID, hr);
		_exit(0);
		return hr;
	}

	// 结束可变参数部分
	va_end(marker);

	delete[] pArgs;

	return hr;
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
DWORD GetModuleDirectory(LPWSTR pszDir, DWORD nSize);


//
// 函数: SafeArrayPutName(SAFEARRAY*, long, PCWSTR, PCWSTR);
//
// 作用: 这是该示例中的辅助功能函数。它将用户名(姓,名)
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
// * index -  数组中的一行，即姓名 (姓，名)的下标。
// 例如：两维数组中的第一维
// * pszFirstName - 姓
// * pszLastName - 名
//
// 返回值: HRESULT的值用于指示函数是否调用成功。
//
HRESULT SafeArrayPutName(SAFEARRAY* psa, long index, PCWSTR pszFirstName, 
						 PCWSTR pszLastName);


//
// 函数: AutomateExcelByCOMAPI(LPVOID)
//
// 作用: 使用C++和COM API自动化Microsoft Excel
//
DWORD WINAPI AutomateExcelByCOMAPI(LPVOID lpParam)
{
	// 在当前线程上初始化COM类库并通过调用CoInitializeEx或CoInitialize方法确认
    // 并发模型为单线程单元(STA)。
	// [-或者-] CoInitialize(NULL);
	// [-或者-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
	

	/////////////////////////////////////////////////////////////////////////
	// 使用C++和COM API创建Excel.Application COM对象
	// 

	// 获取服务器的CLSID 
	
	CLSID clsid;
	HRESULT hr;
	
	// 选择1. 通过CLSIDFromProgID从ProgID获取CLSID 
	LPCOLESTR progID = L"Excel.Application";
	hr = CLSIDFromProgID(progID, &clsid);
	if (FAILED(hr))
	{
		wprintf(L"CLSIDFromProgID(\"%s\") failed w/err 0x%08lx\n", progID, hr);
		return 1;
	}
	// 选择2. 直接创建CLSID 
	/*const IID CLSID_Application = 
	{0x00024500,0x0000,0x0000,{0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x46}};
	clsid = CLSID_Application;*/

	// 启动服务器并获取IDispatch接口 

	IDispatch *pXlApp = NULL;
	hr = CoCreateInstance(		// [-或者-] CoCreateInstanceEx, CoGetObject
		clsid,					// 服务器的CLSID 
		NULL,
		CLSCTX_LOCAL_SERVER,	// Excel.Application为本地服务器
		IID_IDispatch,			// 查询IDispatch接口
		(void **)&pXlApp);		// 输出

	if (FAILED(hr))
	{
		//Excel没有被正常注册
		wprintf(L"Excel is not registered properly w/err 0x%08lx\n", hr);
		return 1;
	}
	    //Excel.Application已启动
	_putws(L"Excel.Application is started");


	/////////////////////////////////////////////////////////////////////////
	// 令Excel不可见 (如： Application.Visible = 0)
	// 

	{
		VARIANT x;
		x.vt = VT_I4;
		x.lVal = 0;
		hr = AutoWrap(DISPATCH_PROPERTYPUT, NULL, pXlApp, L"Visible", 1, x);
	}


	/////////////////////////////////////////////////////////////////////////
	// 创建一个新的工作簿 (如： Application.Workbooks.Add)

	// 获取 Workbooks 集合
	IDispatch *pXlBooks = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pXlApp, L"Workbooks", 0);
		pXlBooks = result.pdispVal;
	}

	// 调用Workbooks.Add() 获取一个新的workbook对象
	IDispatch *pXlBook = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pXlBooks, L"Add", 0);
		pXlBook = result.pdispVal;
	}
	//新的工作簿已创建
	_putws(L"A new workbook is created");


	/////////////////////////////////////////////////////////////////////////
	// 获取处于活动状态的工作表并为其设置名称
	// 

	IDispatch *pXlSheet = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pXlApp, L"ActiveSheet", 0);
		pXlSheet = result.pdispVal;
	}

	{
		VARIANT vtSheetName;
		vtSheetName.vt = VT_BSTR;
		vtSheetName.bstrVal = SysAllocString(L"Report");
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pXlSheet, L"Name", 1, vtSheetName);
		VariantClear(&vtSheetName);
	}
	 // 处于活动状态的工作表被重命名为Report
	_putws(L"The active worksheet is renamed as Report");


	/////////////////////////////////////////////////////////////////////////
     // 填充数据到工作表单元
	// 

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
	IDispatch *pXlRange = NULL;
	{
		VARIANT param;
		param.vt = VT_BSTR;
		param.bstrVal = SysAllocString(L"A2:B6");

		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pXlSheet, L"Range", 1, param);
		pXlRange = result.pdispVal;

		VariantClear(&param);
	}

	// 用数组设置Range
	AutoWrap(DISPATCH_PROPERTYPUT, NULL, pXlRange, L"Value2", 1, saNames);

	// 清除数组
	VariantClear(&saNames);


	/////////////////////////////////////////////////////////////////////////
	// 将工作簿保存为xlsx文件并关闭
	// 

	_putws(L"Save and close the workbook");

	// pXlBook->SaveAs
	{
		// 确定文件名

		// 获取当前exe的目录
		wchar_t szFileName[MAX_PATH];
		if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
		{
			// 获取模块目录失败
			_putws(L"GetModuleDirectory failed");
			return 1;
		}

		// 将"Sample2.xlsx" 合并到目录
		wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample2.xlsx", 12);

		// 将空终止字符串转换为BSTR
		VARIANT vtFileName;
		vtFileName.vt = VT_BSTR;
		vtFileName.bstrVal = SysAllocString(szFileName);
		
		VARIANT vtFormat;
		vtFormat.vt = VT_I4;
		vtFormat.lVal = 51;	 // XlFileFormat::xlOpenXMLWorkbook

		// 如果有超过1个的参数需要被传递，必须以相反的顺序传递。
		// 否则，将导致错误0x80020009
		AutoWrap(DISPATCH_METHOD, NULL, pXlBook, L"SaveAs", 2, vtFormat, 
			vtFileName);

		VariantClear(&vtFileName);
	}

	// pXlBook->Close()
	AutoWrap(DISPATCH_METHOD, NULL, pXlBook, L"Close", 0);


	/////////////////////////////////////////////////////////////////////////
	// 退出Excel应用程序 (如: Application.Quit())
	// 

	_putws(L"Quit the Excel application");
	AutoWrap(DISPATCH_METHOD, NULL, pXlApp, L"Quit", 0);


	/////////////////////////////////////////////////////////////////////////
	// 释放COM对象
	// 

	if (pXlRange != NULL)
	{
		pXlRange->Release();
	}
	if (pXlSheet != NULL)
	{
		pXlSheet->Release();
	}
	if (pXlBook != NULL)
	{
		pXlBook->Release();
	}
	if (pXlBooks != NULL)
	{
		pXlBooks->Release();
	}
	if (pXlApp != NULL)
	{
		pXlApp->Release();
	}

	// 为该线程撤销COM
	CoUninitialize();

	return hr;
}