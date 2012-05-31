/****************************** 模块头 ******************************\
* 模块名:    WebBrowserEx.cs
* 项目名:	    CSWebBrowserAutomation
* 版权(c) Microsoft Corporation.
* 
* 这个WebBrowserEx类继承WebBrowser类.它支持如下功能:
* 1. 阻止指定的网址.
* 2. 完成自动化输入html元素.
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
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.ComponentModel;

namespace CSWebBrowserAutomation
{
    public partial class WebBrowserEx : WebBrowser
    {

        /// <summary>
        /// 指定当前页是否可以完成自动化加载。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool CanAutoComplete { get; private set; }
    
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WebBrowserEx()
        {
        }
       
        /// <summary>
        /// 当文档完成加载后，检查这个页是否可以加载完。
        /// automatically.
        /// </summary>
         [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {

            // 检查当前页是否被保存。
            StoredSite form = StoredSite.GetStoredSite(this.Url.Host);
            CanAutoComplete = form != null
                && form.Urls.Contains(this.Url.AbsolutePath.ToLower());

            base.OnDocumentCompleted(e);
        }

        /// <summary>
        /// 完成页的自动化。
        /// </summary>
         [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void AutoComplete()
        {
            if (CanAutoComplete)
            {
                StoredSite form = StoredSite.GetStoredSite(this.Url.Host);
                form.FillWebPage(this.Document, true);
            }      
        }

        /// <summary>
        /// 检查URL是否包含在阻止列表中.
        /// </summary>
         [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (BlockSites.Instance.Hosts.Contains(e.Url.Host.ToLower()))
            {
                e.Cancel = true;
                this.Navigate(string.Format(@"{0}\Resources\Block.htm",
                    Environment.CurrentDirectory));
            }
            else
            {
                base.OnNavigating(e);
            }
        }
     
    }
}
