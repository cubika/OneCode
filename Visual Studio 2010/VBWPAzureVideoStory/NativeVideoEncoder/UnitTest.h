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
	
		// Test element name.
		wstring expectedElementName(L"Photos");
		wstring actualElementName = parser->GetCurrentElementName();
		assert(actualElementName.compare(expectedElementName) == 0);
	
		// Test nested element.
		hr = parser->ReadStartElement();
		assert(hr == S_OK);
		expectedElementName = L"Photo";
		actualElementName = parser->GetCurrentElementName();
		assert(actualElementName.compare(expectedElementName) == 0);
	
		// Test attributes.
		assert(parser->GetAttributesCount() == 2);
		map<wstring, wstring> attributes = parser->GetAttributes();
		map<wstring, wstring>::iterator it = attributes.begin();
		assert(it->first.compare(L"Name") == 0);
		assert(it->second.compare(L"C:\\Users\\v-ylluo\\Pictures\\Blue hills.jpg") == 0);
		it++;
		assert(it->first.compare(L"PhotoDuration") == 0);
		assert(it->second.compare(L"5") == 0);
		
		// The next element is a start element, so E_NOTENDELEMENT should be returned.
		hr = parser->ReadEndElement();
		assert(hr == E_NOTENDELEMENT);
		hr = S_OK;
	
		// A failed read should roll back, so we test again if the current element is correct.
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
	
		// Another start element, already covered in the above test scenario.
		parser->ReadStartElement();
	
		// The next element is an end element, so E_NOTSTARTELEMENT should be returned.
		hr = parser->ReadStartElement();
		assert(hr == E_NOTSTARTELEMENT);
		hr = S_OK;
	
		// A failed read should roll back, so we test again if the current element is correct.
		expectedElementName = L"Transition";
		actualElementName = parser->GetCurrentElementName();
		assert(actualElementName.compare(expectedElementName) == 0);
		assert(parser->GetAttributesCount() == 2);
		attributes = parser->GetAttributes();
		it = attributes.begin();
		// TODO: We use a map to store the attributes, which automatically sorts items by key.
		// Currently we do not need to maintain the original order of attributes.
		// In the future, if the order matters, we may have to use pair instead of map,
		// which may be a bit slower due to no binary search support.
		assert(it->first.compare(L"Duration") == 0);
		assert(it->second.compare(L"2") == 0);
		it++;
		assert(it->first.compare(L"Name") == 0);
		assert(it->second.compare(L"Fade Transition") == 0);
	
		// This time it should be an end element.
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

		// Test to see if encoding multiple stories will cause memory leak.
		// Use Task Manager to check the memory usage of QTAgent.
		// This takes a lot of time. Comment it if the test is not needed.
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