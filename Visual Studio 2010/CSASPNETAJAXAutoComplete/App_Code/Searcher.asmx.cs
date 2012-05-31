/************************************* 模块头 **************************************\ 
* 模块名: Searcher.cs
* 项目名: CSASPNETAJAXAutoComplete
* 版权 (c) Microsoft Corporation
*
* 这个项目说明了如何在文本框中输入文字时使用AutoCompleteExtender
* 在文字显示中加上前缀.当用户输入的字符大于指定的最小长度时，
* 将会在用户所输入字符前弹出并显示一些文字或词组.
* 默认的情况下,这些词组列将会放置在textbox的左下角.
* 
* This source is subject to the Microsoft Public License. 
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
* All other rights reserved. 
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\******************************************************************************************/


using System;
using System.Collections.Generic;
using System.Web.Services;
namespace CSASPNETAJAXAutoComplete
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    public class Searcher : System.Web.Services.WebService
    {

        public Searcher()
        {
        }

        [WebMethod]
        public string[] HelloWorld(string prefixText, int count)
        {
            if (count == 0)
            {
                count = 10;
            }

            if (prefixText.Equals("xyz"))
            {
                return new string[0];
            }

            Random random = new Random();
            List<string> items = new List<string>(count);
            char c1;
            char c2;
            char c3;
            for (int i = 0; i < count; i++)
            {
                c1 = (char)random.Next(65, 90);
                c2 = (char)random.Next(97, 122);
                c3 = (char)random.Next(97, 122);
                items.Add(prefixText + c1 + c2 + c3);
            }

            return items.ToArray();
        }

    }
}