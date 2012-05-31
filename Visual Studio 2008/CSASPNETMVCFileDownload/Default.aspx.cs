/********************************* 模块头 *********************************\
 * 模块名:      Default.aspx.cs
 * 项目名:      CSASPNETMVCFileDownload
 * 版权 (c) Microsoft Corporation.
 * 
 * CSASPNETMVCFileDownload项目演示了如何创建ASP.NET MVC FileDownload
 * 应用程序. 应用程序支持基本网站导航, 浏览文件共享中的文件同时
 * 允许客户自文件列表中下载指定文件.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/

using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CSASPNETMVCFileDownload
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
