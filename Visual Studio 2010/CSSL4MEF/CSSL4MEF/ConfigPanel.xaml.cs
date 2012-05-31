/********************************* 模块头 **********************************\
* 模块名:                ConfigPanel.xaml.cs
* 项目:                  CSSL4MEF
* Copyright (c) Microsoft Corporation.
* 
* ConfigPanel 隐藏代码文件.
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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel.Composition;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ConfigControl.Contract;

namespace CSSL4MEF
{
    /// <summary>
    /// ConfigPanel 使用反射获取绑定的 datamodel 属性,
    /// 并为每一个属性自动生成配置控件.
    /// </summary>
    public partial class ConfigPanel : UserControl,IPartImportsSatisfiedNotification
    {
        public ConfigPanel()
        {
            InitializeComponent();
            CompositionInitializer.SatisfyImports(this);
        }

        /// <summary>
        /// 建立 DependencyProperty, ConfigPanel 根据本属性的值生成配置控件 
        /// </summary>
        public object ConfigData
        {
            get { return (object)GetValue(ConfigDataProperty); }
            set { SetValue(ConfigDataProperty, value); }
        }

        public static readonly DependencyProperty ConfigDataProperty =
            DependencyProperty.Register("ConfigData", typeof(object),
            typeof(ConfigPanel),
            new PropertyMetadata(
                new PropertyChangedCallback((s, e) =>
                {
                    // 当 datamodel 发送改变时, 重新生成控件.
                    ((ConfigPanel)s).BindConfig(e.NewValue);
                })));     

        /// <summary>
        /// 进入 ConfigControl 列表
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public Lazy<IConfigControl, IConfigAttributes>[] ConfigControls { set; get; }

        // 
        /// <summary>
        /// 检索 dataModel, 查找每个属性最合适的控件
        /// </summary>
        /// <param name="datamodel">data model</param>
        void BindConfig(object datamodel)
        {
            if (datamodel != null)
            {
                ConfigList.Items.Clear();
                foreach (var property in datamodel.GetType().
                    GetProperties().OrderBy(p=>p.Name))
                {
                    var item = new ListBoxItem();

                    var border = new Border();
                    border.Margin = new Thickness(0, 0, 0, 5);
                    border.Padding = new Thickness(5);
                    border.BorderThickness = new Thickness(0,1,0,0);
                    border.BorderBrush = new SolidColorBrush(Colors.Black);

                    var stackpanel = new StackPanel();
                    //stackpanel.Children.Add(
                    //    new TextBlock
                    //    {
                    //        Text = property.Name,
                    //        FontStyle=FontStyles.Italic
                    //    });
                    TextBlock text = new TextBlock();
                    if (property.Name == "Effect")
                    {
                        text.Text = "特性";
                        text.FontStyle = FontStyles.Italic;
                    }
                    else if (property.Name == "FontColor")
                    {
                        text.Text = "字体颜色";
                        text.FontStyle = FontStyles.Italic;
                    }
                    else if (property.Name == "FontSize")
                    {
                        text.Text = "字体大小";
                        text.FontStyle = FontStyles.Italic;
                    }
                    else
                    {
                        text.Text = "文本";
                        text.FontStyle = FontStyles.Italic;
                    }
                    stackpanel.Children.Add(text);

                    var propertycontrol = GetBestMatch(property);
                    var propertyview = propertycontrol.Value.CreateView(property);
                    stackpanel.Children.Add(propertyview);
                    stackpanel.DataContext = datamodel;

                    item.Content = border;
                    border.Child = stackpanel;

                    item.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    ConfigList.Items.Add(item);
                }
            }
        }

        // 为属性在 configControl 列表中查找最合适的控件
        Lazy<IConfigControl, IConfigAttributes> GetBestMatch(PropertyInfo property)
        {
            Lazy<IConfigControl, IConfigAttributes> bestMatch = null;

            // 如果 UIHint 匹配控件名称, 为此属性使用指定控件
            var uihintAttr = property.GetCustomAttributes(typeof(UIHintAttribute),
                false).FirstOrDefault() as UIHintAttribute;
            if (uihintAttr != null)
            {
                bestMatch = ConfigControls.FirstOrDefault(lz => lz.Metadata.Name.
                    Equals(uihintAttr.UIHint, StringComparison.OrdinalIgnoreCase));
                if (bestMatch != null)
                {
                    return bestMatch;
                }
            }

            // 为属性查找最匹配的控件.
            var maxmatch = MatchResult.NotMatch;
            foreach (var configcontrol in ConfigControls)
            {
                var matchresult = configcontrol.Value.MatchTest(property);

                if (matchresult == MatchResult.Recommended)
                    return configcontrol;

                if (matchresult > maxmatch)
                {
                    maxmatch = matchresult;
                    bestMatch = configcontrol;
                }
            }
            
            return bestMatch;
        }

        // 实现 IPartImportsSatisfiedNotification 接口, 当插入
        // 部分发改变时, 此方法将被调用.
        public void OnImportsSatisfied()
        {
            BindConfig(ConfigData);
        }
    }
}
