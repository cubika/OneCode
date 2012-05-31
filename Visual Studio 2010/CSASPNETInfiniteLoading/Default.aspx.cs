/********************************** 模块头 **********************************\
* 模块名:    Default.aspx.cs
* 项目名:    CSASPNETInfiniteLoading
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何使用AJAX技术实现向下滚动来加载新页面的内容. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using System.Text;

namespace CSASPNETInfiniteLoading
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        [WebMethod]
        public static string Foo()
        {
            StringBuilder getPostsText = new StringBuilder();
            using (DataSet ds = new DataSet())
            {
                ds.ReadXml(HttpContext.Current.Server.MapPath("App_Data/books.xml"));
                DataView dv = ds.Tables[0].DefaultView;

                foreach (DataRowView myDataRow in dv)
                {
                    getPostsText.AppendFormat("<p>作者: {0}</br>", myDataRow["author"]);
                    getPostsText.AppendFormat("种类: {0}</br>", myDataRow["genre"]);
                    getPostsText.AppendFormat("价格: {0}</br>", myDataRow["price"]);
                    getPostsText.AppendFormat("出版时间: {0}</br>", myDataRow["publish_date"]);
                    getPostsText.AppendFormat("简介: {0}</br></p>", myDataRow["description"]);
                }
                getPostsText.AppendFormat("<div style='height:15px;'></div>");

            }
            return getPostsText.ToString();
        }
    }
}
