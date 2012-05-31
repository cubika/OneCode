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

	// 来创建分析器的工厂方法.
	static HRESULT Create(XmlParser** parser, wstring fileName);
	HRESULT ReadDeclaration(void);
	HRESULT ReadStartElement(void);
	HRESULT ReadEndElement(void);
	wstring ReadAttribute(void);
	wstring ReadContent(void);

	//  当前元素的名称.
	wstring GetCurrentElementName()
	{
		return this->m_currentElementName;
	}

	//  当前元素有多少属性 .
	UINT64 GetAttributesCount()
	{
		return this->m_attributes.size();
	}

	map<wstring, wstring> GetAttributes()
	{
		return this->m_attributes;
	}

private:
	// 不允许直接调用构造函数. 应调用Create方法.
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

	// 当前元素的完整的字符串表示形式.
	// Todo: 如果元素中包含大量的属性,
	// 每次读取一个属性可能较读取整个元素更快?
	wstring m_currentElementName;
	map<wstring, wstring> m_attributes;
	bool m_isSelfCloseTag;

	void SaveState(void);
	void RollbackState(void);
};

