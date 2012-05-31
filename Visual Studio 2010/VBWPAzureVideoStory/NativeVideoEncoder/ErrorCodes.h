/****************************** 模块头 ******************************\
* 模块名:	ErrorCodes.h
* 项目名: NativeVideoEncoder
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
// MessageId: E_NOTSTARTELEMENT
//
// MessageText:
//
// When reading start element, encountered a non start element.
//
#define E_NOTSTARTELEMENT                     _HRESULT_TYPEDEF_(0x81000001L)

//
// MessageId: E_NOTENDELEMENT
//
// MessageText:
//
// When reading end element, encountered a non end element.
//
#define E_NOTENDELEMENT                     _HRESULT_TYPEDEF_(0x81000002L)

//
// MessageId: E_NOTSUPPORTEDFORMAT
//
// MessageText:
//
// The image is not in a supported format. Currenly only jpg images are suppoted.
//
#define E_NOTSUPPORTEDFORMAT                     _HRESULT_TYPEDEF_(0x81000003L)

//
// MessageId: E_IMAGESIZEINCORRECT
//
// MessageText:
//
// Currenly we require all images to be of the same size.
//
#define E_IMAGESIZEINCORRECT                     _HRESULT_TYPEDEF_(0x81000004L)

