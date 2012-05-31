/*********************************** 模块头 ***********************************\
* 模块名:  VideoEncoder.cpp
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 对视频进行编码的主类 .
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
#include "StdAfx.h"
#include "VideoEncoder.h"
#include "XmlParser.h"
#include "ErrorCodes.h"

VideoEncoder::VideoEncoder(void) :
	m_pIWICFactory(nullptr),
	m_COMInitializedInDll(false),
	m_inputFile(L""),
	m_outputFile(L""),
	m_outputVideoFormat(MFVideoFormat_H264),
	m_frameWidth(800),
	m_frameHeight(600),
	m_videoBitRate(1500000),
	m_fps(30),
	m_pSinkWriter(nullptr)
{
	this->m_photos = list<Photo*>();
	this->m_frameStride = this->m_frameWidth * 4;
	this->m_frameBufferSize = this->m_frameStride * this->m_frameHeight;
}

VideoEncoder::~VideoEncoder(void)
{
	this->Dispose();
}

void VideoEncoder::Dispose()
{
	SafeRelease(&this->m_pIWICFactory);
	if (this->m_COMInitializedInDll)
	{
		CoUninitialize();
		this->m_COMInitializedInDll = false;
	}
	for (list<Photo*>::iterator it = this->m_photos.begin(); it != this->m_photos.end(); it++)
	{
		if (*it != nullptr)
		{
			delete *it;
			*it = nullptr;
		}
	}
	this->m_photos.clear();
	this->m_logFileStream.close();
	SafeRelease(&this->m_pSinkWriter);
}

HRESULT VideoEncoder::Encode()
{
	HRESULT hr = S_OK;
	Photo* pPhoto = nullptr;
	DWORD streamIndex = 0;
	BYTE* pCurrentBitmap = nullptr;
	BYTE* pPreviousBitmap = nullptr;
	BYTE* pFrameBuffer = nullptr;
	LONGLONG startTime = 0;
	UINT64 sampleDuration = 0;
	UINT64 transitionSampleDuration = 0;

	CheckHR(this->Initialize());
	this->m_logFileStream << "已成功初始化非托管视频编码器" << endl;
	CheckHR(this->ParseInputXml());
	this->m_logFileStream << "Xml配置文件解析成功." << endl;

	CheckHR(this->CreateSinkWriter(&this->m_pSinkWriter, &streamIndex));
	this->m_logFileStream << "Sink writer创建成功." << endl;

	// 循环访问照片.
	for (list<Photo*>::iterator it = this->m_photos.begin(); it != this->m_photos.end(); it++)
	{
		pPhoto = *it;
		int frameSize = 0;
		// 解码位图.
		CheckHR(this->DecodeFrame(pPhoto, &pCurrentBitmap, &frameSize));
		this->m_logFileStream << "照片 " << pPhoto->GetFile().c_str() << " 成功解码." << endl;

		// 编写特效示例.
		if (pPreviousBitmap != nullptr)
		{
			transitionSampleDuration = (UINT64)(pPhoto->GetTransitionDuration() * this->m_fps);
			if (pPhoto->GetTransition() != nullptr)
			{
				pPhoto->GetTransition()->SetForegroundFrame(pPreviousBitmap);
				pPhoto->GetTransition()->SetBackgroundFrame(pCurrentBitmap);
				CheckHR(this->WriteTransitionSample(transitionSampleDuration, pPhoto->GetTransition(), streamIndex, &startTime));
				this->m_logFileStream << "照片 " << pPhoto->GetFile().c_str() << " 的特效示例编写成功." << endl;
			}
		}

		// 编写照片示例.
		sampleDuration = (UINT64)(pPhoto->GetPhotoDuration() * this->m_fps);
		CheckHR(this->WritePhotoSample(sampleDuration, pCurrentBitmap, streamIndex, &startTime));
		this->m_logFileStream << "照片 " << pPhoto->GetFile().c_str() << " 的静态示例编写成功." << endl;

		// 释放与此照片关联的资源.
		delete pPreviousBitmap;
		pPreviousBitmap = pCurrentBitmap;
	}
	hr = this->m_pSinkWriter->Finalize();
	this->m_logFileStream << "提交成功." << endl;
	CheckHR(hr);
	CheckHR(MFShutdown());

cleanup:
	if (pPreviousBitmap != pCurrentBitmap && pPreviousBitmap != nullptr)
	{
		delete pPreviousBitmap;
		pPreviousBitmap = nullptr;
	}
	if (pCurrentBitmap != nullptr)
	{
		delete pCurrentBitmap;
		pCurrentBitmap = nullptr;
	}
	this->Dispose();
	return hr;
}

HRESULT VideoEncoder::Initialize()
{
	HRESULT hr = S_OK;

	this->m_logFileStream = ofstream(this->m_logFile);

	if (this->m_inputFile == L"")
	{
		this->m_logFileStream << "输入短影配置文件无效" << endl;
		return ERROR_FILE_INVALID;
	}

	// 初始化 COM.
	hr = CoInitializeEx(nullptr, COINIT_MULTITHREADED);

	// COM 未被调用代码初始化. 因此我们初始化它.
	if (SUCCEEDED(hr))
	{
		this->m_logFileStream << "COM初始化成功." << endl;
		this->m_COMInitializedInDll = true;
	}

	// COM被调用代码初始化.
	else if (hr == RPC_E_CHANGED_MODE || hr == S_FALSE)
	{
		this->m_COMInitializedInDll = false;
	}

	// COM初始化失败.
	else
	{
		return hr;
	}

	// 创建WIC 工厂
	CheckHR(CoCreateInstance(
		CLSID_WICImagingFactory,
		nullptr,
		CLSCTX_INPROC_SERVER,
		IID_PPV_ARGS(&this->m_pIWICFactory)
		));

	// 启动Media Foundation.
	CheckHR(MFStartup(MF_VERSION));
		
cleanup:
	if (!SUCCEEDED(hr))
	{
		DWORD error = GetLastError();
		this->m_logFileStream << "意外错误: " << error << endl;
	}
	return hr;
}

// 创建sink writer. 返回流索引.
HRESULT VideoEncoder::CreateSinkWriter(IMFSinkWriter** ppSinkWriter, DWORD* pStreamIndex)
{
	HRESULT hr = S_OK;
	if (this->m_outputFile == L"")
	{
		return ERROR_FILE_INVALID;
	}

	// 创建sink writer.
	*ppSinkWriter = nullptr;	
	IMFSinkWriter *pSinkWriter = nullptr;
	IMFMediaType* pOutputMediaType = nullptr;
	IMFMediaType *pInMediaType = nullptr;   
	CheckHR(MFCreateSinkWriterFromURL(this->m_outputFile.c_str(), nullptr, nullptr, &pSinkWriter));

	// 创建和配置输出媒体类型.
	CheckHR(MFCreateMediaType(&pOutputMediaType));
	CheckHR(pOutputMediaType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video));
	CheckHR(pOutputMediaType->SetGUID(MF_MT_SUBTYPE, this->m_outputVideoFormat));
	CheckHR(pOutputMediaType->SetUINT32(MF_MT_AVG_BITRATE, this->m_videoBitRate));
	CheckHR(pOutputMediaType->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive));

	CheckHR(MFSetAttributeSize(pOutputMediaType, MF_MT_FRAME_SIZE, this->m_frameWidth, this->m_frameHeight));
	CheckHR(MFSetAttributeRatio(pOutputMediaType, MF_MT_FRAME_RATE, (UINT32)this->m_fps, 1));
	CheckHR(MFSetAttributeRatio(pOutputMediaType, MF_MT_PIXEL_ASPECT_RATIO, 1, 1));
	DWORD streamIndex;
	CheckHR(pSinkWriter->AddStream(pOutputMediaType, &streamIndex));

	// 设置输入的媒体类型.
    CheckHR(MFCreateMediaType(&pInMediaType));   
    CheckHR(pInMediaType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video));   
    CheckHR(pInMediaType->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_RGB32));     
    CheckHR(pInMediaType->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive)); 

	// 输入的步幅信息不为所有输出编码解码器需要.但某些编解码器需要它，如 H.264.
	// 如果步幅是去掉，或设置为负值，H.264 将从下到上处理图像.
	CheckHR(pInMediaType->SetUINT32(MF_MT_DEFAULT_STRIDE, this->m_frameStride));
    CheckHR(MFSetAttributeSize(pInMediaType, MF_MT_FRAME_SIZE, this->m_frameWidth, this->m_frameHeight));
    CheckHR(MFSetAttributeRatio(pInMediaType, MF_MT_FRAME_RATE, (UINT32)this->m_fps, 1));   
    CheckHR(MFSetAttributeRatio(pInMediaType, MF_MT_PIXEL_ASPECT_RATIO, 1, 1));
    CheckHR(pSinkWriter->SetInputMediaType(streamIndex, pInMediaType, nullptr));   

	// 开始编写.
	CheckHR(pSinkWriter->BeginWriting());

	*ppSinkWriter = pSinkWriter;
	(*ppSinkWriter)->AddRef();
	*pStreamIndex = streamIndex;

cleanup:
	if (!SUCCEEDED(hr))
	{
		DWORD error = GetLastError();
		this->m_logFileStream << "意外错误: " << error << endl;
	}
	SafeRelease(&pSinkWriter);
	SafeRelease(&pOutputMediaType);
	return hr;
}

// 解析输入xml文件.
// TODO: 目前我们未执行类型检查.
// 将在下个版本中加入.
HRESULT VideoEncoder::ParseInputXml()
{
	HRESULT hr = S_OK;
	wstring photoCountString = wstring();
	int photoCount = 0;
	XmlParser* pXmlParser;

	CheckHR(XmlParser::Create(&pXmlParser, this->m_inputFile));
	CheckHR(pXmlParser->ReadDeclaration());
	CheckHR(pXmlParser->ReadStartElement());
	photoCountString = pXmlParser->GetAttributes()[L"PhotoCount"];
	photoCount = _wtoi(photoCountString.c_str());
	for (int i = 0; i < photoCount; i++)
	{
		CheckHR(pXmlParser->ReadStartElement());
		Photo* photo = new Photo();
		photo->SetFile(pXmlParser->GetAttributes()[L"Name"]);
		photo->SetPhotoDuration(_wtoi(pXmlParser->GetAttributes()[L"PhotoDuration"].c_str()));

	    // 检查是否有照片没有包含特效.首先尝试读取结束元素的图片.
		HRESULT hrTemp = pXmlParser->ReadEndElement();

		// E_NOTENDELEMENT 意味着我们不能读取结束因为有子元素（特效元素）的元素.
		// 所以让我们来分析特效元素.
		// 否则结束元素的照片已被成功读取 .
		if (hrTemp == E_NOTENDELEMENT)
		{
			CheckHR(pXmlParser->ReadStartElement());
			wstring transitionName = pXmlParser->GetAttributes()[L"Name"];
			photo->SetTransitionName(transitionName);
			photo->SetTransitionDuration(_wtoi(pXmlParser->GetAttributes()[L"Duration"].c_str()));
			TransitionBase* pTransition = nullptr;
			TransitionFactory::CreateTransition(
				transitionName,
				&pTransition,
				this->m_frameWidth,
				this->m_frameHeight);
			if (pTransition != nullptr)
			{
				pTransition->SetFrameWidth(this->m_frameWidth);
				pTransition->SetFrameHeight(this->m_frameHeight);
				pTransition->ParseXml(pXmlParser);
				photo->SetTransition(pTransition);
			}

			// 特效元素结束.
			CheckHR(pXmlParser->ReadEndElement());

			// 照片元素结束.
			CheckHR(pXmlParser->ReadEndElement());
		}
		this->m_photos.push_back(photo);
	}

cleanup:
	if (!SUCCEEDED(hr))
	{
		DWORD error = GetLastError();
		this->m_logFileStream << "意外错误: " << error << endl;
	}
	if (pXmlParser != nullptr)
    {
        delete pXmlParser;
        pXmlParser = nullptr;
    }
	return hr;
}

// 解码照片的基础位图文件的.
HRESULT VideoEncoder::DecodeFrame(Photo* pPhoto, BYTE** ppOutputBitmap, int* pBitmapSize)
{
	HRESULT hr = S_OK;

	IWICBitmapDecoder *pDecoder  = nullptr;
	IWICBitmapFrameDecode *pFrame  = nullptr;
	IWICBitmap* pSourceBitmap = nullptr;
	IWICBitmapLock* pLock = nullptr;
	BYTE* pSourceBuffer = nullptr;
	BYTE* pDestinationBuffer = nullptr;
	UINT pixelWidth;
	UINT pixelHeight;
	WICRect lockRect;

	*ppOutputBitmap = nullptr;
		hr = m_pIWICFactory->CreateDecoderFromFilename(
		pPhoto->GetFile().c_str(),
		nullptr,
		GENERIC_READ,
		WICDecodeMetadataCacheOnDemand,
		&pDecoder
		);
	CheckHR(hr);
	this->m_logFileStream << "WIC解码器创建成功." << endl;
	GUID containerFormat;
	CheckHR(pDecoder->GetContainerFormat(&containerFormat));

	// 我们仅支持jpg 文件.
	if (containerFormat != GUID_ContainerFormatJpeg)
	{
		this->m_logFileStream << "仅支持jpeg文件." << endl;
		return E_NOTSUPPORTEDFORMAT;
	}

	// 我们仅支持jpg 文件. 因此只有一桢.
	CheckHR(pDecoder->GetFrame(0, &pFrame));

	// TODO: 目前我们需要所有照片有相同的大小.
	// 如果需求在将来发生变化，修改代码.
	pFrame->GetSize(&pixelWidth, &pixelHeight);
	if (pixelWidth != this->m_frameWidth || pixelHeight != this->m_frameHeight)
	{
		this->m_logFileStream << "所有的照片必须使用固定的大小." << endl;
		return E_IMAGESIZEINCORRECT;
	}

	// 创建源位图对象.
	CheckHR(this->m_pIWICFactory->CreateBitmapFromSource(pFrame, WICBitmapCacheOnLoad, &pSourceBitmap));
	this->m_logFileStream << "位图资源创建成功." << endl;

	lockRect.X = 0;
	lockRect.Y = 0;
	lockRect.Width = pixelWidth;
	lockRect.Height = pixelHeight;
	CheckHR(pSourceBitmap->Lock(&lockRect, WICBitmapLockWrite, &pLock));
	UINT sourceBufferSize;
	CheckHR(pLock->GetDataPointer(&sourceBufferSize, &pSourceBuffer));

	// Jpg BGR 位图转换 RGBA 格式.
	UINT destinationBufferSize = sourceBufferSize / 3 * 4;
	pDestinationBuffer = new BYTE[destinationBufferSize];
	for (UINT i = 0, j = 0; i < sourceBufferSize; i+=3, j+=4)
	{
		pDestinationBuffer[j] = pSourceBuffer[i]; // R
		pDestinationBuffer[j + 1] = pSourceBuffer[i + 1]; // G
		pDestinationBuffer[j + 2] = pSourceBuffer[i + 2]; // B
		pDestinationBuffer[j + 3] = 255; // A
	}

	*ppOutputBitmap = pDestinationBuffer;
	*pBitmapSize = destinationBufferSize;

cleanup:
	if (!SUCCEEDED(hr))
	{
		DWORD error = GetLastError();
		this->m_logFileStream << "意外错误: " << error << endl;
		this->m_logFileStream << "HResult是: " << hr << endl;
	}
	if (pSourceBuffer != nullptr)
	{
		// 删除pSourceBuffer;
	}
	SafeRelease(&pDecoder);
	SafeRelease(&pFrame);
	SafeRelease(&pSourceBitmap);
	SafeRelease(&pLock);

	return hr;
}

HRESULT VideoEncoder::WritePhotoSample(UINT64 sampleDuration, BYTE* pBitmap, DWORD streamIndex, LONGLONG* startTime)
{
	HRESULT hr = S_OK;
	IMFMediaBuffer* pMediaBuffer = nullptr;
	BYTE* pFrameBuffer = nullptr;
	IMFSample* pSample = nullptr;

	for (DWORD i = 0; i < sampleDuration; i++)
    {
		CheckHR(MFCreateMemoryBuffer(this->m_frameBufferSize, &pMediaBuffer));
		pMediaBuffer->Lock(&pFrameBuffer, nullptr, nullptr);
		CheckHR(MFCopyImage(pFrameBuffer, this->m_frameStride, pBitmap, this->m_frameStride, this->m_frameStride, this->m_frameHeight));
		CheckHR(pMediaBuffer->Unlock());
		CheckHR(pMediaBuffer->SetCurrentLength(this->m_frameBufferSize));
		CheckHR(MFCreateSample(&pSample));
		CheckHR(pSample->AddBuffer(pMediaBuffer));
		CheckHR(pSample->SetSampleTime(*startTime));
		CheckHR(pSample->SetSampleDuration(this->GetFrameDuration()));
		CheckHR(this->m_pSinkWriter->WriteSample(streamIndex, pSample));
		(*startTime) += this->GetFrameDuration();

	    // 释放示例资源.
		SafeRelease(&pMediaBuffer);
		SafeRelease(&pSample);
	}

cleanup:
	if (!SUCCEEDED(hr))
	{
		DWORD error = GetLastError();
		this->m_logFileStream << "意外错误: " << error << endl;
	}
	SafeRelease(&pMediaBuffer);
	SafeRelease(&pSample);
	return hr;
}

HRESULT VideoEncoder::WriteTransitionSample(UINT64 sampleDuration, TransitionBase* pTransition, DWORD streamIndex, LONGLONG* startTime)
{
	HRESULT hr = S_OK;
	IMFMediaBuffer* pMediaBuffer = nullptr;
	BYTE* pFrameBuffer = nullptr;
	IMFSample* pSample = nullptr;
	BYTE* pOutputFrame = nullptr;

	for (DWORD i = 0; i < sampleDuration; i++)
    {
		CheckHR(MFCreateMemoryBuffer(this->m_frameBufferSize, &pMediaBuffer));
		pMediaBuffer->Lock(&pFrameBuffer, nullptr, nullptr);
		float time = (float)i / (float)sampleDuration;
		pOutputFrame = pTransition->GetOutputFrame(time);
		CheckHR(MFCopyImage(pFrameBuffer, this->m_frameStride, pOutputFrame, this->m_frameStride, this->m_frameStride, this->m_frameHeight));
		CheckHR(pMediaBuffer->Unlock());
		CheckHR(pMediaBuffer->SetCurrentLength(this->m_frameBufferSize));
		CheckHR(MFCreateSample(&pSample));
		CheckHR(pSample->AddBuffer(pMediaBuffer));
		CheckHR(pSample->SetSampleTime(*startTime));
		CheckHR(pSample->SetSampleDuration(this->GetFrameDuration()));
		CheckHR(this->m_pSinkWriter->WriteSample(streamIndex, pSample));
		(*startTime) += this->GetFrameDuration();

		// 释放示例资源.
		SafeRelease(&pMediaBuffer);
		SafeRelease(&pSample);
		if (pOutputFrame != nullptr)
		{
			delete pOutputFrame;
			pOutputFrame = nullptr;
		}
	}

cleanup:
	if (!SUCCEEDED(hr))
	{
		DWORD error = GetLastError();
		this->m_logFileStream << "意外错误: " << error << endl;
	}
	SafeRelease(&pMediaBuffer);
	SafeRelease(&pSample);
	if (pOutputFrame != nullptr)
	{
		delete pOutputFrame;
		pOutputFrame = nullptr;
	}
	return hr;
}