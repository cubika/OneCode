/*********************************** 模块头 *********************************\
* 模块名: ShowMe.aspx.cs
* 项目名: CSASPNETEmbedLanguageInUrl
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何在URL中插入语言编码,例如：
*  http://domain/en-us/ShowMe.aspx. 页面会根据语言
* 编码呈现不同的内容,在例子中使用url-地址和源文件
* 来本地化页面的内容.
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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using Resources;
using System.Resources;
using System.Globalization;

namespace CSASPNETEmbedLanguageInUrl
{
    public partial class Default : BasePage
    {
        /// <summary>
        /// 用一种确定的语言加载页面.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] elementArray = Session["info"].ToString().Split(',');
            string language = elementArray[0];
            string pageName = elementArray[1];
            if (language == "error")
            {
                Response.Write("url地址错误:请在Default.aspx页面重新启动应用程序.");
                return;
            }
            string xmlPath = Server.MapPath("~/XmlFolder/Language.xml");
            string strTitle = string.Empty;
            string strText = string.Empty;
            string strElement = string.Empty;
            bool flag = false;

            // 加载xml数据.
            XmlLoad xmlLoad = new XmlLoad();
            xmlLoad.XmlLoadMethod(language, out strTitle, out strText, out strElement, out flag);

            // 如果特定的语言不存在,返回这个网页的英文版.
            if (flag == true)
            {
                language = "en-us";
                Response.Write("没有该语言的资源,将使用英文网页.");
                xmlLoad.XmlLoadMethod(language, out strTitle, out strText, out strElement, out flag);
            }
            lbTitleContent.Text = strTitle;
            lbTextContent.Text = strText;
            lbTimeContent.Text = DateTime.Now.ToLongDateString();
            lbCommentContent.Text = strElement;
            
 
        }
    }
}