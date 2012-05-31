/*********************************** 模块头 *********************************\
* 模块名: BasePage.cs
* 项目名: CSASPNETEmbedLanguageInUrl
* 版权 (c) Microsoft Corporation
*
* 不同语言的网页是继承的这个类.
* BasePage类会检查请求的url语言部分和和名称部分,
* 并且设置页面的Culture和UICultrue属性.
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
using System.Threading;
using System.Globalization;

namespace CSASPNETEmbedLanguageInUrl
{
    public class BasePage:Page
    {
        public BasePage()
        {

        }

        /// <summary>
        /// BasePage类用来设置Page.Culture和Page.UICulture.
        /// </summary>
        protected override void InitializeCulture()
        {
            try
            {
                string language = RouteData.Values["language"].ToString().ToLower();
                string pageName = RouteData.Values["pageName"].ToString();
                Session["info"] = language + "," + pageName;
                Page.Culture = language;
                Page.UICulture = language;
            }
            catch (Exception)
            {
                Session["info"] = "error,error";
            }
            
        }
    }
}