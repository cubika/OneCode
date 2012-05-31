/**************************************** 模块头 *****************************************\
* 模块名:  Default.aspx.cs
* 项目名:  CSASPNETuserControlEventsExpose
* 版权 (c) Microsoft Corporation.
* 
* 这个页面载入了usercontrol同时添加usercontrol到网页.
* 然后预定用户控件的MyEvent以反应用户控件的按钮单击.
* 当单击usercontrol的按钮, 网页会显示dropdownlist的选择值.

* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

using System;

namespace CSASPNETuserControlEventExpose
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MyUserControl myUserControl = LoadControl("MyUserControl.ascx") as MyUserControl;
            if (myUserControl != null)
            {
                myUserControl.MyEvent += new MyUserControl.MyEventHandler(userControlBtnClick);
                this.PlaceHolder1.Controls.Add(myUserControl);
            }
        }
        public void userControlBtnClick(object sender, EventArgs e)
        {
            Response.Write("主页面事件句柄<BR/>所选择的值:" + ddlTemp.SelectedItem.Text + "<BR/>");

        }
    }
}
