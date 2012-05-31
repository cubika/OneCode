/*********************************** 模块头 ***********************************\
* 模块名:  FadeTransition.cpp
* 项目名:  NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 实现简单的淡入淡出特效.
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
#include "FadeTransition.h"


FadeTransition::FadeTransition(void)
{
}


FadeTransition::~FadeTransition(void)
{
}

BYTE* FadeTransition::GetOutputFrame(float time)
{
	if (this->m_backgroundFrame == nullptr || this->m_foregroundFrame == nullptr)
	{
		return nullptr;
	}
	this->m_outputFrame = new BYTE[this->GetFrameSize()];
	for (int i = 0; i < this->GetFrameSize(); i += 4)
	{
		//  只是计算混合的像素的颜色.
		this->m_outputFrame[i] = (BYTE)(this->m_backgroundFrame[i] * time + this->m_foregroundFrame[i] * (1 - time)); //R
		this->m_outputFrame[i + 1] = (BYTE)(this->m_backgroundFrame[i + 1] * time + this->m_foregroundFrame[i + 1] * (1 - time)); //G
		this->m_outputFrame[i + 2] = (BYTE)(this->m_backgroundFrame[i + 2] * time + this->m_foregroundFrame[i + 2] * (1 - time)); //B
		this->m_outputFrame[i + 3] = 255; // A
	}
	return this->m_outputFrame;
}