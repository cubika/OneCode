/****************************************模块头 ***************************************\
* 模块名:  MainForm.cs
* 项目名:	 CSWinFormTreeViewLoad
* 版权(c) Microsoft Corporation.
* 
* 该示例展示了如何从一个数据表(DataTable)载入数据，
* 并将数据显示到一个树形视图控件(TreeView)中。
* 
* 更多关于 TreeView 控件的信息，请参考:
* 
*  Windows Forms TreeView 控件
*  http://msdn.microsoft.com/zh-cn/library/ch6etkw4.aspx
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************************/

using System;
using System.Data;
using System.Windows.Forms;

namespace CSWinFormTreeViewLoad
{
    public partial class MainForm : Form
    {
        // 用于填充 TreeView 的 DataTable 对象
        private DataTable _dtEmployees;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this._dtEmployees = this.CreateDataTable();

            // 调用 BuildTree 方法来创建 TreeView
            CreateTreeViewFromDataTable.BuildTree(
                this._dtEmployees, this.treeView1,
                true, "名字", "员工标识", "上级");
        }

        public DataTable CreateDataTable()
        {
            DataTable dataTable = new DataTable();

            // 这一列的值用来标识每个树节点 TreeNode
            dataTable.Columns.Add("员工标识");

            // 这一列的值用来显示在树节点中
            dataTable.Columns.Add("名字");

            // 这一列的值用来标识树节点的父节点
            dataTable.Columns.Add("上级");

            // 填充并且构建数据表 DataTable
            dataTable.Rows.Add(1, "小王", 2);
            dataTable.Rows.Add(2, "小李", DBNull.Value);
            dataTable.Rows.Add(3, "小孙", 2);
            dataTable.Rows.Add(4, "小张", 2);
            dataTable.Rows.Add(5, "小刘", 2);
            dataTable.Rows.Add(6, "小田", 5);
            dataTable.Rows.Add(7, "小金", 5);
            dataTable.Rows.Add(8, "小赵", 2);
            dataTable.Rows.Add(9, "小钱", 5);
            return dataTable;
        }
    }
}
