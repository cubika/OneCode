/************************************* 模块头 **************************************\
* 模块名:	MainForm.cs
* 项目  :		CSWinFormGroupRadioButtons
* Copyright (c) Microsoft Corporation.
* 
* 这个例子展示了怎样在不同的容器中组织RadioButtons。
* 
* 关于RadioButton控件的更多信息,请看:
* 
*  Windows Forms RadioButton control
*  http://msdn.microsoft.com/en-us/library/f5h102xz.aspx
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
using System.Windows.Forms;

namespace CSWinFormGroupRadioButtons
{
    public partial class MainForm : Form
    {
        // 存储老的RadioButton
        private RadioButton radTmp = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 选中MainForm.Designer.cs文件中的rad1，然后这个RadioButton变为老的。
            radTmp = this.rad1;
        }

        // 在MainForm.Designer.cs文件中让4个Radiobutton使用这个方法处理它们的CheckedChanged 事件
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            // 不选中老的
            radTmp.Checked = false;
            radTmp = (RadioButton)sender;
            
            //找到选中的
            if (radTmp.Checked)
            {
                this.lb.Text = radTmp.Name + " 已经选中";
            }
        }
    }
}
