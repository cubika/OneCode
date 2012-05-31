/********************************* 模块头 *********************************\
* 模块名:             DynamicBreadcrumb.aspx.cs
* 项目名:        CSASPNETBreadcrumbWithQueryString
* 版权(c) Microsoft Corporation
*
* 此页面显示即使一个页面不在站点地图中,我们依然可以动态创建breadcrumb. 
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
    public partial class DynamicBreadcrumb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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

            // 我们可以在此创建很多SiteMapNodes.
            SiteMapNode childNode = new SiteMapNode(SiteMap.Provider, "2");
            childNode.Url = "/child.aspx";
            childNode.Title = "子页面";

            childNode.ParentNode = new SiteMapNode(SiteMap.Provider, "1");
            childNode.ParentNode.Url = "/root.aspx";
            childNode.ParentNode.Title = "根页面";

            // 我们也可以关联动态节点到已有的站点地图.
            SiteMapNode nodeFromSiteMap = GetSiteMapNode("item");
            if (nodeFromSiteMap != null)
            {
                childNode.ParentNode.ParentNode = nodeFromSiteMap;
            }

            // 在breadcrumb中使用已被修改的项.
            return childNode;
        }

        /// <summary>
        /// 自站点地图获取siteMapNode.
        /// </summary>
        /// <param name="key">
        /// siteMapNode的resourceKey.
        /// </param>
        /// <returns></returns>
        SiteMapNode GetSiteMapNode(string key)
        {
            return GetSiteMapNode(SiteMap.RootNode, key);
        }
        SiteMapNode GetSiteMapNode(SiteMapNode rootNode, string key)
        {
            if (rootNode.ResourceKey == key)
            {
                return rootNode;
            }
            else if (rootNode.HasChildNodes)
            {
                foreach (SiteMapNode childNode in rootNode.ChildNodes)
                {
                    SiteMapNode resultNode = GetSiteMapNode(childNode, key);
                    if (resultNode != null)
                    {
                        return resultNode;
                    }
                }
            }
            return null;
        }
    }
}