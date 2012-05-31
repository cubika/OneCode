/********************************* 模块头 *********************************\
* 模块名: TransitionFactory.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
*用于创建特效和他们的设计器的工厂类.
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
using System.Collections.Generic;
using System.Windows.Controls;

namespace VideoStoryCreator.Transitions
{
    public class TransitionFactory
    {
        private static Dictionary<string, Type> _transitionNameTypes = new Dictionary<string, Type>();
        private static Dictionary<string, Type> _transitionNameDesigners = new Dictionary<string, Type>();
        private static List<string> _transitionNames = new List<string>();

        static TransitionFactory()
        {
            _transitionNameTypes.Add("Fade Transition", typeof(FadeTransition));
            _transitionNames.Add("Fade Transition");

            _transitionNameTypes.Add("Fly In Transition", typeof(FlyInTransition));
            _transitionNameDesigners.Add("Fly In Transition", typeof(FlyInTransition_Design));
            _transitionNames.Add("Fly In Transition");

            //  在此注册更多特效...
        }

        private TransitionFactory()
        {
        }

        /// <summary>
        /// 基于名称创建特效.
        /// </summary>
        /// <param name="name">特效名.</param>
        /// <returns> ITransition 对象.</returns>
        public static ITransition CreateTransition(string name)
        {
            if (_transitionNameTypes.ContainsKey(name))
            {
                Type transitionType = _transitionNameTypes[name];
                try
                {
                    return Activator.CreateInstance(transitionType) as ITransition;
                }
                catch
                {
                    // TODO: 是否应该抛出异常或者返回空?
                }
            }
            return null;
        }

        /// <summary>
        /// 当前, 我们默认创建淡入淡出特效.
        /// </summary>
        public static ITransition CreateDefaultTransition()
        {
            return new FadeTransition();
        }

        /// <summary>
        /// 基于名称创建特效设计器.
        /// </summary>
        /// <param name="name">特效名.</param>
        /// <returns>用户控件. 所有设计器必须继承自 UserControl.</returns>
        public static UserControl CreateTransitionDesigner(string name)
        {
            if (_transitionNameDesigners.ContainsKey(name) && _transitionNameDesigners[name] != null)
            {
                Type transitionDeginerType = _transitionNameDesigners[name];
                try
                {
                    return Activator.CreateInstance(transitionDeginerType) as UserControl;
                }
                catch
                {
                    // Todo: 是否应该抛出异常或者返回空?
                }
            }
            return null;
        }

        /// <summary>
        /// 返回特效名列表.
        /// </summary>
        public static List<string> AvailableTransitions
        {
            get { return _transitionNames; }
        }
    }
}
