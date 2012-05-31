/********************************** 模块头 ***********************************\
* 模块名:        SharedSessionModule.cs
* 项目名:        CSASPNETShareSessionBetweenSubDomains
* 版权(c) Microsoft Corporation
*
* SharedSessionModule使网站使用相同的应用程序ID和会话ID来实现共享会话.
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
using System.Web;
using System.Reflection;
using System.Configuration;


namespace CSASPNETShareSessionBetweenSubDomainsModule
{
    /// <summary>
    /// 子域内应用程序间用来共享对话的HttpModule.
    /// </summary>
    public class SharedSessionModule : IHttpModule
    {
        // 内存缓存设定.
        protected static string applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        protected static string rootDomain = ConfigurationManager.AppSettings["RootDomain"];


        #region IHttpModule Members
        /// <summary>
        /// 初始化模块准备处理请求.
        /// </summary>
        /// <param name="context">
        /// 提供了在ASP.NET应用程序对所有的应用程序对象的
        /// 共同的方法，属性和事件的访问的System.Web.HttpApplication.
        /// </param>
        public void Init(HttpApplication context)
        {
            // 模块同时需要应用程序名和根域才能工作k.
            if (string.IsNullOrEmpty(applicationName) || 
                string.IsNullOrEmpty(rootDomain))
            {
                return;
            }

            // 运行时改变应用程序名.
            FieldInfo runtimeInfo = typeof(HttpRuntime).GetField("_theRuntime", 
                BindingFlags.Static | BindingFlags.NonPublic);
            HttpRuntime theRuntime = (HttpRuntime)runtimeInfo.GetValue(null);
            FieldInfo appNameInfo = typeof(HttpRuntime).GetField("_appDomainAppId", 
                BindingFlags.Instance | BindingFlags.NonPublic);

            appNameInfo.SetValue(theRuntime, applicationName);

            // 订阅事件.
            context.PostRequestHandlerExecute += new EventHandler(context_PostRequestHandlerExecute);
        }

        /// <summary>
        /// 处理模块实现所使用的资源(除内存外).
        /// </summary>
        public void Dispose()
        {
        }
        #endregion


        /// <summary>
        /// 在发送到客户端的响应内容，更改cookie到根域
        /// 保存当前会话ID.
        /// </summary>
        /// <param name="sender">
        /// 提供了在ASP.NET应用程序对所有的应用程序对象的
        /// 共同的方法，属性和事件的访问的System.Web.HttpApplication实例.
        /// </param>
        void context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication context = (HttpApplication)sender;

            //  在cookie中保存指定当前会话ASP.NET会话Id.
            HttpCookie cookie = context.Response.Cookies["ASP.NET_SessionId"];

            if (context.Session != null &&
                !string.IsNullOrEmpty(context.Session.SessionID))
            {
                // 需要在每次请求保存当前会话Id.
                cookie.Value = context.Session.SessionID;

                // 所有使用保存此Cookie的应用程序在同一根域
                // 因此可以被共享.
                if (rootDomain != "localhost")
                {
                    cookie.Domain = rootDomain;
                }

                // 所有虚拟应用程序和文件夹也共享此Cookie.
                cookie.Path = "/";
            }
        }
    }
}
