========================================================================
         ASP.NET 应用程序 : CSASPNETExcelLikeGridView 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法和介绍:

  此项目说明了如何做一个批量插入,删除和更新，而不是逐行插入,删除,更新.

/////////////////////////////////////////////////////////////////////////////

演示:

 1) 打开项目然后右击"Default.aspx", 选择"在浏览器中查看";
 2) 当您在GridView中勾选复选框来标记要删除的行时,单元格将变红.
 3) 当您通过单击添加按钮创建新行时,新行默认为绿色.
 4) 当您改变GridView中单元格的值时, 单元格背景将变蓝.
 5) 当您单击保存按钮，所有的改变（包括修改,删除以及添加的数据）将被执行批处理.

代码逻辑:

步骤1. 在Visual Studio 2010 RC/Visual Web Developer 2010中创建一个
C# ASP.NET Web 应用程序并命名为CSASPNETExcelLikeGridView.

[ 备注: 你可以自下列链接下载免费的Web Developer:
 http://www.microsoft.com/express/Web/ ]

步骤2. 右击你的项目标签, 选择 “添加…”-->“新项目…”. 
展开“Visual Studio C#” 标签选择 “Sql Server 数据库”. 命名为“db_Persons.mdf”.
 然后在“App_Data” 文件夹中右击新创建的数据库, 选择 “打开”. 在左侧
“服务器浏览器”面板中咱开标签“db_Persons.mdf”, 右击名为 “Tables”的文件夹, 
选择“添加新表” 创建一张如App_Data文件架结构所见的表并保存为
"tb_personInfo". 


[ 备注: 你可以下载免费的Web开发器:
 http://www.microsoft.com/express/Web/ ]

[ 备注: 你也可以下载免费的SQL 2008:
 http://www.microsoft.com/express/Database/ ]

步骤3. 删除下列由Visual Studio自动创建的默认文件夹和文件.

Account folder
Script folder
Style folder
About.aspx file
Default.aspx file
Global.asax file
Site.Master file



步骤4. 在web.config文件中添加一个连接字符串:
<connectionStrings>
    <add name="MyConn" 
    connectionString="server=.\SQLEXPRESS;DataBase=db_Persons;
    integrated security=true"/>
</connectionStrings>

步骤5. 右击鼠标按钮，创建一个名为“DBProcess.cs“的类.在文件中创建
来处理与数据库相关操作的类.这个“DBProcess.cs“
将创建一个“table.dat“序列化文件，以保持DataTable状态更多功能请参阅详细注释）.

步骤6. 
拖放一个GridView,添加一些模板字段,以及一些复选框,
一个“添加“按钮,一个“保存“按钮,并在ASPX实现中设置为我所提到的所有属性.

步骤7. 为了实现不刷新的修改符号（单元格背景色）。
我们应该写一些jQuery函数.您可以在相同的Default.aspx他们
HTML实现中找到详细注释.

/////////////////////////////////////////////////////////////////////////////
参考资料:

ASP.NET QuickStart Torturial:

1）http://www.asp.net/data-access/tutorials/batch-deleting-cs

2）http://www.asp.net/data-access/tutorials/batch-updating-vb

3）http://www.asp.net/data-access/tutorials/batch-inserting-vb

/////////////////////////////////////////////////////////////////////////////