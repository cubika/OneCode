/****************************** 模块头 ******************************\
* 模块名:  Solution2.cpp
* 项目名:  CppAutomatePowerPoint
* 版权 (c) Microsoft Corporation.
* 
* Solution2.h/cpp中的代码演示了使用C/C++和COM API自动化操作PowerPoint。原始的自动化
* 非常难操作，但是有时又需要避免使用MFC带来的开销和#import的问题。基本上，你在工作中
* 使用的是一些这样的API，如CoCreateInstance()和COM接口（如IDispatch和IUnknown）。
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
#include <stdio.h>
#include <windows.h>
#include "Solution2.h"
#pragma endregion


//
//   函数: AutoWrap(int, VARIANT*, IDispatch*, LPOLESTR, int,...)
//
//   功能: 自动化帮助函数。它简化了大部分的直接参与使用 IDispatch 低级别的详
//	细信息。在你的实现中可以随意的使用它。一个附加说明是，如果你传递多个
//	参数，需要将这些参数倒序传入。
//
//   参数:
//      * autoType - 应该是这个集合里的值: DISPATCH_PROPERTYGET, 
//      DISPATCH_PROPERTYPUT, DISPATCH_PROPERTYPUTREF, DISPATCH_METHOD.
//      * pvResult - 将返回值存入VARIANT
//      * pDisp - IDispatch接口
//      * ptName - 接口暴露出的属性/方法
//      * cArgs - 参数个数
//
//   返回值: 返回值HRESULT标识了函数的执行成功与否
//
//   样例: 
//      AutoWrap(DISPATCH_METHOD, NULL, pDisp, L"call", 2, parm[1], parm[0]);
//
HRESULT AutoWrap(int autoType, VARIANT *pvResult, IDispatch *pDisp, 
				 LPOLESTR ptName, int cArgs...) 
{
	// 准备变量参数列表
	va_list marker;
	va_start(marker, cArgs);

	if (!pDisp) 
	{
		_putws(L"NULL IDispatch 传入了 AutoWrap()");
		_exit(0);
		return E_INVALIDARG;
	}

	// 使用的变量
	DISPPARAMS dp = { NULL, NULL, 0, 0 };
	DISPID dispidNamed = DISPID_PROPERTYPUT;
	DISPID dispID;
	HRESULT hr;

	// 获取传递的名字的DISPID
	hr = pDisp->GetIDsOfNames(IID_NULL, &ptName, 1, LOCALE_USER_DEFAULT, &dispID);
	if (FAILED(hr))
	{
		wprintf(L"IDispatch::GetIDsOfNames(\"%s\") 出错 w/err 0x%08lx\n", 
			ptName, hr);
		_exit(0);
		return hr;
	}

	// 为参数分配内存
	VARIANT *pArgs = new VARIANT[cArgs + 1];
	// 提取参数...
	for(int i=0; i < cArgs; i++) 
	{
		pArgs[i] = va_arg(marker, VARIANT);
	}

	// 建立DISPPARAMS
	dp.cArgs = cArgs;
	dp.rgvarg = pArgs;

	// 为属性输出捕获特殊的情况
	if (autoType & DISPATCH_PROPERTYPUT)
	{
		dp.cNamedArgs = 1;
		dp.rgdispidNamedArgs = &dispidNamed;
	}

	// 做一个调用
	hr = pDisp->Invoke(dispID, IID_NULL, LOCALE_SYSTEM_DEFAULT,
		autoType, &dp, pvResult, NULL, NULL);
	if (FAILED(hr)) 
	{
		wprintf(L"IDispatch::Invoke(\"%s\"=%08lx) 出错 w/err 0x%08lx\n", 
			ptName, dispID, hr);
		_exit(0);
		return hr;
	}

	// 结束变量参数阶段
	va_end(marker);

	delete[] pArgs;

	return hr;
}


//
//   函数: GetModuleDirectory(LPWSTR, DWORD);
//
//   功能: 在此例中这是一个帮助函数。它可以获取当前进程中的可执行文件的目录的
//         完全限定路径。例如, "D:\Samples\"。
//
//   参数:
//      * pszDir - 一个指向缓冲区（存放当前进程中的可执行文件的目录的完全限定
//         路径）的指针。如果路径长度小于nSize参数指定的长度，则函数成功执行，
//	  路径作为一个非终止的字符串返回。
//      * nSize - 存放在缓冲区的lpFilename字符串长度。
//
//   返回值: 如果函数执行成功，返回值是被复制到缓冲区的字符串的长度，以字符计算，
//	  不包含NULL终止字符。如果缓冲区太小不足以保存目录名，函数返回0并设置最
//	  终错误为ERROR_INSUFFICIENT_BUFFER。如果函数执行失败, 返回值为0.可以调用
//	  GetLastError获得更多的错误信息。
//
DWORD GetModuleDirectory(LPWSTR pszDir, DWORD nSize);


//
//   函数: AutomatePowerPointByCOMAPI(LPVOID)
//
//   功能: 使用C++和COM API的自动化Microsoft PowerPoint。
//
DWORD WINAPI AutomatePowerPointByCOMAPI(LPVOID lpParam)
{
	// 初始化当前线程中的COM库并标识此并发模型为单线程（STA）。
	// [-或-] CoInitialize(NULL);
	// [-或-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);


	/////////////////////////////////////////////////////////////////////////
	// 使用C++和COM API创建PowerPoint.Application COM对象。
	// 

	// 获取服务的CLSID
	
	CLSID clsid;
	HRESULT hr;
	
	// 选项 1. 通过CLSIDFromProgID从ProgID获取CLSID。
	LPCOLESTR progID = L"PowerPoint.Application";
	hr = CLSIDFromProgID(progID, &clsid);
	if (FAILED(hr))
	{
		wprintf(L"CLSIDFromProgID(\"%s\") 出错 w/err 0x%08lx\n", progID, hr);
		return 1;
	}
	// 选项 2. 直接创建CLSID。
	/*const IID CLSID_Application = 
	{0x91493441,0x5A91,0x11CF,{0x87,0x00,0x00,0xAA,0x00,0x60,0x26,0x3B}};
	clsid = CLSID_Application;*/

	// 启动服务，获取IDispatch接口

	IDispatch *pPpApp = NULL;
	hr = CoCreateInstance(		// [-或-] CoCreateInstanceEx, CoGetObject
		clsid,					// 服务的CLSID
		NULL,
		CLSCTX_LOCAL_SERVER,	// PowerPoint.Application是一个本地服务
		IID_IDispatch,			// 查询IDispatch接口
		(void **)&pPpApp);		// 输出

	if (FAILED(hr))
	{
		wprintf(L"PowerPoint没有得到恰当的注册 w/err 0x%08lx\n", hr);
		return 1;
	}

	_putws(L"PowerPoint.Application已经启动");


	/////////////////////////////////////////////////////////////////////////
	// 使PowerPoint不可见 (如 Application.Visible = 0)
	// 

	// PowerPoint默认不可见，直到你设置它为可见:
	//{
	//	VARIANT x;
	//	x.vt = VT_I4;
	//	x.lVal = 0;	// Office::MsoTriState::msoFalse
	//	hr = AutoWrap(DISPATCH_PROPERTYPUT, NULL, pPpApp, L"Visible", 1, x);
	//}


	/////////////////////////////////////////////////////////////////////////
	// 创建一个演示文稿 (如 Application.Presentations.Add)
	// 

	// 获得演示文档集合
	IDispatch *pPres = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pPpApp, L"Presentations", 0);
		pPres = result.pdispVal;
	}

	// 调用Presentations.Add生成一个新的演示文稿
	IDispatch *pPre = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pPres, L"Add", 0);
		pPre = result.pdispVal;
	}

	_putws(L"一个新的演示文稿被建立");


	/////////////////////////////////////////////////////////////////////////
	// 插入一个幻灯片并加入一些文本
	// 

	// 获得幻灯片集合
	IDispatch *pSlides = NULL;
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pPre, L"Slides", 0);
		pSlides = result.pdispVal;
	}

	// 插入一个幻灯片
	_putws(L"插入一个幻灯片");

	IDispatch *pSlide = NULL;
	{
		VARIANT vtIndex;
		vtIndex.vt = VT_I4;
		vtIndex.lVal = 1;
		
		VARIANT vtLayout;
		vtLayout.vt = VT_I4;
		vtLayout.lVal = 2;	// PowerPoint::PpSlideLayout::ppLayoutText

		VARIANT result;
		VariantInit(&result);
		// 有超过一个参数传递时，必须倒序传递。
		// 否则，将会得到错误码为0x80020009的错误。
		AutoWrap(DISPATCH_METHOD, &result, pSlides, L"Add", 2, vtLayout, vtIndex);
		pSlide = result.pdispVal;
	}

	// 在幻灯片上添加一些文本
	_putws(L"添加一些文本");

	IDispatch *pShapes = NULL;		// pSlide->Shapes
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pSlide, L"Shapes", 0);
		pShapes = result.pdispVal;
	}

	IDispatch *pShape = NULL;		// pShapes->Item(1)
	{
		VARIANT vtIndex;
		vtIndex.vt = VT_I4;
		vtIndex.lVal = 1;

		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_METHOD, &result, pShapes, L"Item", 1, vtIndex);
		pShape = result.pdispVal;
	}

	IDispatch *pTxtFrame = NULL;	// pShape->TextFrame
	{
		VARIANT result;
		VariantInit(&result);
		hr = AutoWrap(DISPATCH_PROPERTYGET, &result, pShape, L"TextFrame", 0);
		pTxtFrame = result.pdispVal;
	}

	IDispatch *pTxtRange = NULL;	// pTxtFrame->TextRange
	{
		VARIANT result;
		VariantInit(&result);
		AutoWrap(DISPATCH_PROPERTYGET, &result, pTxtFrame, L"TextRange", 0);
		pTxtRange = result.pdispVal;
	}

	{
		VARIANT x;
		x.vt = VT_BSTR;
		x.bstrVal = SysAllocString(L"一站式代码框架");
		AutoWrap(DISPATCH_PROPERTYPUT, NULL, pTxtRange, L"Text", 1, x);
		VariantClear(&x);
	}


	/////////////////////////////////////////////////////////////////////////
	// 保存演示文稿为pptx文件并关闭它。
	// 

	_putws(L"保存并关闭演示文稿");

	{
		// 设置文件名

		// 获取当前exe文件名
		wchar_t szFileName[MAX_PATH];
		if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
		{
			_putws(L"GetModuleDirectory 出错");
			return 1;
		}

		// 将"Sample2.pptx"连接到目录字符串
		wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample2.pptx", 12);

		VARIANT vtFileName;
		vtFileName.vt = VT_BSTR;
		vtFileName.bstrVal = SysAllocString(szFileName);

		VARIANT vtFormat;
		vtFormat.vt = VT_I4;
		vtFormat.lVal = 24;	// PpSaveAsFileType::ppSaveAsOpenXMLPresentation

		VARIANT vtEmbedFont;
		vtEmbedFont.vt = VT_I4;
		vtEmbedFont.lVal = -2;	// MsoTriState::msoTriStateMixed

		// 有超过一个参数传递时，必须倒序传递。
		// 否则，将会得到错误码为0x80020009的错误。
		AutoWrap(DISPATCH_METHOD, NULL, pPre, L"SaveAs", 3, vtEmbedFont, 
			vtFormat, vtFileName);

		VariantClear(&vtFileName);
	}

	// pPre->Close()
	AutoWrap(DISPATCH_METHOD, NULL, pPre, L"Close", 0);


	/////////////////////////////////////////////////////////////////////////
	// 退出PowerPoint应用程序。 (如 Application.Quit())
	// 

	_putws(L"退出PowerPoint应用程序");
	AutoWrap(DISPATCH_METHOD, NULL, pPpApp, L"Quit", 0);


	/////////////////////////////////////////////////////////////////////////
	// 释放COM对象.
	// 

	if (pTxtRange != NULL)
	{
		pTxtRange->Release();
	}
	if (pTxtFrame != NULL)
	{
		pTxtFrame->Release();
	}
	if (pShape != NULL)
	{
		pShape->Release();
	}
	if (pShapes != NULL)
	{
		pShapes->Release();
	}
	if (pSlide != NULL)
	{
		pSlide->Release();
	}
	if (pSlides != NULL)
	{
		pSlides->Release();
	}
	if (pPre != NULL)
	{
		pPre->Release();
	}
	if (pPres != NULL)
	{
		pPres->Release();
	}
	if (pPpApp != NULL)
	{
		pPpApp->Release();
	}

	// 在此线程取消初始化
	CoUninitialize();

	return 0;
}