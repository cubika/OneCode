/******************************** 模块头 ********************************\
 * 模块名:           MessageModifierActionFilter.cs
 * 项目名:           CSASPNETMVCCustomActionFilter
 * 版权 (c) Microsoft Corporation.
 * 
 * CSASPNETMVCCustomActionFilter示例演示了如何使用C# 代码为ASP.NET MVC
 * web应用程序创建自定义ActionFilters. 在本示例中
 * sample,	有两个自定义ActionFilters, 一个用来在页面视图结果执行和渲染
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
using System.Text;
using System.Web.Mvc;

namespace CSASPNETMVCCustomActionFilter.ActionFilters
{
    public class MessageModifierActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // 如果我们修改此处的ViewData, 变更将不会被反映到最终的PageView 
            // 因为已经太晚了(结果已被执行)

        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // 此事件是一个我们可以自定义ViewData集合的预处理事件 

            ViewResult view = filterContext.Result as ViewResult;
            if (view != null)
            {
                view.ViewData["Message"] = "[由MessageModifierActionFilter.OnResultExecuting修改]"
                    + view.ViewData["Message"].ToString();
            }
        }
    }
}
