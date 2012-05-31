/**************************************** 模块头 *****************************************\
* 模块名:   RunCmd.aspx.cs
* 项目名:      CSASPNETCMD
* 版权 (c) Microsoft Corporation
*
* RunCmd类反应了用户输入的处理和结果的输出.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************************/

#region Using directives

using System;
using System.Web.UI;
using System.IO;
using System.Web;
using System.Security;

#endregion

[assembly: SecurityCritical]
namespace CSASPNETCMD
{
    public partial class RunCmd : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }


        protected void btnRunBatch_Click(object sender, EventArgs e)
        {
            // 上传文件
            string fileName = HttpContext.Current.Server.MapPath(@"Batchs\" +
                System.Guid.NewGuid().ToString() + "_" + fileUpload.FileName);
            fileUpload.SaveAs(fileName);

            // 运行这个批处理文件.
            string output = RunBatch(fileName);
            // 设定结果
            tbResult.Text = output;

            // 删除临时文件.
            System.IO.File.Delete(fileName);
        }

        protected void btnRunCmd_Click(object sender, EventArgs e)
        {
            // 根据命令行代码创建批处理文件.
            string commandLine = this.tbCommand.Text;
            string fileName = HttpContext.Current.Server.MapPath(@"Batchs\" +
                System.Guid.NewGuid().ToString() + ".bat");
            using (StreamWriter sw = System.IO.File.CreateText(fileName))
            {
                sw.Write(commandLine);
                sw.Flush();
            }

            // 运行这个批处理文件.
            string output = RunBatch(fileName);

            // 设定结果
            tbResult.Text = output;

            // 删除临时文件.
            System.IO.File.Delete(fileName);
        }

        /// <summary>
        /// 使用BatchRunner类运行批处理文件.
        /// </summary>
        /// <param name="fileName">批处理文件的全名.</param>
        /// <returns></returns>
        private string RunBatch(string fileName)
        {
            // 设定批处理执行信息.
            BatchRunner batchRunner = new BatchRunner();
            batchRunner.DomainName = tbDomainName.Text.Trim();
            batchRunner.UserName = tbUserName.Text.Trim();
            batchRunner.Password = tbPassword.Text.Trim();

            // 运行批处理文件.
            string output = batchRunner.ExecuteBatch(fileName);

            return output;
        }
    }
}
