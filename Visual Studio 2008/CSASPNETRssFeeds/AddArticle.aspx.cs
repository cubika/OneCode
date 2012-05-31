/**************************************** 模块头 *****************************************\
* 模块名:    AddArticle.aspx.cs
* 项目名:    CSASPNETRssFeeds
* 版权 (c) Microsoft Corporation
*
* 这个页面支持一个更新数据库的特性
* 用来测试RSS页面是否正常工作.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETRssFeeds
{
    public partial class AddArticle : System.Web.UI.Page
    {
        protected void ArticleFormView_PreRender(object sender, EventArgs e)
        {
            TextBox PubDateTextBox = (TextBox)ArticleFormView.FindControl("PubDateTextBox");
            if (PubDateTextBox != null)
            {
                PubDateTextBox.Text = DateTime.Now.ToShortDateString();
            }
        }
    }
}
