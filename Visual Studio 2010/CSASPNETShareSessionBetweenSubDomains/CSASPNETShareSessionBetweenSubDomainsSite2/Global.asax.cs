/********************************** 模块头 ***********************************\
* 模块名:        Global.asax.cs
* 项目名:        CSASPNETShareSessionBetweenSubDomainsSite2
* 版权(c) Microsoft Corporation
*
* Global.asax中的代码只是为了确保您已在SQLServer设置会话状态
* 这个范例可以在没有任何配置或命令行时运行
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

namespace CSASPNETShareSessionBetweenSubDomainsSite2
{
    public class Global : System.Web.HttpApplication
    {
        // 需要在运行示例前配置Sql Server.
        protected void Application_Error(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Write(@"运行此示例前, 请先执行下列命令:<br />");
            Response.Write(@"""<b>C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe -S localhost\sqlexpress -E -ssadd</b>""<br />");
            Response.Write("配置本机的Sql Server Experssion支持会话状态.<br /><br />");
            Response.Write("要了解如何回滚这项配置, 请参阅 ReadMe.txt文件.");
            Response.End();
        }
    }
}