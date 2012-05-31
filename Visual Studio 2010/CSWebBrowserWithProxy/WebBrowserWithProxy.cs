/****************************** Module Header ******************************\
* 模块名称: WebBrowserControl.cs
* 项目名称:		    CSWebBrowserWithProxy
* Copyright (c) Microsoft Corporation.
* 
* WebBrowserControl继承了WebBrowser类并且具有设置代理的功能。
* 原始的internet 设置将被备份，被指定的代理将在浏览过程中被
* 使用，并且，在浏览过程被重置时，原始的internet 设置将被恢复。
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
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Permissions;
using System.ComponentModel;

namespace CSWebBrowserWithProxy
{
    public class WebBrowserWithProxy : WebBrowser
    {

        //代理服务器连接
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public InternetProxy Proxy { get; set; }

        // 储存原始的internet连接选项以便在连接以后你能恢复它。
        INTERNET_PER_CONN_OPTION_LIST currentInternetSettings;

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WebBrowserWithProxy()
        {          
        }
    

        /// <summary>
        /// 处理Navigating事件。在Navigating事件中，连接还没有被打开并且你还能编辑它。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            base.OnNavigating(e);

            // 备份现有的连接选项。
            currentInternetSettings = WinINet.BackupConnectionProxy();

            // 设置或激活代理
            if (Proxy != null && !string.IsNullOrEmpty(Proxy.Address))
            {
                WinINet.SetConnectionProxy(Proxy.Address);
            }
            else
            {
                WinINet.DisableConnectionProxy();
            }
        }

        /// <summary>
        /// 处理Navigated事件。在Navigated事件中，与internet的连接被完成。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnNavigated(WebBrowserNavigatedEventArgs e)
        {
            base.OnNavigated(e);

            // 恢复原始的连接选项
            WinINet.RestoreConnectionProxy(currentInternetSettings); 
        }          

        /// <summary>
        /// 封装Navigate方法并且在需要的情况下设置代理授信头信息。
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Goto(string url)
        {
            System.Uri uri = null;
            bool result = System.Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
            if (!result)
            {
                throw new ArgumentException("The url is not valid. ");
            }

            // 如果代理包含有用户名和密码，代理授信头信息将被要求设置
            if (Proxy != null && !string.IsNullOrEmpty(Proxy.UserName)
                && !string.IsNullOrEmpty(Proxy.Password))
            {

                // 这个头信息是由个64位字符串储存的证书。
                var credentialStringValue = string.Format("{0}:{1}",
                    Proxy.UserName, Proxy.Password);
                var credentialByteArray = ASCIIEncoding.ASCII.GetBytes(credentialStringValue);
                var credentialBase64String = Convert.ToBase64String(credentialByteArray);
                string authHeader = string.Format("Proxy-Authorization: Basic {0}",
                    credentialBase64String);

                Navigate(uri, string.Empty, null, authHeader);
            }
            else
            {
                Navigate(uri);
            }
        }
    }
}
