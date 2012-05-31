========================================================================
         网站 程序 : CSADONETDataService 项目 概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用途:

本示例展示了用3种不同的数据源建立ADO.NET Data Services。分别为 ADO.NET Entity Data Model,
Linq to SQL Data Classes,以及 普通的非关系型数据。 

请注意：Web.Config文件中的connection strings需要被修改以连接到您本地的SQL Server数据库


/////////////////////////////////////////////////////////////////////////////
项目 相关:

CSADONETDataService -> SQLServer2005DB
CSADONETDataService 调用由 SQLServer2005 创建的数据库。


/////////////////////////////////////////////////////////////////////////////
创建 步骤:

A. 创建一个普通的ASP.NET 网站项目

   1. 在 Visual Studio 2008中添加一个新的ASP.NET 网站项目。(C#->Web->ASP.NET Web Application),
      命名为 CSADONETDataService。

   2. 删除自动建立的 Default.aspx 页面以及 App_Data 文件夹。  

   3. 添加2个新的文件夹分别名为 LinqToEntities 以及 LinqToSQL。

B. 创建一个基于 ADO.NET Entity Data Model 的 ADO.NET Data Service。

   1. 在 Visual Studio 2008 中创建一个 ADO.NET Entity Data Model 作为数据源。

      1) 在 LinqToEntities 文件夹中, 添加一个 Entity Data Model 命名为 
         SchoolLinqToEntities.edmx.

      2) 在 Entity Data Model 创建向导中, 配置连接字符串以及数据库信息， 
         设置实体名为 SQLServer2005DBEntities,Data Model 名为 SQLServer2005DBModel.
         (连接到由 All-In-One Code Framework部署的SQLServer2005DB数据库)

      3) 在表格标签中, 选择表 Course, CourseGrade, CourseInstructor, 
         以及 Person.
         (详情请参考： How to: Create a New Entity Data Model.)

    2. 创建以及配置基于 ADO.NET Entity Data Model 的 ADO.NET Data Service。
    
      1) 添加一个新的 ADO.NET Data Service 命名为 SchoolLinqToEntities.svc。

      2) 把 SchoolLinqToEntities Data Service class 设置为基于 SQLServer2005DBEntities。

      3) 创建一个服务操作 CoursesByPersonID 通过主键PersonID来获得他的课程。

      4) 创建一个服务操作 GetPersonUpdateException 来获得 Person 表的更新操作错误信息。

      5) 创建一个服务拦截来过滤 Course 集合，只返回 CourseID 大于4000的 Course记录。

      6) 创建一个服务改变拦截来检查新添加 Person 对象的 PersonCategory值。

      7) 重写 HandleException 方法来抛出 400 Bad Request 错误。

      8) 设置规则表示哪个实体集合以及服务操作是可见的，可操作的，等等。

C. 创建一个基于 LINQ to SQL Classes 的 ADO.NET Data Service。

   1. 在 Visual Studio 2008 中创建一个 LINQ to SQL Class 来当作数据源。

      1) 在 LinqToSQL 文件夹下, 添加一个 LINQ to SQL Class 命名为 SchoolLinqToSQL.dbml.

      2) 将 Course, CourseGrade, CourseInstructor, 以及 Person 表从
         服务器视图拖到 O/R 视图。详细信息, 请参考 Walkthrough: Creating LINQ to SQL Classes (O/R Designer).
         (服务器视图中的数据库连接，是连接到由All-In-One部署的SQLServer2005DB数据库上。)

   2. 实现 IUpdatable 接口来允许 LinqToSQL DataContext Class 的增删改。

      1) 下载 ADO.NET Data Services IUpdatable 对于 LinqToSQL 的实现
         (它包含了 C# 以及 VB.NET 版本).   
         它通过 .NET 反射以及 LinqToSQL 相关的 API 实现了基于 LinqToSQL 的大部分 IUpdatable 的方法，
         也留了一些可以自定义的空间。
         详细信息，请参考：IUpdateable for Linq To Sql 以及 
         Updated Linq to Sql IUpdatable implementation 

      2) 添加下载的 IUpdatableLinqToSQL.cs 到 LinqToSQL 文件夹, 修改 DataContext class， 
         命名为 SchoolLinqToSQLDataContext。

    3. 对 LinqToSQL Data Class 设置数据服务的键，通过使用 DataServiceKey 属性。 

    4. 创建并配置基于 LinqToSQL Classes 的 ADO.NET Data Service。

       1) 添加一个新的 ADO.NET Data Service 命名为 SchoolLinqToSQL.svc。

       2) 设置 SchoolLinqToSQL Data Service class 使其指向 SchoolLinqToSQL.

       3) 创建一个服务操作 SearchCourses ，它通过 T-SQL 命令搜索 Course 对象并返回一个
          Course 对象的 IQueryable 集合。

       4) 设置规则，用以确定实体集合以及服务操作是否可见，是否可更改，等等。

D. 创建一个 ADO.NET Data Service 基于非关系型的 CLR 对象。  

   1. 添加一个新的 ADO.NET Data Service 命名为 Samples.svc。

   2. 创建一个新的数据类。

      1) 创建 Category 类，并添加属性 CategoryName。该类表示示例项目类别。 
         把数据服务的键设置成  CategoryName.

      2) 创建 Project 类，并添加属性 ProjectName, Owner, 以及ProjectCategory。
         该类表示示例项目, 把数据服务的键设置为 ProjectName.

   3. 创建一个 ADO.NET Data Service entity class 基于非关系型的 CLR 对象。

      1) 创建一个类命名为 SampleProjects 并设置 Samples.svc Data 
         Service 指向该 SampleProjects 类。
 
      2) 为 SampleProjects 类声明2个静态成员，categories 类型为 List<Category>
         以及 projects 类型为 List<Project>。 再添加一个静态的构造函数，来初始化
         这2个成员。

      3) 声明2个公共的只读的属性 Categories 以及 Projects 来返回2个静态成员的 IQuerable 表示形态。
         这样 ADO.NET Data Service 的客户端就可以通过2个 IQuerable 属性来获取非关系型的数据。

    4. 实现 IUpdatable 接口的方法来允许对于 SampleProjects 类的 增删改。

       1) 声明一个临时对象来保存临时添加的数据(Category or Project)。

       2) 实现方法 CreateResource ，它使用 Activator.CreateInstance 方法创建给定类型的资源，并附属与它的容器。
       
       3) 实现 SetValue 方法，通过.NET 反射来设置给定属性的值

       4) 实现 ResolveResource 方法来返回 resource 对象所代表的具体类型的值。

       5) 实现 SaveChanges 方法来保存所有当下未保存的操作。
          这个方法首先检查临时添加对象的类型(Category or Project)，
          然后把他们分别添加到对应的集合中去(categories or projects)。

       6) 其他 IUpdatable 方法可以直接抛异常，因为我们现在只需要添加功能。 
          这些方法包括了，AddReferenceToCollection, ClearChanges, DeleteResouce, 
          GetResource, GetValue, RemoveReferenceFromCollection, ResetResource, 
          以及 SetReference. 在 CSADONETDataService 以及 VBADONETDataService
          示例中都有具体实现的例子。

       7) 设置规则，来表示那些实体集合和服务操作是可见的，可更改的，等等。


/////////////////////////////////////////////////////////////////////////////
参考资料:

ADO.NET Data Services
http://msdn.microsoft.com/en-us/data/bb931106.aspx

Overview: ADO.NET Data Services
http://msdn.microsoft.com/en-us/library/cc956153.aspx

How Do I: Getting Started with ADO.NET Data Services over a Relational 
Database
http://msdn.microsoft.com/en-us/data/cc745957.aspx

How Do I: Getting Started with ADO.NET Data Services over a Non-Relational 
Data Source
http://msdn.microsoft.com/en-us/data/cc745968.aspx

How to: Create a New Entity Data Model
http://msdn.microsoft.com/en-us/library/cc716703.aspx

Walkthrough: Creating LINQ to SQL Classes (O/R Designer)
http://msdn.microsoft.com/en-us/library/bb384428.aspx

MSDN API Reference Documentation – Server
http://msdn.microsoft.com/en-us/library/system.data.services.aspx

Using Microsoft ADO.NET Data Services
http://msdn.microsoft.com/en-us/library/cc907912.aspx

ADO.NET Data Services Team Blog
http://blogs.msdn.com/astoriateam/

ADO.NET Data Services IUpdateable implementation for Linq to Sql
http://code.msdn.microsoft.com/IUpdateableLinqToSql

IUpdateable for Linq To Sql 
http://blogs.msdn.com/aconrad/archive/2008/11/05/iupdateable-for-linq-to-sql.aspx

Updated Linq to Sql IUpdatable implementation 
http://blogs.msdn.com/aconrad/archive/2009/03/17/updated-linq-to-sql-iupdatable-implementation.aspx

Service Operations (ADO.NET Data Services)
http://msdn.microsoft.com/en-us/library/cc668788.aspx

Interceptors (ADO.NET Data Services)
http://msdn.microsoft.com/en-us/library/dd744842.aspx

Injecting Custom Logic in ADO.NET Data Services
http://weblogs.asp.net/cibrax/archive/2009/06/08/injecting-custom-logic-in-ado-net-data-services.aspx

IUpdatable &amp; ADO.NET Data Services Framework
http://blogs.msdn.com/astoriateam/archive/2008/04/10/iupdatable-ado-net-data-services-framework.aspx


/////////////////////////////////////////////////////////////////////////////