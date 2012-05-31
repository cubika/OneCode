/******************************** 模块头 ********************************\
 * 模块名:           Global.asax.cs
 * 项目名:           CSASPNETMVCCustomActionFilter
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CSASPNETMVCCustomActionFilter
{
    // 备注: 关于启用IIS6或IIS7传统模式的说明, 
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 注册MVC Url路由规则的默认代码

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route名
                "{controller}/{action}/{id}",                           // 带参的URL
                new { controller = "Home", action = "Index", id = "" }  // 默认参数
            );

        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}