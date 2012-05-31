/*********************************** 模块头 *********************************\
* 模块名: UrlRoutingHandlers.cs
* 项目名: CSASPNETEmbedLanguageInUrl
* 版权 (c) Microsoft Corporation
*
* UrlRoutingHandlers将会检查请求的url.
* 这个接口会截断url字符串,检查文件名如果不存在  
* 就把它们传到InvalidPage.aspx页面.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/



using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using System.Text;
using System.Web.Compilation;
using System.Web.UI;

namespace CSASPNETEmbedLanguageInUrl
{
    public class UrlRoutingHandlers : IRouteHandler
    {
        /// <summary>
        /// 创建RoutingHandler来检查Http请求和 
        /// 返回正确的url地址.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IHttpHandler GetHttpHandler(RequestContext context)
        {
            string language = context.RouteData.Values["language"].ToString().ToLower();
            string pageName = context.RouteData.Values["pageName"].ToString();
            if (pageName == "ShowMe.aspx")
            {
                return BuildManager.CreateInstanceFromVirtualPath("~/ShowMe.aspx", typeof(Page)) as Page;
            }
            else
            {
                return BuildManager.CreateInstanceFromVirtualPath("~/InvalidPage.aspx", typeof(Page)) as Page;
            }
        }
    }
}