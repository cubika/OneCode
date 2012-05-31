========================================================================
    ASP.NET 应用程序 : CSASPNETFormViewUpload 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

这个CSASPNETFormViewUpload示例演示了如何在一个ASP.NET FormView控件中显示和上传图片
以及如何实现这个控件的插入、编辑、更新、删除和分页功能. 

这个项目包含两个页面: Default 、 Image.

Default页面 填充了一个从SQL Server数据库中读取数据的FromView控件 
同时还提供了处理数据所需的UI.

Image页面 从SQL Server数据库中获取一个图片并将其显示在网页上.



/////////////////////////////////////////////////////////////////////////////
创建过程:

步骤1. 在Visual Studio 2008或Visual Web Developer中创建一个C# ASP.NET Web应用程序
 命名为 CSASPNETFormViewUpload .


步骤2. 拖一个 FormView控件到Default页面.

    (1) 将FormView重命名为fvPerson.
     
    (2) 在代码视图中, 复制并粘贴示例中下列三个模板的标记:

    ItemTemplate: 渲染FormView中的特定数据记录.
    EditItemTemplate: FormView中的自定义编辑界面.
    InsertItemTemplate: FormView中的自定义插入界面. 

    相关参考:
    
    ASP.NET: Using the FormView's Templates	
    MSDN: FormView 类
    MSDN: Image 类
        
    
步骤3. 复制示例中的下列方法方法,并将他们粘贴到Default页面的
code-behind中:

    Page_Load 方法:
    当页面被初次访问时, 初始化底层对象.

    BindFormView 方法:
    将一张SQL Server表中的数据绑定到FormView控件.

    相关参考:
    
    MSDN: using 语句 (C# 参考)
        
    
步骤4. 转到fvPerson的属性面板 然后切换到事件. 
双击下列事件以生成事件句柄. 
然后用示例代码填充生成的方法.

    (1)	ModeChanging 事件: 当FormView控件在
    编辑、插入和只读模式之间切换时发生 , 但是发生在模式切换以前.
    
    在这个事件中, 我们需要切换FormView控件到新模式 
    然后重新绑定FormView控件以新模式显示数据.	
    
    ////////////////////////////////////////////////////////////////
    fvPerson.ChangeMode(e.NewMode);
    BindFormView();
    ////////////////////////////////////////////////////////////////

    相关参考:
    
    MSDN: FormView.ModeChanging	

    (2)	PageIndexChanging 事件: 当这个控件中的一个页面按钮 
    被单击时发生.
    
    为了在新页面中显示数据,我们需要设定新页面的索引 
    然后重新绑定FormView控件. 	

    ////////////////////////////////////////////////////////////////    
    fvPerson.PageIndex = e.NewPageIndex;
    BindFormView();
    ////////////////////////////////////////////////////////////////
    
    相关参考:	
    
    MSDN: FormView.PageIndexChanging 事件


    (3)	ItemInserting 事件: 当FormView中的一个Insert按钮被单击时发生,  
    但是发生在插入操作前.
    
    单击Insert按钮后, 我们需要从 
    FormView控件的InsertItemTemplate中获得姓, 名 
    和特定的图像文件 (bytes) .
    
    ////////////////////////////////////////////////////////////////
    string strLastName = ((TextBox)fvPerson.Row.FindControl("tbLastName")).Text;
    string strFirstName = ((TextBox)fvPerson.Row.FindControl("tbFirstName")).Text;

    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = strLastName;
    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = strFirstName;

    FileUpload uploadPicture = (FileUpload)fvPerson.FindControl("uploadPicture");

    if (uploadPicture.HasFile)
    {
        cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = uploadPicture.FileBytes;
    }
    else
    {
        cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = DBNull.Value;
    }
    ////////////////////////////////////////////////////////////////
                

    相关参考:	
    
    MSDN: FormView.ItemInserting 事件	

    (4)	ItemUpdating 事件: 当FormView中的一个Update按钮被单击时发生,  
    但是发生在更新操作前.
    
    单击Update按钮后, 我们需要从 
    FormView控件的EditItemTemplate中获得和传递PersonID姓, 名 
    和特定的图像文件 (bytes) .
    
    //////////////////////////////////////////////////////////////// 
    string strPersonID = ((Label)fvPerson.Row.FindControl("lblPersonID")).Text;    
    ////////////////////////////////////////////////////////////////

    相关参考:	
    MSDN: FormView.ItemUpdating 事件

    (5)	ItemDeleting事件: 当FormView中的一个Delete按钮被单击时发生,  
    但是发生在删除操作前.
    
    我们从FormView控件的ItemTemplate中获得PersonID 
    然后根据PersonID从数据库中删除这条个人记录.
    
    ////////////////////////////////////////////////////////////////
    string strPersonID = ((Label)fvPerson.Row.FindControl("lblPersonID")).Text;
    ////////////////////////////////////////////////////////////////

    相关参考:	
    
    MSDN: FormView.ItemDeleting 事件


步骤5. 在项目中新建一个名为Image的Web页面. 从示例中复制Page_Load 
方法, 然后粘贴到Image页面的code-behind :

在这个页面, 我们从数据库中获取图片数据, 将其转换为一个字节数组 
然后将这个数组写入HTTP输出流中 
以显示图片.

    //////////////////////////////////////////////////////////////// 
    Byte[] bytes = (byte[])cmd.ExecuteScalar();

    if (bytes != null)
    {
        Response.ContentType = "image/jpeg";
        Response.BinaryWrite(bytes);
        Response.End();
    }
    //////////////////////////////////////////////////////////////// 

相关参考:

MSDN: Request 对象
MSDN: Response 对象


/////////////////////////////////////////////////////////////////////////////
参考资料:

ASP.NET: Using the FormView's Templates
http://www.asp.net/learn/data-access/tutorial-14-cs.aspx

MSDN: Image 类
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.image.aspx

MSDN: FormView 类
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.formview.aspx

MSDN: using 语句 (C# 参考)
http://msdn.microsoft.com/zh-cn/library/yh598w02.aspx

MSDN: FormView.ModeChanging
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.formview.modechanging.aspx

MSDN: FormView.PageIndexChanging 事件
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.formview.pageindexchanging.aspx

MSDN: FormView.ItemInserting 事件
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.formview.iteminserting.aspx

MSDN: FormView.ItemUpdating 事件
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.formview.itemupdating.aspx

MSDN: FormView.ItemDeleting 事件
http://msdn.microsoft.com/zh-cn/library/system.web.ui.webcontrols.formview.itemdeleting.aspx

MSDN: Request 对象
http://msdn.microsoft.com/zh-cn/library/ms524948.aspx

MSDN: Response 对象
http://msdn.microsoft.com/zh-cn/library/ms525405.aspx


/////////////////////////////////////////////////////////////////////////////