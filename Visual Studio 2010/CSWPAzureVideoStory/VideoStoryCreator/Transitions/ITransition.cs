/********************************* 模块头 *********************************\
* 模块名: FlyInTransition.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
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


using System;
using System.Windows;
using System.Xml.Linq;

namespace VideoStoryCreator.Transitions
{
    public interface ITransition
    {
        string Name { get; }
        TimeSpan TransitionDuration { get; set; }
        bool ImageZIndexModified { get; }

        // Foreground/BackgroundElement 可以是Image 或 MediaElement.
        // 在PreviewPage中设置这些属性.
        FrameworkElement ForegroundElement { get; set; }
        FrameworkElement BackgroundElement { get; set; }

        event EventHandler TransitionCompleted;

        ITransition Clone();

        // 开始/停止特效.
        void Begin();
        void Stop();

        // 序列化/饭序列化特效.
        void Save(XElement transitionElement);
    }
}
