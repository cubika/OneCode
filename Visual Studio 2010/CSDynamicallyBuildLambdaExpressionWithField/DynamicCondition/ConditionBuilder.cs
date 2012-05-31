/*************************************** 模块头 *************************************\
* 模块名:   ConditionBuilder.cs
* 项目名：  CSDynamicallyBuildLambdaExpressionWithField
* 版权 (c) Microsoft Corporation.
*
* ConditionBuilder.cs file 为第一个Condition定义了一个UserControl.
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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.ComponentModel;

namespace DynamicCondition
{
    /// <summary>
    /// Designer 元数据
    /// </summary>
    [Designer(typeof(ConditionBuilderDesigner))]
    public partial class ConditionBuilder
    {

        public ConditionBuilder()
        {
            InitializeComponent();
        }


        #region Properties
        private const string cName = "ConditionLine";

        /// <summary>
        /// 枚举，用于条件类型的定义.
        /// </summary>
        public enum Compare : int
        {
            And = DynamicCondition.DynamicQuery.Condition.Compare.And,
            Or = DynamicCondition.DynamicQuery.Condition.Compare.Or
        }

        private int _lines = 1;
        private Type _type;
        private Compare _operatorType = Compare.And;

        /// <summary>
        /// 要显示的 ConditionLine 控件数目.
        /// </summary>
        public int Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("行数不能小于1");
                }

                if (value > _lines)
                {
                    // 构造新的 ConditionLine.
                    for (var i = _lines; i < value; i++)
                    {
                        ConditionLine cLine = new ConditionLine
                        {
                            Name = cName + (i + 1),
                            Left = ConditionLine1.Left,
                            Width = ConditionLine1.Width,
                            Top = ConditionLine1.Top + i * (ConditionLine1.Height + 1),
                            OperatorType = (DynamicQuery.Condition.Compare)_operatorType,
                            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
                        };

                        this.Controls.Add(cLine);
                    }

                }
                else if (value < _lines)
                {

                    // 去除多余的 ConditionLine.
                    for (var i = value; i <= _lines; i++)
                    {
                        this.Controls.RemoveByKey(cName + (i + 1));
                    }

                }
                _lines = value;
            }
        }

        /// <summary>
        /// 用于每一个 ConditionLine 的默认运算符 (And/Or).
        /// </summary>
        public Compare OperatorType
        {
            get
            {
                return _operatorType;
            }
            set
            {
                _operatorType = value;
                for (var i = 1; i <= _lines; i++)
                {
                    GetConditionLine(cName + i).OperatorType = (DynamicCondition.DynamicQuery.Condition.Compare)value;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 用 dataSource 中含有的列来填充下拉列表.  dataSource 可以是
        /// IEnumerable(Of T) (本地), 或者是 IQueryable(Of T) (远程).
        /// </summary>
        public void SetDataSource<T>(IEnumerable<T> dataSource)
        {
            _type = typeof(T);

            // 筛选掉不是内部类型的所有属性.
            // 例如，一个 Customers 对象可能拥有 EntityRef（Of Order）类型的一个Orders 属性，但是在
            // 列表中显示该属性意义并不大. 
            // 注意， 根本的 Condition API 确实支持嵌套属性的访问，只是 ConditionBuilder 控件没有
            // 为用户提供一个机制来指定它.                      
            var props = from p in _type.GetProperties()
                        where DynamicCondition.ConditionLine.GetSupportedTypes().Contains(p.PropertyType)
                        select p;

            // 将列装载到每一个 ConditionLine.
            for (var i = 1; i <= _lines; i++)
            {
                GetConditionLine(cName + i).DataSource = (System.Reflection.PropertyInfo[])(props.ToArray().Clone());
            }
        }



        /// <summary>
        /// 使用此方法来得到，用于表示用户输入 ConditionBuilder 的所有数据一个的 Condition 对象
        /// </summary>
        public DynamicCondition.DynamicQuery.Condition<T> GetCondition<T>()
        {

            // 这只用来推断类型，因此无需将其实例化.
            T dataSrc = default(T);
            var finalCond = GetConditionLine(cName + "1").GetCondition<T>(dataSrc);

            // 从每一个 ConditionLine 中提取 Condition,然后将它与 finalCond 结合起来. 
            for (var i = 2; i <= _lines; i++)
            {
                var cLine = GetConditionLine(cName + i);
                finalCond = DynamicCondition.DynamicQuery.Condition.Combine<T>(finalCond, cLine.OperatorType,
                    cLine.GetCondition<T>(dataSrc));
            }

            return finalCond;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 接受 "ConditionLine2" ，返回该控件的实际实例.
        /// </summary>
        private ConditionLine GetConditionLine(string name)
        {
            return (ConditionLine)(this.Controls[name]);
        }

        /// <summary>
        /// 当加载 ConditionBuilder 时执行.
        /// </summary>
        private void ConditionBuilder_Load(object sender, EventArgs e)
        {
            this.ConditionLine1.lb.Visible = false;
        }
        #endregion

    }
}