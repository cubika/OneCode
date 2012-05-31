================================================================================
       Windows 应用程序: CSRichTextBoxSyntaxHighlighting 项目概述                       
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:
这个例子证明了怎么在RichTextBoxControl中格式化xml文件和突出显示元素。

RichTextBoxControl能处理RTF（富文本格式）文件，这是由微软开发的，采用了已发布的规范格式的专有的文档文件。

一个简单的RTF文件，如下：

{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe2052
{\fonttbl{\f0\fnil Courier New;}}
{\colortbl ;\red0\green0\blue255;\red139\green0\blue0;\red255\green0\blue0;\red0\green0\blue0;}
\viewkind4\uc1\pard\cf1\f0\fs24 
<?\cf2 xml \cf3 version\cf1 =\cf0 "\cf1 1.0\cf0 " \cf3 encoding\cf1 =\cf0 "\cf1 utf-8\cf0 "\cf1 ?>\par
<\cf2 html\cf1 >\par
    <\cf2 head\cf1 >\par
        <\cf2 title\cf1 >\par
            \cf4 My home page\par
        \cf1 </\cf2 title\cf1 >\par
    </\cf2 head\cf1 >\par
    <\cf2 body \cf3 bgcolor\cf1 =\cf0 "\cf1 000000\cf0 " \cf3 text\cf1 =\cf0 "\cf1 ff0000\cf0 " \cf1 >\par
        \cf4 Hello World!\par
    \cf1 </\cf2 body\cf1 >\par
</\cf2 html\cf1 >\par
}


它包含两个部分：头和内容。这个在头部的colorbl元素包含了所有在文件中定义的颜色。\cfN 代表前景颜色而\par代表一个新的段。



////////////////////////////////////////////////////////////////////////////////
演示:

第一步:在vs2010中创建项目。

第二步：运行CSRichTextBoxSyntaxHighlighting .exe文件。

第三步：在UI界面中，黏贴下面的xml脚本到RichTextBox中。

	   <?xml version="1.0" encoding="utf-8" ?><html><head><title>My home page</title></head><body bgcolor="000000" text="ff0000">Hello World!</body></html>


Step4. Click the "Process" button, then the text in the RichTextBox will be changed to 
       following XML with colors.

	   <?xml version="1.0" encoding="utf-8"?>
	   <html>
			<head>
				<title>
					My home page
				</title>
			</head>
			<body bgcolor="000000" text="ff0000" >
				Hello World!
			</body>
	   </html>


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1.设计一个XMLViewerSettings类，这个类定义了在XMLViewer中使用的颜色和一些用来在RTF中指定颜色顺序的常量。

 
2.设计一个VMLViewer类，这个类继承自 System.Windows.Forms.RichTextBox。它用来以规范的格式来显示一个xml文件。RichTextBox使用RTF格式来显示测试。XMLViewer类通过使用在XMLViewSettings中规定的一些格式来实现xml到RTF的转换，然后设置RTF属性为这个值。

3.CharacterEncoder类提供一个Static方法，去编码在XML和RTF中的一个特殊的字符，例如 '<', '>', '"', '&', ''', '\',
   '{' and '}' 。

/////////////////////////////////////////////////////////////////////////////
参考资料:

RichTextBox类
http://msdn.microsoft.com/en-us/library/system.windows.forms.richtextbox.aspx
RTF规范，版本1.6
http://msdn.microsoft.com/en-us/library/aa140277(office.10).aspx


/////////////////////////////////////////////////////////////////////////////
