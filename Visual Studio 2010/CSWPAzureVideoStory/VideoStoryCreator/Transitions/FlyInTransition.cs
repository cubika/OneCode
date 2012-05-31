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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace VideoStoryCreator.Transitions
{
    public class FlyInTransition : TransitionBase
    {
        private Storyboard _transitionStoryboard;
        private DoubleAnimationUsingKeyFrames _flyAnimation;
        private FlyDirection _direction;
        private FrameworkElement _backgroundElement;
        private FrameworkElement _foregroundElement;
        public override event EventHandler TransitionCompleted;

        public FlyInTransition()
        {
            this.TransitionDuration = TimeSpan.FromSeconds(2d);
            this._transitionStoryboard = new Storyboard();
            this._flyAnimation = new DoubleAnimationUsingKeyFrames();
            this._transitionStoryboard.Children.Add(_flyAnimation);
            this.Direction = FlyDirection.Left;
            this._transitionStoryboard.Completed += new EventHandler(TransitionStoryboard_Completed);
        }

        public override string Name
        {
            get { return "Fly In Transition"; }
        }

        public FlyDirection Direction
        {
            get { return this._direction; }
            set
            {
                this._direction = value;

                // 重置关键帧.
                this._flyAnimation.KeyFrames.Clear();
                EasingDoubleKeyFrame frame1 = null;
                EasingDoubleKeyFrame frame2 = null;

                // 如果BackgroundElement为空,稍后添加关键帧.
                if (this.BackgroundElement != null)
                {
                    frame1 = new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) };
                    frame2 = new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(this.TransitionDuration) };
                }

                // 根据飞入方向配置动画属性.
                if (value == FlyDirection.Left || value == FlyDirection.Right)
                {
                    Storyboard.SetTargetProperty(this._flyAnimation, new PropertyPath("RenderTransform.(TranslateTransform.X)"));
                    if (this.BackgroundElement != null)
                    {
                        frame1.Value = (value == FlyDirection.Left) ? -this.BackgroundElement.ActualWidth : this.BackgroundElement.ActualWidth;
                        frame2.Value = 0;
                    }
                }
                else
                {
                    Storyboard.SetTargetProperty(this._flyAnimation, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));
                    if (this.BackgroundElement != null)
                    {
                        frame1.Value = (value == FlyDirection.Up) ? this.BackgroundElement.ActualHeight : -this.BackgroundElement.ActualHeight;
                        frame2.Value = 0;
                    }
                }

                if (this.BackgroundElement != null)
                {
                    this._flyAnimation.KeyFrames.Add(frame1);
                    this._flyAnimation.KeyFrames.Add(frame2);
                }
            }
        }

        public override FrameworkElement BackgroundElement
        {
            get
            {
                return this._backgroundElement;
            }
            set
            {
                if (value != null)
                {
                    this._backgroundElement = value;
                    this._backgroundElement.SetValue(Canvas.ZIndexProperty, 2);
                    Storyboard.SetTarget(this._flyAnimation, value);
                    this._backgroundElement.RenderTransform = new TranslateTransform();

                    // 重置关键帧.
                    this._transitionStoryboard.Stop();
                    this._flyAnimation.KeyFrames.Clear();
                    EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) }; ;
                    EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(this.TransitionDuration) };

                    //根据飞入方向配置动画属性.
                    if (this.Direction == FlyDirection.Left || this.Direction == FlyDirection.Right)
                    {
                        Storyboard.SetTargetProperty(this._flyAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                        frame1.Value = (this.Direction == FlyDirection.Left) ? -value.ActualWidth : value.ActualWidth;
                        frame2.Value = 0;
                    }
                    else
                    {
                        Storyboard.SetTargetProperty(this._flyAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                        frame1.Value = (this.Direction == FlyDirection.Up) ? -value.ActualHeight : value.ActualHeight;
                        frame2.Value = 0;
                    }

                    this._flyAnimation.KeyFrames.Add(frame1);
                    this._flyAnimation.KeyFrames.Add(frame2);
                }
            }
        }

        public override FrameworkElement ForegroundElement
        {
            get { return this._foregroundElement; }
            set
            {
                if (value != null)
                {
                    this._foregroundElement = value;
                    if (value != null)
                    {
                        this._foregroundElement.SetValue(Canvas.ZIndexProperty, 0);
                    }
                }
            }
        }

        public override bool ImageZIndexModified
        {
            get
            {
                // 特效替换背景/前景图片的z轴.
                // 因此必须返回True.
                return true;
            }
        }

        public override void Begin()
        {
            this._transitionStoryboard.Begin();
        }

        public override void Stop()
        {
            this._transitionStoryboard.Stop();
        }

        void TransitionStoryboard_Completed(object sender, EventArgs e)
        {
            // 重置不需要的特效.
            this.BackgroundElement.RenderTransform = null;
            if (this.TransitionCompleted != null)
            {
                this.TransitionCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        // 特效替换背景/前景图片的z轴.
        // 因此必须返回True.
        /// </summary>
        public override void Save(XElement transitionElement)
        {
            transitionElement.Add(new XAttribute("Direction", this.Direction));
            base.Save(transitionElement);
        }

        /// <summary>
        /// 反序列化此特效专属属性.
        /// 即 FlyDirection 属性.
        /// </summary>
        protected override void LoadChild(XElement transitionElement)
        {
            try
            {
                this.Direction = (FlyDirection)Enum.Parse(typeof(FlyDirection), transitionElement.Attribute("Direction").Value, true);
            }
            catch
            {
                // 忽略异常仅使用默认值是否安全?
            }
            base.LoadChild(transitionElement);
        }

        public override ITransition Clone()
        {
            return new FlyInTransition()
            {
                BackgroundElement = this.BackgroundElement,
                ForegroundElement = this.ForegroundElement,
                TransitionDuration = this.TransitionDuration,
                Direction = this.Direction
            };
        }

        public enum FlyDirection
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
