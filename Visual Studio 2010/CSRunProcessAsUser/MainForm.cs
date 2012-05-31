/***********************************  模块头  **************************************\
* 模块名:  MainForm.cs
* 项目名:  CSRunProcessAsUser
* 版权 (c) Microsoft Corporation.
* 
* MainForm.cs文件是为CSRunProcessAsUser主窗体工作的后台代码.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;
using System.ComponentModel;


namespace CSRunProcessAsUser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 使用OpenFileDialog对象输入执行命令行.
        /// </summary>
        private void btnCommand_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofdOpen = new OpenFileDialog())
            {
                if (ofdOpen.ShowDialog(this) == DialogResult.OK)
                {
                    tbCommand.Text = ofdOpen.FileName;
                }
            }
        }

        /// <summary>
        /// 使用 Process.Start来用指定的用户身份运行进程.
        /// </summary>
        private void btnRunCommand_Click(object sender, EventArgs e)
        {
            try
            {
                // Check the parameters.
                if (!string.IsNullOrEmpty(tbUserName.Text) &&
                    !string.IsNullOrEmpty(tbPassword.Text) &&
                    !string.IsNullOrEmpty(tbCommand.Text))
                {
                    SecureString password = StringToSecureString(tbPassword.Text);
                    Process proc = Process.Start(
                        this.tbCommand.Text,
                        this.tbUserName.Text,
                        password,
                        this.tbDomain.Text);

                    ProcessStarted(proc.Id);

                    proc.EnableRaisingEvents = true;
                    proc.Exited += new EventHandler(ProcessExited);
                }
                else
                {
                    MessageBox.Show("请输入用户名、密码和命令");
                }
            }
            catch (Win32Exception w32e)
            {
                ProcessStartFailed(w32e.Message);
            }
        }

        /// <summary>
        /// 当目标进程启动时激发.
        /// </summary>
        private void ProcessStarted(int processId)
        {
            MessageBox.Show("进程 " + processId.ToString() + " 启动");
        }

        /// <summary>
        /// 当目标进程退出时激发.
        /// </summary>
        private void ProcessExited(object sender, EventArgs e)
        {
            Process proc = sender as Process;
            if (proc != null)
            {
                MessageBox.Show("进程 " + proc.Id.ToString() + " 退出");
            }
        }

        /// <summary>
        /// 当目标进程启动失败时激发.
        /// </summary>
        private void ProcessStartFailed(string error)
        {
            MessageBox.Show(error, "进程启动失败",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 演示如何弹出凭据提示以便输入用户凭据.
        /// </summary>
        private void btnCredentialUIPrompt_Click(object sender, EventArgs e)
        {
            try
            {
                using (Kerr.PromptForCredential dialog = new Kerr.PromptForCredential())
                {
                    dialog.Title = "请指定用户";
                    dialog.DoNotPersist = true;
                    dialog.ShowSaveCheckBox = false;
                    dialog.TargetName = Environment.MachineName;
                    dialog.ExpectConfirmation = true;

                    if (DialogResult.OK == dialog.ShowDialog(this))
                    {
                        tbPassword.Text = SecureStringToString(dialog.Password);
                        string[] strSplit = dialog.UserName.Split('\\');
                        if (strSplit.Length == 2)
                        {
                            this.tbUserName.Text = strSplit[1];
                            this.tbDomain.Text = strSplit[0];
                        }
                        else
                        {
                            this.tbUserName.Text = dialog.UserName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        /// <summary>
        /// Helper函数.它将SecureString转换为String.
        /// </summary>
        static String SecureStringToString(SecureString secureStr)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(secureStr);
            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        /// <summary>
        /// Helper函数.它将String转换为SecureString.
        /// </summary>
        static SecureString StringToSecureString(String str)
        {
            SecureString secureStr = new SecureString();
            char[] chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                secureStr.AppendChar(chars[i]);
            }
            return secureStr;
        }
    }
}
