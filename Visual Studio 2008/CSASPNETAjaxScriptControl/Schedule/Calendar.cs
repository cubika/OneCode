/**************************************** 模块头 *****************************************\
* 模块名:  Calendar
* 项目名:  CSASPNETAjaxScriptControl
* 版权 (c) Microsoft Corporation
*
* 日程控件的日历模板. 
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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Drawing;
namespace CSASPNETAjaxScriptControl
{
    public class Calendar: System.Web.UI.WebControls.Calendar
    {
        public Calendar(){}
        public string HighLightDayListType{get;set;}
        public List<string> HighLightDayList{get;set;}
        protected override void OnDayRender(TableCell cell, CalendarDay day)
        {
            cell.Attributes["onmouseover"] = "javasript:this.className='cellDayHightLight';";
            cell.Attributes["onmouseout"] = "javasript:this.className='cellDay';";
            cell.Attributes["RelatedDate"] = day.Date.ToString("yyyy-M-d");
            if (HighLightDayList == null)
                throw new Exception("HighLightDayList can't be null.");
            if (HighLightDayList.Contains(day.Date.ToString(HighLightDayListType)))
            {
                cell.ForeColor = Color.Black;
                cell.Font.Bold = true;
            }
            else
                cell.ForeColor = Color.FromArgb(102,102,102);
            base.OnDayRender(cell, day);
        }
        protected override void OnPreRender(EventArgs e)
        {
            this.SelectedDayStyle.BackColor=Color.FromArgb(197,218,242);  
            base.OnPreRender(e);
        }
    }
}
