========================================================================
    ASP.Net 应用程序 : VBASPNETGridView 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

这个 VBASPNETGridView 项目描述了如何填充 ASP.NET GridView 控件和如何在 ASP.NET
GridView 空间中实现添加,编辑,更新,删除,分页,排序等功能.

这个项目有两个示例: DataInMemory 和 DataFromDatabase.

DataInMemory 用一个简单的 DataTable 填充 GridView. 这个 DataTable 被记录到
ViewState 中以持久化回传数据.

DataFromDatabase 用 ADO.NET 的方式,将SQL Server database的数据填充到 GridView.
这个示例使用 SQLServer2005DB 示例数据库.

/////////////////////////////////////////////////////////////////////////////
创建:

步骤1.在Visual Studio 2008 或 Visual Web Developer,建立一个VB.Net的名为VBASPNETGridView
的 ASP.NET 的 Web 应用程序


步骤2. 拖拽一个GridView控件,一个LinkButton控件,一个Panel控件 到一个 ASP.NET 
页面中.

    (1) 重命名控件的 ID 如下

    GridView1    -> gvPerson
    LinkButton1  -> lbtnAdd
    Panel1		 -> pnlAdd
    
    (2) 将 lbtnAdd 的 Text 属性改为 AddNew.
    
    (3) 右键单击 gvPerson 控件, 选择 显示智能标签 -> 自动格式化, 
    选择样式 Oceanica 并按下 OK 保存
    
    (4))在智能标签中, 选择加入新的列, 选择 CommandField, 
    选中 Delete, Edit/Update and 显示取消按钮, 然后按下 OK.
    
    (5)在智能标签中,  选择加入新的列, 选择 BoundField, 
    并加入以下三列
    
    Header text		Data field		Read only
    -----------------------------------------
    PersonID		PersonID		Y
    LastName		LastName		N
    FirstName		FirstName		N
    
        
    (6))在智能标签中, 选择编辑列, 
    取消选中 Auto-generate fields, 选择 LastName 字段, 
    and 点击把这个字段转换成一个 TemplateField. 
    在 FirstName 字段上做相同的操作. 
    
    相关参考:
    
    ASP.NET: 在 GridView 控件中使用 TemplateFields 
    MSDN: TemplateField Class
    
    
步骤3. 从示例中拷贝以下方法,并把它们粘贴在代码隐藏文件中:

    DataInMemory.aspx.vb
    
        Page_Load方法:
        初始化基本对象，当页面首次被访问.

        InitializeDataSource方法:
        初始化DataTable和并将它存储在ViewState中.
        
        BindGridView方法:
        设置排序列，排序顺序和绑定的GridView
        控制在ViewState中的DataTable。

        BindGridView 方法:
        设置排序的列和排序的顺序, 并用 ViewState 中的 DataTable
        绑定 GridView 控件.

        
    DataFromDatabase.aspx.vb
    
        Page_Load 方法:
        初始化基本对象，当页面首次被访问.
        
        BindGridView 方法:
        设置排序的列和排序的顺序, 并用 SQL Server 中的 table
        绑定 GridView 控件.
        
    相关参考:
    
    MSDN: using语句（C＃参考）
    MSDN: 了解ASP.NET视图状态
    
    
步骤4. 拖拽两个 TextBox 控件和两个 LinkButton 控件到 pnlAdd 控件中.

    (1) 重命名控件的ID:

    TextBox1     -> tbLastName
    TextBox2	 -> tbFirstName
    LinkButton1  ->	lbtnSubmit
    LinkButton2  ->	lbtnCancel
    
    (2) 将 lbtnSubmit 的 Text 属性改为 Submit 和 Cancel.

步骤5. 导航到gvPerson属性面板,然后切换到事件. 
双击下面的事件生成的事件处理程序. 
在此之后，填充示例代码生成的方法.


    (1)	RowDataBound 事件: 当一个数据行绑定到 GridView 控件中的数据发生.

    在这个事件中, 我们添加一个客户端的确认对话框,在删除按钮被点击时弹出.
    它会阻止意外删除行。
    
    相关参考:
    
    MSDN: GridView.RowDataBound事件
    ASP.NET: 编辑，插入和删除数据
    ASP.NET: 添加客户端删除时确认
    MSDN:WebControl.Attributes 属性
    MSDN: DataControlRowType 枚举
    MSDN: GridViewRow.RowState 属性 
    
    (2)	PageIndexChanging 事件: 当某个页按钮被点击时触发, 但在 GridView 
    处理分页操作之前.

    另外,当在一个新的页面中展现数据时, 我们需要设置新的页面的索引值, 
    然后重新绑定 GridView 控件以显示数据.
   
    当点击编辑按钮编辑一个特定的行, GridView 控件将进入编辑模式,
    并显示更新和取消按钮.
    
    相关参考:
    
    MSDN: GridView.PageIndexChanging 事件


    (3)	RowEditing 事件: 当某一行的编辑按钮被点击时触发, 
    但在 GridView 控件进入编辑模式之前.
    
    为了使GridView控件进入编辑模式的选择行, 
    我们需要设置的行的索引,编辑,然后重新绑定
    GridView控件来呈现在编辑模式下的数据.
    
    相关参考:

    MSDN: GridView.RowEditing 事件
    MSDN: GridView.EditIndex 属性
    
    (4)	RowCancelingEdit 事件: 在编辑模式下,点击取消按钮发生, 
    但在该行退出编辑模式.

    我们可以点击取消按钮取消编辑模式并显示数据在普通视图模式.

    在这个事件中, 我们需要设置GridView.EditIndex 属性为-1,这
    意味着没有正在编辑的行,然后重新绑定 GridView 的数据显示
    视图模式.
    
    ////////////////////////////////////////////////////////////////
    gvPerson.EditIndex = -1;
    BindGridView();
    ////////////////////////////////////////////////////////////////
    
    相关参考:
    
    MSDN: GridView.RowCancelingEdit 事件


    (5)	RowUpdating 事件: 当某一行的Update按钮被点击时，
    但在GridView控件对该行进行更新前发生。

    在修改选定的行的值之后, 我们点击更新按钮
    将更改保存回数据源. 

    要标示编辑的人, PersonID 的值是必须的, 
    这个值是只读的,不能修改..
    
    ////////////////////////////////////////////////////////////////
    string strPersonID = gvPerson.Rows[e.RowIndex].Cells[2].Text
    ////////////////////////////////////////////////////////////////

    e.RowIndex 是当前行的索引.

    在 步骤2 we 转换了 LastName 和 FirstName 成 TemplateFields, 所以我们
    不能得到直接编辑值. 由于姓氏和名字是两个字符串值,标签控件用于生成的ItemTemplate
    显示值和TextBox控件生成的EditItemTemplate编辑值.

    我们可以访问单元格，通过以下方式获取的值:
    
    ////////////////////////////////////////////////////////////////
    string strLastName = 
    ((TextBox)gvPerson.Rows[e.RowIndex].FindControl("TextBox1")).Text
    
    string strFirstName = 
    ((TextBox)gvPerson.Rows[e.RowIndex].FindControl("TextBox2")).Text
    ////////////////////////////////////////////////////////////////

    取这些值后，我们可以将它们保存DataTable到ViewState中或在SQL Server表。
    
    相关参考:	
    
    MSDN: GridView.RowUpdating 事件
    ASP.NET: 编辑，插入和删除数据


    (6)	RowDeleting 事件: 当某一行的删除按钮被点击时，
    但在GridView控件删除该行之前发生。.

    要确定用于删除人，PERSONID的值是必需的，
    这是只读的，不能修改。
    
    ////////////////////////////////////////////////////////////////
    string strPersonID = gvPerson.Rows[e.RowIndex].Cells[2].Text
    ////////////////////////////////////////////////////////////////

    在取到 personID, 我们可以删除在ViewState中的DataTable中的人
    或在SQL Server表删除。.
    
    相关参考:	
    
    MSDN: GridView.RowDeleting 事件
    ASP.NET: 编辑，插入和删除数据

    
    (7)	Sorting 事件: 排序列被单击时, 但在GridView控件处理排序操作之前发生.

    当GridView发生数据源绑定时,在GridView上的SortDirection属性更改时.其他情况, 
    排序方向总是返回 "Ascending" 需要进行手动操作.

    在Page_Load事件中，我们存储的默认排序表达式在ViewState中.
    
    ////////////////////////////////////////////////////////////////
    ViewState["SortExpression"] = "PersonID ASC"
    ////////////////////////////////////////////////////////////////

    并在BindGridView方法设置排序列和排序顺序.
    
    ////////////////////////////////////////////////////////////////
    dvPerson.Sort = ViewState["SortExpression"].ToString()
    ////////////////////////////////////////////////////////////////

    因此,当第一次访问该页面, 所有人将按照 PersonID 的升序被显示出来. 
    
    当点击某列的标题进行排序,我们需要得到以前的排序列和排序顺序和
    比较与当前列的排序列.

    如果它们是相同的, 我们只是更改排序顺序, e.g. 将ascending改为descending 
    或者 将descending改为ascending.

    如果不是相同的, 我们指定为排序列,并设置当前列排序为升序.排序表达式存储到
    ViewState中.
    
    相关参考:	
    
    MSDN: DataView.Sort 属性 
    MSDN: GridView.Sorting 事件
    

Step6. 双击Click事件lbtnAdd生成事件处理并写下相关示例代码. 在lbtnSubmit 和 
lbtnCancel 做相同的操作. 

    lbtnAdd.Click 事件:	
    隐藏添加按钮，显示新增面板.	
    
    lbtnSubmit.Click 事件:
    取的TextBox控件的值，并添加了新行到ViewState中的
    DataTable 或 SQL Server表.

    lbtnCancel.Click 事件:
    显示添加按钮，添加隐藏添加面板。



/////////////////////////////////////////////////////////////////////////////
参考:
MSDN: using语句（VB .Net参考）

http://msdn.microsoft.com/en-us/library/yh598w02.aspx

MSDN: 了解ASP.NET视图状态
http://msdn.microsoft.com/en-us/library/ms972976.aspx

ASP.NET: 在GridView控件中使用TemplateField
http://www.asp.net/learn/data-access/tutorial-12-cs.aspx

MSDN: TemplateField 类
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.templatefield.aspx

MSDN: GridView.RowDataBound 事件
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.rowdatabound.aspx

ASP.NET: 编辑，插入和删除数据
http://www.asp.net/learn/data-access/#editinsertdelete

ASP.NET: 添加客户端删除时确认
http://www.asp.net/learn/data-access/tutorial-22-cs.aspx

MSDN: WebControl.Attributes 属性
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.webcontrol.attributes.aspx

MSDN: DataControlRowType 枚举
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.datacontrolrowtype.aspx

MSDN: GridViewRow.RowState 属性 
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridviewrow.rowstate.aspx

MSDN: GridView.PageIndexChanging 事件
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.pageindexchanging.aspx

MSDN: GridView.RowEditing 事件
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.rowediting.aspx

MSDN: GridView.EditIndex 属性  
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.editindex.aspx

MSDN: GridView.RowCancelingEdit 事件
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.rowcancelingedit.aspx

MSDN: GridView.RowUpdating 事件
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.rowupdating.aspx

MSDN: GridView.RowDeleting 事件
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.rowdeleting.aspx

MSDN: DataView.Sort 属性   
http://msdn.microsoft.com/en-us/library/system.data.dataview.sort.aspx

MSDN: GridView.Sorting  事件
http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.sorting.aspx

/////////////////////////////////////////////////////////////////////////////