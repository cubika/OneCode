/********************************* 模块头 *********************************\
 * 模块名:     Global.asax.cs
 * 项目名:     CSASPNETMVCFileDownload
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CSASPNETMVCFileDownload
{
    // 备注: 关于启用IIS6或IIS7传统模式的说明, 
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        // 注册所有MVC Url路由规则的函数
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 忽略axd资源请求规则
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // 文件下载请求规则
            routes.MapRoute(
                "FileDownload",
                 "File/{Action}/{fn}",
                 new { controller = "File", action = "List", fn = "" }
                 );

            // 其他泛用规则
            routes.MapRoute(
                "Default",                                              // Route名
                "{controller}/{action}/{id}",                           // 带参的URL
                new { controller = "Home", action = "Index", id = "" }  // 默认参数
            );


        }

        protected void Application_Start()
        {
            // 所有规则都必须在应用程序开始前注册
            RegisterRoutes(RouteTable.Routes);
        }
    }
}