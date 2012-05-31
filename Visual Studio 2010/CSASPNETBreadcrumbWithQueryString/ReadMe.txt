==============================================================================
 ASP.NET 应用程序 : CSASPNETBreadcrumbWithQueryString 项目概述
==============================================================================

//////////////////////////////////////////////////////////////////////////////
摘要:

默认情况下,SiteMapPath控件是很静态的.只能显示出可以在站点地图匹配的节点位置.
有时我们想根据查询字符串值改变SiteMapPath控件的名称和路径.
有时我们要建立动态的SiteMapPath.
此代码示例演示如何通过处理SiteMap.SiteMapResolve事件实现这些目标.

//////////////////////////////////////////////////////////////////////////////
演示示例:

1. 打开Default.aspx页面,单击分类列表中的链接来浏览
   Category.aspx页面，然后点击一个链接以浏览Item.aspx页面.
   你可以看到breadcrumb是根据查询字符串值动态显示的节点.

2. 打开DynamicBreadcrumb.aspx页面查看动态创建的breadcrumb.

//////////////////////////////////////////////////////////////////////////////
代码逻辑:

这个示例项目的要点是我们处理SiteMap.SiteMapResolve事件动态创建/改变当前的SiteMapNode.

	SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);

    SiteMapNode SiteMap_SiteMapResolve(object sender, SiteMapResolveEventArgs e)
    {
        // 一次请求只执行一次.
        SiteMap.SiteMapResolve -= new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);

        if (SiteMap.CurrentNode != null)
        {
            // SiteMap.CurrentNode是只读的,因此我们必须复制一份进行操作.
            SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);

            currentNode.Title = Request.QueryString["name"];

            // 在breadcrumb中使用已被修改的项.
            return currentNode;
        }
        return null;
    }


//////////////////////////////////////////////////////////////////////////////
参考资料:

SiteMapPath Web服务器控件概述
http://msdn.microsoft.com/zh-cn/library/x20z8c51.aspx

SiteMap 类
http://msdn.microsoft.com/zh-cn/library/system.web.sitemap.aspx

SiteMap.SiteMapResolve 事件
http://msdn.microsoft.com/zh-cn/library/system.web.sitemap.sitemapresolve.aspx