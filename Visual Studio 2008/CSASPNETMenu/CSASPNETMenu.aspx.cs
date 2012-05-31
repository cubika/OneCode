/****************************** 模块头 ******************************\
* 模块名:    CSASPNETMenu.aspx.cs
* 项目名:        CSASPNETMenu
* 版权 (c) Microsoft Corporation.
*
* 这个示例展示了如何绑定 ASP.NET 菜单控件到数据库.
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace CSASPNETMenu
{
    public partial class CSASPNETMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GenerateMenuItem();
            }
        }

        public void GenerateMenuItem()
        {
            // 从数据库中获得数据.
            DataSet ds = GetData();

            foreach (DataRow mainRow in ds.Tables[0].Rows)
            {
                // 从主表中加载记录到菜单中.
                MenuItem masterItem = new MenuItem(mainRow["mainName"].ToString());
                masterItem.NavigateUrl = mainRow["mainUrl"].ToString();
                Menu1.Items.Add(masterItem);

                foreach (DataRow childRow in mainRow.GetChildRows("Child"))
                {
                    // 根据主表和子表的关系, 加载子表的数据.
                    MenuItem childItem = new MenuItem((string)childRow["childName"]);
                    childItem.NavigateUrl = childRow["childUrl"].ToString();
                    masterItem.ChildItems.Add(childItem);
                }
            }
        }

        public DataSet GetData()
        {
            // 为了测试, 我们使用内存的DataTable代替数据库.
            DataTable mainTB = new DataTable();
            DataColumn mainIdCol = new DataColumn("mainId");
            DataColumn mainNameCol = new DataColumn("mainName");
            DataColumn mainUrlCol = new DataColumn("mainUrl");
            mainTB.Columns.Add(mainIdCol);
            mainTB.Columns.Add(mainNameCol);
            mainTB.Columns.Add(mainUrlCol);

            DataTable childTB = new DataTable();
            DataColumn childIdCol = new DataColumn("childId");
            DataColumn childNameCol = new DataColumn("childName");

            // 子表中的 MainId 列是连接到主表的外键.
            DataColumn childMainIdCol = new DataColumn("MainId");         
            DataColumn childUrlCol = new DataColumn("childUrl");

            childTB.Columns.Add(childIdCol);
            childTB.Columns.Add(childNameCol);
            childTB.Columns.Add(childMainIdCol);
            childTB.Columns.Add(childUrlCol);


            // 添加一些记录到主表.
            DataRow dr = mainTB.NewRow();
            dr[0] = "1";
            dr[1] = "Home";
            dr[2] = "test.aspx";
            mainTB.Rows.Add(dr);
            DataRow dr1 = mainTB.NewRow();
            dr1[0] = "2";
            dr1[1] = "Articles";
            dr1[2] = "test.aspx";
            mainTB.Rows.Add(dr1);
            DataRow dr2 = mainTB.NewRow();
            dr2[0] = "3";
            dr2[1] = "Help";
            dr2[2] = "test.aspx";
            mainTB.Rows.Add(dr2);
            DataRow dr3 = mainTB.NewRow();
            dr3[0] = "4";
            dr3[1] = "DownLoad";
            dr3[2] = "test.aspx";
            mainTB.Rows.Add(dr3);


            // 添加一些记录到子表
            DataRow dr5 = childTB.NewRow();
            dr5[0] = "1";
            dr5[1] = "ASP.NET";
            dr5[2] = "2";
            dr5[3] = "test.aspx";
            childTB.Rows.Add(dr5);
            DataRow dr6 = childTB.NewRow();
            dr6[0] = "2";
            dr6[1] = "SQL Server";
            dr6[2] = "2";
            dr6[3] = "test.aspx";
            childTB.Rows.Add(dr6);
            DataRow dr7 = childTB.NewRow();
            dr7[0] = "3";
            dr7[1] = "JavaScript";
            dr7[2] = "2";
            dr7[3] = "test.aspx";
            childTB.Rows.Add(dr7);

            // 使用一个 DataSet 包含这两个表.
            DataSet ds = new DataSet();          
            ds.Tables.Add(mainTB);
            ds.Tables.Add(childTB);

            // 绑定主表和子表的关系.
            ds.Relations.Add("Child", ds.Tables[0].Columns["mainId"], ds.Tables[1].Columns["MainId"]);
           

            return ds;
        }  

    }
}
