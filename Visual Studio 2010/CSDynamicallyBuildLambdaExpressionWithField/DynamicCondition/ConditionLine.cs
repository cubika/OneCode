/******************************************* 模块头 ***********************************************\
* 模块名:  ConditionLine.cs
* 项目名:  CSDynamicallyBuildLambdaExpressionWithField
* 版权 (c) Microsoft Corporation.
*
* ConditionLine.cs 文件定义了一些子条件连接运算符和一些属性框.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicCondition
{
	internal partial class ConditionLine
	{

		internal ConditionLine()
		{
			InitializeComponent();
		}

        /// <summary>
        /// DataType 属性.
        /// </summary>
		private Type _dataType;
        public Type DataType
		{
			get
			{
				return _dataType;
			}
			set
			{
				_dataType = value;
			}
		}

        /// <summary>
        /// DataSource属性.
        /// </summary>
		private PropertyInfo[] _dataSource;
        public PropertyInfo[] DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
				cmbColumn.DataSource = value;
				cmbColumn.DisplayMember = "Name";
			}
		}

        /// <summary>
        /// 条件比较运算符.
        /// </summary>
		public DynamicCondition.DynamicQuery.Condition.Compare OperatorType
		{
			get
			{
                return ((lb.Text == "AND") ? DynamicCondition.DynamicQuery.Condition.Compare.And :
                    DynamicQuery.Condition.Compare.Or);
			}
			set
			{
                if (value != DynamicQuery.Condition.Compare.And & value != DynamicQuery.Condition.Compare.Or)
				{
					throw new ArgumentException("OperatorType 必须是 \"And\" 或者 \"Or\"");
				}
				lb.Text = value.ToString().ToUpper();
			}
		}

		/// <summary>
        /// 返回 Condition(Of T），它表示了存储在 UserControl 控件中的条件.
        /// </summary>
        public DynamicCondition.DynamicQuery.Condition<T> GetCondition<T>(T dataSrc)
		{

			var pType = ((PropertyInfo)cmbColumn.SelectedItem).PropertyType;

            //  CheckType 确保 T 和 T? 被同样对待.
			if (CheckType<bool>(pType))
			{
				 return MakeCond(dataSrc, pType, chkValue.Checked);
			}
		
			else if (CheckType<DateTime>(pType))
			{
				 return MakeCond(dataSrc, pType, dtpValue.Value);
			}
			else if (CheckType<char>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToChar(tbValue.Text));
			}
			else if (CheckType<long>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToInt64(tbValue.Text));
			}
			else if (CheckType<short>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToInt16(tbValue.Text));
			}
           	else if (CheckType<ulong>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToUInt64(tbValue.Text));
			}
			else if (CheckType<ushort>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToUInt16(tbValue.Text));
			}
			else if (CheckType<float>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToSingle(tbValue.Text));
			}
			else if (CheckType<double>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToDouble(tbValue.Text));
			}
			else if (CheckType<decimal>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToDecimal(tbValue.Text));
			}
			else if (CheckType<int>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToInt32(SimulateVal.Val(tbValue.Text)));
			}
			else if (CheckType<uint>(pType))
			{
				 return MakeCond(dataSrc, pType, Convert.ToUInt32(tbValue.Text));
			}

            // 这只能是字符串，因为我们筛选了添加到复合框中的类型.
			else 
			{
				return MakeCond(dataSrc, pType, tbValue.Text);
			}
		}


		public static List<Type> typeList;

        /// <summary>
        /// where 关键字后面的协议.
        /// </summary>
        public static List<Type> GetSupportedTypes()
		{
			if (typeList == null)
			{
				typeList = new List<Type>();
				typeList.AddRange(new Type[] {typeof(DateTime), typeof(DateTime?), typeof(char), 
                    typeof(char?), typeof(long), typeof(long?), typeof(short), typeof(short?), 
                    typeof(ulong), typeof(ulong?), typeof(ushort), typeof(ushort?), typeof(float),
                    typeof(float?), typeof(double), typeof(double?), typeof(decimal), typeof(decimal?),
                    typeof(bool), typeof(bool?), typeof(int), typeof(int?), typeof(uint), typeof(uint?), 
                    typeof(string)});
			}

			return typeList;
		}

        /// <summary>
        /// 组合条件. 
        /// </summary>
  		private void ConditionLine_Load(object sender, EventArgs e)
		{
			cmbOperator.DisplayMember = "Name";
			cmbOperator.ValueMember = "Value";
            var opList = MakeList(new { Name = "等于", Value = DynamicQuery.Condition.Compare.Equal }, 
                new { Name = "不等于", Value = DynamicQuery.Condition.Compare.NotEqual }, 
                new { Name = ">", Value = DynamicQuery.Condition.Compare.GreaterThan }, 
                new { Name = ">=", Value = DynamicQuery.Condition.Compare.GreaterThanOrEqual },
                new { Name = "<", Value = DynamicQuery.Condition.Compare.LessThan }, 
                new { Name = "<=", Value = DynamicQuery.Condition.Compare.LessThanOrEqual },
                new { Name = "Like", Value = DynamicQuery.Condition.Compare.Like });
			cmbOperator.DataSource = opList;
		}

        /// <summary>
        /// 当获得用户选取的属性时，选择用于演示的控件.
        /// </summary>
        private void cboColumn_SelectedIndexChanged(object sender, EventArgs e)
		{

            // 获取用户选取的属性的基础类型. 
			var propType = ((PropertyInfo)cmbColumn.SelectedItem).PropertyType;

            // 为属性类型展示合适的控件（CheckBox/TextBox/DateTimePicker).
			if (CheckType<bool>(propType))
			{
				 SetVisibility(true, false, false);
			}
			
			else if (CheckType<DateTime>(propType))
			{
				 SetVisibility(false, true, false);
			}
			else
			{
				 SetVisibility(false, false, true);
			}
		}

		/// <summary>
        /// 设置控件的可见性.
        /// </summary>
   		private void SetVisibility(bool chkBox, bool datePicker, bool txtBox)
		{
			chkValue.Visible = chkBox;
			tbValue.Visible = txtBox;
			dtpValue.Visible = datePicker;
		}

		/// <summary>
        /// AND与OR间的切换.
        /// </summary>
        private void lblOperator_Click(object sender, System.EventArgs e)
		{
			lb.Text = ((lb.Text == "AND") ? "OR" : "AND");
		}

        /// <summary>
        /// MakeCond 运算符. 
        /// </summary>
   		private DynamicCondition.DynamicQuery.Condition<T> MakeCond<T, S>(T dataSource, Type propType, S value)
		{
			IEnumerable<T> dataSourceType = null;
            return DynamicCondition.DynamicQuery.Condition.Create<T>(dataSourceType, cmbColumn.Text,
                (DynamicQuery.Condition.Compare)cmbOperator.SelectedValue, value, propType);
		}

        /// <summary>
        ///当 proType 是 T 类型或者 Nullable（Of T) 时，返回true.
        /// </summary>
		private static bool CheckType<T>(Type propType) where T: struct
		{
			return (propType.Equals(typeof(T)) | propType.Equals(typeof(T?)));
		}

        /// <summary>
        /// 将参数列表转换到 IEnumerable(Of T) (其中T在本例中是匿名类型)中.
        /// </summary>
        private static IEnumerable<T> MakeList<T>(params T[] items)
		{
			return items;
		}
	}
}