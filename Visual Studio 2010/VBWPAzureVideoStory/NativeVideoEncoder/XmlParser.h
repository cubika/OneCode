/*********************************** 模块头 ***********************************\
* 模块名:  XmlParser.h
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 简单的 XML 解析器.不支持高级的功能（例如命名空间和架构验证）.
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
#include "iostream"
#include "map"
using namespace std;

class XmlParser
{
public:
	~XmlParser(void);

	// Factory method to create the parser.
	static HRESULT Create(XmlParser** parser, wstring fileName);
	HRESULT ReadDeclaration(void);
	HRESULT ReadStartElement(void);
	HRESULT ReadEndElement(void);
	wstring ReadAttribute(void);
	wstring ReadContent(void);

	// The name of the current element.
	wstring GetCurrentElementName()
	{
		return this->m_currentElementName;
	}

	// How many attributes are there in the current element.
	UINT64 GetAttributesCount()
	{
		return this->m_attributes.size();
	}

	map<wstring, wstring> GetAttributes()
	{
		return this->m_attributes;
	}

private:
	// Do not allow caller to invoke constructor directly. They should invoke the Create method instead.
	XmlParser();
	
	struct ParserState
	{
		int currentChar;
		wstring currentElementName;
		map<wstring, wstring> attributes;
		bool isSelfCloseTag;
	};

	ParserState m_parserState;
	wchar_t* m_fileContent;
	int m_fileSize;
	int m_currentChar;

	// The complete string representation of the current element.
	// Todo: If the element contains a lot of attributes,
	// would it be better to read one attribute a time, instead of reading the whole element?
	wstring m_currentElementName;
	map<wstring, wstring> m_attributes;
	bool m_isSelfCloseTag;

	void SaveState(void);
	void RollbackState(void);
};

