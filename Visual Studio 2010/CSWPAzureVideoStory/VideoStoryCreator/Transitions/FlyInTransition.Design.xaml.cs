/********************************* 模块头 *********************************\
* 模块名:       FlyInTransition_Design.cs
* 项目名:		VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 提供用来指定飞入特效的额外设计界面.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.Windows.Controls;

namespace VideoStoryCreator.Transitions
{
    public partial class FlyInTransition_Design : UserControl
    {
        public FlyInTransition_Design()
        {
            InitializeComponent();
            List<VideoStoryCreator.Transitions.FlyInTransition.FlyDirection> directions = new List<VideoStoryCreator.Transitions.FlyInTransition.FlyDirection>()
            {
                VideoStoryCreator.Transitions.FlyInTransition.FlyDirection.Left,
                VideoStoryCreator.Transitions.FlyInTransition.FlyDirection.Right,
                VideoStoryCreator.Transitions.FlyInTransition.FlyDirection.Up,
                VideoStoryCreator.Transitions.FlyInTransition.FlyDirection.Down
            };
            this.directionList.ItemsSource = directions;
        }
    }
}
