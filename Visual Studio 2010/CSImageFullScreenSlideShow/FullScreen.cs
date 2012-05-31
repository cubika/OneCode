/********************************** 模块头 ******************************************\
* 模块名:  FullScreen.cs
* 项目名:  CSImageFullScreenSlideShow
* 版权 (c) Microsoft Corporation.
*
* 该类定义了两个辅助方法，用于使Windows窗体进入全屏模式和离开全屏模式. 
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using System.Windows.Forms;
using System.Drawing;


namespace CSImageFullScreenSlideShow
{
    public class FullScreen
    {
        private FormWindowState winState;
        private FormBorderStyle brdStyle;
        private bool topMost;
        private Rectangle bounds;

        public FullScreen()
        {
            IsFullScreen = false;
        }

        public bool IsFullScreen
        {
            get;
            set;
        }

        /// <summary>
        /// 将窗口最大化至全屏.
        /// </summary>
        public void EnterFullScreen(Form targetForm)
        {
            if (!IsFullScreen)
            {
                Save(targetForm);  // 保存原始的窗体状态.

                targetForm.WindowState = FormWindowState.Maximized;
                targetForm.FormBorderStyle = FormBorderStyle.None;
                targetForm.TopMost = true;
                targetForm.Bounds = Screen.GetBounds(targetForm);

                IsFullScreen = true;
            }
        }

        /// <summary>
        /// 保存当前的窗体状态.
        /// </summary>
        private void Save(Form targetForm)
        {
            winState = targetForm.WindowState;
            brdStyle = targetForm.FormBorderStyle;
            topMost = targetForm.TopMost;
            bounds = targetForm.Bounds;
        }

        /// <summary>
        /// 离开全屏模式,回到原始的窗口状态.
        /// </summary>
        public void LeaveFullScreen(Form targetForm)
        {
            if (IsFullScreen)
            {
                // 回到原始的窗口状态.
                targetForm.WindowState = winState;
                targetForm.FormBorderStyle = brdStyle;
                targetForm.TopMost = topMost;
                targetForm.Bounds = bounds;

                IsFullScreen = false;
            }
        }
    }
}