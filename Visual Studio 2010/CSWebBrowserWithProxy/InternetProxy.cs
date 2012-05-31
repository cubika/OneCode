/****************************** Module Header ******************************\
* 模块名称:  InternetProxy.cs
* 项目名称:	    CSWebBrowserWithProxy
* Copyright (c) Microsoft Corporation.
*
* 这个类是对代理服务器和其使用的证书的描述。
* 请先在ProxyList.xml文件中设定它。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.Xml.Linq;

namespace CSWebBrowserWithProxy
{
    public class InternetProxy
    {
        public string ProxyName { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public static readonly InternetProxy NoProxy = new InternetProxy
        {
            Address = string.Empty,
            Password = string.Empty,
            ProxyName = string.Empty,
            UserName = string.Empty
        };
        
        static List<InternetProxy> proxyList = null;
        public static List<InternetProxy> ProxyList
        {
            get
            {
                // 从ProxyList.xml中获取代理服务器。
                if (proxyList == null)
                {
                    proxyList = new List<InternetProxy>();
                    try
                    {
                        XElement listXml = XElement.Load("ProxyList.xml");
                        foreach (var proxy in listXml.Elements("Proxy"))
                        {
                            proxyList.Add(
                                new InternetProxy
                                {
                                    ProxyName = proxy.Element("ProxyName").Value,
                                    Address = proxy.Element("Address").Value,
                                    UserName = proxy.Element("UserName").Value,
                                    Password = proxy.Element("Password").Value
                                });
                        }
                    }
                    catch (System.Exception)
                    {
                    }                  
                }
                return proxyList;
            }
        }
    }
}

