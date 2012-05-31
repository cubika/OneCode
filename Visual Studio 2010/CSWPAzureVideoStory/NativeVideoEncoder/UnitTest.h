/*********************************** 模块头 ***********************************\
* 模块名:  UnitTest.h
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 单元测试类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once
#include "stdafx.h"
#include "XmlParser.h"
#include "assert.h"
#include "ErrorCodes.h"
#include "VideoEncoder.h"

class UnitTest
{
public:
	inline HRESULT TestXmlParser()
	{
		XmlParser* parser;
		HRESULT hr = XmlParser::Create(&parser, L"D:\\WPAzureVideoStory\\CloudTest\\bin\\Debug\\test.xml");
		assert(hr == S_OK);
		hr = parser->ReadStartElement();
		assert(hr == S_OK);
	
		// 测试元素名.
		wstring expectedElementName(L"Photos");
		wstring actualElementName = parser->GetCurrentElementName();
		assert(actualElementName.compare(expectedElementName) == 0);
	
		// 测试集成元素.
		hr = parser->ReadStartElement();
		assert(hr == S_OK);
		expectedElementName = L"Photo";
		actualElementName = parser->GetCurrentElementName();
		assert(actualElementName.compare(expectedElementName) == 0);
	
		// 测试属性.
		assert(parser->GetAttributesCount() == 2);
		map<wstring, wstring> attributes = parser->GetAttributes();
		map<wstring, wstring>::iterator it = attributes.begin();
		assert(it->first.compare(L"Name") == 0);
		assert(it->second.compare(L"C:\\Users\\v-ylluo\\Pictures\\Blue hills.jpg") == 0);
		it++;
		assert(it->first.compare(L"PhotoDuration") == 0);
		assert(it->second.compare(L"5") == 0);
		
		// 下一个元素是一个开始元素，所以应返回 E_NOTENDELEMENT.
		hr = parser->ReadEndElement();
		assert(hr == E_NOTENDELEMENT);
		hr = S_OK;
	
		// 失败的读取应该回滚，所以我们再次测试当前元素是否正确.
		actualElementName = parser->GetCurrentElementName();
		assert(actualElementName.compare(expectedElementName) == 0);
		assert(parser->GetAttributesCount() == 2);
		attributes = parser->GetAttributes();
		it = attributes.begin();
		assert(it->first.compare(L"Name") == 0);
		assert(it->second.compare(L"C:\\Users\\v-ylluo\\Pictures\\Blue hills.jpg") == 0);
		it++;
		assert(it->first.compare(L"PhotoDuration") == 0);
		assert(it->second.compare(L"5") == 0);
	
		// 另一个开始元素，已包括在上述测试场景.
		parser->ReadStartElement();
	
		// 下一个元素是结束元素，所以应返回 E_NOTSTARTELEMENT.
		hr = parser->ReadStartElement();
		assert(hr == E_NOTSTARTELEMENT);
		hr = S_OK;
	
		// 失败的读取应该回滚，所以我们再次测试当前元素是否正确.
		expectedElementName = L"Transition";
		actualElementName = parser->GetCurrentElementName();
		assert(actualElementName.compare(expectedElementName) == 0);
		assert(parser->GetAttributesCount() == 2);
		attributes = parser->GetAttributes();
		it = attributes.begin();
		// TODO: 我们使用映射存储的属性，项目根据键自动进行排序.
		// 我们目前不需要保持原始属性的顺序.
		// 将来如果顺序很重要，我们可能要使用对而不是映射.
		// 可能由于没有二进制搜索支持有点慢.
		assert(it->first.compare(L"Duration") == 0);
		assert(it->second.compare(L"2") == 0);
		it++;
		assert(it->first.compare(L"Name") == 0);
		assert(it->second.compare(L"Fade Transition") == 0);
	
		// 这次应该是结束元素.
		hr = parser->ReadEndElement();
		assert(hr == S_OK);
	
		hr = parser->ReadEndElement();
		assert(hr == S_OK);
	
		delete parser;
	
		return hr;
	}

	inline HRESULT TestEncoder()
	{
		HRESULT hr = S_OK;
		VideoEncoder* encoder = new VideoEncoder();
		encoder->SetInputFile(L"D:\\WPAzureVideoStory\\CloudTest\\bin\\Debug\\test.xml");
		encoder->SetOutputFile(L"D:\\WPAzureVideoStory\\CloudTest\\bin\\Debug\\test.wmv");
		hr = encoder->Encode();
		assert(hr == S_OK);

		// 测试编码多个短影是否会导致内存泄漏.
		// 使用任务管理器来检查 QTAgent 的内存使用.
		// 这要花很多时间.如果不需要测试注释之. 
		//for (int i = 0; i < 1000; i++)
		//{
		//	hr = encoder->Encode();
		//}

		delete encoder;
		return hr;
	}

	inline HRESULT DoUnitTest()
	{
		_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF|_CRTDBG_LEAK_CHECK_DF|_CRTDBG_CHECK_ALWAYS_DF);
		//return this->TestXmlParser();
		return this->TestEncoder();
	}
};