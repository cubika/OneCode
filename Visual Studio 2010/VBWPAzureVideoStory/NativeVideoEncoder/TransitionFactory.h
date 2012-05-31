/*********************************** 模块头 ***********************************\
* 模块名:  TransitionFactory.h
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 提供了一些默认实现的基类.用于创建特效效果的工厂类.
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
#include "string"
#include "TransitionBase.h"

using namespace std;

class TransitionFactory
{
public:
	TransitionFactory(void);
	~TransitionFactory(void);

	static void CreateTransition(wstring transitionName, TransitionBase** output, int frameWidth, int frameHeight);
};

