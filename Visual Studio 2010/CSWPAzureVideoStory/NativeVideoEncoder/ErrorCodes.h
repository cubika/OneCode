/*********************************** 模块头 ***********************************\
* 模块名:  ErrorCodes.h
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 自定义的 HRESULT 错误代码.
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
//
// 消息 Id： E_NOTSTARTELEMENT
//
// 消息文本：
//
// 阅读开始元素，遇到非开始元素.
//
#define E_NOTSTARTELEMENT                     _HRESULT_TYPEDEF_(0x81000001L)

//
// 消息 Id： E_NOTENDELEMENT
//
// 消息文本：
//
// 阅读结束元素，遇到非结束元素。
//
#define E_NOTENDELEMENT                     _HRESULT_TYPEDEF_(0x81000002L)

//
//  消息 Id： E_NOTSUPPORTEDFORMAT
//
// 消息文本：
//
// 图像是不受支持的格式.目前只支持 jpg 图像.
//
#define E_NOTSUPPORTEDFORMAT                     _HRESULT_TYPEDEF_(0x81000003L)

//
// 消息 Id： E_IMAGESIZEINCORRECT
//
// 消息文本：
//
// 我们目前需要有所有的图像相同大小。 
//
#define E_IMAGESIZEINCORRECT                     _HRESULT_TYPEDEF_(0x81000004L)

