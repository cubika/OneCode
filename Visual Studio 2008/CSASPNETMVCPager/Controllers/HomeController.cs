/**************************************** 模块头 *****************************************\
* 模块名:   HomeController.cs
* 项目名:   CSMVCPager
* 版权 (c) Microsoft Corporation
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSASPNETMVCPager.Models;
using CSASPNETMVCPager.Helper;
namespace CSASPNETMVCPager.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        /// <summary>
        /// 用来显示分页
        /// </summary>
        /// <param name="id">当前页面序号</param>
        /// <returns>操作结果</returns>
        public ActionResult Index(int? id)
        {
            int pageIndex = Convert.ToInt32(id);
            List<Employee> empList = EmployeeSet.Employees;
            int pagesSize = 5;

            Pager<Employee> pager = new Pager<Employee>(empList, pageIndex, Url.Content("~/Home/Index"), Url.Content("~/images/left.gif"), Url.Content("~/images/right.gif"), pagesSize);

            ViewData["pager"] = pager;
            return View(empList.Skip(pager.pageSize * pageIndex).Take(pager.pageSize));
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
