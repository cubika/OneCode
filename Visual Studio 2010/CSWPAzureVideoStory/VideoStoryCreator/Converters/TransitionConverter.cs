/********************************* 模块头 *********************************\
* 模块名: TransitionConverter.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 将transition转为string，反之亦然.
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
using System.Windows.Data;
using VideoStoryCreator.Transitions;

namespace VideoStoryCreator.Converters
{
    public class TransitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                // Default to fade transition.
                return "Fade Transition";
            }
            return ((ITransition)value).Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return TransitionFactory.CreateTransition("Fade Transition");
            }
            return TransitionFactory.CreateTransition((string)value);
        }
    }
}
