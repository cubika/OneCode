/************************************* 模块头 **************************************\
* 模块名:	MainForm.cs
* 项目名:		CSWinFormMultipleColumnComboBox
* 版权		(c) Microsoft Corporation.
* 
* 这个示例展示了如何在组合框的下拉框中显示多列数据.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
#endregion

namespace CSWinFormMultipleColumnComboBox
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //  创建数据源:
            //  
			//  创建一个名为Test的表并且添加两个列 
            //  ID:     int
            //  Name:   string
            //
            DataTable dtTest = new DataTable();
            dtTest.Columns.Add("ID", typeof(int));
            dtTest.Columns.Add("Name", typeof(string));

            dtTest.Rows.Add(1, "John");
            dtTest.Rows.Add(2, "Amy");
            dtTest.Rows.Add(3, "Tony");
            dtTest.Rows.Add(4, "Bruce");
            dtTest.Rows.Add(5, "Allen");

            // 将组合框绑定到数据表
            this.comboBox1.DataSource = dtTest;
            this.comboBox1.DisplayMember = "Name";
            this.comboBox1.ValueMember = "ID";

            // 启用组合框的自我绘制功能
            this.comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            // 通过处理DrawItem事件来绘制每一项.
            this.comboBox1.DrawItem += delegate(object cmb, DrawItemEventArgs args)
            {
                // 绘制默认背景
                args.DrawBackground();


                // 组合框已经绑定到数据表,
                // 所以每一项均为DataRowView对象.
                DataRowView drv = (DataRowView)this.comboBox1.Items[args.Index];

                // 获取每一项中对应列的值.
                string id = drv["id"].ToString();
                string name = drv["name"].ToString();

                // 获取第一列的边界
                Rectangle r1 = args.Bounds;
                r1.Width /= 2;

                // 在第一列上绘制文本
                using (SolidBrush sb = new SolidBrush(args.ForeColor))
                {
                    args.Graphics.DrawString(id, args.Font, sb, r1);
                }

                // 绘制线来分隔列
                using (Pen p = new Pen(Color.Black))
                {
                    args.Graphics.DrawLine(p, r1.Right, 0, r1.Right, r1.Bottom);
                }

                // 获取第二列的边界
                Rectangle r2 = args.Bounds;
                r2.X = args.Bounds.Width / 2;
                r2.Width /= 2;

                // 在第二列上绘制文本
                using (SolidBrush sb = new SolidBrush(args.ForeColor))
                {
                    args.Graphics.DrawString(name, args.Font, sb, r2);
                }
            };
        }
    }
}
