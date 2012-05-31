========================================================================
         ASP.NET 应用程序 : CSASPNETMVCDataView 项目 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

  此项目 演示了如何在MVC中使用DataViews执行数据处理工作.



/////////////////////////////////////////////////////////////////////////////
前提条件:

必需Visual Studio 2008的MVC2.0 可以自下列链接下载:

http://www.microsoft.com/downloads/en/details.aspx?FamilyID=c9ba1fe1-3ba8-439a-9e21-def90a8615a9&displaylang=en


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. 确保已安装 Visual Studio 2008上的MVC2.0.

步骤2. 在Visual Studio 2008中创建一个C# ASP.NET MVC 2 Web应用程序并命名
为CSASPNETMVCDataView.

步骤3. 在Controllers文件夹下创建AccountController.cs,HomeController.cs,
PersonalController.cs.

步骤4. 在Models文件夹下创建AccountModels.cs,Person.cs.      

步骤5. 在Views文件夹下添加Account,Home,Personal,Shared子目录, 
并根据示例项目创建对应文件.

步骤6. 根据示例项目创建对应文件夹和文件, 
包括Script文件夹下的脚本文件.

步骤7. 根据示例项目创建模型和控制器逻辑.

[备注] 对于视图页面,映射模型的注册信息将会被自动添加.
参见:

Inherits="System.Web.Mvc.ViewPage<CSASPNETMVCDataView.Models.ChangePasswordModel"

关于如何在MVC2.0中处理数据的详细信息, 参见:

http://www.asp.net/mvc/tutorials/creating-model-classes-with-the-entity-framework-cs

步骤8: 在Models文件夹中的AccountModels.cs文件添加下列代码.

  public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Current password")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm new password")]
        public string ConfirmPassword { get; set; }
    }


步骤9: 根据示例添加其他代码到相关文件.

步骤10: 现在, 你可以运行页面察看我们之前所取得的成就 :)

/////////////////////////////////////////////////////////////////////////////
参考资料:

MVC Tutorials:
http://www.asp.net/mvc/tutorials