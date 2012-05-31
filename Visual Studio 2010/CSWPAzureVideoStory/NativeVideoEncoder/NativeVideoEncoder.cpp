/*********************************** 模块头 ***********************************\
* 模块名:  NativeVideoEncoder.h
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* dll的入口.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "stdafx.h"
#include "NativeVideoEncoder.h"
#include "VideoEncoder.h"
#include "XmlParser.h"

// 发布时需要删除写代码.
#ifdef DEBUG
#include "UnitTest.h"
#endif

// 这是输出函数的示例.
NATIVEVIDEOENCODER_API HRESULT EncoderVideo(wchar_t* pInputFile, wchar_t* pOutputFile, wchar_t* pLogFile)
{
	HRESULT hr = S_OK;
	VideoEncoder* encoder = new VideoEncoder();
	encoder->SetInputFile(pInputFile);
	encoder->SetOutputFile(pOutputFile);
	encoder->SetLogFile(pLogFile);
	hr = encoder->Encode();

	return hr;	
}
