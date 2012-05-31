/****************************** 模块头 ******************************\
* 模块名:    Default.aspx.cs
* 项目名:    CSASPNETEmailAddresseValidator
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何发送一封确认的邮件去检查一个邮件地址的可用性.
* 
* 在本文件中，我们创建一些控件来调用发送确认邮件的代码.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CSASPNETEmailAddresseValidator.Module;

namespace CSASPNETEmailAddresseValidator
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // 如果我们单击验证按钮, 它将会开始发送验证邮件.
        protected void btnValidate_Click(object sender, EventArgs e)
        {
            btnSendAgain.Visible = false;
            try
            {

                // 实例化一个EmailValidation类，准备开始发送确认邮件.
                EmailValidation validator = new EmailValidation(
                    tbSendMail.Text,
                    Session["password"].ToString(),
                    tbHost.Text,
                    chkUseSSL.Checked);

                // 调用StartToValidateEmail方法验证邮箱地址并且发送确认邮件.
                ValidateEmailResult rsl =
                    validator.StartToValidateEmail(tbValidateEmail.Text);
                switch (rsl)
                {
                    case ValidateEmailResult.EmailStartToValidate:
                        lbMessage.Text =
                            "验证邮件已经成功发送." +
                            "请在邮箱查收.";
                        btnSendAgain.Visible = true;
                        break;
                    case ValidateEmailResult.EmailValidated:
                        lbMessage.Text = "此邮箱已经通过验证.";
                        break;
                    case ValidateEmailResult.EmailValidating:
                        lbMessage.Text = "此邮箱正在等待用户" +
                            "在邮件中点击确认链接结束验证";
                        btnSendAgain.Visible = true;
                        break;
                }
            }
            catch (Exception err)
            {
                lbMessage.Text = "错误:" + err.Message;
            }
        }

        // 如果我们已经发送了确认邮件，但是用户仍然没有收到 
        // 邮件，我们可以允许他进行重发.
        protected void btnSendEmailAgain_Click(object sender, EventArgs e)
        {
            try
            {

                // 实例化一个EmailValidation类，准备再次发送确认邮件.
                EmailValidation validator = new EmailValidation(
                    tbSendMail.Text,
                    Session["password"].ToString(),
                    tbHost.Text,
                    chkUseSSL.Checked);

                // 使用ReSendValidationEmail方法再次发送确认邮件.
                validator.ReSendValidationEmail(tbValidateEmail.Text);
                lbMessage.Text = "邮件已经再次发送. 请再次查收.";
            }
            catch (Exception err)
            {
                lbMessage.Text = "错误:" + err.Message;
            }
        }

        protected void ValidationWizard_OnNextButtonClick(object sender,
                                                WizardNavigationEventArgs e)
        {
            if (e.CurrentStepIndex == 0)
            {
                Session["password"] = tbSendMailPassword.Text;
            }
        }

        protected void ValidationWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {

        }

        protected void chkUseSSL_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}