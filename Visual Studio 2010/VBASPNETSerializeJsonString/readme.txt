========================================================================
   ASP.NET 应用程序 : VBASPNETSerializeJsonString 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

这个项目阐述了如何序列化Json字符串. 我们在客户端使用jQuery并且在服务端操作XML数据.

这里通过一个autocomplete的例子展示如何序列化Json数据.  


/////////////////////////////////////////////////////////////////////////////
代码演示：

直接打开VBASPNETSerializeJsonString.sln文件, 展开web应用程序的节点并且按下F5键
运行该应用程序.

步骤 1. 在浏览器中查看default.aspx页面

步骤 2. 在默认情况下, 我们可以在页面的顶端看到一个可搜索的文本框, 
你可以输入一个字符，例如字符"m", 你会在输入的字符串之下看到
一个autocomplete的列表, 移动鼠标选择一个书的名称，然后你可以 
在结果区域内找到找到该书的相关信息.


/////////////////////////////////////////////////////////////////////////////
Code Logical:

步骤 1. 在 Visual Studio 2010中创建一个VB ASP.NET空的Web应用程序.

步骤 2. 添加一个名称为book的vb类文件，定义这些类的字段: id,lable,value,author,genre,price,
pubdeclare_datthe class membersscription. 本类用来存储书籍的信息.

步骤 3. 在 Visual Studio 2010中添加一个名称为"AutoComplete"VB语言的.ashx文件. 
在方法"ProcessRequest"中按照以下逻辑写出代码：

	 1, 导入一个XML dataset文件并填充dataset记录
     2, 指定的字段必须与Book类的字段相一致
     3, 实例化一个"Collection<Book>"的类，并往"Collection<Book>"中添加新成员
     4, 序列化"Collection<Book>"对象     


步骤 4. 创建一个新的文件夹, 名称为"Scripts". 右键单击文件夹并选择 添加-新建项-
JScript 文件. 我们需要添加Jquery javascript类库的引用来支持AutoComplete
的效果.本例中需要的类库文件: jquery.min.js,jquery-ui.min.js


步骤 5. 创建一个新的文件夹，名称为"Styles". 右键单击文件夹并选择 添加-新建项-
Style Sheet 文件. 添加Jquery UI 样式文件的引用，名为jquery-ui.css.
为了让示例看起来效果更好，这里添加了一个其他UI类库的引用"site.css".
		 

步骤 6. 打开Default.aspx页，（如果没有Default.aspx页，创建一个.
在<head>标签里, 添加javascript和css样式表的引用，如下所示：
	 [CODE]
    	 <link rel="stylesheet" href="Styles/jquery-ui.css" type="text/css" media="all" />
    	 <link rel="stylesheet" href="Styles/site.css" type="text/css" />
    	 <script type="text/javascript" src="Scripts/jquery.min.js"></script>
    	 <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
	 [/CODE]

	 在autocomplete控件里添加以下jquery代码.
	 [CODE]
	 <script type="text/javascript">
         $(function () {
            $('#<%= tbBookName.ClientID %>').autocomplete({
                source: "AutoComplete.ashx",
                select: function (event, ui) {
                    
                    $(".author").text(ui.item.Author);
                    $(".genre").text(ui.item.Genre);
                    $(".price").text(ui.item.Price);
                    $(".publish_date").text(ui.item.Publish_date);
                    $(".description").text(ui.item.Description);
                }
            });
         });
    	 </script>
	 [CODE]		
		 
	 更多细节信息，请参考示例代码中的Default.aspx页.

步骤 7. 所有的工作已准备好，开始测试程序，希望你可以成功.


/////////////////////////////////////////////////////////////////////////////
参考资料:

http://msdn.microsoft.com/en-us/library/system.web.script.serialization.javascriptserializer.aspx