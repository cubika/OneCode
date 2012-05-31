=============================================================================
     控制台应用程序 ：	   VBEFEntityDataaModel 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
用法：

本例通过多种方式说明了如何使用EDM。 其中包括多对多关系、一对多关系、一对一
关系、表的合并、表的拆分、每个层次结构一个表继承和每种类型一个表继承。 在本例
中您将看到针对实体的插入、更新和查询操作。


/////////////////////////////////////////////////////////////////////////////
先决条件：
1.请将_External_Dependencies文件夹下的EFDemoDB.mdf 数据库文件附加到(attach) 
  您的 SQL Server 2005 Express 或者是SQL Server 2005 数据库实例上.
2.请根据您的数据库实例名称修改存储在 App.config 文件中的链接字符串. 


/////////////////////////////////////////////////////////////////////////////
创建过程：
/////////////////////////////////////////////////////////////////////////////
多对多关系
/////////////////////////////////////////////////////////////////////////////

Tables:(主要的相关的列)
[Course]
 CourseID [PK]
 Title
 
[CourseInstructor]
 CourseID [PK] [FK]
 PersonID [PK] [FK]
 
[Person]
 PersonID [PK]
 LastName
 FirstName
 
创建EDM(实体数据模型)的步骤：
1) 添加 -> 新项 -> ADO.NET 实体数据模型.
2) 选择 EFDemoDB -> 勾选上面的3个表
3) 得到具有多对多关系的模型
4) 生成解决方案

/////////////////////////////////////////////////////////////////////////////
一对多关系
/////////////////////////////////////////////////////////////////////////////

Tables:(主要的相关的列)
[Department]
DepartmentID [PK]
Name

[Course]
 CourseID [PK]
 Title
 DepartmentID [FK]
 

创建EDM(实体数据模型)的步骤：
1) 添加 -> 新项 -> ADO.NET 实体数据模型.
2) 选择 EFDemoDB -> 勾选上面的2个表
3) 得到具有一对多关系的模型
4) 生成解决方案


/////////////////////////////////////////////////////////////////////////////
一对一关系
/////////////////////////////////////////////////////////////////////////////

Tables:(主要的相关的列)
[Person]
 PersonID [PK]
 LastName
 FirstName

[PersonAddress]
 PersonID [PK] [FK]
 Address
 Postcode


创建EDM(实体数据模型)的步骤：
1) 添加 -> 新项 -> ADO.NET 实体数据模型.
2) 选择 EFDemoDB -> 勾选上面的2个表
3) 得到具有一对一关系的模型
4) 生成解决方案


/////////////////////////////////////////////////////////////////////////////
合并表
/////////////////////////////////////////////////////////////////////////////

Tables:(主要的相关的列)
[Person]
 PersonID [PK]
 LastName
 FirstName

[PersonAddress]
 PersonID [PK] [FK]
 Address
 Postcode


创建EDM(实体数据模型)的步骤：
1) 添加 -> 新项 -> ADO.NET 实体数据模型.
2) 选择 EFDemoDB -> 勾选上面的2个表
3) 在PersonAddress实体中剪切(Ctrl+X)Address和Postcode属性并且将它们粘帖
   (Ctrl+V)到Person实体中.   
4) 删除PersonAddress实体.
5) 右键单击Person实体 -> 合并表 -> 添加表或试图 -> 
   在下拉列表中选择PersonAddress表
6) 生成解决方案.


/////////////////////////////////////////////////////////////////////////////
每个层次结构一个表继承
/////////////////////////////////////////////////////////////////////////////
Tables:(主要的相关的列)
[Person]
 PersonID [PK]
 LastName
 FirstName
 PersonCategory
 HireDate
 EnrollmentDate
 Picture
 BusinessCredits
 AdminDate
 

创建EDM(实体数据模型)的步骤：
1) 添加 -> 新项 -> ADO.NET 实体数据模型.
2）选择EFDemoDB -> 勾选Person表.
3）右键单击空白处 -> 添加 -> 实体
4）实体名称：Student 基类型：Person
5）重复第三步并且参照以下添加实体
   实体名: Instructor      基类型: Person
   实体名: Admin           基类型: Person
   实体名: BusinessStudent 基类型: Student
6）在Person实体中的属性上单击并且按ctrl+x，然后在适当的实体的Scalar 属性部分
   单击并且按ctrl + v.
   
   要剪切和粘帖的属性如下：
   EnrollmentDate –> Student 
   AdminDate      –> Admin 
   HireDate       –> Instructor 
   BusinessCredits–> BusinessStudent
7）右键单击Student实体 -> 映射表 -> 将这个实体映射到Person表.对Admin,Instructor 
   和BusinessStudent执行相同的操作.
8) 右键单击Person实体 -> 映射表 -> 点击 "Add a Condition" -> 选择PersonCategory
   -> Value = 0. 
   
   对于其他实体执行同样的操作并且设置如下： 
   Student         1
   Instrutor       2
   Admin           3
   BusinessStudent 4
9）在Person实体中删除PersonCategory.必须在第7步后执行.
10)生成解决方案. 


/////////////////////////////////////////////////////////////////////////////
每种类型一个表继承
/////////////////////////////////////////////////////////////////////////////
Tables:(主要的相关的列)
[PersonTPT]
 PersonID [PK]
 LastName
 FirstName

[StudentTPT]
 PersonID [PK] [FKtoPersonTPT]
 Degree
 EnrollmentDate

[InstructorTPT]
 PersonID [PK] [FKtoPersonTPT]
 HireDate

[AdminTPT]
 PersonID [PK] [FKtoPersonTPT]
 AdminDate
 
[BusinessStudentTPT]
 PersonID [PK] [FKtoStudentTPT]
 BusinessCredits

创建EDM(实体数据模型)的步骤：
1) 添加 -> 新项 -> ADO.NET 实体数据模型.
2) 选择 EFDemoDB -> 选择这5张表
3) 将PersonTPT重命名为Person，对于其他实体执行同样的操作
4) 将Person实体的实体集名重命名为People.
5) 删除所有生产的一对多关联.
6) 对于Person (基类型) 和 StudentCreate创建继承.对Instructor和Admin entities.
   执行同样的操作
7) 对于Student(基类型) 和 BusinessStudent创建继承
8) 保留Person实体中的PersonID实体，删除所有继承的实体中的PersonID实体.
9) 右键单击Student实体 -> 合并表 -> 在下拉列表里将PersonID属性映射到PersonID.
   对Instructor,Admin和BusinessStudent执行同样的操作.
10)生成解决方案


/////////////////////////////////////////////////////////////////////////////
拆分表
/////////////////////////////////////////////////////////////////////////////

Tables:(主要的相关的列)
[Person]
 PersonID [PK]
 LastName
 FirstName
 PersonCategory
 Picture

创建EDM(实体数据模型)的步骤：
1) 添加 -> 新项 -> ADO.NET 实体数据模型.
2) 选择EFDemoDB -> 选择Person表.
3) 点击Person表 -> 拷贝Person实体(Ctrl+ C).
4) 单击空白处 -> 粘帖Person实体(Ctrl + V) -> 将它重名为"PersonDetail".
5) 删除Person实体内除了PersonID, LastName和FirstName外的所有其他属性.
6) 删除PersonDetail实体内的LastName和FirstName属性.
7) 在"Person"和"PersonDetail"间添加一个1:1关联，命名为PersonPersonDetail.
8) 右键单击PersonDetail -> 映射表 -> 将这个实体映射到Person表.
9) 右键单击TblSplitEntitie.edmx -> 打开方式 -> XML 编辑器.
10)在CSDL部分添加ReferentialConstraint元素到PersonPersonDetail，如下所示

	<Association Name="PersonPersonDetail">
	  <End Type="EFTblSplitModel.Person" Role="Person" Multiplicity="1" />
	  <End Type="EFTblSplitModel.PersonDetail" Role="PersonDetail" 
	  Multiplicity="1" />
	  <ReferentialConstraint>
		<Principal Role="Person">
		  <PropertyRef Name="PersonID"/>
		</Principal>
		<Dependent Role="PersonDetail">
		  <PropertyRef Name="PersonID"/>
		</Dependent>
	  </ReferentialConstraint>
	</Association>
	
11)生成解决方案.


/////////////////////////////////////////////////////////////////////////////
参考资料：

ADO.NET Entity Framework
http://msdn.microsoft.com/en-us/library/bb399572.aspx

实体数据模型
http://msdn.microsoft.com/en-us/library/bb387122.aspx

合并表
http://blogs.msdn.com/simonince/archive/2009/03/23/mapping-two-tables-to-one-entity-in-the-entity-framework.aspx

每个层次结构一个表继承
http://www.robbagby.com/entity-framework/entity-framework-modeling-table-per-Hierarchy-inheritance/

每种类型一个表继承
http://www.robbagby.com/entity-framework/entity-framework-modeling-table-per-type-inheritance/

表的拆分
http://blogs.msdn.com/adonet/archive/2008/12/05/table-splitting-mapping-multiple-entity-types-to-the-same-table.aspx


/////////////////////////////////////////////////////////////////////////////
