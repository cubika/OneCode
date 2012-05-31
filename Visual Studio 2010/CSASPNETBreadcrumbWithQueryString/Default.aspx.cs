/********************************* 模块头 *********************************\
* 模块名:             Default.aspx.cs
* 项目名:        CSASPNETBreadcrumbWithQueryString
* 版权(c) Microsoft Corporation
*
* 这是用来显示分类列表的根页面.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/

using System;

namespace CSASPNETBreadcrumbWithQueryString
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 显示分类列表.
                gvCategories.DataSource = Database.Categories;
                gvCategories.DataBind();
            }
        }
    }
}