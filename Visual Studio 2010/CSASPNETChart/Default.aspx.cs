/**************************************** 模块头 *****************************************\
* 模块名:    Default.aspx.cs
* 项目名:        CSASPNETChart
* 版权 (c) Microsoft Corporation
*
* 这个项目演示了如何使用Chart控件在页面中创建一个图表. 
* Chart类的两个重要属性是Series和ChartAreas属性,两者都是集合属性.
* Series集合属性储存Series对象,用来储存将被显示的数据,以及数据的属性.  
* ChartAreas集合属性储存ChartArea对象, 主要用来描绘一个或更多用一组轴的图表.
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
using System.Data;
using System.Web.UI.DataVisualization.Charting;

namespace CSASPNETChart
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = default(DataTable);
            dt = CreateDataTable();

            //设定Chart控件的DataSource属性为DataTabel
            Chart1.DataSource = dt;

            //设定Y轴的两组数据
            Chart1.Series[0].YValueMembers = "Volume1";
            Chart1.Series[1].YValueMembers = "Volume2";

            //设定x轴为日期值
            Chart1.Series[0].XValueMember = "Date";
            
            //用以上设定绑定Chart控件
            Chart1.DataBind();
        }

        
        private DataTable CreateDataTable()
        {
            //创建一个DataTable作为Chart控件的数据源
            DataTable dt = new DataTable();

            //向DataTable添加3列
            dt.Columns.Add("Date");
            dt.Columns.Add("Volume1");
            dt.Columns.Add("Volume2");

            DataRow dr;

            //向表中添加些演示用随机数据
            dr = dt.NewRow();
            dr["Date"] = "Jan";
            dr["Volume1"] = 3731;
            dr["Volume2"] = 4101;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "Feb";
            dr["Volume1"] = 6024;
            dr["Volume2"] = 4324;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "Mar";
            dr["Volume1"] = 4935;
            dr["Volume2"] = 2935;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "Apr";
            dr["Volume1"] = 4466;
            dr["Volume2"] = 5644;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "May";
            dr["Volume1"] = 5117;
            dr["Volume2"] = 5671;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "Jun";
            dr["Volume1"] = 3546;
            dr["Volume2"] = 4646;
            dt.Rows.Add(dr);

            return dt;
        }
    }
}