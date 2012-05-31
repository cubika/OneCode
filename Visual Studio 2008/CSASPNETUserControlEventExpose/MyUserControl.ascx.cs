/**************************************** 模块头 *****************************************\
* 模块名:  MyUserControl.ascx.cs
* 项目名:  CSASPNETuserControlEventsExpose
* 版权 (c) Microsoft Corporation.
* 
* 用户控件以公用方式声明了委托和事件.
* 因此事件将被预定.
* 当btnTest被单击,他将触发名为'MyEvent'的事件.
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

namespace CSASPNETuserControlEventExpose
{
    public partial class MyUserControl : System.Web.UI.UserControl
    {
        public delegate void MyEventHandler(object sender, EventArgs e);
        public event MyEventHandler MyEvent;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            if (MyEvent != null) this.MyEvent(sender, e);
            Response.Write("用户控件按钮被点击<BR/>");

        }
    }
}