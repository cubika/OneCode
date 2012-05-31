/******************************** 模块头 ********************************\
 * 模块名:           HomeController.cs
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

namespace CSASPNETMVCCustomActionFilter.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        // 使用声明属性来注册MessageModifier ActionFilter
        [CSASPNETMVCCustomActionFilter.ActionFilters.MessageModifierActionFilter]
        public ActionResult Index()
        {
            // 通过ViewData集合返回的Message数据初始值 
            ViewData["Message"] = "欢迎来到ASP.NET MVC!";

            return View();
        }

        // 使用声明属性来注册Logging ActionFilter
        [CSASPNETMVCCustomActionFilter.ActionFilters.TextLogActionFilter(LogFileName = @"D:\temp\logs\mvc_action_filter.log")]
        public ActionResult About()
        {
            return View();
        }
    }
}
