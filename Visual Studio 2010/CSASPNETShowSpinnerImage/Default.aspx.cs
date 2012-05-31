/********************************* 模块头 *********************************\
* 模块名:     Default.aspx.cs
* 项目名:     CSASPNETShowSpinnerImage
* 版权(c) Microsoft Corporation
* 
* 本页面是用于从XML文件中检索数据,并包含了PopupProgeress用户控件。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Data;

namespace CSASPNETShowSpinnerImage
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            // 在这里我们使用Thread.sleep()代码暂停线程10秒模仿
            // 一个昂贵、耗时的操作检索数据（如连接网络
            // 数据库检索海量数据）
            // 所以在实际的应用中,您可以删除此行。 
            System.Threading.Thread.Sleep(10000);

            // 从XML文件中检索数据作为示例数据.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "XMLFile/XMLData.xml");
            DataTable tabXML = new DataTable();
            DataColumn columnName = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn columnAge = new DataColumn("Age", Type.GetType("System.Int32"));
            DataColumn columnCountry = new DataColumn("Country", Type.GetType("System.String"));
            DataColumn columnComment = new DataColumn("Comment", Type.GetType("System.String"));
            tabXML.Columns.Add(columnName);
            tabXML.Columns.Add(columnAge);
            tabXML.Columns.Add(columnCountry);
            tabXML.Columns.Add(columnComment);
            XmlNodeList nodeList = xmlDocument.SelectNodes("Root/Person");
            foreach (XmlNode node in nodeList)
            {
                DataRow row = tabXML.NewRow();
                row["Name"] = node.SelectSingleNode("Name").InnerText;
                row["Age"] = node.SelectSingleNode("Age").InnerText;
                row["Country"] = node.SelectSingleNode("Country").InnerText;
                row["Comment"] = node.SelectSingleNode("Comment").InnerText;
                tabXML.Rows.Add(row);
            }
            gvwXMLData.DataSource = tabXML;
            gvwXMLData.DataBind();
        }


    }
}