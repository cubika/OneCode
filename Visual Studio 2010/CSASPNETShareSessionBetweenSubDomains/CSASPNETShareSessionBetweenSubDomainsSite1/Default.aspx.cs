/********************************** 模块头 ***********************************\
* 模块名:        Default.aspx.cs
* 项目名:        CSASPNETShareSessionBetweenSubDomainsSite1
* 版权(c) Microsoft Corporation
*
* 此页面是用来设定值到会话和读取会话值测试
* 会话的值是否已被网站2改变.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;


namespace CSASPNETShareSessionBetweenSubDomainsSite1
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Web站点1显示会话.
            lbMsg.Text = (string)Session["MySession"];
        }

        protected void btnSetSession_Click(object sender, EventArgs e)
        {
            // Web站点1设定会话.
            Session["MySession"] = "来自站点1的会话内容.";
        }
    }
}