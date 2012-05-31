/*********************************** 模块头 ***********************************\
* 模块名:  TransitionFactory.h
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 提供了一些默认实现的基类. 用于创建特效效果的工厂类.
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
#include "TransitionFactory.h"
#include "FadeTransition.h"
#include "FlyinTransition.h"

TransitionFactory::TransitionFactory(void)
{
}


TransitionFactory::~TransitionFactory(void)
{
}

//  因为 c++ 不支持反射，我们不得不使用 if 块 ...
void TransitionFactory::CreateTransition(wstring transitionName, TransitionBase** output, int frameWidth, int frameHeight)
{
	*output = nullptr;
	if (transitionName == L"Fade Transition")
	{
		*output = new FadeTransition();
	}
	else if (transitionName == L"Fly In Transition")
	{
		*output = new FlyinTransition();
	}
	// 将添加更多特效.

	if ((*output) != nullptr)
	{
		(*output)->SetFrameWidth(frameWidth);
		(*output)->SetFrameHeight(frameHeight);
	}
}