/********************************* 模块头 *********************************\
* 模块名:           CommonEncryption.aspx.cs
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
\****************************************************************************/

using System;
using System.Web.UI;
using System.Security.Cryptography;

namespace CSASPNETEncryptAndDecryptConfiguration
{
    public partial class CommonEncryption : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AddKeyUpEventOnTextControl();
            this.btnDecrypt.PreRender += new EventHandler(btnDecrypt_PreRender);
            this.btnEncrypt.PreRender += new EventHandler(btnEncrypt_PreRender);
        }

        private void RSAEncryption()
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = "MyKeyContainer";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                string plaintext = this.tbData.Text;
                byte[] plaindata = System.Text.Encoding.Default.GetBytes(plaintext);
                byte[] encryptdata = rsa.Encrypt(plaindata, false);
                string encryptstring = Convert.ToBase64String(encryptdata);
                this.tbEncryptData.Text = encryptstring;
            }
        }

        private void RSADecryption()
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = "MyKeyContainer";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] encryptdata = Convert.FromBase64String(this.tbEncryptData.Text);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                string plaindata = System.Text.Encoding.Default.GetString(decryptdata);
                this.tbDecryptData.Text = plaindata;
            }
        }

        protected void Encrypt_Click(object sender, EventArgs e)
        {
            RSAEncryption();
        }

        protected void Decrypt_Click(object sender, EventArgs e)
        {
            RSADecryption();
        }

        void btnDecrypt_PreRender(object sender, EventArgs e)
        {
            EnableDecryptButton();
        }

        void btnEncrypt_PreRender(object sender, EventArgs e)
        {
            EnableEncryptButton();
        }

        private void AddKeyUpEventOnTextControl()
        {
            string script = string.Format(@"function PressFn(sender) {{
                                            document.getElementById('{0}').disabled = sender.value == '' ? true : false;
                                            }}", btnEncrypt.ClientID);
            tbData.Attributes["onkeyup"] = "PressFn(this)";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "DataKeyUp", script, true);
        }

        private void EnableDecryptButton()
        {
            btnDecrypt.Enabled = this.tbEncryptData.Text != string.Empty ? true : false;
        }

        private void EnableEncryptButton()
        {
            btnEncrypt.Enabled = this.tbData.Text != string.Empty ? true : false;
        }

    }
}