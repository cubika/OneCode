/************************************* 模块头 **************************************\
* 模块名称:  MainForm.cs
* 项目:      CSWinFormControls
* Copyright (c) Microsoft Corporation.
* 
* 这个示例阐述了怎样自定义Windows Forms控件。
*
* 本示例中，有4个小例子：
* 
* 1. 拥有多列的组合框。
*    展示了怎样在组合框的下拉列表中显示多列数据。
* 
* 2. 每个列表项拥有不同提示的列表框。
*    展示了怎样为列表框中的每个列表项显示不同的提示。     
*
* 3. 只能输入数字的文本框。
*    展示了怎样使文本框只允许输入数字。
*
* 4. 一个椭圆形的按钮。
*    展示了怎样创建一个不规则形状的按钮。 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
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


namespace CSWinFormControls
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void frmCtrlCustomization_Load(object sender, EventArgs e)
        {
            #region  示例 1 -- 拥有多列的组合框

            //  初始化DataTable:
            //  
            //  创建一个名为dtTest的数据表，为其添加2列
            //   ID:     int
            //   Name:   string
            //
            DataTable dtTest = new DataTable();
            dtTest.Columns.Add("ID", typeof(int));
            dtTest.Columns.Add("Name", typeof(string));

            dtTest.Rows.Add(1, "John");
            dtTest.Rows.Add(2, "Amy");
            dtTest.Rows.Add(3, "Tony");
            dtTest.Rows.Add(4, "Bruce");
            dtTest.Rows.Add(5, "Allen");

            // 将组合框的数据源设置为DataTable。
            this.comboBox1.DataSource = dtTest;
            this.comboBox1.DisplayMember = "Name";
            this.comboBox1.ValueMember = "ID";

            // 将组合框的 DrawMode 设置为OwnerDrawFixed。
            this.comboBox1.DrawMode = DrawMode.OwnerDrawFixed;

            // 在DrawItem事件中绘制子项。
            this.comboBox1.DrawItem += delegate(object cmb, DrawItemEventArgs args)
            {
                // 绘制默认的背景
                args.DrawBackground();

                // 因为组合框被绑定到DataTable，所以组合框的子项是DataRowView对象。
                DataRowView drv = (DataRowView)this.comboBox1.Items[args.Index];

                // 取出每一列的值。
                string id = drv["id"].ToString();
                string name = drv["name"].ToString();

                // 获得第一列的边界。
                Rectangle r1 = args.Bounds;
                r1.Width /= 2;

                // 绘制第一列的文本。
                using (SolidBrush sb = new SolidBrush(args.ForeColor))
                {
                    args.Graphics.DrawString(id, args.Font, sb, r1);
                }

                // 绘制一条分割线分割不同的列。
                using (Pen p = new Pen(Color.Black))
                {
                    args.Graphics.DrawLine(p, r1.Right, 0, r1.Right, r1.Bottom);
                }

                // 获取第二列的边界。
                Rectangle r2 = args.Bounds;
                r2.X = args.Bounds.Width/2;
                r2.Width /= 2;

                // 绘制第二列的文本。
                using (SolidBrush sb = new SolidBrush(args.ForeColor))
                {
                    args.Graphics.DrawString(name, args.Font, sb, r2);
                }
            };

            #endregion

            #region 示例 2 -- 每个列表项拥有不同提示的列表框

            // 初始化列表项。
            this.listBox1.Items.Add("Item1");
            this.listBox1.Items.Add("Item2");
            this.listBox1.Items.Add("Item3");
            this.listBox1.Items.Add("Item4");
            this.listBox1.Items.Add("Item5");

            this.listBox1.MouseMove += delegate(object lst, MouseEventArgs args)
            {
                // 获取鼠标悬停所在项的索引。
                int hoverIndex = this.listBox1.IndexFromPoint(args.Location);

                //如果鼠标悬停在列表项上，显示一个提示。
                if (hoverIndex >= 0 && hoverIndex < listBox1.Items.Count)
                {
                    this.toolTip1.SetToolTip(listBox1, listBox1.Items[hoverIndex].ToString());
                }
            };

            #endregion

            #region 示例 3 -- 只能输入数字的文本框

            // 处理TextBox.KeyPress事件，过滤输入的字符。
            this.textBox1.KeyPress += delegate(object tb, KeyPressEventArgs args)
            {
                if (!(char.IsNumber(args.KeyChar) || args.KeyChar == '\b'))
                {
                    // 如果输入字符不是数字或者回格键
                    // 则将Handled属性设置为true，目的是
                    // 表明KeyPress事件被处理过了，这将使
                    // 文本框过滤掉该输入字符。
                    args.Handled = true;
                }
            };


            #endregion

            #region 示例 4 -- 一个椭圆形的按钮

            this.roundButton1.Click += delegate(object btn, EventArgs args)
            {
                MessageBox.Show("点击了!");
            };
            #endregion
        }
    }

    #region RoundButton 类

    public class RoundButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            // 改变按钮的Region属性，将会导致在该区域以外的单击
            // 不再触发Click事件。
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new Region(path);

            base.OnPaint(pevent);
        }
    }

    #endregion
}
