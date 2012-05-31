/*************************************** 模块头 *************************************\
* 模块名:  ConditionBuilderDesigner.cs
* 项目名： CSDynamicallyBuildLambdaExpressionWithField
* 版权 (c) Microsoft Corporation.
* 
* ConditionBuilderDesigner.cs 文件定义了一些集合，来演示如何与其他控件相结合.
*
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

// 设计器，用于为 ConditionBuilderDesigner 显示智能标签.
namespace DynamicCondition
{
    internal class ConditionBuilderDesigner : ControlDesigner
    {
        private DesignerActionListCollection actions = new DesignerActionListCollection();

        /// <summary>
        /// 重写属性，用于集成 PropertyGrid 控件.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (actions.Count == 0)
                {
                    actions.Add(new ConditionBuilderActionList(this.Component));
                }
                return actions;
            }
        }

        /// <summary>
        /// 为类型提供基类.这些类型定义了一系列用于创建智能标记面板的项目.       
        /// </summary>
        public class ConditionBuilderActionList : DesignerActionList
        {
            private ConditionBuilder cBuilder;
            public ConditionBuilderActionList(IComponent component)
                : base(component)
            {
                cBuilder = (ConditionBuilder)component;
            }

            /// <summary>
            /// Lines 属性.
            /// </summary>
            public int Lines
            {
                get
                {
                    return cBuilder.Lines;
                }
                set
                {
                    GetPropertyByName("Lines").SetValue(cBuilder, value);
                }
            }

            /// <summary>
            /// OperatorType属性.
            /// </summary>
            public ConditionBuilder.Compare OperatorType
            {
                get
                {
                    return cBuilder.OperatorType;
                }
                set
                {
                    GetPropertyByName("OperatorType").SetValue(cBuilder, value);
                }
            }

            /// <summary>
            /// Box 属性.
            /// </summary>
            private PropertyDescriptor GetPropertyByName(string propName)
            {
                var prop = TypeDescriptor.GetProperties(cBuilder)[propName];
                if (prop == null)
                {
                    throw new ArgumentException("无效的属性.", propName);
                }
                return prop;
            }

            /// <summary>
            /// 创建将在智能标签中显示的元素.
            /// </summary>
            public override System.ComponentModel.Design.DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection items = new DesignerActionItemCollection();
                items.Add(new DesignerActionHeaderItem("Appearance"));
                items.Add(new DesignerActionPropertyItem("Lines", "Number of Lines:", "Appearance",
                    "Set the number of lines in the ConditionBuilder"));
                items.Add(new DesignerActionPropertyItem("OperatorType", "Default Operator:", "Appearance",
                    "Default operator to use"));
                return items;
            }
        }
    }
}