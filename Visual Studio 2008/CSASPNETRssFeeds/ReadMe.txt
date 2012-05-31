===========================================================================
             CSASPNETRssFeeds 项目 概述
===========================================================================

/////////////////////////////////////////////////////////////////////////////
开始前的备注:

  当你继续浏览这个示例时, 我们假设你熟悉RSS,  
  包括它的用法,  格式, 以及其他. 如果不是，
  请参阅下列链接. 它将告诉你什么是RSS以及RSS的标准文件格式.

  http://www.mnot.net/rss/tutorial/

/////////////////////////////////////////////////////////////////////////////
用途:

  这个项目展示了如何使用ASP.NET新建一个RSS源. 
  示例中的AddArticle页面用来更新数据库. 我们可以插入, 编辑, 更新
  和删除一条记录, 即示例中的一份文献, 然后转到RSS页面显示更新. 
  通过使用XML命名空间写的各种类, RSS页面新建了一个
  能被用户订阅的RSS源, 这样用户可以在网站有任何变更时收到通知.  

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1: 在Visual Studio 2008 / Visual Web Developer中新建一个
C# ASP.NET Web应用程序将其命名为CSASPNETRssFeeds.

步骤2: 向应用程序的App_Data文件夹中添加一个数据库文件. 简化起见,
这个数据库和应用程序同名.

步骤3: 在数据库中新建一个名为Article的表， 并加入如下列: 
ArticleID, Title, Author, Link, Description, PubDate.

步骤4: 向数据库中插入一条测试记录. 后面我们将用到这条测试记录.

步骤5: 向项目中添加一个名为AddArticle.aspx的ASP.NET页面, 它将
用来更新数据库表.

步骤6: 向上述页面添加一个DataSource和一个FormView控件. 简而言之, 
设定DataSource链接到Article表然后将这个DataSource绑定到FormView. 
启用FormView的分页接着运行这个页面确定DataTable完全受控制, 
即记录可以直接通过FormView插入, 编辑, 更新和删除.

步骤7:  向项目中添加一个名为Rss.aspx的ASP.NET页面. 这是本项目的起始页.

步骤8: 在rss页面的codebehind中编写一段从Article表中获取数据的代码. 
这段代码看上去像这样.

    private DataTable GetDateSet()
    {
        DataTable ArticlesRssTable = new DataTable();
        
        string strconn = ConfigurationManager.ConnectionStrings["ConnStr4Articles"].ConnectionString;
        SqlConnection conn = new SqlConnection(strconn);
        string strsqlquery = "SELECT * FROM [Articles]";

        SqlDataAdapter da = new SqlDataAdapter(strsqlquery, conn);
        da.Fill(ArticlesRssTable);

        return ArticlesRssTable;
    }

[备注] 这个函数返回一个包含的通过以上SQL查询字符串从Article表中选择的记录DataTable. 
作为测试, 它返回表中所有的记录. 
然而, 通常我们只需要从表中获取最近的文献的前20条记录
这样处理过程不会花很长时间RSS页面也不会太大.
总之, 这取决于对RSS源的需求.

步骤9: 编写新建RSS XML文件的代码. 因为只是编写代码的工作,  
请参照这份示例中的Rss.aspx.cs文件.

[备注] 因为RSS源是一个XML格式的文件而不是一个通常的web页面, 
我们需要定义Response.ContentType属性. 同时, unicode
必须包含在RSS源中, 我们也需要定义当前回应的ContentEncoding 
属性.

    Response.ContentType = "application/rss+xml";
    Response.ContentEncoding = Encoding.UTF8;

因为RSS源内容包含三个部分: 开始, 本体和结尾. 
我们将用三种方法分别处理: WriteRssOpening, WriteRssBody
和WriteRssEnding. 当写入本体时, 我们使用Each语句循环遍历
DataTable中的数据将其写入一个节点并写入title, author, link, 
description和pubDate字段到节点下的属性中.

    foreach (DataRow rssitem in data.Rows)
    {
        rsswriter.WriteStartElement("item");
        rsswriter.WriteElementString("title", rssitem[1].ToString());
        rsswriter.WriteElementString("author", rssitem[2].ToString());
        rsswriter.WriteElementString("link", rssitem[3].ToString());
        rsswriter.WriteElementString("description", rssitem[4].ToString());
        rsswriter.WriteElementString("pubDate", rssitem[5].ToString());
        rsswriter.WriteEndElement();
    }

最终, 当所有的工作都已完成, 我们需要结束这个回应. 否则,
将会产生一些毁掉之前所有工作的错误, 只能在页面上返回一个
"Internet Explorer cannot display this feed"的错误信息. 
因此, *请勿*忘记在Page_Load事件句柄的末尾加上如下代码.

    Response.End();

/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: XmlTextWriter 类
http://msdn.microsoft.com/zh-cn/library/system.xml.xmltextwriter.aspx

MSDN: RSS Tutorial
http://www.mnot.net/rss/tutorial/

/////////////////////////////////////////////////////////////////////////////