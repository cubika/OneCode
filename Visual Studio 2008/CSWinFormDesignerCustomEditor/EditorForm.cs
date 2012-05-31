/************************************* 模块头 **************************************\
* 模块名称:	EditorForm.cs
* 项目名称:		CSWinFormDesignerCustomEditor
* 版权所有 (c) 微软公司
* 
* 
* 这个CSWinFormDesignerCustomEditor例子说明了如何在设计阶段使用一个自定义编辑器来编辑一个特殊的属性。
* 
* 
* 这个资源受到微软公共许可的。
* 参见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* 保留所有其他权利。
* 
* 这里所提供的原样的明示的或者暗示的代码和信息不受任何性质的担保，
* 但不限于那些特定用途的适销性和/或适用性的隐式担保。
\******************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSWinFormDesignerCustomEditor
{
    public partial class EditorForm : Form
    {
        public EditorForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(EditorForm_Load);
        }

        public EditorForm(SubClass value)
        {
            InitializeComponent();
            subCls = value;
            this.Load += new EventHandler(EditorForm_Load);
        }

        void EditorForm_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.subCls.Name;
            this.monthCalendar1.SelectionStart = this.subCls.Date;
            this.monthCalendar1.SelectionEnd = this.subCls.Date;
        }
        private SubClass subCls = new SubClass();

        public SubClass SubCls
        {
            get
            {
                return subCls;
            }
            set
            {
                this.subCls = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.subCls.Date = this.monthCalendar1.SelectionStart;
            this.subCls.Name = this.textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
