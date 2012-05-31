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
using System.IO;

namespace CSASPNETMVCCustomActionFilter.ActionFilters
{
    public class TextLogActionFilter : ActionFilterAttribute
    {
        // 指定日志文件路径的属性
        public string LogFileName { get; set; }

        // 默认构造器
        public TextLogActionFilter() { LogFileName = "MVC_ACTION_FILTER_LOG.TXT"; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            StreamWriter sw = File.AppendText(LogFileName);
            sw.WriteLine(">>>TextLogActionFilter.OnActionExecuted, {0}", DateTime.Now);
            sw.Close(); 
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            StreamWriter sw = File.AppendText(LogFileName);
            sw.WriteLine(">>>TextLogActionFilter.OnActionExecuting, {0}", DateTime.Now);
            sw.Close();
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            StreamWriter sw = File.AppendText(LogFileName);
            sw.WriteLine(">>>TextLogActionFilter.OnResultExecuted, {0}", DateTime.Now);
            sw.Close();
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        { 
            StreamWriter sw = File.AppendText(LogFileName);
            sw.WriteLine(">>>TextLogActionFilter.OnResultExecuting, {0}", DateTime.Now);
            sw.Close();
        }

    }
}
