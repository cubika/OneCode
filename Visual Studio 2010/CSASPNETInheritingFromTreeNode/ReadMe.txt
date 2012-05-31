==============================================================================
    ASP.NET 应用程序 : CSASPNETInheritingFromTreeNode 项目概述
==============================================================================

//////////////////////////////////////////////////////////////////////////////
总结:

在Windows窗体的TreeView中,树节点有可以用来存储自定义对象称为“Tag“的属性. 
有时我们希望ASP.NET TreeView拥有同样的特性.此项目创建了名为"CustomTreeView"的自定义
TreeView控件实现这一目标.


//////////////////////////////////////////////////////////////////////////////
演示:

在浏览器中打开页面Default.aspx中,你可以在页面中看到一个TreeView控件.
选择一个TreeNode,然后将显示分配到选定节点的自定义对象. 


//////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 创建自定义TreeView.

   定义名为“CustomTreeNode“包含“Tag“的属性的自定义TreeNode.
   我们将在该属性中存储自定义对象.为了在视图状态中保存
   新的属性,我们重写方法LoadViewState()和
   SaveViewState()来实现保存和检索.

        protected override void LoadViewState(object state)
        {
            object[] arrState = state as object[];

            this.Tag = arrState[0];
            base.LoadViewState(arrState[1]);
        }
        protected override object SaveViewState()
        {
            object[] arrState = new object[2];
            arrState[1] = base.SaveViewState();
            arrState[0] = this.Tag;

            return arrState;
        }
    
    在回传时,ASP.NET运行时将重新创建TreeView控件.
    为了让TreeView控件自动创建自定义的TreeNode,我们重用了助手方法CreateNode().

        protected override TreeNode CreateNode()
        {
            return new CustomTreeNode(this, false);
        }

2. 定义自定义对象.

   为了保存对象到视图状态,该对象必须可序列化.

        [Serializable]
        public class MyItem
        {
            public string Title { get; set; }
        }

3. 创建测试页面.

   在页面中添加一个CustomTreeView控件.在Page_Load()方法中,创建一些CustomTreeNodes
   并分配一个自定义对象给每个CustomTreeNode.

   在CustomTreeView控制的SelectedNodeChanged事件句柄中,获取选定的树节点对应的
   自定义对象,然后显示它.


//////////////////////////////////////////////////////////////////////////////
参考资料:

TreeNode.Tag 属性 
http://msdn.microsoft.com/zh-cn/library/system.windows.forms.treenode.tag.aspx

Control.SaveViewState 方法 
http://msdn.microsoft.com/zh-cn/library/system.web.ui.control.saveviewstate.aspx

Control.LoadViewState 方法
http://msdn.microsoft.com/zh-cn/library/system.web.ui.control.loadviewstate.aspx

TreeView.CreateNode 方法
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.treeview.createnode.aspx


//////////////////////////////////////////////////////////////////////////////