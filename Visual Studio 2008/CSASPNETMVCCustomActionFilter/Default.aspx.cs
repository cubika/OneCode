/********************************* 模块头 *********************************\
 * 模块名:  Default.aspx.cs
 * 项目名:  CSASPNETMVCCustomActionFilter
 * 版权 (c) Microsoft Corporation.
 * 
 * CSASPNETMVCCustomActionFilter示例演示了如何使用C# 代码为ASP.NET MVC
 * web应用程序创建自定义ActionFilters. 在本示例中,	
 * 有两个自定义ActionFilters, 一个用来在页面视图结果执行和渲染
 * 前自定义ViewData ; 另一个在ActionResult运行的各种事件中记录信息的
 * 自定义ActionFilter.
 *  
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CSASPNETMVCCustomActionFilter
{
    public partial class _Default : Page
    {
        public void Page_Load(object sender, System.EventArgs e)
        {
            // 改变当前路径路由处理程序可以正确解释请求,
            // 然后恢复原来的路径,以便在OutputCache模块
            // 可以正确处理响应(如果缓存已启用).

            string originalPath = Request.Path;
            HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(HttpContext.Current);
            HttpContext.Current.RewritePath(originalPath, false);
        }
    }
}
