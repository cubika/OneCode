/****************************** 模块头 ******************************\
* 模块名:    EmailAvailableValidationHandler.cs
* 项目名:    CSASPNETEmailAddresseValidator
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了如何发送一封确认的邮件去检查一个邮箱地址的可用性.
* 
* 在本文件，我们创建一个HttpHandler用来更新数据库中的记录并且完成验证.
* 我们需要在Web.config文件中注册这个HttpHandler.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Web;
using System.Web.Configuration;
using System.Configuration;
using CSASPNETEmailAddresseValidator.Module;
using System.Linq;

namespace CSASPNETEmailAddresseValidator.Handler
{
    public class EmailAvailableValidationHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }


        public void ProcessRequest(HttpContext context)
        {

            // 取得唯一键与数据库中的所存储的键相比较.
            string key = context.Request.QueryString["k"];

            if (!string.IsNullOrEmpty(key))
            {
                using (EmailAddressValidationDataContext service =
                    new EmailAddressValidationDataContext())
                {
                    tblEmailValidation EValidation =
                        service.tblEmailValidations.Where(
                        t => t.ValidateKey.Trim() == key).FirstOrDefault();
                    if (EValidation != null) 
                    {

                        // 通过验证后更新记录.
                        EValidation.IsValidated = true;
                        service.SubmitChanges();

                        // 我们可以定制返回的值并输出.
                        // 这里只是简单的输出信息.
                        context.Response.Write("恭喜! 你的邮箱地址: " +
                            EValidation.EmailAddress + "已经验证成功!");
                    }
                    else
                    {
                        context.Response.Write("请先提交你的邮箱地址.");
                    }
                }
            }
        }



    }
}
