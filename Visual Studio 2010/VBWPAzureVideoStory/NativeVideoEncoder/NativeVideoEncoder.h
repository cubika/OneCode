/****************************** 模块头 ******************************\
* 模块名:	NativeVideoEncoder.h
* 项目名: NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* dll入口.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the NATIVEVIDEOENCODER_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// NATIVEVIDEOENCODER_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef NATIVEVIDEOENCODER_EXPORTS
#define NATIVEVIDEOENCODER_API __declspec(dllexport)
#else
#define NATIVEVIDEOENCODER_API __declspec(dllimport)
#endif

extern "C"
{
	NATIVEVIDEOENCODER_API HRESULT EncoderVideo(wchar_t* pInputFile, wchar_t* pOutputFile, wchar_t* pLogFile);
}
