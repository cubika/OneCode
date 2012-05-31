========================================================================
  ASP.NET 应用程序 :  VBASPNETInfiniteLoading 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

无限滚动,也被称为:自动加载分页,无页码,无终止页面. 但是它的本质是从后面
的页面预先取内容,并且把它直接添加到用户的当前页面. 这个代码实例演示把
大量数据全部加载到一个XML文件中. 它使用了AJAX技术支持无限滚动. 


/////////////////////////////////////////////////////////////////////////////
代码演示： 

直接打开VBASPNETInfiniteLoading.sln,展开应用程序的节点并按下F5来测试应用程序. 

步骤1.  在浏览器中查看default.aspx .  

步骤2.  默认情况下,我们可以看到页面上有一个垂直滚动条,拖动它向下滚动,
         你将会看到新的内容无限加载,同时滚动条变得越来越小. 
	 注意: 如果页面上没有垂直滚动条,只要适当的缩放页面直到垂直滚动条出现.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1.  在Visual Studio 2010创建一个VB ASP.NET空应用程序. 


步骤2.  创建一个新的目录,"Scripts".右击目录并单击 添加->新建项->JScript文件. 
         我们需要引用jquery的javascript库文件jquery-1.4.1.min.js . 	 


步骤3.  创建一个新目录,"Styles". 右击目录并单击 添加->新建项->样式表文件.引用site.css.  
		 

步骤4.  打开Default.aspx.（如果没有Default.aspx，创建一个.） 
         在头部,添加javascript和样式引用如下： 

	 [代码]    	
    	 <link rel="stylesheet" href="Styles/Site.css" type="text/css" />
         <script type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script>
	 [/代码]

	 编写自动执行的javascript如下：
	 [代码]
	
         $(document).ready(function () {

            function lastPostFunc() {
                $('#divPostsLoader').html('<img src="images/bigLoader.gif">');

                // 向服务器发送一个查询来显示新的内容 
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/Foo",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {

                        if (data != "") {
                            $('.divLoadData:last').after(data.d);
                        }
                        $('#divPostsLoader').empty();
                    }

                })
            };

            // 向下滚动,当滚动条滚动到底部的时候会关联到下面的函数,并会触发lastPostFunc函数.
	    
            $(window).scroll(function () {
                if ($(window).scrollTop() == $(document).height() - $(window).height()) {
                    lastPostFunc();
                }
            });

         });
    
	 [/代码]		
		 
	 要知道更多详细信息，请参考本实例中的Default.aspx.

步骤7.  所有步骤都完成以后,通过向下滚动页面看发生了什么,来测试应用程序. 


/////////////////////////////////////////////////////////////////////////////
参考文献:

http://www.webresourcesdepot.com/load-content-while-scrolling-with-jquery/ 


/////////////////////////////////////////////////////////////////////////////