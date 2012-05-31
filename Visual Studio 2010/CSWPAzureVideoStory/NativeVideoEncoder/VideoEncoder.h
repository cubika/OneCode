/*********************************** 模块头 ***********************************\
* 模块名:  VideoEncoder.h
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 对视频进行编码的主类.
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
#include "windows.h"
#include "Mfidl.h"
#include "Mfapi.h"
#include "Mfreadwrite.h"
#include "Wincodec.h"
#include "Common.h"
#include "string"
#include "Photo.h"
#include "list"
#include "TransitionBase.h"
#include "TransitionFactory.h"
#include "fstream"

using namespace std;

class VideoEncoder
{
public:
	VideoEncoder(void);
	~VideoEncoder(void);

	HRESULT Encode(void);

	wstring GetInputFile()
	{
		return this->m_inputFile;
	}

	void SetInputFile(wstring value)
	{
		this->m_inputFile = value;
	}

	wstring GetOutputFile()
	{
		return this->m_outputFile;
	}

	void SetOutputFile(wstring value)
	{
		this->m_outputFile = value;
	}

	wstring GetLogFile()
	{
		return this->m_logFile;
	}

	void SetLogFile(wstring value)
	{
		this->m_logFile = value;
	}

private:
	IWICImagingFactory* m_pIWICFactory;


	bool m_COMInitializedInDll;
	wstring m_inputFile; // xml
	wstring m_outputFile; // 视频
	wstring m_logFile; // 日志

	// 目前我们在构造函数中的硬编码视频设置.
	// 以下字段均为未来的扩展.
	GUID m_outputVideoFormat;
	UINT32 m_frameWidth;
	UINT32 m_frameHeight;
	UINT32 m_frameBufferSize;
	UINT32 m_frameStride;
	UINT32 m_videoBitRate;
	float m_fps;

	IMFSinkWriter* m_pSinkWriter;
	list<Photo*> m_photos;

	ofstream m_logFileStream;

	HRESULT Initialize(void);
	HRESULT CreateSinkWriter(IMFSinkWriter** ppSinkWriter, DWORD* pStreamIndex);
	HRESULT ParseInputXml(void);
	HRESULT DecodeFrame(Photo* pPhoto, BYTE** ppOutputBitmap, int* pBitmapSize);
	HRESULT WritePhotoSample(UINT64 sampleDuration, BYTE* pBitmap, DWORD streamIndex, LONGLONG* startTime);
	HRESULT WriteTransitionSample(UINT64 sampleDuration, TransitionBase* pTransition, DWORD streamIndex, LONGLONG* startTime);
	void Dispose(void);

	// 目前我们硬编码为 30 帧/秒, 所以我们简单地返回 333333.
	UINT64 GetFrameDuration()
	{
		return 333333;
	}
};

