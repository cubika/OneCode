/********************************** 模块头 ****************************************\
* 模块名:  MainForm.cs
* 项目名:  CSDynamicallyBuildLambdaExpressionWithField
* 版权 (c) Microsoft Corporation.
*
* 这个实例演示了如何动态创建lambda表达式,和将数据显示在 DataGridView 控件中.
* 
* 这个实例演示了如何将多个条件连结在一起，并动态生成LINQ到SQL. LINQ是非常好的方法，它用
* 类型安全、直观、极易表现的方式，来声明过滤器和查询数据. 例如，该应用
* 程序中的搜索功能，可以让客户找到满足多个列所定义条件的一切记录.
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
using System.Linq;
using DynamicCondition;


namespace CSDynamicallyBuildLambdaExpressionWithField
{
    public partial class MainForm
    {
        internal MainForm()
        {
            InitializeComponent();
        }
        private NorthwindDataContext db = new NorthwindDataContext();

        /// <summary>
        /// 当Winform加载时，将字段列表加载到控件中.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // 将字段列表加载到控件中.
            ConditionBuilder1.SetDataSource(db.Orders);
        }

        /// <summary>
        /// 动态生成LINQ查询，并将数据填入DataGridView控件中.
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // 从控件中获取条件.
            var c = ConditionBuilder1.GetCondition<Order>();

            // 筛选掉不符合条件的所有Orders.
            // 注意，由于延迟执行，实际上尚未执行查询.
            var filteredQuery = db.Orders.Where(c);

            // 我们现在可以执行其他任何操作（例如Order By或者Select)在filteredQuery中.
            var query = from row in filteredQuery
                        orderby row.OrderDate, row.OrderID
                        select row;

            // 执行查询，并将结果显示在DataGridView控件中.
            dgResult.DataSource = query;
        }

        /// <summary>
        /// DefaultInstance属性.
        /// </summary>
        private static MainForm _defaultInstance;
        public static MainForm DefaultInstance
        {
            get
            {
                if (_defaultInstance == null)
                    _defaultInstance = new MainForm();
                return _defaultInstance;
            }
        }
    }

}