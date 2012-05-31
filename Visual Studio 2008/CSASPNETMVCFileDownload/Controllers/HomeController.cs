/********************************* 模块头 *********************************\
模块名:      HomeController.cs
项目名:      CSASPNETMVCFileDownload
版权 (c) Microsoft Corporation.

CSASPNETMVCFileDownload项目演示了如何创建ASP.NET MVC FileDownload
应用程序. 应用程序支持基本网站导航, 浏览文件共享中的文件同时
允许客户自文件列表中下载指定文件.
 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSASPNETMVCFileDownload.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        // 索引请求行为
        public ActionResult Index()
        {
            ViewData["Message"] = "欢迎来到ASP.NET MVC!";

            return View();
        }

        // 关于请求行为
        public ActionResult About()
        {
            return View();
        }
    }
}
