/************************************* 模块头 *******************************************\
* 模块名称:	UC_CustomPropertyTab.cs
* 项目:		CSWinFormDesignerCustomPropertyTab
* 版权所有 (c) Microsoft Corporation.
* 
* 
* CustomPropertyTab 例子演示了如何在属性窗口上添加自定义的PropertyTab
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.PropertyGridInternal;

namespace CSWinFormDesignerCustomPropertyTab
{
    [PropertyTab(typeof(CustomTab), PropertyTabScope.Component)]
    public partial class UC_CustomPropertyTab : UserControl
    {
        private string testProp;

        public UC_CustomPropertyTab()
        {
            InitializeComponent();
        }

        // 显示在自定义选项卡上
        [Browsable(false)]
        [CustomTabDisplayAttribute(true)]
        public string TestProp
        {
            get { return this.testProp; }
            set { this.testProp = value; }
        }
    }

    public class CustomTab : PropertiesTab
    {
        public override bool CanExtend(object extendee)
        {
            // 如果我们扩展这个控件，就返回true            
            return extendee is UC_CustomPropertyTab;
        }

        public override PropertyDescriptorCollection GetProperties(
            ITypeDescriptorContext context,
            object component,
            Attribute[] attrs)
        {
            // 只返回那些被标记为Browserable(false)和CustomTabDisplay(true)的属性
            return TypeDescriptor.GetProperties(
                component,
                new Attribute[] 
                { 
                    new BrowsableAttribute(false), 
                    new CustomTabDisplayAttribute(true) 
                });
        }

        public override string TabName
        {
            get { return "Custom Tab"; }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CustomTabDisplayAttribute : Attribute
    {
        private bool display;

        public CustomTabDisplayAttribute(bool display)
        {
            this.display = display;
        }

        public bool Display
        {
            get { return this.display; }
            set { this.display = value; }
        }
    }
}
