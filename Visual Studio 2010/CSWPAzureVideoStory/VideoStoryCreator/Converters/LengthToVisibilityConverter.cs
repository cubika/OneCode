/********************************* 模块头 *********************************\
* 模块名: LengthToVisibilityConverter.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 将整型转为Visibility.
* 返回Collapsed，仅当integer大于0.
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
using System.Windows.Data;

namespace VideoStoryCreator.Converters
{
    /// <summary>
    /// 一个converter. 值必须为整型.
    /// 如果值大于0, 返回visible. 其他情况返回collapsed.
    /// </summary>
    public class LengthToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                int length = (int)value;
                if (length <= 0)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
