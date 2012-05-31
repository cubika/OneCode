/********************************* 模块头 *********************************\
* 模块名: NullToBoolConverter.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 将object转为boolean.
* 如果object为空, 返回false.
* 其他情况返回true.
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
    /// <summary>
    /// 一个converter. 如果值为空, 返回false. 其他情况下返回true.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value != null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
