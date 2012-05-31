/********************************* 模块头 *********************************\
* 模块名: FadeTransition.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
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

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace VideoStoryCreator.Transitions
{
    public class FadeTransition : TransitionBase
    {
        private Storyboard _transitionStoryboard;
        private DoubleAnimationUsingKeyFrames _fadeAnimation;
        private FrameworkElement _foregroundElement;

        public override event EventHandler TransitionCompleted;

        private string _name = "Fade Transition";
        public override string Name
        {
            get { return this._name; }
        }

        public FadeTransition()
        {
            this.TransitionDuration = TimeSpan.FromSeconds(2d);
            this._transitionStoryboard = new Storyboard();
            this._fadeAnimation = new DoubleAnimationUsingKeyFrames();

            // 只需动态变更 Opacity 属性.
            Storyboard.SetTargetProperty(this._fadeAnimation, new PropertyPath("Opacity"));
            EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = 1d };
            EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(this.TransitionDuration), Value = 0d };
            this._fadeAnimation.KeyFrames.Add(frame1);
            this._fadeAnimation.KeyFrames.Add(frame2);
            this._transitionStoryboard.Children.Add(_fadeAnimation);
            this._transitionStoryboard.Completed += new EventHandler(TransitionStoryboard_Completed);
        }

        void TransitionStoryboard_Completed(object sender, EventArgs e)
        {
            if (this.TransitionCompleted != null)
            {
                this.TransitionCompleted(this, EventArgs.Empty);
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

        private TimeSpan _transitionDuration;
        public override TimeSpan TransitionDuration
        {
            get { return this._transitionDuration; }
            set
            {
                this._transitionDuration = value;
                if (this._fadeAnimation != null)
                {
                    this._fadeAnimation.KeyFrames[1].KeyTime = KeyTime.FromTimeSpan(this.TransitionDuration);
                }
            }
        }

        public override FrameworkElement ForegroundElement
        {
            get { return this._foregroundElement; }
            set
            {
                this._foregroundElement = value;
                if (value != null)
                {
                    Storyboard.SetTarget(this._fadeAnimation, value);
                }
            }
        }

        public override ITransition Clone()
        {
            return new FadeTransition()
            {
                BackgroundElement = this.BackgroundElement,
                ForegroundElement = this.ForegroundElement,
                TransitionDuration = this.TransitionDuration
            };
        }
    }
}
