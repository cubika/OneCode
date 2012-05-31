======================================================================================
      Windows 应用程序: VBDynamicallyBuildLambdaExpressionWithField 概述                        
======================================================================================

//////////////////////////////////////////////////////////////////////////////////////
摘要:  

这个实例演示了如何动态创建lambda表达式,和将数据显示在 DataGridView 控件中.

这个实例演示了如何将多个条件连结在一起，并动态生成LINQ到SQL. LINQ是非常好的方法，它用
类型安全、直观、极易表现的方式，来声明过滤器和查询数据. 例如，该应用
程序中的搜索功能，可以让客户找到满足多个列所定义条件的一切记录.
 
   
//////////////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在Visual Studio 2010中生成这个项目.
 
步骤2. 根据下述网站的提示，下载 SQL2000SampleDb.msi:
       http://www.microsoft.com/downloads/en/details.aspx?FamilyID=06616212-0356-46a0-8da2-eebc53a68034&displaylang=en
      
步骤3. 将它安装到您的系统目录，启动SQL Server Management Studio.

步骤4. 右键单击写成"Databases"的树节点，然后左键单击"Attach...".

步骤5. 请确保Northwind.MDF 和Northwind.LDF已安装在系统目录中并且对它们有完全的访问控制权.
       在上述条件下，在“Attach Databases”对话框内，选择"Add..."按钮，以附加Northwind数
	   据库.
	  
步骤6. 在 VBDynamicallyBuildLambdaExpressionWithField 项目里，选择 app.config，以修改连接
       字符串，或者双击 VBDynamicallyBuildLambdaExpressionWithField 项目中的 
	   Settings.settings，修改名为“Value”的列的值.
	
步骤7. 右击 VBDynamicallyBuildLambdaExpressionWithField 项目，点击"Set as StartUp Project"
       菜单项.

步骤8. 按F5快捷键启动程序，并选择条件字段、 条件运算符和条件值.

步骤9. 点击“搜索”按钮，查看由LINQ到SQL分析的结果.


//////////////////////////////////////////////////////////////////////////////////////
代码逻辑:
1. 有三个主要的类: Condition, Condition(Of T), and Condition(Of T,S)

   a. Condition 是一个抽象类,用于构造泛型版本.通过构建它，我们可以获得泛型类型参数推理
   的好处，——即，我们不用为传递泛型类型参数给方法担心；工厂方法可为我们推导出它.

   b. Condition(Of T) 用来将多重条件连接到一起. T 是元素类型（即，在上面的示例中的Order).

   c. Condition(Of T,S) 是最简单的类; 它表示了一个"object.propery<comparison> value"表达
   式.类型参数 S 将被推断为传入值的类型(即，String,Date,Boolean...).

2. 对于本地执行的查询,我们将Lambda表达式编译到一个委托中，以便在内存中执行它. 用户可以通
   过调用 Matches 方法来激发该委托.
   
  	 ' 将 lambda 表达式编译到一个委托中.
  	del = CType(LambdaExpr.Compile(), Func(Of T, Boolean))

3. 底部的扩展方法定义为用于远程执行的 IQueryable(Of T)，和用于本地执行的 IEnumerable(Of T).


//////////////////////////////////////////////////////////////////////////////////////
References:

Implementing Dynamic Searching Using LINQ 
http://blogs.msdn.com/b/vbteam/archive/2007/08/29/implementing-dynamic-searching-using-linq.aspx

LINQ to SQL (Part 9 - Using a Custom LINQ Expression with the <asp:LinqDatasource> control) 
http://weblogs.asp.net/scottgu/archive/2007/09/07/linq-to-sql-part-9-using-a-custom-linq-expression-with-the-lt-asp-linqdatasource-gt-control.aspx


//////////////////////////////////////////////////////////////////////////////////////
