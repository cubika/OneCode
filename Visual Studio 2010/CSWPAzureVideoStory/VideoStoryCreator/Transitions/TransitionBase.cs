/********************************* 模块头 *********************************\
* 模块名: TransitionBase.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 提供ITransition默认实现的基类.
* 特效类可以继承此基类，
* 或者它直接实现 ITransition.
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
    public abstract class TransitionBase : ITransition
    {
        public virtual event EventHandler TransitionCompleted;

        public abstract string Name { get; }
        public virtual FrameworkElement ForegroundElement { get; set; }
        public virtual FrameworkElement BackgroundElement { get; set; }
        public virtual TimeSpan TransitionDuration { get; set; }

        public virtual bool ImageZIndexModified
        {
            get { return false; }
        }

        public abstract void Begin();
        public abstract void Stop();

        public virtual void Save(XElement transitionElement)
        {
            transitionElement.Add(new XAttribute("Name", this.Name));
            transitionElement.Add(new XAttribute("Duration", this.TransitionDuration.TotalSeconds));
        }

        public static ITransition Load(XElement transitionElement)
        {
            string name = transitionElement.Attribute("Name").Value;
            ITransition transition = TransitionFactory.CreateTransition(name);
            if (transition != null)
            {
                try
                {
                    string durationString = transitionElement.Attribute("Duration").Value;
                    int duration = int.Parse(durationString);
                    transition.TransitionDuration = TimeSpan.FromSeconds(duration);

                    // Todo: 需要复查设计. 我们不希望在ITransition暴露LoadChild.
                    // 但是将ITransition转换为TransitionBase并调用LoadChild可能不是个很好的设计.
                    ((TransitionBase)transition).LoadChild(transitionElement);
                }
                catch
                {
                    throw new Exception("无法载入特效.");
                }
            }
            return transition;
        }

        protected virtual void LoadChild(XElement transitionElement)
        { 
        }

        public abstract ITransition Clone();
    }
}
