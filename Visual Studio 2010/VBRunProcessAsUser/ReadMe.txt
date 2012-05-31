======================================================================================
              Windows 应用程序: VBRunProcessAsUser 概述
======================================================================================

//////////////////////////////////////////////////////////////////////////////////////
摘要:

这个实例演示了如何在不同的用户身份下运行一个进程. 


//////////////////////////////////////////////////////////////////////////////////////
演示：

步骤1. 在Visual Studio 2010中生成这个项目. 

步骤2. 运行VBRunProcessAsUser.exe.

步骤3. 为你想要运行的用户输入: 用户名，域(如果它是一个活动目录（Active Directory）用户账户),和
       密码. 或者，你可以点击靠近“用户名”文本框的“...” 按钮.它将提示一个标准用户凭证集对话框. 
	   你也可以将用户名和密码填入对话框里.
       
步骤4. 点击“命令...”按钮，选择你想用步骤3中指定的用户身份运行的程序.

步骤5. 点击“运行命令”按钮，以指定用户的方式运行程序. 当成功启动进程时，你将看到一个消息框，指明
       “进程 xxx 已经启动”. 你可以在用户管理器中验证该进程是否已经运行. 当你退出这个新的进程时，
       你将再次看到一个消息框，指出“进程 xxx 已经退出”.

//////////////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 该实例使用"Process.Start"函数来实现为不同的用户运行程序. 

            Try
            If (Not String.IsNullOrEmpty(tbUserName.Text)) AndAlso _
                (Not String.IsNullOrEmpty(tbPassword.Text)) AndAlso _
                (Not String.IsNullOrEmpty(tbCommand.Text)) Then
                Dim password As SecureString = StringToSecureString(Me.tbPassword.Text.ToString())
                Dim proc As Process = Process.Start(tbCommand.Text.ToString(), _
                                                    tbUserName.Text.ToString(), _
                                                    password, _
                                                    tbDomain.Text.ToString())
                ProcessStarted(proc.Id)

                proc.EnableRaisingEvents = True
                AddHandler proc.Exited, AddressOf ProcessExited
            Else
                MessageBox.Show("请输入用户名、密码和命令")
                Return
            End If
        Catch w32e As System.ComponentModel.Win32Exception
            ProcessStartFailed(w32e.Message)
        End Try
   
2. 该实例使用C++、CLI库"Kerr.Credentials"来包装本地API CredUIPromptForCredentials来收集用户
   凭据. Kerr.Credentials 库是Kenny Kerr提供的. 可以从这个MSDN文章中下载它：
   http://www.microsoft.com/indonesia/msdn/credmgmt.aspx.
   
        Try
            Using dialog As New Kerr.PromptForCredential()
                dialog.Title = "请指定用户"
                dialog.DoNotPersist = True
                dialog.ShowSaveCheckBox = False
                dialog.TargetName = Environment.MachineName
                dialog.ExpectConfirmation = True

                If DialogResult.OK = dialog.ShowDialog(Me) Then
                    tbPassword.Text = SecureStringToString(dialog.Password)
                    Dim strSplit() As String = dialog.UserName.Split("\"c)
                    If (strSplit.Length = 2) Then
                        Me.tbUserName.Text = strSplit(1)
                        Me.tbDomain.Text = strSplit(0)
                    Else
                        Me.tbUserName.Text = dialog.UserName
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        End Try

3. 在新进程启动后，登记它的Exited事件，以便在该进程退出时候接收到通知.

        proc.EnableRaisingEvents = True
        AddHandler proc.Exited, AddressOf ProcessExited

      	''' <summary>
        ''' 当目标进程退出时激发.
        ''' </summary>
        Private Sub ProcessExited(ByVal sender As Object, ByVal e As EventArgs)
        Dim proc As Process = TryCast(sender, Process)
        If proc IsNot Nothing Then
            MessageBox.Show("进程 " & proc.Id.ToString() & " 退出")
        End If
        End Sub

//////////////////////////////////////////////////////////////////////////////////////
参考:

Process.Start
http://msdn.microsoft.com/en-us/library/system.diagnostics.process.start.aspx

Credential Management with the .NET Framework 2.0
http://www.microsoft.com/indonesia/msdn/credmgmt.aspx


//////////////////////////////////////////////////////////////////////////////////////


