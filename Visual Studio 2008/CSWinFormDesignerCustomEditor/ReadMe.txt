================================================================================
   WINDOWS窗口程序 : CSWinFormDesignerCustomEditor项目预览
                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
使用说明:
这个例子说明了如何在设计阶段使用一个自定义类型编辑器来编辑一个特殊的属性。 
 

/////////////////////////////////////////////////////////////////////////////
创建过程:

1. 创建一个用户控件，命名为UC_CustomEditor的。

2. 添加一个到System.Designer.dll动态链接库的引用。

3. 创建一个名为SubClass的类，然后在该类中添加一些属性：

   public class SubClass
    {
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

4. 在UC_CustomEditor类中创建一个SubClass类型的属性。

5. 创建一个新的窗体，命名为EditorForm;

6. 在EditorForm窗体中添加一个TextBox控件和一个DateTimePicker控件，TextBox控件是用来编辑SubClass类的Name属性值，
   而DateTimePicker控件是用来编辑SubClass类的Date属性值。
   
7. 创建一个SubClass类型的属性。

8. 处理EditorForm的Load事件为TextBox和DateTimePicker控件指定明确的初始值。

9. 在EditorForm窗体中添加两个按钮，一个用来确定编辑的改变，另外一个用来取消编辑的改变。

10. 创建一个从UITypeEditor父类继承的子类。

11. 重写UITypeEditor类的GetEditStyle()方法用来返回"UITypeEditorEditStyle.Modal"类型的值，
    这个返回值明确指定了设计者用一个Modal类型的对话框来编辑这个指定的对象。
 
12. 重写UITypeEditor类的EditValue()方法，通过使用由GetEditStyle()方法指定的编辑器风格来编辑指定对象的值。
   
13. 使用Editor的属性来标记UC_CustomEditor类的cls属性的值，使之能够使用由一下步骤创建的Editor：   

    public partial class UC_CustomEditor : UserControl
    {
        ....

        private SubClass cls = new SubClass();

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Editor(typeof(MyEditor),typeof(UITypeEditor))]
        public SubClass Cls
        {
            get { return this.cls; }
            set { this.cls = value; }
        }
    }

    
14. 建设该项目。

15. 现在在toolbox中所创建的用户控件可以可以被使用了。


/////////////////////////////////////////////////////////////////////////////
参考资料:

1. UITypeEditor类
http://msdn.microsoft.com/en-us/library/system.drawing.design.uitypeeditor.aspx

2. ExpandableObjectConverter类
http://msdn.microsoft.com/en-us/library/system.componentmodel.expandableobjectconverter.aspx

3. Windows Forms FAQs
http://windowsclient.net/blogs/faqs/archive/tags/Custom+Designers/default.aspx


/////////////////////////////////////////////////////////////////////////////