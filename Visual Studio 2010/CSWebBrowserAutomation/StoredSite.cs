/****************************** 模块头 ******************************\
* 模块名:    StoredSite.cs
* 项目名:	    CSWebBrowserAutomation
* 版权    (c) Microsoft Corporation.
* 
* 这个类代表一个站点存储的HTML元素。一个站点是以XML文件的格式被保存在StoredSites下，并且能被反序列化。
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
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Security.Permissions;

namespace CSWebBrowserAutomation
{
    public class StoredSite
    {
        /// <summary>
        /// 主站点
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 在站点中的能完全被自动化连接的超链接
        /// </summary>
        public List<string> Urls { get; set; }

        /// <summary>
        /// 能自动输入的html元素。
        /// </summary>
        public List<HtmlInputElement> InputElements { get; set; }

        public StoredSite()
        {
            InputElements = new List<HtmlInputElement>();
        }

        /// <summary>
        /// 把这实例当XML文件保存。
        /// </summary>
        public void Save()
        {
            string folderPath = string.Format(@"{0}\StoredSites\",
                Environment.CurrentDirectory);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filepath = string.Format(@"{0}\{1}.xml", folderPath, this.Host);

            XMLSerialization<StoredSite>.SerializeFromObjectToXML(this, filepath);
        }

        /// <summary>
        /// 完成网页的自动化
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void FillWebPage(HtmlDocument document, bool submit)
        {
            // 网页中的提交按钮
            HtmlElement inputSubmit = null;

            //设置html元素的值并保存。如果这个元素是提交按钮，则指定为输入提交。
 
            foreach (HtmlInputElement input in this.InputElements)
            {
                HtmlElement element = document.GetElementById(input.ID);
                if (element == null)
                {
                    continue;
                }
                if (input is HtmlSubmit)
                {
                    inputSubmit = element;
                }
                else
                {
                    input.SetValue(element);
                }
            }

            // 自动点击提交按钮
            if (submit && inputSubmit != null)
            {
                inputSubmit.InvokeMember("click");
            }
        }

        /// <summary>
        ///通过宿主名得到一个存储站点
        /// </summary>
        public static StoredSite GetStoredSite(string host)
        {
            StoredSite storedForm = null;

            string folderPath = string.Format(@"{0}\StoredSites\",
                Environment.CurrentDirectory);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filepath = string.Format(@"{0}\{1}.xml", folderPath, host);
            if (File.Exists(filepath))
            {
                storedForm =
                    XMLSerialization<StoredSite>.DeserializeFromXMLToObject(filepath);
            }
            return storedForm;
        }
    }
}
