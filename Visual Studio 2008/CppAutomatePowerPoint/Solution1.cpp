/****************************** 模块头 ******************************\
* 模块名:  Solution1.cpp
* 项目名:  CppAutomatePowerPoint
* 版权 (c) Microsoft Corporation.
* 
* Solution1.h/cpp中的代码演示了#import在自动化的PowerPoint中的使用。#import
* (http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx),从Visual C++ 5.0开始使用
* 的一个新指令，用来从一个规定的类型库创建VC++“智能指针”。它很强大，但由于与
* Microsoft Office应用程序协同操作时一般都有引用计数的问题，所以在此时经常不推荐使
* 用此功能。与Solution2.h/cpp中使用的直接调用API不同，智能指针使我们受益于早期后期
* 绑定对象的类型信息。#import将复杂的guid加入项目，并且COM API都被封装在#import指令
* 生成的自定义类中。
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
#include "Solution1.h"
#pragma endregion


#pragma region Import the type libraries

#import "libid:2DF8D04C-5BFA-101B-BDE5-00AA0044DE52" \
	rename("RGB", "MSORGB") \
	rename("DocumentProperties", "MSODocumentProperties")
// [-或-]
//#import "C:\\Program Files\\Common Files\\Microsoft Shared\\OFFICE12\\MSO.DLL" \
//	rename("RGB", "MSORGB") \
//	rename("DocumentProperties", "MSODocumentProperties")

using namespace Office;

#import "libid:0002E157-0000-0000-C000-000000000046"
// [-或-]
//#import "C:\\Program Files\\Common Files\\Microsoft Shared\\VBA\\VBA6\\VBE6EXT.OLB"

using namespace VBIDE;

#import "libid:91493440-5A91-11CF-8700-00AA0060263B" \
	rename("RGB", "VisioRGB")
// [-或-]
//#import "C:\\Program Files\\Microsoft Office\\Office12\\MSPPT.OLB" \
//	rename("RGB", "VisioRGB")

#pragma endregion


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
//   函数: AutomatePowerPointByImport(LPVOID)
//
//   功能: 使用#import指令智能指针的自动化PowerPoint
// 
DWORD WINAPI AutomatePowerPointByImport(LPVOID lpParam)
{
	// 初始化当前线程中的COM库并标识此并发模型为单线程（STA）。
	// [-或-] CoInitialize(NULL);
	// [-或-] CoCreateInstance(NULL);
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	try
	{

		/////////////////////////////////////////////////////////////////////
		// 使用#import指令智能指针创建PowerPoint.Application COM对象
		// 

		// 选项 1) 使用智能指针构造器创建对象
		// 
		// _ApplicationPtr是原始接口名_Application和后缀"Ptr"的组合
		//PowerPoint::_ApplicationPtr spPpApp(
		//	__uuidof(PowerPoint::Application)	// 组件的CLSID
		//	);

		// 选项 2) 使用智能指针函数CreateInstance创建对象
		PowerPoint::_ApplicationPtr spPpApp;
		HRESULT hr = spPpApp.CreateInstance(__uuidof(PowerPoint::Application));
		if (FAILED(hr))
		{
			wprintf(L"CreateInstance 出错 w/err 0x%08lx\n", hr);
			return 1;
		}

		_putws(L"PowerPoint.Application已经启动");


		/////////////////////////////////////////////////////////////////////
		// 使PowerPoint不可见(如 Application.Visible = 0)
		// 

		// PowerPoint默认不可见，直到你设置它为可见:
		//spPpApp->put_Visible(Office::MsoTriState::msoFalse);


		/////////////////////////////////////////////////////////////////////
		// 创建一个演示文稿 (如 Application.Presentations.Add)
		// 

		PowerPoint::PresentationsPtr spPres = spPpApp->Presentations;
		PowerPoint::_PresentationPtr spPre = spPres->Add(Office::msoTrue);

		_putws(L"一个新的演示文稿被建立");


		/////////////////////////////////////////////////////////////////////
		// 插入一个幻灯片并加入一些文本
		// 

		PowerPoint::SlidesPtr spSlides = spPre->Slides;

		wprintf(L"当前演示文稿有 %ld 个幻灯片\n", 
			spSlides->Count);

        // 插入一个幻灯片
		_putws(L"插入一个幻灯片");
		PowerPoint::_SlidePtr spSlide = spSlides->Add(1, 
			PowerPoint::ppLayoutText);

		// Add some texts to the slide
        _putws(L"添加一些文本");
		PowerPoint::ShapesPtr spShapes = spSlide->Shapes;
		PowerPoint::ShapePtr spShape = spShapes->Item((long)1);
		PowerPoint::TextFramePtr spTxtFrame = spShape->TextFrame;
		PowerPoint::TextRangePtr spTxtRange = spTxtFrame->TextRange;
        spTxtRange->Text = _bstr_t(L"一站式代码框架");


		/////////////////////////////////////////////////////////////////////
		// 保存演示文稿为pptx文件并关闭它。
		// 

        _putws(L"保存并关闭演示文稿");

		// 设置文件名

		// 获取当前exe文件名
		wchar_t szFileName[MAX_PATH];
		if (!GetModuleDirectory(szFileName, ARRAYSIZE(szFileName)))
		{
			_putws(L"GetModuleDirectory 出错");
			return 1;
		}

		// 将"Sample1.pptx"连接到目录字符串
		wcsncat_s(szFileName, ARRAYSIZE(szFileName), L"Sample1.pptx", 12);

		spPre->SaveAs(_bstr_t(szFileName), 
			PowerPoint::ppSaveAsOpenXMLPresentation, Office::msoTriStateMixed);

        spPre->Close();


		/////////////////////////////////////////////////////////////////////
		// 退出PowerPoint应用程序。
		// 

		_putws(L"退出PowerPoint应用程序");
		spPpApp->Quit();


		/////////////////////////////////////////////////////////////////////
		// 释放COM对象
		// 

		// 释放引用对于智能指针不是必须的
		// ...
		// spPowerPointApp.Release();
		// ...

	}
	catch (_com_error &err)
	{
		wprintf(L"PowerPoint抛出异常: %s\n", err.ErrorMessage());
		wprintf(L"描述: %s\n", (LPCWSTR) err.Description());
	}

	// 在此线程取消初始化
	CoUninitialize();

	return 0;
}