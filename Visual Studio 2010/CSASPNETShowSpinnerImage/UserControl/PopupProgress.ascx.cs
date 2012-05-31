/********************************* 模块头 *********************************\
* 模块名:     PopupProgress.ascx.cs
* 项目名:     CSASPNETShowSpinnerImage
* 版权(c) Microsoft Corporation
* 
* 当用户点击Default.aspx页面上的按钮时,这个用户控件将显示一个弹出窗口。
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace CSASPNETShowSpinnerImage
{
    public partial class PopupProgress : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// 此方法用于加载自定义文件的图像和
        /// 注册用户控件上页面的JavaScript代码。
        /// </summary>
        /// <returns></returns>
        public string LoadImage()
        {
            StringBuilder strbScript = new StringBuilder();
            string imageUrl = "";

            strbScript.Append("var imgMessage = new Array();");
            strbScript.Append("var imgUrl = new Array();");
            string[] strs = new string[7];
            strs[0] = "Image/0.jpg";
            strs[1] = "Image/1.jpg";
            strs[2] = "Image/2.jpg";
            strs[3] = "Image/3.jpg";
            strs[4] = "Image/4.jpg";
            strs[5] = "Image/5.jpg";
            strs[6] = "Image/6.jpg";
            for (int i = 0; i < strs.Length; i++)
            {
                imageUrl = strs[i];

                strbScript.Append(String.Format("imgUrl[{0}] = '{1}';", i, imageUrl));
                strbScript.Append(String.Format("imgMessage[{0}] = '{1}';", i, imageUrl.Substring(imageUrl.LastIndexOf('.') - 1)));
            }
            strbScript.Append("for (var i=0; i<imgUrl.length; i++)");
            strbScript.Append("{ (new Image()).src = imgUrl[i]; }");
            return strbScript.ToString();
        }

    }
}