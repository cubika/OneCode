/*********************************** 模块头 ***********************************\
* 模块名:  FlyinTransition.cpp
* 项目名:  NativeVideoEncoder
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

#include "StdAfx.h"
#include "FlyinTransition.h"


FlyinTransition::FlyinTransition(void)
	: m_direction(Left)
{
}


FlyinTransition::~FlyinTransition(void)
{
}

BYTE* FlyinTransition::GetOutputFrame(float time)
{
	if (this->m_backgroundFrame == nullptr || this->m_foregroundFrame == nullptr)
	{
		return nullptr;
	}
	this->m_outputFrame = new BYTE[this->GetFrameSize()];
	for (int y = 0; y < this->m_frameHeight; y++)
	{
		for (int x = 0; x < this->m_frameWidth; x++)
		{
			int startPixel = (y * this->m_frameWidth + x) * 4;
			if (this->m_direction == Left)
			{
				// 应使用多少像素来填充背景图像.
				int backgroundCoveredPixels = (int)(time * this->m_frameWidth);

				// 多少背景图像的像素数需要转换.
				int xTranslated = (int)((1 - time) * this->m_frameWidth);

				// 必须转换的源中的开始像素.
				int startPixelInSource  = (y * this->m_frameWidth + (x + xTranslated)) * 4;

				// 应使用背景图像来填充此像素.
				if (x < backgroundCoveredPixels)
				{
					// 必须转换背景图像.
					this->m_outputFrame[startPixel] = this->m_backgroundFrame[startPixelInSource]; //R
					this->m_outputFrame[startPixel + 1] = this->m_backgroundFrame[startPixelInSource + 1]; //G
					this->m_outputFrame[startPixel + 2] = this->m_backgroundFrame[startPixelInSource + 2]; //B
					this->m_outputFrame[startPixel + 3] = 255; //A
				}

				// 应使用前景图像来填充此像素.
				else
				{
					// 前景图像不需要转换.
					this->m_outputFrame[startPixel] = this->m_foregroundFrame[startPixel]; //R
					this->m_outputFrame[startPixel + 1] = this->m_foregroundFrame[startPixel + 1]; //G
					this->m_outputFrame[startPixel + 2] = this->m_foregroundFrame[startPixel + 2]; //B
					this->m_outputFrame[startPixel + 3] = 255; //A
				}
			}

			else if (this->m_direction == Right)
			{
				// 应使用多少像素来填充前景图像.
				int foregroundCoveredPixels = (int)((1 - time) * this->m_frameWidth);

				// 多少背景图像的像素数需要转换.
				int xTranslated = (int)((time - 1) * this->m_frameWidth);

				// 必须转换的源中的开始像素.
				int startPixelInSource  = (y * this->m_frameWidth + (x + xTranslated)) * 4;

				// 应使用背景图像来填充此像素.
				if (x < foregroundCoveredPixels)
				{
					// 前景图像不需要转换.
					this->m_outputFrame[startPixel] = this->m_foregroundFrame[startPixel]; //R
					this->m_outputFrame[startPixel + 1] = this->m_foregroundFrame[startPixel + 1]; //G
					this->m_outputFrame[startPixel + 2] = this->m_foregroundFrame[startPixel + 2]; //B
					this->m_outputFrame[startPixel + 3] = 255; //A
				}

				// 应使用前景图像来填充此像素.
				else
				{
					/// 必须转换背景图像.					
					this->m_outputFrame[startPixel] = this->m_backgroundFrame[startPixelInSource]; //R
					this->m_outputFrame[startPixel + 1] = this->m_backgroundFrame[startPixelInSource + 1]; //G
					this->m_outputFrame[startPixel + 2] = this->m_backgroundFrame[startPixelInSource + 2]; //B
					this->m_outputFrame[startPixel + 3] = 255; //A
				}
			}

			else if (this->m_direction == Up)
			{
				// 应使用多少像素来填充背景图像.
				int backgroundCoveredPixels = (int)(time * this->m_frameHeight);

				// 多少背景图像的像素数需要转换.
				int yTranslated = (int)((1 - time) * this->m_frameHeight);

				// 必须转换的源中的开始像素.
				int startPixelInSource  = ((y + yTranslated) * this->m_frameWidth + x) * 4;

				// 应使用背景图像来填充此像素.
				if (y < backgroundCoveredPixels)
				{
					// 必须转换背景图像.
					this->m_outputFrame[startPixel] = this->m_backgroundFrame[startPixelInSource]; //R
					this->m_outputFrame[startPixel + 1] = this->m_backgroundFrame[startPixelInSource + 1]; //G
					this->m_outputFrame[startPixel + 2] = this->m_backgroundFrame[startPixelInSource + 2]; //B
					this->m_outputFrame[startPixel + 3] = 255; //A
				}

				// 应使用前景图像来填充此像素.
				else
				{
					// 前景图像不需要转换.
					this->m_outputFrame[startPixel] = this->m_foregroundFrame[startPixel]; //R
					this->m_outputFrame[startPixel + 1] = this->m_foregroundFrame[startPixel + 1]; //G
					this->m_outputFrame[startPixel + 2] = this->m_foregroundFrame[startPixel + 2]; //B
					this->m_outputFrame[startPixel + 3] = 255; //A
				}
			}

			else if (this->m_direction == Down)
			{
				// 应使用多少像素来填充前景图像.
				int foregroundCoveredPixels = (int)((1 - time) * this->m_frameHeight);

				// 多少背景图像的像素数需要转换.
				int yTranslated = (int)((time - 1) * this->m_frameHeight);

				// 必须转换的源中的开始像素.
				int startPixelInSource  = ((y + yTranslated) * this->m_frameWidth + x) * 4;

				// 应使用背景图像来填充此像素.
				if (y < foregroundCoveredPixels)
				{
					// 前景图像不需要转换.
					this->m_outputFrame[startPixel] = this->m_foregroundFrame[startPixel]; //R
					this->m_outputFrame[startPixel + 1] = this->m_foregroundFrame[startPixel + 1]; //G
					this->m_outputFrame[startPixel + 2] = this->m_foregroundFrame[startPixel + 2]; //B
					this->m_outputFrame[startPixel + 3] = 255; //A
				}

				// 应使用前景图像来填充此像素.
				else
				{
					/// 必须转换背景图像.					
					this->m_outputFrame[startPixel] = this->m_backgroundFrame[startPixelInSource]; //R
					this->m_outputFrame[startPixel + 1] = this->m_backgroundFrame[startPixelInSource + 1]; //G
					this->m_outputFrame[startPixel + 2] = this->m_backgroundFrame[startPixelInSource + 2]; //B
					this->m_outputFrame[startPixel + 3] = 255; //A
				}
			}
		}
	}
	return this->m_outputFrame;
}

void FlyinTransition::ParseXml(XmlParser* pParser)
{
	wstring direction = pParser->GetAttributes()[L"Direction"];
	if (direction.compare(L"Left") == 0)
	{
		this->m_direction = Left;
	}
	if (direction.compare(L"Right") == 0)
	{
		this->m_direction = Right;
	}
	if (direction.compare(L"Up") == 0)
	{
		this->m_direction = Up;
	}
	if (direction.compare(L"Down") == 0)
	{
		this->m_direction = Down;
	}
}