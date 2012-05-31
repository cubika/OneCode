========================================================================
    控制台应用程序： VBLinqToXml 工程概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用途：

本例阐述了如何在VB.NET中使用Linq to XML从内存对象和SQL Server数据库来创建XML
文档。它还同时阐明了在VB.NET中如何写Linq to XML查询语句。当从SQL Server数据库
查询数据时它使用了Linq to SQL。 在本例中，您将看到VB.NET XML文字方法来创建，查询和
编辑XML文档。


/////////////////////////////////////////////////////////////////////////////
工程的关联：

VBLinqToXml -> SQLServer2005DB
VBLinqToXml访问由SQLServer2005DB创建的数据库


/////////////////////////////////////////////////////////////////////////////
演示：

下面的步骤展示了LINQ to XML 的例子：

步骤1： 打开DB工程SQLServer2005DB，右键单击工程文件选择Deploy将在你的数据库
		实例中创建SQLServer2005DB数据库

步骤2： 修改连接字符串信息以使得该信息和您的数据库实例和账户匹配
		<add name="...SQLServer2005DBConnectionString" connectionString=...

步骤3：	生成工程VBLinqToXml

步骤4：	运行输出的可执行文件：VBLinqToXml.exe


/////////////////////////////////////////////////////////////////////////////
代码逻辑：

查询内存中的XML文档对象

1.创建基于All-In-One代码框架示例信息的内存对象。
  (VB.NET 9.0 新特性: 对象初始化器）

2.基于内存对象创建XML文档。
  (VB.NET XML 文字)

3.查询内存对象XML文档。
  (VB.NET XML 文字)


查询数据库XML文档

1. 基于在All-In-One代码框架中的SQLServer2005DB数据库的Person表创建XML文档
   (VB.NET XML 文字)
   
2. 查询数据库XML文档.
   (VB.NET XML 文字)
   
注意: 为了查询SQLServer2005DB中的数据, 我们使用了Linq to SQL技术.有关
      Linq to SQL示例的详细信息,请参照All-In-One代码框架中的CSLinqToSQL
      工程。
      

编辑在文件系统中的XML文档

1. 向XML文档中插入新的XML元素.
   {XElement.Add, VB.NET XML 文字}
   
2. 修改特定XML元素的值
   (XElement.Value, VB.NET XML 文字)
   
3. 删除特定XML元素
   (XElement.Remove) 
   

/////////////////////////////////////////////////////////////////////////////
参考资料:

对象初始化器: 命名和匿名类型
http://msdn.microsoft.com/en-us/library/bb385125.aspx

Visual Basic中的命名空间 (LINQ to XML)
http://msdn.microsoft.com/en-us/library/bb387016.aspx

在Visual Basic中创建XML
http://msdn.microsoft.com/en-us/library/bb384808.aspx

XML中的嵌入表达式
http://msdn.microsoft.com/en-us/library/bb384964.aspx


/////////////////////////////////////////////////////////////////////////////