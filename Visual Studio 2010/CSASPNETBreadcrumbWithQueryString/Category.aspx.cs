/********************************* 模块头 *********************************\
* 模块名:             Category.aspx.cs
* 项目名:        CSASPNETBreadcrumbWithQueryString
* 版权(c) Microsoft Corporation
*
* 此页面显示项目列表,并显示breadcrumb分类名称.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/
using System;
using System.Web;

namespace CSASPNETBreadcrumbWithQueryString
{
    public partial class Category : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !string.IsNullOrEmpty(Request.QueryString["name"]))
            {
                // 显示项目列表.
                gvItems.DataSource = Database.GetItemsByCategory(Request.QueryString["name"]);
                gvItems.DataBind();

                // 处理SiteMapResolve事件动态改变当前SiteMapNode.
                SiteMap.SiteMapResolve += new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);
            }
        }

        /// <summary>
        /// 当访问CurrentNode属性时发生.
        /// </summary>
        /// <param name="sender">
        /// 事件源, SiteMapProvider类的实例.
        /// </param>
        /// <param name="e">
        /// 包含事件数据的SiteMapResolveEventArgs.
        /// </param>
        /// <returns>
        /// 表示SiteMapResolveEventHandler处理结果的SiteMapNode.
        /// </returns>
        SiteMapNode SiteMap_SiteMapResolve(object sender, SiteMapResolveEventArgs e)
        {
            // 一次请求只执行一次.
            SiteMap.SiteMapResolve -= new SiteMapResolveEventHandler(SiteMap_SiteMapResolve);

            if (SiteMap.CurrentNode != null)
            {
                // SiteMap.CurrentNode是只读的,因此我们必须复制一份进行操作.
                SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);

                currentNode.Title = Request.QueryString["name"];

                // 在breadcrumb中使用已被修改的项.
                return currentNode;
            }
            return null;
        }
    }
}