========================================================================
                  CSASPNETPrintPartOfPage 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

本项目阐述了如何打印一个网页的特殊部分.一个窗体页面包括
许多部分并且它们中的一些不需要打印,例如按钮控件,你不可能  
在打印的纸上单击它们.所以这个例子提供了一个方法来避免打印
页面上不需要的部分. 

/////////////////////////////////////////////////////////////////////////////
代码演示. 

请参照下面的演示步骤.

步骤1: 打开 CSASPNETPrintPartOfPage.sln.

步骤2: 展开CSASPNETPrintPartOfPage应用程序并按下Ctrl + F5来运行Default.aspx.         

步骤3: 你将会看到Default.aspx页面的许多部分,在页面的中部会有一个“打印这个
        页面”的按钮和四个复选框.

步骤4: 用复选框来选择你想要打印的页面的那个部分,然后单击按钮控件来打印当前 
        页面.如果你没有打印机,选择MicroSoft XPS文件撰写器来测试这个实例.
	    你可以看到除了网页头部的部分页面用.xps文件来打印.			    

步骤5: 验证结束.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. 在Visual Studio 2010或Visual Web Developer 2010中创建一个C#的"ASP.NET空应用程序". 
        把它命名为"CSASPNETPrintPartOfPage". 

步骤2. 在根目录下添加一个窗体页面,命名为"Default.aspx". 

步骤3. 在根目录下添加一个"image"文件夹,添加一个你想在页面上显示的图片.        

步骤4. 在Default.aspx中创建一些表,并用html元素来填充它们,例如：图片,文本,控件等等.  

步骤5. 定义一些公用的字符串来存储html标签并把它们放置在Default.aspx页面上. 
	    [代码]
		    //定义一些字符串,用来截取部分html代码.
            public string printImageBegin, printImageEnd; 

		    //检查复选框的状态,设置div元素. 
            if (CheckBox2.Checked)
            { printImageBegin = string.Empty; printImageEnd = string.Empty; }
            else
            { printImageBegin = enablePirnt; printImageEnd = endDiv; }
		[/代码]
         
步骤6. 使用JavaScript代码根据复选框的状态来打印当前页面,在JavaScript函数
        中来定义按钮的单击事件. 
		css和js代码:
		[代码]
		<style type="text/css" media="print">  
            .nonPrintable
            {
               display: none;
            }
        </style>
        <script type="text/javascript">
            function print_page() {
               window.print();
            }
        </script>
		[/代码]

步骤7. 建立应用程序并且你可以调试它了.


/////////////////////////////////////////////////////////////////////////////
参考文献:

MSDN: window.print 函数
http://msdn.microsoft.com/zh-cn/library/ms536672(VS.85).aspx

MSDN: CSS 参考书
http://msdn.microsoft.com/zh-cn/library/ms531209(VS.85).aspx
/////////////////////////////////////////////////////////////////////////////