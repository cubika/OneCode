/**************************************** 模块头 *****************************************\
* 模块名:   Default.aspx.cs
* 项目名:   CSASPNETImageMap
* 版权 (c) Microsoft Corporation
*
* 这个项目展示了如何使用ImageMap创建用C#语言编写的太阳系行星系统的说明. 
* 当图片中的行星被单击时, 关于这个行星的简要信息将被显示在图片下面
* 同时iframe将被导航到WikiPedia上关联的页面. 
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace CSASPNETImageMap
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imgMapSolarSystem_Click(object sender, ImageMapEventArgs e)
        {
            ifSelectResult.Attributes["src"] = "http://zh.wikipedia.org/wiki/" + e.PostBackValue;

            switch (e.PostBackValue)
            {
                case "太阳":
                    //当用户单击太阳
                    lbDirection.Text = "太阳是位于太阳系中心的恒星.";

                    break;
                case "水星":
                    lbDirection.Text = "水星是太阳系最内侧同时也是最小的行星.";

                    break;
                case "金星":
                    lbDirection.Text = "金星是离太阳第二近的行星.";

                    break;
                case "地球":
                    lbDirection.Text = "地球是太阳的第三颗行星. 同时也是我们所居住的世界, 又称为蓝星.";

                    break;
                case "火星":
                    lbDirection.Text = "火星是太阳系中离太阳第四近的行星.";

                    break;
                case "木星":
                    lbDirection.Text = "木星是太阳的第五颗行星同时也是太阳系最大的行星.";

                    break;
                case "土星":
                    lbDirection.Text = "土星是自太阳起第七颗行星, 也是太阳系中仅次于木星第二大的行星.";

                    break;
                case "天王星":
                    lbDirection.Text = "天王星是自太阳起第七颗行星, 同时也是太阳系中第三大和第四重的行星.";

                    break;
                case "海王星":
                    lbDirection.Text = "海王星是我们的太阳系中自太阳起第八颗行星.";

                    break;
                default:

                    break;
            }
        }
    }
}
