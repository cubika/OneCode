/****************************** 模块头 ******************************\
* 模块名: Default.aspx.cs
* 项目名: CSASPNETPreventMultipleWindows
* 版权 (c) Microsoft Corporation
*
* 本项目阐述如何在应用程序中检测并阻止多个窗体或标签的使用. 
* 当用户想在一个新窗体或标签中打开这个链接,应用程序回拒绝这些请求. 
* 这个方法可以解决许多安全问题,像共享sessions,保护dupicated登陆,数据并发等. 
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
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

    }
}