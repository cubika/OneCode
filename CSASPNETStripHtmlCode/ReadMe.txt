========================================================================
                  CSASPNETStripHtmlCode 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

本项目阐述了怎样截取并解析Html代码.
由于网页经常包含许多有用的信息,例如,标题,文本,图片,链接,表等等.  
有时我们需要从网页上截取关键字或资源,所以这个代码示例用正则表达  
式来帮助我们完成这项工作.

/////////////////////////////////////////////////////////////////////////////
代码演示： 

请参照下面的这些演示步骤.

步骤1: 打开CSASPNETStripHtmlCode.sln.

步骤2: 展开CSASPNETStripHtmlCode应用程序并按住Ctrl + F5来  
        显示Default.aspx.

步骤3: 你将会在页面上看到一个多行的文本框和一些按钮.
        依次单击这些按钮.

步骤4: 截取的信息显示在文本框中,你可以看到整个html代码,脚 
        本代码,纯文本,图片和链接.

步骤5: 验证结束.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1. 在Visual Studio 2010或Visual Web Developer 2010中创建一个
       C#的"ASP.NET空应用程序".把它命名为"CSASPNETStripHtmlCode. 

步骤2. 在根目录中添加两个web窗体,把它们命名为"Default.aspx", 
        "SourcePage.aspx".

步骤3. 在源网页中添加：脚本代码,一些链接,一些图片和文本.
        我们将会从中截取有用信息.

步骤4. 在Default页面添加一个多行文本控件和五个按钮,这个web窗
        体用来从原网页中截取信息.
		
步骤5  添加一些C#代码如下,截取整个的Html代码: 
		[代码]
            /// <summary>
            /// 用WebRequest和WebRespond从SourcePage.aspx中截取完整的html代码.
            ///  我们把html代码的格式转换成uft-8. 
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public string GetWholeHtmlCode(string url)
            {
                string strHtml = string.Empty;
                StreamReader strReader = null;
                WebResponse wrpContent = null;
                try
                {
                    WebRequest wrqContent = WebRequest.Create(url);
                    wrqContent.Timeout = 300000;
                    wrpContent = wrqContent.GetResponse();
                    if (wrpContent != null)
                    {
                        strReader = new StreamReader(wrpContent.GetResponseStream(),
						    Encoding.GetEncoding("utf-8"));
                        strHtml = strReader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    strHtml = e.Message;
                }
                finally 
                {
                    if (strReader != null)
                        strReader.Close();
                    if (wrpContent != null)
                        wrpContent.Close();
                }
                return strHtml;
            }
	    [/代码]
	    使用正则表达式来截取并解析整个html代码的特殊部分 
	    例如,截取纯文本按钮,它的C#代码如下: 
	    [代码]
		    string strRegexScript = @"(?m)<body[^>]*>(\w|\W)*?</body[^>]*>";
            string strRegex = @"<[^>]*>";
            string strMatchScript = string.Empty;
            Match matchText = Regex.Match(strWholeHtml, strRegexScript, 
			    RegexOptions.IgnoreCase);
            strMatchScript = matchText.Groups[0].Value;
            string strPureText = Regex.Replace(strMatchScript, strRegex,
			    string.Empty, RegexOptions.IgnoreCase);
            tbResult.Text = string.Empty;
            tbResult.Text = strPureText;
	    [/代码] 

步骤5. 添加其它按钮控件的单击事件,像例子的"Default.aspx.cs"文件. 
    

步骤6. 建立应用程序并且你可以调试它.


/////////////////////////////////////////////////////////////////////////////
参考文献:

MSDN: .NET Framework Regular Expressions
http://msdn.microsoft.com/zh-cn/library/hs600312.aspx

MSDN: Regex Class
http://msdn.microsoft.com/zh-cn/library/system.text.regularexpressions.regex.aspx

MSDN: WebRequest Class
http://msdn.microsoft.com/zh-cn/library/system.net.webrequest.aspx

MSDN: WebResponse Class
http://msdn.microsoft.com/zh-cn/library/system.net.webresponse.aspx
/////////////////////////////////////////////////////////////////////////////