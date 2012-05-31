/****************************** 模块头 ******************************\
* 模块名: Default.aspx.cs
* 项目名: CSASPNETDragItemInListView
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何使用JQuery在ListView中拖放项。 
* 在本页面中，把两个xml数据文件绑定到ListView并使用项模板来显示它们。 
* 在Default.aspx页面中引用JQuery的javascript库来实现这些功能。  
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
namespace CSASPNETDragItemInListView
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 把两个xml数据文件绑定到ListView控件上，实际上你可以把“打开”属性的值设置为0. 
            // 这样,它将不会显示在ListView控件中。
            XmlDocument xmlDocument = new XmlDocument();
            using (DataTable tabListView1 = new DataTable())
            {
                tabListView1.Columns.Add("value", Type.GetType("System.String"));
                xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/XmlFile/ListView1.xml");
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("root/data[@open='1']");
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    DataRow dr = tabListView1.NewRow();
                    dr["value"] = xmlNode.InnerText;
                    tabListView1.Rows.Add(dr);
                }
                ListView1.DataSource = tabListView1;
                ListView1.DataBind();
            }

            XmlDocument xmlDocument2 = new XmlDocument();
            using (DataTable tabListView2 = new DataTable())
            {
                tabListView2.Columns.Add("value2", Type.GetType("System.String"));
                xmlDocument2.Load(AppDomain.CurrentDomain.BaseDirectory + "/XmlFile/ListView2.xml");
                XmlNodeList xmlNodeList2 = xmlDocument2.SelectNodes("root/data[@open='1']");
                foreach (XmlNode xmlNode in xmlNodeList2)
                {
                    DataRow dr = tabListView2.NewRow();
                    dr["value2"] = xmlNode.InnerText;
                    tabListView2.Rows.Add(dr);
                }
                ListView2.DataSource = tabListView2;
                ListView2.DataBind();
            }
        }
    }
}