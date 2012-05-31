/********************************* 模块头 **********************************\
* 模块名:                NullConfigControl.cs
* 项目:                  CSSL4MEF
* Copyright (c) Microsoft Corporation.
* 
* NullConfigControl
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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ConfigControl.Contract;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace CSSL4MEF.ConfigControls
{
    /// <summary>
    /// 默认配置控件, 显示匹配失败信息. 
    /// </summary>
    [ExportConfigControl(Name = "Null", PropertyValueType = typeof(object))]
    public class NullConfigControl : IConfigControl
    {
        public FrameworkElement CreateView(PropertyInfo property)
        {
            var inputControl = new TextBlock();
            inputControl.Foreground = new SolidColorBrush(Colors.Red);
            inputControl.TextWrapping = TextWrapping.Wrap;
            inputControl.Text = "没有与此属性匹配的控件.";
            return inputControl;
        }

        public MatchResult MatchTest(PropertyInfo property)
        {
            return MatchResult.NotRecommended;
        }
    }
}
