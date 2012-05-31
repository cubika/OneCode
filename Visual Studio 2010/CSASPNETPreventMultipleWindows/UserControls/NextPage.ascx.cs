/****************************** 模块头 ******************************\
* 模块名: NextPage.ascx.cs
* 项目名: CSASPNETPreventMultipleWindows
* 版权 (c) Microsoft Corporation
*
* 这是一个用在其它页面的用户控件.它将得到Window Name 
* 并且检查是否允许这个跳转请求.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.

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
    public partial class NextPage : System.Web.UI.UserControl
    {
        public const string InvalidPage = "InvalidPage";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 这个方法可以从Default.aspx中取得window name.
        /// </summary>
        /// <returns></returns>
        public string GetWindowName()
        {
            if (Session["WindowName"] != null)
            {
                string WindowName = Session["WindowName"].ToString();
                return WindowName;
            }
            else
            {
                return InvalidPage;
            }
        }
    }
}