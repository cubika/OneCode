/********************************* 模块头 *********************************\
* 模块名: FlyDirectionConverter.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 将string转化为FlyDirection, 反之亦然.
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

namespace VideoStoryCreator.Converters
{
    public class FlyDirectionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            return Enum.Parse(typeof(VideoStoryCreator.Transitions.FlyInTransition.FlyDirection), (string)value, true);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            return ((VideoStoryCreator.Transitions.FlyInTransition.FlyDirection)value).ToString();
        }
    }
}
