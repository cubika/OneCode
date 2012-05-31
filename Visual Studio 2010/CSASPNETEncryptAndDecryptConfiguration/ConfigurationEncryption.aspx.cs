/********************************* 模块头 *********************************\
* 模块名:           ConfigurationEncryption.aspx.cs
* 项目名:      CSASPNETEncryptAndDecryptConfiguration
* 版权(c) Microsoft Corporation.
* 
* 此示例演示如何使用RSA加密算法API的加密和解密配置节点,以保护ASP.NET Web应用程序的敏感
* 信息,防止拦截或劫持.
*
* 这个项目包含两个片段.第一个演示了如何使用RSA提供和RSA容器中进行加密和解密一些Web应用
* 程序中的词或值.第一个片段的目的是让我们知道RSA的机制概述.第二个则显示了如何使用RSA
​​* 配置提供加密和解密web.config中的配置节点.
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
using System.Configuration;
using System.Web.Configuration;

namespace CSASPNETEncryptAndDecryptConfiguration
{
    public partial class ConfigurationEncryption : System.Web.UI.Page
    {
        //使用RSA提供器加密配置节点
        private const string provider = "RSAProtectedConfigurationProvider";  

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ddlSection.SelectedValue))
            {
                Response.Write("请选择一个配置节点");
                return;
            }

            string sectionString = this.ddlSection.SelectedValue;

            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection section = config.GetSection(sectionString);
            if (section != null)
            {
                section.SectionInformation.ProtectSection(provider);
                config.Save();
                Response.Write("加密成功,请检查配置文件.");
            }
        }

        protected void btnDecrypt_Click(object sender, EventArgs e)
        {
            string sectionString = this.ddlSection.SelectedValue;

            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection section = config.GetSection(sectionString);
            if (section != null && section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
                config.Save();
                Response.Write("解密成功,请检查配置文件.");
            }

        }


    }
}