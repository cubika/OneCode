/****************************** 模块头 ******************************\
* 模块名: DefaultPage.ascx.cs
* 项目名: CSASPNETPreventMultipleWindows
* 版权 (c) Microsoft Corporation
*
* 这是一个用在开始页面的用户控件.它将得到一个随机的字符串并把它赋值给window.name.
* 最后，跳转到Main.aspx页面. 
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

namespace CSASPNETPreventMultipleWindows
{
    public partial class DefaultPage : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 当被调用时，这个方法将会得到一个随机的字符串
        /// 并存储在session中
        /// </summary>
        /// <returns>返回这个随机字符串</returns>
        public string GetWindowName()
        {
            string WindowName = Guid.NewGuid().ToString().Replace("-", "");
            Session["WindowName"] = WindowName;
            return WindowName;
        }
    }
}