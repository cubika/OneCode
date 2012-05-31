======================================================================================
              Windows 应用程序: CSRunProcessAsUser 概述                        
======================================================================================

//////////////////////////////////////////////////////////////////////////////////////
摘要:

这个实例演示了如何在不同的用户身份下运行一个进程. 


//////////////////////////////////////////////////////////////////////////////////////
演示：

步骤1. 在Visual Studio 2010中生成这个项目. 

步骤2. 运行CSRunProcessAsUser.exe.

步骤3. 为你想要运行的用户输入: 用户名，域(如果它是一个活动目录（Active Directory）用户账户),和
       密码. 或者，你可以点击靠近“用户名”文本框的“...” 按钮.它将提示一个标准用户凭证集对话框. 
	   你也可以将用户名和密码填入对话框里. 
       
       
步骤4. 点击“命令...”按钮，选择你想用步骤3中指定的用户身份运行的程序.

步骤5. 点击“运行命令”按钮，以指定用户的方式运行程序. 当成功启动进程时，你将看到一个消息框，指明
       “进程 xxx 已经启动”. 你可以在用户管理器中验证该进程是否已经运行. 当你退出这个新的进程时，
       你将再次看到一个消息框，指出“进程 xxx 已经退出”. 


////////////////////////////////////////////////////////////////////////////////////////
实现:

1. 该实例使用"Process.Start"函数来实现为不同的用户运行程序. 

            try
            {
                // 检查参数.
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
		   
2. 该实例使用C++、CLI库"Kerr.Credentials"来包装本地API CredUIPromptForCredentials来收集用户
   凭据. Kerr.Credentials 库是Kenny Kerr提供的. 可以从这个MSDN文章中下载它：
   http://www.microsoft.com/indonesia/msdn/credmgmt.aspx.
   
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

3. 在新进程启动后，登记它的Exited事件，以便在该进程退出时候接收到通知.

        proc.EnableRaisingEvents = true;
        proc.Exited += new EventHandler(ProcessExited);

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


////////////////////////////////////////////////////////////////////////////////////////////
参考:

Process.Start
http://msdn.microsoft.com/en-us/library/system.diagnostics.process.start.aspx

Credential Management with the .NET Framework 2.0/
http://www.microsoft.com/indonesia/msdn/credmgmt.aspx


////////////////////////////////////////////////////////////////////////////////////////////


