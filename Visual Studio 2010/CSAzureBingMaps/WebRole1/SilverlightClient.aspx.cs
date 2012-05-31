/********************************* 模块头 **********************************\
* 模块名:  SilverlightClient.aspx.cs
* 项目名:  AzureBingMaps
* 版权 (c) Microsoft Corporation.
* 
* Silverlight客户端主机aspx页面后台代码.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;

namespace AzureBingMaps.WebRole
{
    public partial class SilverlightClient : System.Web.UI.Page
    {
        // 下列属性将作为默认参数传递给Silverlight.
        public bool IsAuthenticated { get; set; }
        public string WelcomeMessage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            // 查询会话数据检查用户是否被验证.
            if (Session["User"] != null)
            {
                this.IsAuthenticated = true;
                this.WelcomeMessage = "欢迎: " + (string)Session["User"] + ".";
            }
            else
            {
                this.IsAuthenticated = false;
                this.WelcomeMessage = null;
            }
        }
    }
}