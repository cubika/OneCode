/****************************** 模块头 ******************************\
* 模块名:	FlyinTransition.h
* 项目名: NativeVideoEncoder
* 版权 (c) Microsoft Corporation.
* 
* 实现简单的飞入特效.
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
#include "transitionbase.h"
#include "string"

using namespace std;

enum FlyDirection
{
	Left,
	Right,
	Up,
	Down
};

class FlyinTransition :
	public TransitionBase
{
public:
	FlyinTransition(void);
	~FlyinTransition(void);

	BYTE* GetOutputFrame(float time);
	void ParseXml(XmlParser* pParser);

	FlyDirection GetDirection()
	{
		return this->m_direction;
	}

	void SetDirection(FlyDirection value)
	{
		this->m_direction = value;
	}

private:
	FlyDirection m_direction;
};
