/*********************************** 模块头 ***********************************\
* 模块名:  NativeVideoEncoder.h
* 项目名:  NativeVideoEncoder
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

#ifdef NATIVEVIDEOENCODER_EXPORTS
#define NATIVEVIDEOENCODER_API __declspec(dllexport)
#else
#define NATIVEVIDEOENCODER_API __declspec(dllimport)
#endif

extern "C"
{
	NATIVEVIDEOENCODER_API HRESULT EncoderVideo(wchar_t* pInputFile, wchar_t* pOutputFile, wchar_t* pLogFile);
}
