/************************************* 模块头 **************************************\
* 模块名称:	UC_CustomEditor.cs
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace CSWinFormDesignerCustomEditor
{
    [Serializable()]
    public partial class UC_CustomEditor : UserControl
    {
        private SubClass cls;

        public UC_CustomEditor()
        {
            InitializeComponent();
            cls = new SubClass("Name", System.DateTime.Now);
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Editor(typeof(MyEditor), typeof(UITypeEditor))]
        public SubClass Cls
        {
            get
            {
                this.lblName.Text = this.cls.Name;
                this.lblDateTime.Text = this.cls.Date.ToString();
                return this.cls;
            }
            set 
            {
                this.lblName.Text = ((SubClass)value).Name;
                this.lblDateTime.Text = ((SubClass)value).Date.ToString();
                this.cls = value;
            }
        }
    }

    public class SubClass
    {
        public SubClass()
        {
        }

        public SubClass(string name, DateTime time)
        {
            this.date = time;
            this.name = name;
        }

        private string name;
        private DateTime date = DateTime.Now;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }
    }

    #region UITypeEditor

    // 这个类展示了一个自定义UITypeEditor的使用。
    // 它允许UC_CustomEditor控件的Cls属性在设计期通过使用那些在属性窗口中被唤醒的自定义的用户界面元素来改变它的值
    // 这个用户界面由EditorForm类来提供。
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class MyEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService = null;

        public override UITypeEditorEditStyle GetEditStyle(
        System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(
            ITypeDescriptorContext context,
            IServiceProvider provider,
            object value)
        {
            if (provider != null)
            {
                editorService =
                    provider.GetService(typeof(IWindowsFormsEditorService))
                    as IWindowsFormsEditorService;
            }

            if (editorService != null)
            {
                EditorForm editorForm = new EditorForm((SubClass)value);
                if (editorService.ShowDialog(editorForm) == DialogResult.OK)
                {
                    value = editorForm.SubCls;
                }
            }
            return value;
        }

        public override bool GetPaintValueSupported(
            ITypeDescriptorContext context)
        {
            return true;
        }


        public override void PaintValue(PaintValueEventArgs e)
        {
            
        }
    }
    #endregion
}
