/********************************* 模块头 **********************************\
模块名:    ColorConverter.cs
项目:      CSSL4DataGridGroupHeaderStyle
Copyright (c) Microsoft Corporation.

根据 Group Name 改变 GroupHeader 的背景.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Windows.Media;
using System.Windows.Data;

namespace CSSL4DataGridGroupHeaderStyle
{
    public class ColorConverter : IValueConverter
    {
        public object Convert (Object value , Type targetType , Object parameter , System.Globalization.CultureInfo culture)
        {
            if (value.Equals("儿童"))
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            if (value.Equals("成人"))
            {
                return new SolidColorBrush(Colors.Orange);
            }
            else
                return new SolidColorBrush(Colors.Gray);
        }
        public object ConvertBack (object value , Type targetType , object parameter , System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("Converter cannot convert back.");
        }
    }
}
