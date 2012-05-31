/**************************************** 模块头 *****************************************\
* 模块名:   BatchRunner.cs
* 项目名:      CSASPNETCMD
* 版权  (c) Microsoft Corporation.
*
* Batch Runner类反映了如何处理批处理文件.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************************/

#region Using directives
using System;
using System.Security;
#endregion

namespace CSASPNETCMD
{
    public class BatchRunner
    {
        #region fields
        private string domainName;
        private string userName;
        private string password;
        #endregion

        #region properties
        /// <summary>
        /// 设置和获取验证用户的DomainName.
        /// </summary>
        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
        }
        /// <summary>
        /// 设置和获取验证用户的UserName.
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        /// <summary>
        /// 设置和获取验证用户的Password.
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        #endregion       

        #region constructor
        /// <summary>
        /// 默认构造器
        /// </summary>
        public BatchRunner()
        {
        }

        /// <summary>
        /// 验证信息的构造器.
        /// </summary>
        /// <param name="domain">验证用户的DomainName.</param>
        /// <param name="username">验证用户的username.</param>
        /// <param name="password">验证用户的password.</param>
        public BatchRunner(string domain, string username,string password)
        {
            this.domainName = domain;
            this.userName = username;
            this.password = password;
        }

        #endregion

        /// <summary>
        /// 执行批处理文件.
        /// </summary>
        /// <param name="fileName">执行的批处理文件的全名.</param>
        /// <returns>批处理文件的执行结果.</returns>
        public string ExecuteBatch(string fileName)
        {
            if (!fileName.ToLower().EndsWith(".bat"))
            {
                throw new ArgumentException("the file's suffix must be .bat");
            }
            string retOutPut = "No Output!";

            try
            {
                System.Diagnostics.ProcessStartInfo psi = GenerateProcessInfo(fileName);

                System.Diagnostics.Process processBatch = System.Diagnostics.Process.Start(psi);
                System.IO.StreamReader myOutput = processBatch.StandardOutput;

                processBatch.WaitForExit();
                if (processBatch.HasExited)
                {
                    retOutPut = myOutput.ReadToEnd();
                }
            }
            catch (System.ComponentModel.Win32Exception winException)
            {
                if (winException.Message.Contains("bad password"))
                {
                    retOutPut = winException.Message;
                }
                else
                {
                    retOutPut = "Win32Exception occured";
                }
                // 记录执行信息.
            }
            catch (System.Exception exception)
            {
                retOutPut = exception.Message;

                // 记录异常信息.
            }
            return retOutPut;
        }

        /// <summary>
        /// 通过(domainName,userName,password)字段生成ProcessInfo.
        /// </summary>
        /// <param name="fileName">执行的批处理文件的全名.</param>
        /// <returns></returns>
        private System.Diagnostics.ProcessStartInfo GenerateProcessInfo(string fileName)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(fileName);
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            if (!string.IsNullOrEmpty(domainName))
            {
                psi.Domain = domainName;
            }
            if (!string.IsNullOrEmpty(userName))
            {
                psi.UserName = userName;
            }
            if (!string.IsNullOrEmpty(password))
            {
                SecureString sstring = new SecureString();
                foreach (char c in password)
                {
                    sstring.AppendChar(c);
                }
                psi.Password = sstring;
            }
            return psi;
        } 

    }
}