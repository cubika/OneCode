/**************************************** 模块头 *****************************************\
* 模块名:  CSASPNETPageValidationServerSide
* 项目名:  CSASPNETPageValidation
* 版权 (c) Microsoft Corporation
*
* CSASPNETPageValidationServerSide页面演示了ASP.NET验证控件在服务器端验证值. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/
#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
#endregion


namespace CSASPNETPageValidation
{
    public partial class CSASPNETPageValidationServerSide : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void RequiredFieldValidator1_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                // 获得要验证的输入TextBox.
                TextBox tx = (TextBox)this.FindControl(RequiredFieldValidator1.ControlToValidate);
                if (string.IsNullOrEmpty(tx.Text))
                {
                    RequiredFieldValidator1.ErrorMessage = "Required field cannot be left blank.";
                }
            }
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // 检测值的长度是否有6位
            if (args.Value.Length >= 6)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }
    }
}
