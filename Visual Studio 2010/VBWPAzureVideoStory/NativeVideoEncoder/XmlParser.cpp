/*********************************** 模块头 ***********************************\
* 模块名:  XmlParser.cpp
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

#include "StdAfx.h"
#include "XmlParser.h"
#include "codecvt" 
#include "cstdlib"
#include "locale" 
#include "fstream"
#include "ErrorCodes.h"

using namespace std;

XmlParser::XmlParser() :
	m_fileContent(nullptr),
	m_fileSize(0),
	m_currentChar(1),
	m_currentElementName(),
	m_isSelfCloseTag(false)
{
	this->m_attributes = map<wstring, wstring>();
}

XmlParser::~XmlParser(void)
{
	if (this->m_fileContent != nullptr)
	{
		delete this->m_fileContent;
	}
	this->m_attributes.clear();
}

// 创建 XmlParser，并读取源文件.
// 我们会对文件执行2次 I/O操作.
// 第一，使用 Win32 API 获取文件长度，使我们可以分配缓冲区.
// 第二，使用 c++ STL 读取该文件.
// 使用 Win32 API 可以帮助我们获取文件属性，
// 而 STL 可以帮助我们将 utf-8 文件转换为宽字符.
// 性能影响最小，
// 因为 Win32 API 只是读取文件属性没有读取文件内容.
HRESULT XmlParser::Create(XmlParser** parser, wstring fileName)
{
	HRESULT hr = S_OK;

	*parser = new XmlParser();

	// 打开文件.
	HANDLE hFileHandle = CreateFile(
		fileName.c_str(),
		GENERIC_READ,
		FILE_SHARE_READ,
		nullptr,
		OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL,
		nullptr);

	if (hFileHandle == INVALID_HANDLE_VALUE)
	{
		return ERROR_FILE_INVALID;
	}

	// 获取文件大小.
	(*parser)->m_fileSize = GetFileSize(hFileHandle, nullptr);
	if ((*parser)->m_fileSize == INVALID_FILE_SIZE)
	{
		return INVALID_FILE_SIZE;
	}

	// 关闭该文件.我们将使用 c++ STL 来读取.
	CloseHandle(hFileHandle);

	(*parser)->m_fileContent = new wchar_t[(*parser)->m_fileSize];
	for (int i = 0; i < (*parser)->m_fileSize; i++)
	{
		(*parser)->m_fileContent[i] = 0;
	}
	
	// 再使用 STL打开该文件.
	wifstream fileStream;
	fileStream.open(fileName);

	// 将 utf-8 流转换为宽字符串.
	codecvt_utf8<wchar_t>* converter = new codecvt_utf8<wchar_t>(); 
	const locale empty_locale = locale::empty(); 
	const locale utf8_locale = locale(empty_locale, converter);
	fileStream.imbue(utf8_locale);

	// 读取文件内容.
	fileStream.read((*parser)->m_fileContent, (*parser)->m_fileSize);

	fileStream.close();
	return hr;
}

// 读取 xml 声明.
// <?xml version="1.0" encoding="utf-8"?>
// 在此版本中，我们只是跳过字符.
HRESULT XmlParser::ReadDeclaration()
{
	if (this->m_fileContent == nullptr)
	{
		return ERROR_FILE_INVALID;
	}

	//  在此版本中，我们不做很多的错误检查.
	// 这样简单将字符移动到第一个 ' >'.
	while (this->m_fileContent[this->m_currentChar] != '>')
	{
		this->m_currentChar++;
	}
	return S_OK;
}

// 取当前节点并将读取器推进到下一个节点.
HRESULT XmlParser::ReadStartElement()
{
	if (this->m_fileContent == nullptr)
	{
		return ERROR_FILE_INVALID;
	}

	this->SaveState();

	// 跳过直到我们遇到一个开始元素 .
	while (this->m_fileContent[this->m_currentChar] != '<')
	{
		this->m_currentChar++;
	}

	// 我们需要跳过 ' <'，所以我们可以开始使用该元素的名称.
	this->m_currentChar++;
	
	// 我们遇到的结束元素，所以我们无法读取开始元素.
	if (this->m_fileContent[this->m_currentChar] == '/')
	{
		this->RollbackState();
		return E_NOTSTARTELEMENT;
	}

	bool spaceFound = false;
	wstring attributeName = L"";
	wstring attributeValue = L"";
	bool quoteFound = false;

	while (this->m_fileContent[this->m_currentChar] != '>')
	{
		// 未发现空格.
		if (!spaceFound)
		{
			// 第一个空格，指示元素名称结束.
			if (this->m_fileContent[this->m_currentChar] == ' ')
			{
				spaceFound = true;
			}
			else
			{
				this->m_currentElementName.append(1, this->m_fileContent[this->m_currentChar]);
			}
		}
		// 现在去属性。我们假定 xml 文件格式化好的，
		// 因此空格表示至少一个后续属性.
		else
		{			
			if (this->m_fileContent[this->m_currentChar] == '\"')
			{
				// 属性值的开始.
				if (!quoteFound)
				{
					quoteFound = true;
				}
				// 属性值的结尾。插入对到属性映射，重置变量.
				else
				{
					// TODO: c++ 映射以键自动排列项目.
					// 我们目前不需要保持原始属性的顺序.
					// 将来如果顺序很重要，我们可能要使用对，而不是映射.
					// 这可能由于没有二进制搜索支持有点慢.
					this->m_attributes[attributeName] = attributeValue;
					attributeName.clear();
					attributeValue.clear();
					quoteFound = false;
				}
			}
			else
			{
				// 继续阅读属性名称.
				if (!quoteFound)
				{
					// 我们假定 xml 格式正确，因此该元素的名称不能包含 '=' 或 ' '.
					// 我们应跳过'"'前的'='.
					// ' ' 可能在属性进行分析后遇到，这时我们开始一个新属性.
					// 在这种情况下我们也应跳过.
					if (this->m_fileContent[this->m_currentChar] != '='
						&& this->m_fileContent[this->m_currentChar] != ' ')
					{
						attributeName.append(1, this->m_fileContent[this->m_currentChar]);
					}
				}
				//  读取属性值 .
				else
				{
					attributeValue.append(1, this->m_fileContent[this->m_currentChar]);
				}
			}
		}

		this->m_currentChar++;

		// 非有效xml 文件.
		// 虽然我们假设 xml 文件是有效的我们还需做一些简单的检查.
		if (this->m_currentChar > this->m_fileSize)
		{
			return E_UNEXPECTED;
		}
	}

	// 检查标签是否自我封闭.
	if (this->m_fileContent[this->m_currentChar - 1] == '/')
	{
		this->m_isSelfCloseTag = true;
	}

	// While 循环没有读 ' >'.所以现在读.
	this->m_currentChar++;
	return S_OK;
}

// 阅读结束元素.
// 因为我们假设是有效的 xml 文件，我们可以简单地推进指针到'>'.
HRESULT XmlParser::ReadEndElement(void)
{
	if (this->m_fileContent == nullptr)
	{
		return ERROR_FILE_INVALID;
	}

	// 该元素是自我封闭的，所以立即返回.
	if (this->m_isSelfCloseTag)
	{
		this->m_isSelfCloseTag = false;
		return S_OK;
	}

	this->SaveState();

	while (this->m_fileContent[this->m_currentChar] != '>')
	{
		if (this->m_fileContent[this->m_currentChar] == '<')
		{
			// 遇到另一个开始元素，因此我们不能阅读结束元素.
			if (this->m_fileContent[this->m_currentChar + 1] != '/')
			{
				this->RollbackState();
				return E_NOTENDELEMENT;
			}
		}

		this->m_currentChar++;
		
		
		// 非有效xml 文件.
		// 虽然我们假设 xml 文件是有效的我们还需做一些简单的检查.
		if (this->m_currentChar > this->m_fileSize)
		{
			return E_UNEXPECTED;
		}
	}

	// 推进指针向前到'>'.
	this->m_currentChar++;
	return S_OK;
}

// 保存当前的状态，所以如果发生了错误，我们可以回滚.
// TODO: 复制所有属性可能会非常耗时.
// 当前版本，我们的每个元素只有几个属性，
// 所以应该是大致无碍.
// 但在将来，考虑到检查的条件，我们没有必要复制的所有属性.
void XmlParser::SaveState()
{
	this->m_parserState.currentChar = this->m_currentChar;
	this->m_parserState.attributes = map<wstring, wstring>(this->m_attributes);
	this->m_attributes.clear();
	this->m_parserState.currentElementName = this->m_currentElementName;
	this->m_currentElementName.clear();
	this->m_parserState.isSelfCloseTag = this->m_isSelfCloseTag;
	this->m_isSelfCloseTag = false;
}

//  回滚以前的状态 .
void XmlParser::RollbackState()
{
	this->m_currentChar = this->m_parserState.currentChar;
	this->m_attributes = map<wstring, wstring>(this->m_parserState.attributes);
	this->m_currentElementName = this->m_parserState.currentElementName;
	this->m_isSelfCloseTag = this->m_parserState.isSelfCloseTag;
}