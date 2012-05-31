========================================================================
    控制台应用程序 ： VBEFForeignKeyAssociation项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用途：

VBEFForeignKeyAssociation示例展示了Entity Framework(EF) 4.0的一个新特性，
Foreign Key Association。此示例比较了新的Foreign Key Association和Independent Association，
并且展示了怎样插入一个新的关联实体，通过两个关联插入已存在的实体和更新已存在实体。


/////////////////////////////////////////////////////////////////////////////
先决条件：

1. 请将文件夹_External_Dependencies中的数据库文件EFDemoDB.mdf附加到SQL Server 2008。

2. 请根据你的数据库实例名更改App.config中的连接字符串。

/////////////////////////////////////////////////////////////////////////////
代码逻辑：

Foreign Key Association：

1. 使用Foreign Key Association建立实体数据模型。  
   1) 建立文件夹FKAssociation。
   2) 添加一个ADO.NET实体数据模型并命名为FKAssociation.edmx放入文件夹FKAssociation中。  
   3) 设置数据库EFDemoDB的连接字符串信息。
      注意: 请首先阅读“先决条件”部分！
   4) 选中数据表Course和Department。
      注意：记住要选中复选框"在模型中加入外键列"，使其允许Foreign Key Association！

2. 创建一个静态类FKAssociationClass来测试利用Foreign Key Association的插入和更新方法。
   1) 创建方法InsertNewRelatedEntities来利用Foreign Key Association来插入一条新的Course和Department实体。
   2) 创建方法InsertByExistingEntities来利用Foreign Key Association来插入一条新的Course并将其设置成附属于
	  一个已存在的Department。
   3) 创建方法UpdateExistingEntities来利用Foreign Key Association关系更新一个已存在的Course实体。
   4) 创建方法Query来查询Course实体和对应的Department实体。
   5) 创建方法Test来执行利用Foreign Key Association的插入和更新方法。

Independent Association：

1. 创建一个Independent Association的实体数据模型。
   1) 创建文件夹IndependentAssociation。
   2) 向文件夹IndependentAssociation中添加一个ADO.NET实体数据模型并命名为
   IndependentAssociation.edmx。
   3) 设置数据库EFDemoDB的连接字符串信息。
	  注意: 请首先阅读“先决条件”部分！
   4) 选中数据表Course和Department。
      注意： 记住不要选中复选框"在模型中加入外键列"，使其允许Independent Association！

2. 创建一个静态类IndependentAssociationClass来测试Independent Association的插入和更新方法。
   1) 创建方法InsertNewRelatedEntities在Independent Association下插入一条新的Course和Department实体。
   2) 创建方法InsertByExistingEntities在Independent Association下插入一条新的Course并将其设置成附属于
	  一个已存在的Department。
   3) 创建方法UpdateExistingEntities来更新一个已存在的Course实体(只有常规属性)。
   4) 创建方法Query来查询Course实体和对应的Department实体。
   5) 创建方法Test来执行Independent Association的插入和更新方法。


/////////////////////////////////////////////////////////////////////////////
参考资料:

Defining and Managing Relationships (Entity Framework)
http://msdn.microsoft.com/en-us/library/ee373856%28VS.100%29.aspx

How to: Use EntityReference Object to Change Relationships Between Objects(Entity Framework)
http://msdn.microsoft.com/en-us/library/cc716754(VS.100).aspx

How to: Use the Foreign Key Property to Change Relationships Between Objects
http://msdn.microsoft.com/en-us/library/ee473440(VS.100).aspx

Foreign Keys in the Entity Framework
http://blogs.msdn.com/efdesign/archive/2009/03/16/foreign-keys-in-the-entity-framework.aspx

Foreign Keys in the Conceptual and Object Models
http://blogs.msdn.com/efdesign/archive/2008/10/27/foreign-keys-in-the-conceptual-and-object-models.aspx

Foreign Key Relationships in the Entity Framework
http://blogs.msdn.com/adonet/archive/2009/11/06/foreign-key-relationships-in-the-entity-framework.aspx


/////////////////////////////////////////////////////////////////////////////